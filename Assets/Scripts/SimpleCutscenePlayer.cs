using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimpleCutscenePlayer : MonoBehaviour
{
    [Tooltip("Gösterilecek görseller (sıralı olacak)")]
    public Sprite[] cutsceneImages;

    [Tooltip("Görsellerin fade süresi")]
    public float fadeDuration = 1f;

    [Tooltip("Görsellerin ekranda kalma süresi")]
    public float displayDuration = 2f;

    [Tooltip("Tüm görseller oynatıldıktan sonra nesneyi devre dışı bırak")]
    public bool disableAfterPlay = true;

    private Image _imageComponent;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        // Image ve CanvasGroup setup
        _imageComponent = GetComponent<Image>();
        _canvasGroup = GetComponent<CanvasGroup>();

        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        StartCoroutine(PlayCutscene());
    }

    private IEnumerator PlayCutscene()
    {
        foreach (var sprite in cutsceneImages)
        {
            _imageComponent.sprite = sprite;
            yield return StartCoroutine(Fade(0, 1)); // fade in
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(Fade(1, 0)); // fade out
        }

        if (disableAfterPlay)
            gameObject.SetActive(false);
    }

    private IEnumerator Fade(float from, float to)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
            yield return null;
        }
        _canvasGroup.alpha = to;
    }
}