using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI; // Belangrijk voor UI interactie
using System.Collections;

public class LevelTransitie : MonoBehaviour
{
    [Header("Referenties")]
    public GameObject spelerObject; // Sleep hier je Player (bijv. FirstPersonController) naartoe
    public CanvasGroup fadeGroup;   // Sleep hier de 'FadeImage' naartoe (die de tekst als 'child' heeft)
    
    [Header("Instellingen")]
    public string volgendeSceneNaam; // De exacte naam van je volgende level
    public float fadeSnelheid = 0.8f; // Hoe snel het zwart wordt (lager = langzamer)

    private bool isAanHetFaden = false;

    private void Start()
    {
        // Zorg dat het scherm bij de start van de game ALTIJD doorzichtig is
        if (fadeGroup != null)
        {
            fadeGroup.alpha = 0;
            fadeGroup.blocksRaycasts = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // We controleren of het object dat de kubus aanraakt exact jouw spelerObject is
        if (other.gameObject == spelerObject && !isAanHetFaden)
        {
            StartCoroutine(FadeEnLaad());
        }
    }

    IEnumerator FadeEnLaad()
    {
        isAanHetFaden = true;
        
        // Zorg dat de UI eventuele muisklikken blokkeert tijdens het faden
        fadeGroup.blocksRaycasts = true;

        // De "Fade-out" loop: verhoogt de alpha van 0 naar 1
        while (fadeGroup.alpha < 1)
        {
            fadeGroup.alpha += Time.deltaTime * fadeSnelheid;
            yield return null;
        }

        // Een korte pauze van 1 seconde zodat de speler de tekst even kan lezen
        yield return new WaitForSeconds(1f);

        // Wissel naar de volgende scene
        SceneManager.LoadScene(volgendeSceneNaam);
    }
}