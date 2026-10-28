using UnityEngine;

public class WeaponAim : MonoBehaviour
{
    [Header("Posición de la Cámara al Apuntar")]
    [Tooltip("Offset (X: der/izq, Y: arriba/abajo, Z: adelante/atrás) relativo al pivote.")]
    public Vector3 aimOffset = new Vector3(0.5f, 0.0f, -1.2f);

    [Header("Altura del Pivote al Apuntar")]
    [Tooltip("A qué altura del personaje se ubica la mira (ej: 0.6m para Torso en Ametralladora, 1.6m para Ojos/1ra Persona en Francotirador).")]
    public float aimTargetHeightOffset = 0.6f;

    [Header("Zoom y Sensibilidad")]
    [Tooltip("Campo de visión (FOV). Menor valor = Más zoom (ej: 15f Francotirador, 45f Ametralladora).")]
    public float aimFOV = 45f;

    [Tooltip("Multiplicador de velocidad de ratón al apuntar.")]
    [Range(0.1f, 1f)]
    public float aimSensitivityMultiplier = 0.5f;

    [Header("Ralentización de Movimiento al Apuntar")]
    [Tooltip("Multiplicador de velocidad de movimiento del personaje al apuntar (ej: 0.5 = 50% de velocidad normal).")]
    [Range(0.05f, 1f)]
    public float aimMovementSpeedMultiplier = 0.5f;
}