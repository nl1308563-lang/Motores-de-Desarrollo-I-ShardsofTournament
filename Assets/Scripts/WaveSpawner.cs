
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private GameManager gameManager;

    [Header("Configuración de Olas y Spawns")]
    [SerializeField] private List<WaveData> waves;
    [SerializeField] private Transform[] spawnPoints;

    [Header("Configuración de la Tienda")]
    [SerializeField] private GameObject shopPodPrefab;
    [SerializeField] private Transform[] shopSpawnPoints;

    private GameObject activeShopPod;

    private int currentWaveIndex = 0;
    private int activeEnemiesCount = 0;
    private int totalEnemiesInCurrentWave = 0;
    private int enemiesKilledInCurrentWave = 0;

    private bool isSpawning = false;
    private float waveTimer = 0f;
    private bool isCountingDown = false;

    private float activeWaveRemainingTime = 0f;
    private bool isWaveActive = false;

    private PlayerInputActions inputActions;
    private Coroutine waveCoroutine;

    private List<GameObject> spawnedEnemies = new List<GameObject>();

    public int CurrentWaveNumber => currentWaveIndex + 1;
    public int TotalWaves => waves.Count;
    public int EnemiesRemaining => activeEnemiesCount;
    public int EnemiesKilled => enemiesKilledInCurrentWave;
    public int TotalEnemiesInWave => totalEnemiesInCurrentWave;
    public float WaveTimer => waveTimer;
    public bool IsCountingDown => isCountingDown;
    public float ActiveWaveRemainingTime => activeWaveRemainingTime;
    public bool IsWaveActive => isWaveActive;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.PassTime.performed += _ => SkipCountdown();
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Start()
    {
        if (waves.Count > 0)
        {
            waveCoroutine = StartCoroutine(StartNextWave());
        }
        else
        {
            Debug.LogWarning("No se asignaron WaveData al WaveSpawner.");
        }
    }

    private void Update()
    {
        // ATAJO DE TESTING: F7 salta la oleada actual.
        if (Keyboard.current != null &&
            Keyboard.current.f7Key.wasPressedThisFrame)
        {
            SkipCurrentWave();
        }

        if (isWaveActive)
        {
            activeWaveRemainingTime -= Time.deltaTime;

            if (activeWaveRemainingTime <= 0f)
            {
                activeWaveRemainingTime = 0f;
                isWaveActive = false;

                if (gameManager != null)
                {
                    gameManager.GameOver();
                }
                else
                {
                    Debug.LogError("No se asignó el GameManager en WaveSpawner.");
                }
            }
        }
    }

    public void SkipCountdown()
    {
        if (isCountingDown)
        {
            waveTimer = 0f;
        }
    }

    // SALTAR DIRECTAMENTE A LA SIGUIENTE OLEADA.
    public void SkipCurrentWave()
    {
        if (currentWaveIndex >= waves.Count)
            return;

        Debug.Log("Saltando oleada " + CurrentWaveNumber);

        // Detener la generación de enemigos actual.
        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
        }

        // Eliminar los enemigos que sigan vivos.
        foreach (GameObject enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();

        // Cerrar y eliminar la tienda.
        CloseAndDespawnShop();

        // Reiniciar estados de la oleada.
        activeEnemiesCount = 0;
        totalEnemiesInCurrentWave = 0;
        enemiesKilledInCurrentWave = 0;

        isSpawning = false;
        isCountingDown = false;
        isWaveActive = false;

        waveTimer = 0f;
        activeWaveRemainingTime = 0f;

        // Avanzar a la siguiente oleada.
        currentWaveIndex++;

        waveCoroutine = StartCoroutine(StartNextWave());
    }

    private IEnumerator StartNextWave()
    {
        if (currentWaveIndex >= waves.Count)
        {
            Debug.Log("¡Felicidades! Has sobrevivido a todas las olas.");

            if (gameManager != null)
            {
                gameManager.Win();
            }

            yield break;
        }

        WaveData currentWave = waves[currentWaveIndex];

        // FASE DE DESCANSO / TIENDA.
        waveTimer = currentWave.timeBeforeWave;
        isCountingDown = true;
        isWaveActive = false;

        SpawnShopPod();

        while (waveTimer > 0)
        {
            waveTimer -= Time.deltaTime;
            yield return null;
        }

        isCountingDown = false;

        CloseAndDespawnShop();

        // FASE DE COMBATE.
        isWaveActive = true;
        activeWaveRemainingTime = currentWave.waveDuration;

        List<GameObject> enemiesToSpawn = BuildEnemyList(currentWave);

        totalEnemiesInCurrentWave = enemiesToSpawn.Count;
        activeEnemiesCount = totalEnemiesInCurrentWave;
        enemiesKilledInCurrentWave = 0;

        spawnedEnemies.Clear();
        isSpawning = true;

        foreach (GameObject prefab in enemiesToSpawn)
        {
            if (spawnPoints == null || spawnPoints.Length == 0)
            {
                Debug.LogError("No hay Spawn Points asignados.");
                break;
            }

            Transform spawnPoint =
                spawnPoints[Random.Range(0, spawnPoints.Length)];

            GameObject enemyInstance = Instantiate(
                prefab,
                spawnPoint.position,
                spawnPoint.rotation
            );

            spawnedEnemies.Add(enemyInstance);

            if (enemyInstance.TryGetComponent<EnemyHealth>(
                out EnemyHealth health))
            {
                health.OnEnemyDiedWithReward +=
                    (coinReward) => OnEnemyKilled(coinReward);
            }
            else
            {
                Debug.LogError(
                    "El prefab " + prefab.name +
                    " no tiene el componente EnemyHealth."
                );
            }

            yield return new WaitForSeconds(currentWave.spawnInterval);
        }

        isSpawning = false;

        // Comprobar si todos murieron mientras se generaban.
        if (activeEnemiesCount <= 0)
        {
            CompleteCurrentWave();
        }
    }

    private void SpawnShopPod()
    {
        if (shopPodPrefab != null &&
            shopSpawnPoints != null &&
            shopSpawnPoints.Length > 0)
        {
            Transform randomPoint =
                shopSpawnPoints[Random.Range(0, shopSpawnPoints.Length)];

            activeShopPod = Instantiate(
                shopPodPrefab,
                randomPoint.position,
                randomPoint.rotation
            );
        }
    }

    private void CloseAndDespawnShop()
    {
        if (shopManager != null)
        {
            shopManager.ForceCloseShop();
        }

        if (activeShopPod != null)
        {
            Destroy(activeShopPod);
            activeShopPod = null;
        }
    }

    private List<GameObject> BuildEnemyList(WaveData wave)
    {
        List<GameObject> list = new List<GameObject>();

        foreach (var group in wave.enemyGroups)
        {
            for (int i = 0; i < group.count; i++)
            {
                list.Add(group.enemyPrefab);
            }
        }

        // Mezclar aleatoriamente los enemigos.
        for (int i = 0; i < list.Count; i++)
        {
            GameObject temp = list[i];

            int randomIndex = Random.Range(i, list.Count);

            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }

        return list;
    }

    private void OnEnemyKilled(int coinReward)
    {
        activeEnemiesCount--;
        enemiesKilledInCurrentWave++;

        if (shopManager != null)
        {
            shopManager.AddCoins(coinReward);
        }

        if (activeEnemiesCount <= 0 && !isSpawning)
        {
            CompleteCurrentWave();
        }
    }

    private void CompleteCurrentWave()
    {
        isWaveActive = false;

        Debug.Log("¡Ola " + CurrentWaveNumber + " completada!");

        currentWaveIndex++;

        waveCoroutine = StartCoroutine(StartNextWave());
    }
}
