using UnityEngine;
using System.Collections;
using Pathfinding;

public class NinjaController : MonoBehaviour
{
    public float Speed = 10;
    public float WaypointTolerance = 0.05f;
    public float ThinkTime = 0.2f;

    private Seeker seeker;

    private int currentWaypoint = 0;
    private Path currentPath;

    public void Awake()
    {
        seeker = GetComponent<Seeker>();
    }

    public void Start()
    {
        InvokeRepeating("Think", 0, ThinkTime);
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
            }
            else
            {
                // Reached target
            }
        }
    }

    public void Think()
    {
        if (GameController.Instance.AreNinjasAttacking)
        {
            seeker.StartPath(transform.position, GameController.Instance.Player.transform.position, OnPathComplete);
        }
    }

    public void OnPathComplete(Path path)
    {
        currentPath = path;
        currentWaypoint = 0;
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
