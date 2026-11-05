using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [Header("Configuración de Munición")]
    public AmmoType ammoType;
    public int ammoAmount = 30;

    [Header("Audio")]
    [Tooltip("Sonido que se reproduce al recoger la munición")]
    public AudioClip pickupSound;

    [Header("Efecto Visual (Opcional)")]
    public float rotationSpeed = 50f;
    public float floatSpeed = 2f;
    public float floatAmount = 0.25f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        PlayerAmmoInventory playerInventory = other.GetComponent<PlayerAmmoInventory>();

        if (playerInventory == null)
        {
            playerInventory = other.GetComponentInParent<PlayerAmmoInventory>();
        }

        if (playerInventory != null)
        {
            // Intenta añadir la munición y nos dice cuántas unidades se añadieron realmente
            int amountAdded = playerInventory.AddAmmo(ammoType, ammoAmount);

            if (amountAdded > 0)
            {
                // Reproducir sonido de recogida si está asignado
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                Debug.Log($"¡Recogiste {amountAdded} unidades de munición para {ammoType}!");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log($"Reserva de {ammoType} está al máximo ({playerInventory.GetMaxReserveAmmo(ammoType)}). Caja no recogida.");
            }
        }
    }
}