using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class ScaleBoxColliderToRectTransform : MonoBehaviour
{
    private bool isEditor;
    private RectTransform rectTransform;
    private BoxCollider2D boxCollider;

    public void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        boxCollider = GetComponent<BoxCollider2D>();

        isEditor = !Application.isPlaying;
        Scale();
        if (!isEditor)
        {
            Destroy(this);
        }
    }

    public void Update()
    {
        Scale();
    }

    public void Scale()
    {
        var size = rectTransform.sizeDelta;
        boxCollider.size = size;
    }
}
