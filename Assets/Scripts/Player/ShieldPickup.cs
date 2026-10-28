using UnityEngine;

public class ShieldPickup : MonoBehaviour
{
    [Header("Configuración de Escudo")]
    public float shieldAmount = 25f; // Cantidad de escudo que recarga

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
        // Movimiento de rotación y flotación idéntico al botiquín de vida
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter(Collider other)
    {
        // Buscar el componente PlayerShield en el objeto, padres o hijos
        PlayerShield playerShield = other.GetComponent<PlayerShield>();
        if (playerShield == null)
        {
            playerShield = other.GetComponentInParent<PlayerShield>();
        }
        if (playerShield == null)
        {
            playerShield = other.GetComponentInChildren<PlayerShield>();
        }

        if (playerShield != null)
        {
            // Validar si al escudo le falta energía para recargarse
            if (playerShield.currentShield < playerShield.maxShield)
            {
                playerShield.RechargeShield(shieldAmount);
                Debug.Log($"¡Escudo recogido! Recargado: {shieldAmount} de escudo.");
                Destroy(gameObject); // Se consume el objeto
            }
            else
            {
                Debug.Log("El escudo del jugador ya está al máximo. Objeto no recogido.");
            }
        }
    }
}