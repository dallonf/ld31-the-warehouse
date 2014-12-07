using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour
{
    public AudioSource LightsHumming;
    public AudioSource Effect;

    public AudioClip LightsOut;
    public AudioClip LightsOn;
    public AudioClip GameOver;

    private bool lightsOnLastFrame;

    void Start()
    {
        lightsOnLastFrame = GameController.Instance.LightsOn;
    }

    // Update is called once per frame
    void Update()
    {
        bool lightsOn = GameController.Instance.LightsOn;
        if (lightsOn)
        {
            LightsHumming.enabled = true;
            if (!lightsOnLastFrame)
            {
                // Lights On
            }
        }
        else
        {
            LightsHumming.enabled = false;
            if (lightsOnLastFrame)
            {
                Effect.PlayOneShot(LightsOut);
            }
        }

        lightsOnLastFrame = lightsOn;
    }

    public void OnGameOver()
    {
        Effect.PlayOneShot(GameOver);
    }
}
