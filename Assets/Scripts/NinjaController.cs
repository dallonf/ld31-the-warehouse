using UnityEngine;
using System.Collections;
using Pathfinding;
using System.Collections.Generic;

public class NinjaController : MonoBehaviour
{
    public float Speed = 10;
    public float WaypointTolerance = 0.05f;
    public float ThinkTime = 0.2f;
    public float ClusterDistance = 2f; // Ninjas will spread out when they get closer to this from each other

    private Seeker seeker;

    private int currentWaypoint = 0;
    private Path currentPath;
    private bool requestingRandomPath;

    public void Awake()
    {
        seeker = GetComponent<Seeker>();
    }

    public void Start()
    {
        StartCoroutine(Think());
    }

    void FixedUpdate()
    {
        if (GameController.Instance.AreNinjasAttacking && !GameController.Instance.LightsOn)
        {
            if (currentPath != null)
            {
                float distanceLeft = Speed * Time.deltaTime;

                while (distanceLeft > 0 && currentWaypoint < currentPath.vectorPath.Count)
                {
                    var waypoint = currentPath.vectorPath[currentWaypoint];

                    var moveVector = waypoint - transform.position;
                    var moveAmount = moveVector.magnitude;

                    if (moveAmount <= 0.5f)
                    {
                        // Close enough, just skip ahead to the next waypoint
                        currentWaypoint++;
                        continue;
                    }

                    if (moveAmount <= distanceLeft)
                    {
                        distanceLeft -= moveAmount;
                        currentWaypoint++;
                    }
                    else
                    {
                        moveVector = moveVector.normalized * distanceLeft;
                        distanceLeft = 0;
                    }
                    transform.position += moveVector;
                }

                if (currentWaypoint >= currentPath.vectorPath.Count)
                {
                    // Reached target
                    currentPath = null;
                }
            }
        }
    }

    public IEnumerator Think()
    {
        while (true)
        {
            if (GameController.Instance.AreNinjasAttacking)
            {
                //Debug.Log(gameObject.name);
                var ninjasInRange = new List<Transform>();
                foreach (var otherNinja in GameController.Instance.Ninjas)
                {
                    if (otherNinja.gameObject != this.gameObject && Vector3.SqrMagnitude(otherNinja.transform.position - transform.position) < ClusterDistance * ClusterDistance)
                    {
                        ninjasInRange.Add(otherNinja.transform);
                    }
                }

                float sqrDistance = Vector3.SqrMagnitude(transform.position - GameController.Instance.Player.transform.position);
                foreach (var otherNinja in ninjasInRange)
                {
                    if (Vector3.SqrMagnitude(otherNinja.transform.position - GameController.Instance.Player.transform.position) < sqrDistance)
                    {
                        // This is not the closest ninja, so spread out a bit
                        // Start the random path sequence
                        requestingRandomPath = true;

                        // Ack, hardcoded coordinates, oh well, it's submission day
                        Vector3 randomTarget = new Vector3(Random.Range(-15f, 15f), Random.Range(-11.25f, 11.25f));
                        seeker.StartPath(transform.position, randomTarget, OnRandomPathComplete);

                        while (requestingRandomPath || currentPath != null)
                        {
                            // Wait to follow this path
                            yield return new WaitForSeconds(ThinkTime);
                        }
                        break;
                    }
                }

                seeker.StartPath(transform.position, GameController.Instance.Player.transform.position, OnPathComplete);
            }
            yield return new WaitForSeconds(ThinkTime);
        }

    }

    public void OnPathComplete(Path path)
    {
        currentPath = path;
        currentWaypoint = 0;
    }

    public void OnRandomPathComplete(Path path)
    {
        currentPath = path;
        currentWaypoint = 0;
        requestingRandomPath = false;
    }


    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.GetComponent<PlayerController>();
        if (player)
        {
            GameController.Instance.GameOver();
        }
    }


}
