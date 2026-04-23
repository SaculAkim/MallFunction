using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHelper : MonoBehaviour
{
    [Header("Scene Instellingen")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Audio Instellingen")]
    public AudioSource sceneMusic; // Sleep hier de AudioSource in die moet loopen
    public bool playOnStart = true;

    void Start()
    {
        // Als je wilt dat de muziek direct begint zodra de scene laadt
        if (playOnStart && sceneMusic != null)
        {
            sceneMusic.loop = true; // Forceer loop op true
            sceneMusic.Play();
        }
    }

    // Functie om naar het menu te gaan
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // Functie om de game af te sluiten
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Extra: Functie om muziek handmatig te stoppen via een knop
    public void StopMusic()
    {
        if (sceneMusic != null) sceneMusic.Stop();
    }
}