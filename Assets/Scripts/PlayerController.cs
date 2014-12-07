using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float Speed = 3.5f;
    public bool IsCarryingBox;
    public Renderer BoxArmsRenderer;
    public Renderer IdleArmsRenderer;

    private Rigidbody2D rigidbody2d;
    private Animator animator;

    private Vector3 lastPosition;
    private Vector3 startPosition;

    void Awake()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Use this for initialization
    void Start()
    {
        startPosition = transform.position;
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
        animator.SetBool("HaveBox", IsCarryingBox);

        
        var currentPosition = transform.position;
        var actualVelocity = (currentPosition - lastPosition).magnitude / Time.deltaTime;
        bool walking = (actualVelocity > 0.3f);
        audio.mute = !walking; // Footsteps
        animator.SetBool("Walking", walking);
        lastPosition = currentPosition;
    }

    public void Respawn()
    {
        transform.position = startPosition;
        lastPosition = startPosition;
        IsCarryingBox = false;
        transform.rotation = Quaternion.identity;
        rigidbody2d.velocity = Vector3.zero;
        audio.mute = true;
    }
}
