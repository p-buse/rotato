using UnityEngine;
using System.Collections;

public class WinAndLoseText : MonoBehaviour {

    TypogenicText levelComplete;
    TypogenicText loseMessage;

    void Start()
    {
        levelComplete = transform.Find("levelComplete").GetComponent<TypogenicText>();
        loseMessage = transform.Find("loseMessage").GetComponent<TypogenicText>();

        float vertExtent = Camera.main.orthographicSize * 2;
        float horzExtent = (vertExtent * Screen.width / Screen.height);
        
        levelComplete.Tracking = horzExtent;
        levelComplete.WordWrap = horzExtent;
        loseMessage.Tracking = horzExtent;
        loseMessage.WordWrap = horzExtent;

        levelComplete.enabled = false;
        loseMessage.enabled = false;
    }

    public void WinLevel()
    {
        levelComplete.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.75f, 1f));
        levelComplete.enabled = true;
        StartCoroutine(EaseText(levelComplete, 1f));
    }

    private IEnumerator EaseText(TypogenicText text, float seconds)
    {
        float t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / seconds;
            text.Tracking = Mathf.SmoothStep(text.Tracking, 0f, t);
            yield return null;
        }
    }

    public void LoseLevel(string loseText)
    {
        loseMessage.Text = loseText;
        loseMessage.transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.75f, 1f));
        loseMessage.enabled = true;
        StartCoroutine(EaseText(loseMessage, 1f));
    }
}
