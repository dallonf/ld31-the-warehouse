using UnityEngine;
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

    private SoundController soundController;

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
    }

    public void OnBoxPickedUp()
    {
        if (CurrentState == GameState.Tutorial && BoxesDelivered == 0)
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

    private IEnumerator ShowFirstNinja()
    {
        yield return new WaitForSeconds(1.5f); // Let the process process that they just picked something up
        LightsOn = false;
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        Ninjas[0].SetActive(true);
        yield return new WaitForSeconds(0.1f);
        LightsOn = true;
    }

    private IEnumerator FlickeringLights()
    {
        yield return new WaitForEndOfFrame();
        while (CurrentState == GameState.Flickering)
        {
            LightsOn = true;
            var offTime = Random.Range(FlickeringConfig.MinLightsOnTime, FlickeringConfig.MaxLightsOnTime);
            yield return new WaitForSeconds(offTime);
            if (CurrentState != GameState.Flickering) break;
            LightsOn = false;
            yield return new WaitForSeconds(FlickeringConfig.LightsOutTime);
        }
    }

    public void GameOver()
    {
        GoToState(GameState.Dead);
    }
}
