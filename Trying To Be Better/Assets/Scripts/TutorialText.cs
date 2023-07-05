using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour
{
    private bool activated = false;
    private CanvasGroup alpha;

    // Start is called before the first frame update
    void Awake()
    {
        alpha = GetComponent<CanvasGroup>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //LeanTween.alphaCanvas(alpha, 1f, 1.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //LeanTween.alphaCanvas(alpha, 0f, 1.5f);
        }
    }
}
