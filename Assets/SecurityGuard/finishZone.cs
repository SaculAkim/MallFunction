using UnityEngine;
using UnityEngine.SceneManagement; // Nodig om naar de volgende scene te gaan
using System.Collections;

public class FinishZone : MonoBehaviour
{
    [Header("Instellingen")]
    public string endSceneName = "EndScene"; // De naam van de scene waar je naartoe wilt
    public float waitTime = 10f;             // Hoe lang de tekst in beeld blijft

    private bool hasFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        // Controleer of de speler de zone binnenloopt en of we niet al gefinisht zijn
        if (other.CompareTag("Player") && !hasFinished)
        {
            hasFinished = true; // Zorg dat de timer maar één keer start
            
            // 1. Roep de tekst aan in gameStartEscape (zoals je oude script deed)
            if (gameStartEscape.instance != null)
            {
                gameStartEscape.instance.FinishRun();
            }

            // 2. Start de timer van 10 seconden in dit script
            StartCoroutine(WaitAndLoadScene());
        }
    }

    IEnumerator WaitAndLoadScene()
    {
        Debug.Log("Finish bereikt! Wachten op scene wissel...");

        // Wacht de ingestelde tijd (10 seconden)
        yield return new WaitForSeconds(waitTime);

        Debug.Log("Laden van scene: " + endSceneName);

        // Laad de nieuwe scene
        SceneManager.LoadScene(endSceneName);
    }
}