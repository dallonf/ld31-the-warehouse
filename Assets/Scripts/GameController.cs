using UnityEngine;
using System.Collections;

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
    
    [Header("Dynamic")]
    public GameState CurrentState;
    public BoxController CurrentBox;
    public int BoxesDelivered;

    public static GameController Instance {get; private set;}

    public void Awake()
    {
        Instance = this;
    }
    
    void Start()
    {
        CurrentBox = BoxSequence[0];
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
}
