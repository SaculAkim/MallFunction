using UnityEngine;
using System;

public class ClockTimer : MonoBehaviour
{
    [Header("Hand Transforms")]
    public Transform hoursHand;
    public Transform minutesHand;
    public Transform secondsHand;

    [Header("Settings")]
    public bool continuous = true;

    // We berekenen hoeveel graden elke wijzer per tijdseenheid beweegt
    private const float hoursToDegrees = 30f, minutesToDegrees = 6f, secondsToDegrees = 6f;

    void Update()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;

        if (continuous)
        {
            // Vloeiende beweging
            float hoursRotation = (float)time.TotalHours * hoursToDegrees;
            float minutesRotation = (float)time.TotalMinutes * minutesToDegrees;
            float secondsRotation = (float)time.TotalSeconds * secondsToDegrees;

            ApplyRotation(hoursRotation, minutesRotation, secondsRotation);
        }
        else
        {
            // Tikkende beweging (stap voor stap)
            float hoursRotation = (float)time.Hours * hoursToDegrees;
            float minutesRotation = (float)time.Minutes * minutesToDegrees;
            float secondsRotation = (float)time.Seconds * secondsToDegrees;

            ApplyRotation(hoursRotation, minutesRotation, secondsRotation);
        }
    }

    void ApplyRotation(float h, float m, float s)
    {
        // Let op: afhankelijk van hoe je model is geïmporteerd moet je 
        // mogelijk de 'Vector3.forward' aanpassen naar 'Vector3.right' of 'Vector3.up'
        hoursHand.localRotation = Quaternion.Euler(0f, 0f, -h);
        minutesHand.localRotation = Quaternion.Euler(0f, 0f, -m);
        secondsHand.localRotation = Quaternion.Euler(0f, 0f, -s);
    }
}