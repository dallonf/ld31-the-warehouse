using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DeliveryZoneText : MonoBehaviour
{
    private Text text;

    public void Awake()
    {
        text = GetComponent<Text>();
    }

    public void Start()
    {
        UpdateText();
    }

    public void FixedUpdate()
    {
        UpdateText();
    }

    public void UpdateText()
    {
        text.text = string.Format("BOXES\n{0}/{1}", GameController.Instance.BoxesDelivered, GameController.Instance.BoxSequence.Length);
    }
}
