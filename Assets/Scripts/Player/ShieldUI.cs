using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShieldUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Slider shieldSlider;          // La barra de escudo (Slider)
    public TextMeshProUGUI shieldText;   // El texto de la cifra del escudo (TMP)

    [Header("Referencia del Jugador")]
    public PlayerShield playerShield;    // Script PlayerShield del personaje

    [Header("Efecto Visual")]
    public float fillSpeed = 5f;         // Velocidad de suavizado de la barra

    void Start()
    {
        if (playerShield != null && shieldSlider != null)
        {
            // Configurar rangos del Slider de escudo
            shieldSlider.minValue = 0f;
            shieldSlider.maxValue = playerShield.maxShield;
            shieldSlider.value = playerShield.currentShield;
        }
    }

    void Update()
    {
        if (playerShield == null) return;

        // Transición suave del valor de la barra de escudo
        if (shieldSlider != null)
        {
            shieldSlider.value = Mathf.Lerp(shieldSlider.value, playerShield.currentShield, Time.deltaTime * fillSpeed);
        }

        // Actualizar cifra de escudo en pantalla
        if (shieldText != null)
        {
            int currentS = Mathf.CeilToInt(playerShield.currentShield);
            int maxS = Mathf.CeilToInt(playerShield.maxShield);

            shieldText.text = $"{currentS} / {maxS}";
        }
    }
}