using UnityEngine;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour
{
    public float fadeSpeed = 1.5f;          // Speed that the screen fades to and from black.
    private bool fadingIn = true;


    void Update()
    {
        if (fadingIn)
        {
            FadeToClear();
            if (GetComponent<GUITexture>().color.a <= 0.01f)
                fadingIn = false;
        }
        else
        {
            FadeToBlack();
            if (GetComponent<GUITexture>().color.a >= 0.9f)
            {
                Application.LoadLevel(Application.loadedLevel + 1);
            }
        }
    }


    void FadeToClear()
    {
        // Lerp the colour of the texture between itself and transparent.
        GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.clear, fadeSpeed * Time.deltaTime);
    }

    void FadeToBlack()
    {
        // Lerp the colour of the texture between itself and black.
        GetComponent<GUITexture>().color = Color.Lerp(GetComponent<GUITexture>().color, Color.black, fadeSpeed * Time.deltaTime);
    }

}