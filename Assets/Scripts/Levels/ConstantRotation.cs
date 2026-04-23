using UnityEngine;

public class ConstantRotation : MonoBehaviour
{
    [Tooltip("Snelheid per as (graden per seconde)")]
    public Vector3 draaiSnelheid = new Vector3(0, 50, 0);

    [Tooltip("Draaien om eigen as (Self) of de wereld as (World)")]
    public Space rotatieRuimte = Space.Self;

    void Update()
    {
        // Time.deltaTime zorgt ervoor dat de rotatie soepel loopt, 
        // ongeacht hoe snel je computer is (frames per seconde).
        transform.Rotate(draaiSnelheid * Time.deltaTime, rotatieRuimte);
    }
}