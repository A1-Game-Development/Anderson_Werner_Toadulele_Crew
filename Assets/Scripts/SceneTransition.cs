using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    private void Start()
    {
        // Start black, then fade in
        Color c = fadeImage.color;
        c.a = 1f;
        fadeImage.color = c;

        StartCoroutine(FadeIn());
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeAndLoad(sceneName));
    }

    IEnumerator FadeIn()
    {
        float time = 0f;
        Color color = fadeImage.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, time / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        // ensure fully transparent at end
        color.a = 0f;
        fadeImage.color = color;
    }

    IEnumerator FadeAndLoad(string sceneName)
    {
        float time = 0f;
        Color color = fadeImage.color;

        // Fade OUT (to black)
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, time / fadeDuration);
            fadeImage.color = color;
            yield return null;
        }

        SceneManager.LoadScene(sceneName);
    }
}