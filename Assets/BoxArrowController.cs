using UnityEngine;
using System.Collections;

public class BoxArrowController : MonoBehaviour
{
    private BoxController box;
    private Animation animation;
    private SpriteRenderer sprite;

    void Awake()
    {
        box = GetComponentInParent<BoxController>();
        animation = GetComponent<Animation>();
        sprite = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        if (GameController.Instance.IsGameplay && GameController.Instance.CurrentBox == box)
        {
            animation.enabled = true;
            sprite.enabled = true;
        }
        else
        {
            animation.enabled = false;
            sprite.enabled = false;
        }
    }
}
