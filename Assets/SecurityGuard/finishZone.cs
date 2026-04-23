using UnityEngine;

public class FinishZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Make sure your Player object has the Tag "Player"
        if (other.CompareTag("Player"))
        {
            if (gameStartEscape.instance != null)
            {
                gameStartEscape.instance.FinishRun();
            }
        }
    }
}