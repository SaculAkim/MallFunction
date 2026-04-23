using UnityEngine;
using UnityEngine.SceneManagement; // Nodig om tussen scenes te wisselen

public class MenuHelper : MonoBehaviour
{
    [Header("Instellingen")]
    public string mainMenuSceneName = "MainMenu"; // Typ hier de exacte naam van je menu-scene

    // Deze functie koppelen we aan de knop
    public void GoToMainMenu()
    {
        Debug.Log("Terugkeren naar het hoofdmenu...");
        
        // Zorg dat de tijd weer op normaal staat (mocht je de game gepauzeerd hebben)
        Time.timeScale = 1f;

        // Laad de menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // De Quit functie kun je eronder laten staan voor het geval je die ergens anders nodig hebt
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}