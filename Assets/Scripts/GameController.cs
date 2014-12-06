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

public class GameController : MonoBehaviour
{
    [Header("Configuration")]
    public BoxController[] BoxSequence;

    [Header("References")]
    public PlayerController Player;
    public GameObject TitleScreen;
    public GameObject VictoryScreen;
    
    [Header("Dynamic")]
    public GameState CurrentState;
    public BoxController CurrentBox;
    public int BoxesDelivered;

    public bool IsGameplay 
    { 
        get 
        {
            return CurrentState != GameState.TitleScreen && CurrentState != GameState.Dead && CurrentState != GameState.Victory;
        }
    } 

    public static GameController Instance {get; private set;}

    public void Awake()
    {
        Instance = this;
        GoToState(CurrentState); // Might have to move this to Start
    }
    
    void Start()
    {
        CurrentBox = BoxSequence[0];
    }

    public void GoToState(GameState newState)
    {
        // Uninitialize current state
        switch (CurrentState)
        {
            case GameState.TitleScreen:
                TitleScreen.SetActive(false);
                Player.gameObject.SetActive(true);
                break;
            case GameState.Tutorial:
                break;
            case GameState.Flickering:
                break;
            case GameState.Switches:
                break;
            case GameState.Dead:
                break;
            case GameState.Victory:
                break;
        }

        switch (newState)
        {
            case GameState.TitleScreen:
                TitleScreen.SetActive(true);
                Player.gameObject.SetActive(false);
                break;
            case GameState.Tutorial:
                break;
            case GameState.Flickering:
                break;
            case GameState.Switches:
                break;
            case GameState.Dead:
                break;
            case GameState.Victory:
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
            CurrentState = GameState.Victory;
            CurrentBox = null;
        }
    }

    public void TitleScreenDone()
    {
        GoToState(GameState.Tutorial);
    }
}
