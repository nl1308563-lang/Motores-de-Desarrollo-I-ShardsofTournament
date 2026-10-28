


using UnityEngine;
using TMPro;
using System.Collections;

public enum PassiveType { None, Frenzy, Tank, Greed }

public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    [Header("Economía")]
    [SerializeField] private int coins = 0;

    [Header("Tienda UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject mainShopButtons;
    [SerializeField] private GameObject attributesSectionPanel;
    [SerializeField] private GameObject passiveSectionPanel;
    [SerializeField] private GameObject ammoSectionPanel;

    [Header("Textos UI")]
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private TextMeshProUGUI speedLevelText;
    [SerializeField] private TextMeshProUGUI speedPriceText;
    [SerializeField] private TextMeshProUGUI healthLevelText;
    [SerializeField] private TextMeshProUGUI healthPriceText;
    [SerializeField] private TextMeshProUGUI shieldLevelText;
    [SerializeField] private TextMeshProUGUI shieldPriceText;

    [Header("Efectos UI")]
    [SerializeField] private CoinUI coinUI;

    [Header("Referencias del Jugador")]
    [SerializeField] private MovementPlayerController playerMovement;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerShield playerShield;
    [SerializeField] private PlayerAmmoInventory playerAmmoInventory;

    [Header("Niveles Máximos")]
    [SerializeField] private int maxSpeedLevel = 3;
    private int speedLevel = 0;

    [SerializeField] private int maxHealthLevels = 3;
    private int healthLevel = 0;

    [SerializeField] private int maxShieldLevels = 3;
    private int shieldLevel = 0;

    [Header("Sistema de Pasivas")]
    public PassiveType currentPassive = PassiveType.None;

    [Header("Precios de Pasivas")]
    [SerializeField] private int frenzyPrice = 300;
    [SerializeField] private int tankPrice = 400;
    [SerializeField] private int greedPrice = 500;

    [Header("Frenesí")]
    public float frenzyDuration = 5f;
    public bool frenzyActive = false;
    public float frenzyIncomingDamageMultiplier = 1.5f;
    public float frenzyRecoilMultiplier = 1.6f;
    private Coroutine frenzyCoroutine;

    [Header("Tanque")]
    public int tankKillStacks = 0;
    public int maxTankStacks = 10;
    public float tankStackDuration = 20f;
    private Coroutine tankTimerCoroutine;

    [Header("Codicia")]
    public float greedIncomingDamageMultiplier = 1.8f;

    [Header("Precio de Munición")]
    
    [SerializeField] private int mediumAmmoPrice = 90;
    

    private bool isShopOpen = false;

    public int Coins => coins;
    public bool IsShopOpen => isShopOpen;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            currentPassive = PassiveType.None;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (shopPanel != null) shopPanel.SetActive(false);
        if (attributesSectionPanel != null) attributesSectionPanel.SetActive(false);
        if (passiveSectionPanel != null) passiveSectionPanel.SetActive(false);
        if (ammoSectionPanel != null) ammoSectionPanel.SetActive(false);
        if (mainShopButtons != null) mainShopButtons.SetActive(true);

        UpdateShopUI();
    }

    public void AddCoins(int amount)
    {
        if (currentPassive == PassiveType.Greed)
            amount *= 2;

        coins += amount;
        UpdateShopUI();

        if (coinUI != null)
            coinUI.PlayCoinGainedEffect(amount);

        Debug.Log("Monedas obtenidas: +" + amount + ". Total: " + coins);
    }

    public void EquipPassive1_Frenzy()
    {
        BuyAndEquipPassive(PassiveType.Frenzy, frenzyPrice);
    }

    public void EquipPassive2_Tank()
    {
        BuyAndEquipPassive(PassiveType.Tank, tankPrice);
    }

    public void EquipPassive3_Greed()
    {
        BuyAndEquipPassive(PassiveType.Greed, greedPrice);
    }

    public void BuyAndEquipPassive(PassiveType newPassive, int price)
    {
        if (currentPassive == newPassive)
            return;

        if (coins < price)
            return;

        coins -= price;
        ResetPassiveStates();
        currentPassive = newPassive;

        UpdateShopUI();
    }

    private void ResetPassiveStates()
    {
        frenzyActive = false;

        if (frenzyCoroutine != null)
            StopCoroutine(frenzyCoroutine);

        tankKillStacks = 0;

        if (tankTimerCoroutine != null)
            StopCoroutine(tankTimerCoroutine);
    }

    public void OnEnemyKilled()
    {
        if (currentPassive == PassiveType.Frenzy)
        {
            if (frenzyCoroutine != null)
                StopCoroutine(frenzyCoroutine);

            frenzyCoroutine = StartCoroutine(FrenzyRoutine());
        }
        else if (currentPassive == PassiveType.Tank)
        {
            if (tankKillStacks < maxTankStacks)
                tankKillStacks++;

            if (tankTimerCoroutine != null)
                StopCoroutine(tankTimerCoroutine);

            tankTimerCoroutine = StartCoroutine(TankStackTimerRoutine());
        }
    }

    private IEnumerator FrenzyRoutine()
    {
        frenzyActive = true;
        yield return new WaitForSeconds(frenzyDuration);
        frenzyActive = false;
    }

    private IEnumerator TankStackTimerRoutine()
    {
        yield return new WaitForSeconds(tankStackDuration);
        tankKillStacks = 0;
    }

    public float GetPlayerDamageMultiplier()
    {
        if (currentPassive == PassiveType.Tank)
            return 1f + tankKillStacks * 0.10f;

        return 1f;
    }

    public float GetReloadTimeMultiplier()
    {
        if (currentPassive == PassiveType.Frenzy && frenzyActive)
            return 0.70f;

        return 1f;
    }

    public void OpenShop()
    {
        if (shopPanel == null)
            return;

        isShopOpen = true;
        shopPanel.SetActive(true);
        Time.timeScale = 0f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMainShopMenu();
        UpdateShopUI();
    }

    public void CloseShop()
    {
        if (shopPanel == null)
            return;

        isShopOpen = false;
        shopPanel.SetActive(false);
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ForceCloseShop()
    {
        if (shopPanel != null && shopPanel.activeSelf)
        {
            isShopOpen = false;
            shopPanel.SetActive(false);
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ShowMainShopMenu()
    {
        if (mainShopButtons != null) mainShopButtons.SetActive(true);
        if (attributesSectionPanel != null) attributesSectionPanel.SetActive(false);
        if (passiveSectionPanel != null) passiveSectionPanel.SetActive(false);
        if (ammoSectionPanel != null) ammoSectionPanel.SetActive(false);
    }

    public void OpenAttributesSection()
    {
        if (mainShopButtons != null) mainShopButtons.SetActive(false);
        if (attributesSectionPanel != null) attributesSectionPanel.SetActive(true);
        if (passiveSectionPanel != null) passiveSectionPanel.SetActive(false);
        if (ammoSectionPanel != null) ammoSectionPanel.SetActive(false);
    }

    public void OpenPassiveSection()
    {
        if (mainShopButtons != null) mainShopButtons.SetActive(false);
        if (attributesSectionPanel != null) attributesSectionPanel.SetActive(false);
        if (passiveSectionPanel != null) passiveSectionPanel.SetActive(true);
        if (ammoSectionPanel != null) ammoSectionPanel.SetActive(false);
    }

    public void OpenAmmoSection()
    {
        if (mainShopButtons != null) mainShopButtons.SetActive(false);
        if (attributesSectionPanel != null) attributesSectionPanel.SetActive(false);
        if (passiveSectionPanel != null) passiveSectionPanel.SetActive(false);
        if (ammoSectionPanel != null) ammoSectionPanel.SetActive(true);
    }

    public void BuySpeedUpgrade()
    {
        if (speedLevel >= maxSpeedLevel)
            return;

        int cost = 100 * (speedLevel + 1);

        if (coins < cost)
            return;

        coins -= cost;
        speedLevel++;

        if (playerMovement != null)
        {
            playerMovement.walkSpeed *= 1.30f;
            playerMovement.sprintSpeed *= 1.30f;
        }

        UpdateShopUI();
    }

    public void BuyHealthUpgrade()
    {
        if (healthLevel >= maxHealthLevels)
            return;

        int cost = 75 * (healthLevel + 1);

        if (coins < cost)
            return;

        coins -= cost;
        healthLevel++;

        if (playerHealth != null)
            playerHealth.maxHealth += 10f;

        UpdateShopUI();
    }

    public void BuyShieldUpgrade()
    {
        if (shieldLevel >= maxShieldLevels)
            return;

        int cost = 75 * (shieldLevel + 1);

        if (coins < cost)
            return;

        coins -= cost;
        shieldLevel++;

        if (playerShield != null)
            playerShield.maxShield += 10f;

        UpdateShopUI();
    }

    private void BuyAmmo(AmmoType type, int amount, int price)
    {
        if (playerAmmoInventory == null)
        {
            Debug.LogWarning("Falta asignar PlayerAmmoInventory en ShopManager.");
            return;
        }

        if (amount <= 0 || price < 0 || coins < price)
            return;

        int reserve = playerAmmoInventory.GetReserveAmmo(type);
        int maxReserve = playerAmmoInventory.GetMaxReserveAmmo(type);

        if (maxReserve <= 0 || reserve >= maxReserve)
            return;

        if (maxReserve - reserve < amount)
        {
            Debug.Log("No hay espacio suficiente para el paquete de " + type + ".");
            return;
        }

        int amountAdded = playerAmmoInventory.AddAmmo(type, amount);

        if (amountAdded != amount)
            return;

        coins -= price;
        UpdateShopUI();

        Debug.Log("Compraste " + amountAdded + " unidades de " + type + " por " + price + " monedas.");
    }

    public void BuyPistolMedium()
    {
        BuyAmmo(AmmoType.Pistol, 20, mediumAmmoPrice);
    }

    public void BuyShotgunMedium()
    {
        BuyAmmo(AmmoType.Shotgun, 10, mediumAmmoPrice);
    }

    public void BuyBattleRifleMedium()
    {
        BuyAmmo(AmmoType.BattleRifle, 20, mediumAmmoPrice);
    }

    public void BuyMachineGunMedium()
    {
        BuyAmmo(AmmoType.MachineGun, 40, mediumAmmoPrice);
    }

    public void BuySniperMedium()
    {
        BuyAmmo(AmmoType.Sniper, 5, mediumAmmoPrice);
    }

    public void BuyRocketMedium()
    {
        BuyAmmo(AmmoType.Rocket, 10, mediumAmmoPrice);
    }

   

    private void UpdateShopUI()
    {
        if (coinsText != null)
            coinsText.text = "Monedas: " + coins;

        if (speedLevelText != null)
            speedLevelText.text = "Nivel: " + speedLevel + " / " + maxSpeedLevel;

        if (speedPriceText != null)
            speedPriceText.text = speedLevel >= maxSpeedLevel
                ? "MÁXIMO"
                : "Costo: " + (100 * (speedLevel + 1));

        if (healthLevelText != null)
            healthLevelText.text = "Nivel: " + healthLevel + " / " + maxHealthLevels;

        if (healthPriceText != null)
            healthPriceText.text = healthLevel >= maxHealthLevels
                ? "MÁXIMO"
                : "Costo: " + (75 * (healthLevel + 1));

        if (shieldLevelText != null)
            shieldLevelText.text = "Nivel: " + shieldLevel + " / " + maxShieldLevels;

        if (shieldPriceText != null)
            shieldPriceText.text = shieldLevel >= maxShieldLevels
                ? "MÁXIMO"
                : "Costo: " + (75 * (shieldLevel + 1));
    }
}
