using UnityEngine;
using UnityEngine.InputSystem;

public class RocketLauncher : MonoBehaviour
{
    [Header("Permisos Locales")]
    public bool canShoot = true;

    [Header("Referencias")]
    public Transform firePoint;
    public Camera mainCamera;
    public GameObject rocketPrefab;
    public PlayerHealth playerHealth;

    [Header("Ajustes de Disparo")]
    public float fireRate = 0.8f;
    public float maxAimDistance = 100f;
    public LayerMask shootableMask;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip shootSound;
    public AudioClip emptyClickSound;
    [Range(0f, 1f)] public float shootVolume = 1f;
    [Range(0f, 1f)] public float emptyClickVolume = 1f;

    private float nextFireTime = 0f;
    private WeaponAmmo weaponAmmo;
    private PlayerInputActions inputActions;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
    }

    private void OnEnable() => inputActions.Enable();
    private void OnDisable() => inputActions.Disable();

    void Start()
    {
        weaponAmmo = GetComponent<WeaponAmmo>();

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (playerHealth == null)
        {
            playerHealth = GetComponentInParent<PlayerHealth>();
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // 🔒 BLOQUEO: Si la tienda está abierta, cancela el disparo inmediatamente
        if ((ShopManager.Instance != null && ShopManager.Instance.IsShopOpen) ||
            !canShoot || (playerHealth != null && playerHealth.isDead) || GameManager.IsPaused)
        {
            return;
        }

        bool isTriggerPressed = inputActions.Player.Fire.IsPressed();
        bool isTriggerPressedThisFrame = inputActions.Player.Fire.WasPressedThisFrame();

        if (isTriggerPressed && Time.time >= nextFireTime)
        {
            if (weaponAmmo != null && !weaponAmmo.CanShoot())
            {
                if (isTriggerPressedThisFrame && audioSource != null && emptyClickSound != null)
                {
                    audioSource.PlayOneShot(emptyClickSound, emptyClickVolume);
                }

                if (weaponAmmo.currentAmmo == 0) weaponAmmo.TryReload();
                return;
            }

            nextFireTime = Time.time + fireRate;

            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound, shootVolume);
            }

            ShootRocket();

            if (weaponAmmo != null) weaponAmmo.ConsumeBullet();
        }
    }

    void ShootRocket()
    {
        if (mainCamera == null || firePoint == null) return;

        Ray cameraRay = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        Vector3 targetPoint;

        if (Physics.Raycast(cameraRay, out RaycastHit cameraHit, maxAimDistance, shootableMask))
        {
            targetPoint = cameraHit.point;
        }
        else
        {
            targetPoint = cameraRay.GetPoint(maxAimDistance);
        }

        Vector3 launchDirection = (targetPoint - firePoint.position).normalized;
        Quaternion launchRotation = Quaternion.LookRotation(launchDirection);
        Instantiate(rocketPrefab, firePoint.position, launchRotation);
    }
}