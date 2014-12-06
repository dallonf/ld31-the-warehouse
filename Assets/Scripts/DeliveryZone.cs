using UnityEngine;
using System.Collections;

public class DeliveryZone : MonoBehaviour
{
    public void OnTriggerEnter2D(Collider2D other)
    {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player && player.IsCarryingBox)
        {
            player.IsCarryingBox = false;
        }
    }
}
