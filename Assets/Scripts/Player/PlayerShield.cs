using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Configuración de Escudo")]
    public float maxShield = 100f;
    public float currentShield;

    [Header("Mitigación de Daño Base")]
    [Range(0f, 1f)]
    public float shieldMitigation = 0.70f; // 70% mitigado por el escudo por defecto

    [Header("Referencias")]
    public PlayerHealth playerHealth;

    void Start()
    {
        currentShield = maxShield;

        if (playerHealth == null)
        {
            playerHealth = GetComponent<PlayerHealth>();
        }
    }

    public void TakeDamage(float amount)
    {
        if (amount <= 0f) return;
        if (playerHealth != null && playerHealth.isDead) return;

        float incomingDamage = amount;

        // --- PENALIZACIONES DE DAÑO RECIBIDO POR PASIVAS ---
        if (ShopManager.Instance != null)
        {
            var passive = ShopManager.Instance.currentPassive;

            if (passive == PassiveType.Frenzy && ShopManager.Instance.frenzyActive)
            {
                incomingDamage *= ShopManager.Instance.frenzyIncomingDamageMultiplier; // Daño mayor en Frenesí
            }
            else if (passive == PassiveType.Greed)
            {
                incomingDamage *= ShopManager.Instance.greedIncomingDamageMultiplier; // Daño mayor en Codicia
            }
        }

        // --- PASIVA 2: TANQUE ---
        if (ShopManager.Instance != null && ShopManager.Instance.currentPassive == PassiveType.Tank)
        {
            if (currentShield > 0f)
            {
                // El daño se dirige 100% al escudo y se reduce en un 80% (entra solo el 20%)
                float damageToShield = incomingDamage * 0.20f;
                currentShield -= damageToShield;

                if (currentShield <= 0f)
                {
                    currentShield = 0f;
                    Debug.Log("¡El escudo del Tanque se ha roto!");
                }

                // La salud permanece 100% intacta mientras exista escudo
                return;
            }
            else
            {
                // Con escudo roto (0), el daño pasa a la salud pero reducido un 70% (entra el 30%)
                float damageToHealth = incomingDamage * 0.30f;
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageToHealth);
                }
                return;
            }
        }

        // --- COMPORTAMIENTO NORMAL (SIN PASIVA TANQUE) ---
        if (currentShield > 0f)
        {
            float damageToShield = incomingDamage * shieldMitigation;
            float damageToHealth = incomingDamage * (1f - shieldMitigation);

            currentShield -= damageToShield;

            if (currentShield <= 0f)
            {
                float excessDamage = -currentShield;
                currentShield = 0f;

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageToHealth + excessDamage);
                }
            }
            else
            {
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damageToHealth);
                }
            }
        }
        else
        {
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(incomingDamage);
            }
        }
    }

    public void RechargeShield(float amount)
    {
        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, 0f, maxShield);
    }
}