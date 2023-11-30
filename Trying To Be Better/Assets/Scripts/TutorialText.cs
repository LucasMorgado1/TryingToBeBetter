using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialText : MonoBehaviour
{
    private bool fadeIsComplete = true;
    private TextMeshProUGUI _text;

    // Start is called before the first frame update
    void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && fadeIsComplete)
        {
            StartCoroutine(FadeIn());
            
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && fadeIsComplete)
        {
            StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeIn ()
    {
        if (_text.color.a >= 0.9f)
        {
            yield break;
        }

        fadeIsComplete = false;

        for (float i = 0; i <= 1f; i += Time.deltaTime)
        {
            _text.color = new Color(1, 1, 1, i);
            yield return null;
        } 

        if (_text.color.a >= 0.9)
        {
            _text.color = new Color(1,1,1,1);
        }

        Debug.Log(_text.color.a);
        fadeIsComplete = true;
    }

    IEnumerator FadeOut ()
    {
        if (_text.color.a <= 0.1f)
        {
            yield break; 
        }

        fadeIsComplete = false;

        for (float i = 1; i >= 0f; i -= Time.deltaTime)
        {
            _text.color = new Color(1, 1, 1, i);
            yield return null;
        }

        fadeIsComplete = true;
    }
}
