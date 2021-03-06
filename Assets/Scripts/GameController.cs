﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public enum GameState
{
    TitleScreen,
    Tutorial,
    Flickering,
    Switches,
    Dead,
    Victory
}

[System.Serializable]
public class FlickeringConfig
{
    public float LightsOutTime = 0.1f;
    public float MinLightsOnTime = 0.5f;
    public float MaxLightsOnTime = 1.2f;
}

public class GameController : MonoBehaviour
{
    [Header("Configuration")]
    public BoxController[] BoxSequence;
    public GameObject[] Ninjas;
    public FlickeringConfig FlickeringConfig;

    [Header("References")]
    public PlayerController Player;
    public GameObject TitleScreen;
    public GameObject VictoryScreen;
    public GameObject DeathScreen;
    public Light Light;
    
    [Header("Dynamic")]
    public GameState CurrentState;
    public BoxController CurrentBox;
    public int BoxesDelivered;
    public bool LightsOn = true;
    public int NinjasActive = 0;

    private SoundController soundController;

    private GameState lastStateBeforeGameOver;

    public bool IsGameplay 
    { 
        get 
        {
            return CurrentState != GameState.TitleScreen && CurrentState != GameState.Dead && CurrentState != GameState.Victory;
        }
    } 

    public bool AreNinjasAttacking 
    { 
        get 
        {
            return CurrentState == GameState.Flickering || CurrentState == GameState.Switches;
        }
    } 


    public static GameController Instance {get; private set;}

    public void Awake()
    {
        Instance = this;
        UninitializeState(GameState.TitleScreen);
        UninitializeState(GameState.Tutorial);
        UninitializeState(GameState.Flickering);
        UninitializeState(GameState.Switches);
        UninitializeState(GameState.Dead);
        UninitializeState(GameState.Victory);
        foreach (var ninja in Ninjas)
        {
            ninja.SetActive(false);
        }
        soundController = GetComponentInChildren<SoundController>();
        GoToState(CurrentState); // Might have to move this to Start
    }
    
    void Start()
    {
        CurrentBox = BoxSequence[0];
    }

    public void FixedUpdate()
    {
        Light.enabled = LightsOn;

        if (!LightsOn)
        {
            for (int i = 0; i < NinjasActive; i++)
            {
                Ninjas[i].SetActive(true);
            }
        }
    }

    private void UninitializeState(GameState lastState)
    {
        switch (lastState)
        {
            case GameState.TitleScreen:
                TitleScreen.SetActive(false);
                Player.gameObject.SetActive(true);
                break;
            case GameState.Tutorial:
                break;
            case GameState.Flickering:
                LightsOn = true;
                break;
            case GameState.Switches:
                break;
            case GameState.Dead:
                DeathScreen.SetActive(false);
                break;
            case GameState.Victory:
                VictoryScreen.SetActive(false);
                break;
        }
    }

    public void GoToState(GameState newState)
    {
        UninitializeState(CurrentState);

        switch (newState)
        {
            case GameState.TitleScreen:
                StartCoroutine(TitleScreenFlickeringLights());
                TitleScreen.SetActive(true);
                Player.gameObject.SetActive(false);
                break;
            case GameState.Tutorial:
                break;
            case GameState.Flickering:
                StartCoroutine(FlickeringLights());
                break;
            case GameState.Switches:
                break;
            case GameState.Dead:
                lastStateBeforeGameOver = CurrentState;
                soundController.OnGameOver();
                DeathScreen.SetActive(true);
                break;
            case GameState.Victory:
                VictoryScreen.SetActive(true);
                break;
        }
        CurrentState = newState;
    }

