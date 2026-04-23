using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.UI;
using System.Collections;

public class gameStartEscape : MonoBehaviour
{
    public static gameStartEscape instance;

    // Static variable stays through scene reloads, but resets when the game is closed/stopped
    private static int sessionRestarts = 0;

    [Header("UI Settings - Start")]
    public GameObject startPanel;
    public TMP_Text restartCounter;

    [Header("UI Settings - Caught")]
    public GameObject caughtText;
    public Image fadeOverlay;

    [Header("UI Settings - Escape")]
    public GameObject finishText;
    public Image fadeImage2;

    [Header("Visual Effects")]
    public Volume blurVolume;  // Drag object "d" here
    public Volume colorVolume; // Drag object "CC" here

    [Header("Player Control")]
    public MonoBehaviour playerMovementScript; // Drag your player movement script here

    [Header("Audio Settings")]
    public AudioSource musicSource;        // The looping background music
    public AudioSource caughtSoundSource; // The one-shot sound when guard hits you

    [Header("Timing Settings")]
    [Tooltip("How long to wait on the black screen before the scene restarts")]
    public float restartDelay = 5f;

    private bool gameStarted = false;
    private bool isEnding = false;

    void Awake()
    {
        // Singleton setup
        instance = this;

        // Ensure time is normal when the script wakes up
        Time.timeScale = 1f;

        // Auto-assign volumes if they were forgotten in the Inspector
        if (blurVolume == null)
        {
            GameObject dObj = GameObject.Find("d");
            if (dObj != null) blurVolume = dObj.GetComponent<Volume>();
        }

        if (colorVolume == null)
        {
            GameObject ccObj = GameObject.Find("CC");
            if (ccObj != null) colorVolume = ccObj.GetComponent<Volume>();
        }
    }

    void Start()
    {
        // Start State: Pause game and show cursor
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Ensure audio is silent at the menu
        if (musicSource != null) musicSource.Stop();
        if (caughtSoundSource != null) caughtSoundSource.Stop();

        // UI Reset
        if (startPanel != null) startPanel.SetActive(true);
        if (caughtText != null) caughtText.SetActive(false);
        if (finishText != null) finishText.SetActive(false);

        // Hide black overlays (set alpha to 0)
        if (fadeOverlay != null) { Color c = fadeOverlay.color; c.a = 0f; fadeOverlay.color = c; }
        if (fadeImage2 != null) { Color c = fadeImage2.color; c.a = 0f; fadeImage2.color = c; }

        UpdateRestartUI();

        // Menu Visuals: Blur is ON
        if (blurVolume != null) { blurVolume.enabled = true; blurVolume.weight = 1f; }
        if (colorVolume != null) { colorVolume.enabled = false; }
    }

    void Update()
    {
        // Press Space to start the game
        if (!gameStarted && Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        gameStarted = true;

        // Hide and lock cursor for gameplay
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // Start looping music
        if (musicSource != null)
        {
            musicSource.loop = true;
            musicSource.Play();
        }

        if (startPanel != null) startPanel.SetActive(false);

        // Gameplay Visuals: Blur OFF, Color Correction ON
        if (blurVolume != null) blurVolume.enabled = false;
        if (colorVolume != null) colorVolume.enabled = true;

        Time.timeScale = 1f;
    }

    void UpdateRestartUI()
    {
        if (restartCounter != null)
            restartCounter.text = $"Restarts: {sessionRestarts}";
    }

    // --- CALLED BY THE GUARD ---
    public void RestartRun()
    {
        if (isEnding) return;
        isEnding = true;

        // Audio logic
        if (musicSource != null) musicSource.Stop();
        if (caughtSoundSource != null) caughtSoundSource.Play();

        // Count this restart for the current session
        sessionRestarts++;

        ShowEndSequence(caughtText, fadeOverlay);
    }

    // --- CALLED BY finishRun CUBE ---
    public void FinishRun()
    {
        if (isEnding) return;
        isEnding = true;

        // Stop music for the win screen
        if (musicSource != null) musicSource.Stop();

        ShowEndSequence(finishText, fadeImage2);
    }

    private void ShowEndSequence(GameObject textObj, Image fadeImg)
    {
        // Stop the player from moving
        if (playerMovementScript != null) playerMovementScript.enabled = false;

        // Show the mouse again
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Visuals: Snap black screen to 92% alpha
        if (fadeImg != null)
        {
            Color c = fadeImg.color;
            c.a = 0.92f;
            fadeImg.color = c;
        }

        // Show the specific message
        if (textObj != null) textObj.SetActive(true);

        // Turn Blur back on
        if (blurVolume != null)
        {
            blurVolume.enabled = true;
            blurVolume.priority = 99;
            blurVolume.weight = 1f;
        }

        StartCoroutine(WaitAndReload());
    }

    private IEnumerator WaitAndReload()
    {
        // Uses the restartDelay variable from the inspector
        yield return new WaitForSecondsRealtime(restartDelay);

        instance = null;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}