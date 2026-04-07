using UnityEngine;

public class AnomalyManager : MonoBehaviour
{
    public Transform playerTransform;    
    public float anomalyChance = 0.5f; 
    public GameObject[] allAnomalies; 

    [Header("Status")]
    public int currentStreak = 0;
    public bool isAnomalyActive = false;
    
    private int startTriggerID = 0; 

    void Start() {
        startTriggerID = 0; 
        ResetRoom();
        DetermineNextAnomaly();
    }

    public void PlayerDecided(int touchedTriggerID, Transform nextSpawn)
    {
        bool hasWalkedToOtherSide = (touchedTriggerID != startTriggerID);
        bool correct = false;

        if (isAnomalyActive) 
        {
            // Er IS een anomalie -> Je MOET doorlopen naar de overkant (+1)
            if (hasWalkedToOtherSide) correct = true;
        } 
        else 
        {
            // Er is GEEN anomalie -> Je MOET teruggaan (+1)
            if (!hasWalkedToOtherSide) correct = true;
        }

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

        TeleportPlayer(nextSpawn);
        ResetRoom();
        DetermineNextAnomaly();
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
        // NIEUWE LOGICA: Als de streak 0 is, mag er NOOIT een anomalie zijn.
        if (currentStreak == 0)
        {
            isAnomalyActive = false;
            Debug.Log("<color=cyan>Eerste ronde/Reset:</color> Geforceerd GEEN anomalie.");
            return; // We stoppen hier, dus de rest van de kansberekening wordt overgeslagen.
        }

        // Normale kansberekening voor streaks hoger dan 0
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