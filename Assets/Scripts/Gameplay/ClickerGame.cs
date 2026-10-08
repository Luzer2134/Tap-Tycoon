using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClickerGame : MonoBehaviour
{
    [Header("UI — основной")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI perClickText;
    public TextMeshProUGUI perSecondText;

    [Header("UI — Улучшение клика")]
    public TextMeshProUGUI clickLevelText;
    public TextMeshProUGUI clickUpgradeCostText;
    public Button clickUpgradeButton;

    [Header("UI — Автокликер")]
    public TextMeshProUGUI autoLevelText;
    public TextMeshProUGUI autoUpgradeCostText;
    public Button autoUpgradeButton;

    [Header("UI — Кнопки")]
    public Button tapButton;
    public Button resetButton;

    [Header("Настройки — Клик")]
    public int startClickCost = 500;
    public int clickCostMultiplier = 2;
    public int clickPowerMultiplier = 2;

    [Header("Настройки — Автокликер")]
    public int startAutoCost = 1000;
    public int autoCostMultiplier = 2;
    public int autoIncomePerLevel = 1;

    private int coins = 0;
    private int coinsPerClick = 1;
    private int coinsPerSecond = 0;
    private int clickLevel = 1;
    private int autoLevel = 0;
    private int clickUpgradeCost;
    private int autoUpgradeCost;

    private const string KEY_COINS = "coins";
    private const string KEY_PER_CLICK = "per_click";
    private const string KEY_CLICK_LEVEL = "click_level";
    private const string KEY_AUTO_LEVEL = "auto_level";
    private const string KEY_CLICK_COST = "click_cost";
    private const string KEY_AUTO_COST = "auto_cost";

    private float autoTimer = 0f;

    private void Start()
    {
        Load();

        if (tapButton != null) tapButton.onClick.AddListener(OnTap);
        if (clickUpgradeButton != null) clickUpgradeButton.onClick.AddListener(OnUpgradeClick);
        if (autoUpgradeButton != null) autoUpgradeButton.onClick.AddListener(OnUpgradeAuto);
        if (resetButton != null) resetButton.onClick.AddListener(OnReset);

        UpdateUI();
    }

    private void Update()
    {
        if (coinsPerSecond > 0)
        {
            autoTimer += Time.deltaTime;
            if (autoTimer >= 1f)
            {
                autoTimer -= 1f;
                coins += coinsPerSecond;
                UpdateUI();
                Save();
            }
        }
    }

    public void OnTap()
    {
        coins += coinsPerClick;
        UpdateUI();
        Save();
    }

    public void OnUpgradeClick()
    {
        if (coins < clickUpgradeCost) return;

        coins -= clickUpgradeCost;
        clickLevel++;
        coinsPerClick *= clickPowerMultiplier;
        clickUpgradeCost = Mathf.RoundToInt(startClickCost * Mathf.Pow(1.15f, clickLevel));

        UpdateUI();
        Save();
    }

    public void OnUpgradeAuto()
    {
        if (coins < autoUpgradeCost) return;

        coins -= autoUpgradeCost;
        autoLevel++;
        coinsPerSecond += autoIncomePerLevel;
        autoUpgradeCost *= autoCostMultiplier;

        UpdateUI();
        Save();
    }

    public void OnReset()
    {
        PlayerPrefs.DeleteAll();
        coins = 0;
        coinsPerClick = 1;
        coinsPerSecond = 0;
        clickLevel = 1;
        autoLevel = 0;
        clickUpgradeCost = startClickCost;
        autoUpgradeCost = startAutoCost;

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinsText != null) coinsText.text = coins.ToString();
        if (perClickText != null) perClickText.text = "+" + coinsPerClick;
        if (perSecondText != null) perSecondText.text = "+" + coinsPerSecond + "/сек";

        if (clickLevelText != null) clickLevelText.text = "Ур. " + clickLevel;
        if (clickUpgradeCostText != null) clickUpgradeCostText.text = clickUpgradeCost.ToString();
        if (clickUpgradeButton != null) clickUpgradeButton.interactable = coins >= clickUpgradeCost;

        if (autoLevelText != null) autoLevelText.text = "Ур. " + autoLevel;
        if (autoUpgradeCostText != null) autoUpgradeCostText.text = autoUpgradeCost.ToString();
        if (autoUpgradeButton != null) autoUpgradeButton.interactable = coins >= autoUpgradeCost;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(KEY_COINS, coins);
        PlayerPrefs.SetInt(KEY_PER_CLICK, coinsPerClick);
        PlayerPrefs.SetInt(KEY_CLICK_LEVEL, clickLevel);
        PlayerPrefs.SetInt(KEY_AUTO_LEVEL, autoLevel);
        PlayerPrefs.SetInt(KEY_CLICK_COST, clickUpgradeCost);
        PlayerPrefs.SetInt(KEY_AUTO_COST, autoUpgradeCost);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        coins = PlayerPrefs.GetInt(KEY_COINS, 0);
        coinsPerClick = PlayerPrefs.GetInt(KEY_PER_CLICK, 1);
        clickLevel = PlayerPrefs.GetInt(KEY_CLICK_LEVEL, 1);
        autoLevel = PlayerPrefs.GetInt(KEY_AUTO_LEVEL, 0);
        clickUpgradeCost = PlayerPrefs.GetInt(KEY_CLICK_COST, startClickCost);
        autoUpgradeCost = PlayerPrefs.GetInt(KEY_AUTO_COST, startAutoCost);
        coinsPerSecond = autoLevel * autoIncomePerLevel;
    }
}