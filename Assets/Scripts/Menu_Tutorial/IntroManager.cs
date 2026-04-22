using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("Panelen")]
    public GameObject mainMenuPanel;
    public GameObject introPanel;

    [Header("Uitleg Stappen")]
    public GameObject[] textSteps; 
    public GameObject loadingStep; 

    [Header("Instellingen")]
    public string gameSceneName;
    public float forcedWaitTime = 10f; // De 10 seconden timer

    private int currentStep = 0;
    private bool isIntroActive = false;
    private bool loadingStarted = false;

    void Start()
    {
        if(mainMenuPanel) mainMenuPanel.SetActive(true);
        if(introPanel) introPanel.SetActive(false);
    }

    public void StartIntro()
    {
        mainMenuPanel.SetActive(false);
        introPanel.SetActive(true);
        currentStep = 0;
        isIntroActive = true;
        ShowStep(0);
    }

    void Update()
    {
        // Alleen reageren op Spatie voor de eerste stappen
        // Zodra het laden begint (bij de laatste stap), negeren we Spatie
        if (isIntroActive && !loadingStarted && Input.GetKeyDown(KeyCode.Space))
        {
            NextStep();
        }
    }

    void NextStep()
    {
        currentStep++;

        if (currentStep < textSteps.Length)
        {
            ShowStep(currentStep);
        }
    }

    void ShowStep(int stepIndex)
    {
        foreach (GameObject go in textSteps) go.SetActive(false);
        textSteps[stepIndex].SetActive(true);
        
        // Is dit de allerlaatste tekst stap?
        if (stepIndex == textSteps.Length - 1)
        {
            loadingStarted = true; // Stop het reageren op Spatie
            if(loadingStep) loadingStep.SetActive(true);
            
            // Start de timer en het laden tegelijk
            StartCoroutine(WaitAndLoad());
        }
    }

    IEnumerator WaitAndLoad()
    {
        Debug.Log("De 10 seconden timer is gestart...");

        // 1. Start het laden van de scene op de achtergrond
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        
        // Voorkom dat de scene direct opent als hij binnen die 10 sec klaar is
        asyncLoad.allowSceneActivation = false;

        // 2. Wacht exact 10 seconden
        // De GIF blijft bewegen omdat de Coroutine elke frame terugkeert naar Unity
        yield return new WaitForSeconds(forcedWaitTime);

        Debug.Log("10 seconden zijn voorbij. Scene wordt nu geactiveerd.");

        // 3. Laat de scene nu echt openen
        asyncLoad.allowSceneActivation = true;
    }

    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}