    public void OnBoxDelivered()
    {
        soundController.OnScore();
        BoxesDelivered += 1;
        if (BoxSequence.Length > BoxesDelivered)
        {
            CurrentBox = BoxSequence[BoxesDelivered];
        }
        else
        {
            GoToState(GameState.Victory);
            CurrentBox = null;
            return;
        }

        if (BoxesDelivered >= 1 && CurrentState == GameState.Tutorial)
        {
            GoToState(GameState.Flickering);
        }
        else if (BoxesDelivered >= 3 && NinjasActive < 2)
        {
            NinjasActive = 2;
        }
        else if (BoxesDelivered >= 6 && NinjasActive < 3)
        {
            NinjasActive = 3;
        }
    }

    public void OnBoxPickedUp()
    {
        soundController.OnBoxPickup();
        if (CurrentState == GameState.Tutorial && BoxesDelivered == 0 && NinjasActive < 1)
        {
            StartCoroutine(ShowFirstNinja());
        }
    }

    public void TitleScreenDone()
    {
        GoToState(GameState.Tutorial);
    }

    public void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    public void Respawn()
    {
        Player.Respawn();
        CurrentBox.gameObject.SetActive(true); // Respawn the box in case it got picked up
        foreach (var ninja in Ninjas)
        {
            if (ninja.activeInHierarchy)
            {
                ninja.SendMessage("Respawn");
            }
        }
        GoToState(lastStateBeforeGameOver);
    }

    private IEnumerator FlickerLights()
    {
        soundController.OnLightsFlicker();
        Light.intensity /= 2;
        yield return new WaitForSeconds(0.1f);
        Light.intensity *= 2;
    }

    private IEnumerator ShowFirstNinja()
    {
        yield return new WaitForSeconds(1.5f); // Let the player process that they just picked something up

        yield return StartCoroutine(FlickerLights());
        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(FlickerLights());
        yield return new WaitForSeconds(0.5f);

        LightsOn = false;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        NinjasActive += 1;
        yield return new WaitForSeconds(0.1f);
        LightsOn = true;
    }

    static int lightCoroutines = 0;
    private IEnumerator FlickeringLights()
    {
        lightCoroutines++;
        int thisCoroutineId = lightCoroutines;
        yield return new WaitForEndOfFrame();
        while (CurrentState == GameState.Flickering || lightCoroutines > thisCoroutineId)
        {
            LightsOn = true;
            var onTime = Random.Range(FlickeringConfig.MinLightsOnTime, FlickeringConfig.MaxLightsOnTime);

            // Sometimes, flicker the lights
            if (Random.value > 0.5f)
            { 
                var flickerTime = 0f;
                flickerTime = Random.Range(0, onTime);
                onTime -= flickerTime;
                yield return new WaitForSeconds(flickerTime);
                if (CurrentState != GameState.Flickering || lightCoroutines > thisCoroutineId) break;
                yield return StartCoroutine(FlickerLights());
                // Sometimes flicker them twice
                if (Random.value > 0.5f)
                {
                    yield return new WaitForSeconds(0.1f);
                    if (CurrentState != GameState.Flickering || lightCoroutines > thisCoroutineId) break;
                    yield return StartCoroutine(FlickerLights());
                }
            }

            yield return new WaitForSeconds(onTime);
            if (CurrentState != GameState.Flickering || lightCoroutines > thisCoroutineId) break;
            LightsOn = false;
            yield return new WaitForSeconds(FlickeringConfig.LightsOutTime);
        }
    }

    private IEnumerator TitleScreenFlickeringLights()
    {
        while(CurrentState == GameState.TitleScreen)
        {
            var onTime = Random.Range(FlickeringConfig.MinLightsOnTime, FlickeringConfig.MaxLightsOnTime);
            yield return new WaitForSeconds(onTime);
            if (CurrentState != GameState.TitleScreen) break;
            yield return StartCoroutine(FlickerLights());
            if (Random.value > 0.5f)
            {
                yield return new WaitForSeconds(0.1f);
                if (CurrentState != GameState.TitleScreen) break;
                yield return StartCoroutine(FlickerLights());
            }
        }
    }

    public void GameOver()
    {
        GoToState(GameState.Dead);
    }
}
