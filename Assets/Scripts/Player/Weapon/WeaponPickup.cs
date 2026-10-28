using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    [Header("Configuración del Pick Up")]
    [Tooltip("Índice del arma en el WeaponHolder (0 a 5)")]
    public int weaponIndex = 0;

    [Tooltip("Efecto de sonido opcional")]
    public AudioClip pickupSound;

    [Header("Efecto Visual")]
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
        // Rotación sobre su propio eje vertical
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Movimiento de flotación vertical (senoidal)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmount;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Buscar el WeaponSwitcher en el collider o en los padres
        WeaponSwitcher switcher = other.GetComponent<WeaponSwitcher>();
        if (switcher == null)
        {
            switcher = other.GetComponentInChildren<WeaponSwitcher>();
        }

        if (switcher != null)
        {
            // Desbloquea el arma y la equipa automáticamente
            switcher.UnlockWeapon(weaponIndex, autoEquip: true);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            Destroy(gameObject);
        }
    }
}