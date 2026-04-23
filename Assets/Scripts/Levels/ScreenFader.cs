using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public Image fadeImage;
    public TextMeshProUGUI statusText;
    public float fadeSpeed = 1.0f;

    [Header("Audio Settings")]
    public AudioSource fadeInSound;  // Geluid bij start van level
    public AudioSource fadeOutSound; // Geluid bij einde/transitie

    void Start()
    {
        // Start de game met een fade-in
        if (fadeImage != null) StartCoroutine(FadeIn());
    }

    public void SetStatusText(string text)
    {
        if (statusText != null)
        {
            statusText.text = text;
        }
    }

    public IEnumerator FadeOut()
    {
        // --- NIEUW: Speel Fade Out geluid ---
        if (fadeOutSound != null) fadeOutSound.Play();

        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }
    }

    public IEnumerator FadeIn()
    {
        // --- NIEUW: Speel Fade In geluid ---
        if (fadeInSound != null) fadeInSound.Play();

        float alpha = 1;
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            SetAlpha(alpha);
            yield return null;
        }
    }

    private void SetAlpha(float alpha)
    {
        float clampedAlpha = Mathf.Clamp01(alpha);

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = clampedAlpha;
            fadeImage.color = c;
        }

        if (statusText != null)
        {
            Color tc = statusText.color;
            tc.a = clampedAlpha;
            statusText.color = tc;
        }
    }
}