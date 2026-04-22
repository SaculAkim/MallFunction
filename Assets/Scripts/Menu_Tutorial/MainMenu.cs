using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Instellingen")]
    [Tooltip("Typ hier de exacte naam van de scene waar je heen wilt")]
    public string sceneNaam;

    public void PlayGame()
    {
        if (!string.IsNullOrEmpty(sceneNaam))
        {
            SceneManager.LoadScene(sceneNaam);
        }
        else
        {
            Debug.LogError("Oeps! Je bent vergeten de Scene Naam in te vullen op het Canvas object.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Het spel sluit nu af...");
        
        // Dit zorgt ervoor dat het afsluiten werkt in de uiteindelijke game
        Application.Quit();

        // Dit zorgt ervoor dat je in de Unity Editor ziet dat het werkt
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}