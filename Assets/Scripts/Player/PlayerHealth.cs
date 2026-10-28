using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Configuración de Salud")]
    public float maxHealth = 100f;
    public float currentHealth;

    [Header("Mitigación por Vida Baja")]
    [SerializeField] private float lowHealthThreshold = 25f; // Umbral de activación (25 puntos)
    [SerializeField] private float damageReductionMultiplier = 0.10f; // Multiplicador (10% del daño entra, es decir, se reduce un 90%)

    [Header("Estados y Permisos")]
    public bool isInvincible = false; // Interruptor de invencibilidad
    public bool isDead = false;

    [Header("Referencias")]
    public GameManager gameManager;

    void Start()
    {
        currentHealth = maxHealth;

        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
    }

    public void TakeDamage(float amount)
    {
        // Si el jugador está muerto o la invencibilidad está activa, ignorar el daño
        if (isDead || isInvincible) return;

        // Comprobamos si la salud actual es menor o igual al umbral (25 puntos)
        float finalDamage = amount;
        if (currentHealth <= lowHealthThreshold)
        {
            finalDamage = amount * damageReductionMultiplier;
            Debug.Log($"¡Activada la reducción de daño por vida baja! Daño original: {amount} -> Daño reducido (90% menos): {finalDamage}");
        }

        currentHealth -= finalDamage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Debug.Log($"Jugador recibió {finalDamage} de daño. Salud actual: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    private void Die()
    {
        isDead = true;
        Debug.Log("El jugador ha muerto.");

        if (gameManager != null)
        {
            gameManager.GameOver();
        }
    }
}