using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SplashScreenFade : MonoBehaviour
{
    [Header("Canvas/Logo")]
    [SerializeField] private Image logoImage;

    [Header("Settings")]
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float showDuration = 2f;
    [SerializeField] private string nextSceneName = "MainMenu";

    private void Start()
    {
        logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, 0f);
        StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(t / fadeDuration);
            logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, alpha);
            yield return null;
        }

        yield return new WaitForSeconds(showDuration);

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Clamp01(1f - t / fadeDuration);
            logoImage.color = new Color(logoImage.color.r, logoImage.color.g, logoImage.color.b, alpha);
            yield return null;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}
