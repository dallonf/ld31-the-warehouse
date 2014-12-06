﻿using UnityEngine;
using System.Collections;

public class BoxController : MonoBehaviour
{
    public bool IsTargetBox = true;

    public void OnCollisionEnter2D(Collision2D coll)
    {
        var player = coll.gameObject.GetComponent<PlayerController>();
        if (player && !player.IsCarryingBox)
        {
            player.IsCarryingBox = true;
            gameObject.SetActive(false);
        }
    }
}