using UnityEngine;
using System.Collections;

public class BoxArrowController : MonoBehaviour
{
    private BoxController box;
    private Animation animationPlayer;
    private SpriteRenderer sprite;

    void Awake()
    {
        box = GetComponentInParent<BoxController>();
        animationPlayer = GetComponent<Animation>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (GameController.Instance.IsGameplay && GameController.Instance.CurrentBox == box)
        {
            animationPlayer.enabled = true;
            sprite.enabled = true;
        }
        else
        {
            animationPlayer.enabled = false;
            sprite.enabled = false;
        }
    }
}
