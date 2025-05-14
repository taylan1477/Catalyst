using UnityEngine;
using TMPro;
using System.Collections;

public class CatSpeech : MonoBehaviour
{
    public GameObject speechBubblePrefab;
    public Transform bubblePoint;

    public void Speak(string text, float duration = 3f, float charDelay = 0.05f)
    {
        GameObject bubble = Instantiate(speechBubblePrefab, bubblePoint.position, Quaternion.identity);
        bubble.transform.SetParent(bubblePoint);
        TMP_Text textComp = bubble.GetComponentInChildren<TMP_Text>();
        StartCoroutine(TypeText(textComp, text, duration, bubble, charDelay));
    }

    private IEnumerator TypeText(TMP_Text textComp, string fullText, float duration, GameObject bubble, float charDelay)
    {
        textComp.text = ""; // Temizle
        foreach (char c in fullText)
        {
            textComp.text += c;
            yield return new WaitForSeconds(charDelay);
        }

        yield return new WaitForSeconds(duration); // yazı bitince ekranda kalma süresi
        Destroy(bubble);
    }
}