using UnityEngine;
using System.Collections;

public class BoxController : MonoBehaviour
{
    public bool IsTargetBox { get { return GameController.Instance.CurrentBox == this; } }

    public void OnCollisionEnter2D(Collision2D coll)
    {
        var player = coll.gameObject.GetComponent<PlayerController>();
        if (player && !player.IsCarryingBox && IsTargetBox)
        {
            player.IsCarryingBox = true;
            gameObject.SetActive(false);
            GameController.Instance.OnBoxPickedUp();
        }
    }
}