using UnityEngine;
using System.Collections;

public class AnomalyManager : MonoBehaviour
{
    public Transform playerTransform;    
    public float anomalyChance = 0.5f; 
    public GameObject[] allAnomalies; 

    [Header("Status")]
    public int currentStreak = 0;
    public bool isAnomalyActive = false;

    [Header("Fade Instellingen")]
    public ScreenFader fader; 
    
    private int startTriggerID = 0; 

    void Start() {
        startTriggerID = 0; 
        ResetRoom();
        DetermineNextAnomaly();
    }

    public void PlayerDecided(int touchedTriggerID, Transform nextSpawn)
    {
        StartCoroutine(HandleDecision(touchedTriggerID, nextSpawn));
    }

    private IEnumerator HandleDecision(int touchedTriggerID, Transform nextSpawn)
    {
        if (fader != null) yield return StartCoroutine(fader.FadeOut());

        bool hasWalkedToOtherSide = (touchedTriggerID != startTriggerID);
        bool correct = false;

        // --- NIEUWE LOGICA VOOR LEVEL 0 ---
        if (currentStreak == 0)
        {
            // In de eerste ronde (of na reset) is elke keuze goed
            correct = true;
            Debug.Log("<color=cyan>Level 0:</color> Keuze maakt niet uit, je gaat door.");
        }
        else if (isAnomalyActive) 
        {
            // Er IS een anomalie -> Je MOET TERUGGAAN (Zelfde kant blijven)
            if (!hasWalkedToOtherSide) correct = true;
        } 
        else 
        {
            // Er is GEEN anomalie -> Je MOET DOORLOPEN (Naar de overkant)
            if (hasWalkedToOtherSide) correct = true;
        }
        // ----------------------------------

        if (correct) 
        {
            currentStreak++;
            Debug.Log("<color=green>GOED!</color> Streak: " + currentStreak);
        } 
        else 
        {
            currentStreak = 0;
            Debug.Log("<color=red>FOUT!</color> Streak gereset naar 0.");
        }

        startTriggerID = touchedTriggerID;

        // Teleporteer speler via CharacterController
        TeleportPlayer(nextSpawn);
        ResetRoom();
        DetermineNextAnomaly();

        yield return new WaitForSeconds(0.2f);

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
        // Bij streak 0 forceren we dat er geen anomalie is zodat de speler een referentiepunt heeft
        if (currentStreak == 0)
        {
            isAnomalyActive = false;
            Debug.Log("<color=cyan>Eerste ronde/Reset:</color> Geforceerd GEEN anomalie.");
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
            Debug.Log("Anomalie geactiveerd!");
        } else {
            isAnomalyActive = false;
            Debug.Log("Geen anomalie.");
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