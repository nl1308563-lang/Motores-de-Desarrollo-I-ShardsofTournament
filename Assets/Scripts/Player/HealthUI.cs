using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthUI : MonoBehaviour
{
    [Header("Referencias UI")]
    public Slider healthSlider;          // La barra de vida (Slider)
    public TextMeshProUGUI healthText;   // El texto de la cifra (TMP)

    [Header("Referencia del Jugador")]
    public PlayerHealth playerHealth;    // Script PlayerHealth del personaje

    [Header("Efecto Visual")]
    public float fillSpeed = 5f;        // Velocidad de suavizado de la barra

    void Start()
    {
        if (playerHealth != null && healthSlider != null)
        {
            // Configurar rangos del Slider
            healthSlider.minValue = 0f;
            healthSlider.maxValue = playerHealth.maxHealth;
            healthSlider.value = playerHealth.currentHealth;
        }
    }

    void Update()
    {
        if (playerHealth == null) return;

        // Transición suave del valor de la barra de vida
        if (healthSlider != null)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, playerHealth.currentHealth, Time.deltaTime * fillSpeed);
        }

        // Actualizar cifra en pantalla
        if (healthText != null)
        {
            int currentHP = Mathf.CeilToInt(playerHealth.currentHealth);
            int maxHP = Mathf.CeilToInt(playerHealth.maxHealth);

            healthText.text = $"{currentHP} / {maxHP}";
        }
    }
}