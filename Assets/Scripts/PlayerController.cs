using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float Speed = 3.5f;
    public bool IsCarryingBox;
    public Renderer BoxArmsRenderer;
    public Renderer IdleArmsRenderer;

    private Rigidbody2D rigidbody2d;

    private Vector3 lastPosition;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
    }

    // Use this for initialization
    void Start()
    {
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameController.Instance.IsGameplay)
        {
            var moveVector = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
            if (moveVector.sqrMagnitude > 1)
            {
                moveVector.Normalize();
            }

            rigidbody2d.velocity = moveVector * Speed;

            if (moveVector.sqrMagnitude > 0)
            {
                transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(moveVector.y, moveVector.x) - 90, Vector3.forward);
            }
        }
        else
        {
            rigidbody2d.velocity = Vector2.zero;
        }
    }

    void FixedUpdate()
    {
        BoxArmsRenderer.enabled = IsCarryingBox;
        IdleArmsRenderer.enabled = !IsCarryingBox;

        // Footsteps
        var currentPosition = transform.position;
        var actualVelocity = (currentPosition - lastPosition).magnitude / Time.deltaTime;
        audio.mute = (actualVelocity <= 0.3f);
        lastPosition = currentPosition;
    }
}
