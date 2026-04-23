using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AnomalyManager : MonoBehaviour
{
    public Transform playerTransform;    
    public float anomalyChance = 0.5f; 
    public GameObject[] allAnomalies; 

    [Header("Status")]
    public int currentStreak = 10; 
    public bool isAnomalyActive = false;
    public string volgendeSceneNaam = "MallStore2lvl0"; 

    [Header("Fade Instellingen")]
    public ScreenFader fader; 
    
    private int startTriggerID = 0; 

    void Start() {
        startTriggerID = 0; 
        currentStreak = 10; 
        ResetRoom();
        DetermineNextAnomaly();
        
        // Zet de tekst bij de start op 10 Left
        if (fader != null) fader.SetStatusText(currentStreak + " Left");
    }

    public void PlayerDecided(int touchedTriggerID, Transform nextSpawn)
    {
        StartCoroutine(HandleDecision(touchedTriggerID, nextSpawn));
    }

    private IEnumerator HandleDecision(int touchedTriggerID, Transform nextSpawn)
    {
        // 1. Fade naar zwart (tekst fadet mee in naar wit/zichtbaar)
        if (fader != null) yield return StartCoroutine(fader.FadeOut());

        bool hasWalkedToOtherSide = (touchedTriggerID != startTriggerID);
        bool correct = false;

        // Logica: Level 10 is altijd veilig/goed
        if (currentStreak == 10)
        {
            correct = true;
        }
        else if (isAnomalyActive) 
        {
            // Er is een anomalie: Je moet omdraaien (Zelfde kant blijven als spawn)
            if (!hasWalkedToOtherSide) correct = true;
        } 
        else 
        {
            // Geen anomalie: Je moet doorlopen naar de overkant
            if (hasWalkedToOtherSide) correct = true;
        }

        // Streak bijwerken
        if (correct) 
        {
            currentStreak--;
        } 
        else 
        {
            currentStreak = 10; // Reset naar het beginpunt
        }

        // Check of we de volgende scene moeten laden
        if (currentStreak <= 0)
        {
            SceneManager.LoadScene(volgendeSceneNaam);
            yield break;
        }

        // 2. Teleport en Reset (gebeurt terwijl alles 100% zwart is)
        startTriggerID = touchedTriggerID;
        TeleportPlayer(nextSpawn);
        ResetRoom();
        DetermineNextAnomaly();

        // 3. Tekst aanpassen naar het nieuwe getal terwijl het nog pikdonker is
        if (fader != null)
        {
            fader.SetStatusText(currentStreak + " Left");
        }

        // Korte pauze voor de sfeer
        yield return new WaitForSeconds(0.3f);

        // 4. Fade weer naar licht (tekst fadet mee uit naar onzichtbaar)
        if (fader != null) yield return StartCoroutine(fader.FadeIn());
    }

    private void TeleportPlayer(Transform target) {
        if (playerTransform == null || target == null) return;
        
        CharacterController cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;
        
        playerTransform.position = target.position;
        playerTransform.rotation = target.rotation;
        
        if (cc != null) cc.enabled = true;
    }

    private void DetermineNextAnomaly() {
        // Op Level 10 is er NOOIT een anomalie
        if (currentStreak == 10)
        {
            isAnomalyActive = false;
            return;
        }

        if (Random.value < anomalyChance && allAnomalies.Length > 0) {
            int index = Random.Range(0, allAnomalies.Length);
            GameObject group = allAnomalies[index];
            group.SetActive(true);
            
            Transform n = group.transform.Find("Normal");
            if (n != null) n.gameObject.SetActive(false);
            
            Transform a = group.transform.Find("Anomaly");
            if (a != null) a.gameObject.SetActive(true);
            
            isAnomalyActive = true;
        } else {
            isAnomalyActive = false;
        }
    }

    public void ResetRoom() {
        foreach (GameObject g in allAnomalies) {
            if (g == null) continue;
            Transform n = g.transform.Find("Normal");
            if (n != null) n.gameObject.SetActive(true);
            Transform a = g.transform.Find("Anomaly");
            if (a != null) a.gameObject.SetActive(false);
        }
    }
}