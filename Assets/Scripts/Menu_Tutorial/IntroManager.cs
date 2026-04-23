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

    [Header("Audio")]
    public AudioSource menuMusic;  // Sleep hier de AudioSource van het Hoofdmenu in
    public AudioSource introMusic; // Sleep hier de AudioSource van de Intro in

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
        // Bij de start: Panels goedzetten
        if(mainMenuPanel) mainMenuPanel.SetActive(true);
        if(introPanel) introPanel.SetActive(false);

        // Start de menu muziek direct (zorg dat Loop aan staat op de AudioSource)
        if (menuMusic != null) 
        {
            menuMusic.Play();
        }
    }

    public void StartIntro()
    {
        // WISSEL MUZIEK: Stop menu, start intro
        if (menuMusic != null) menuMusic.Stop();
        if (introMusic != null) introMusic.Play();

        mainMenuPanel.SetActive(false);
        introPanel.SetActive(true);
        currentStep = 0;
        isIntroActive = true;
        ShowStep(0);
    }

    void Update()
    {
        // Alleen reageren op Spatie voor de eerste stappen
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
            loadingStarted = true; 
            if(loadingStep) loadingStep.SetActive(true);
            
            StartCoroutine(WaitAndLoad());
        }
    }

    IEnumerator WaitAndLoad()
    {
        Debug.Log("De 10 seconden timer is gestart...");

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(gameSceneName);
        asyncLoad.allowSceneActivation = false;

        yield return new WaitForSeconds(forcedWaitTime);

        Debug.Log("10 seconden zijn voorbij. Scene wordt nu geactiveerd.");
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