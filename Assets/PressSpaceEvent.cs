using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

public class PressSpaceEvent : MonoBehaviour
{

    [System.Serializable]
    public class SpacePressedEvent : UnityEvent { }

    public SpacePressedEvent OnSpacePressed;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnSpacePressed.Invoke();
        }
    }
}
