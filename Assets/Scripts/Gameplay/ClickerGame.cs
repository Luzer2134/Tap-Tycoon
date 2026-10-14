using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClickerGame : MonoBehaviour
{
    public static ClickerGame Instance;

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
    public int startClickCost = 100;
    public float clickCostGrowth = 1.15f;
    public float clickPowerGrowth = 1.15f;

    [Header("Настройки — Автокликер")]
    public int startAutoCost = 500;
    public float autoCostGrowth = 1.15f;
    public int autoIncomePerLevel = 1;

    private int coins = 0;
    private int coinsPerClick = 1;
    private int coinsPerSecond = 0;
    private int clickLevel = 1;
    private int autoLevel = 0;

    private const string KEY_COINS = "coins";
    private const string KEY_PER_CLICK = "per_click";
    private const string KEY_PER_SEC = "per_sec";
    private const string KEY_CLICK_LEVEL = "click_level";
    private const string KEY_AUTO_LEVEL = "auto_level";

    private float autoTimer = 0f;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        Load();
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
                PlayerPrefs.SetInt("total_coins", PlayerPrefs.GetInt("total_coins", 0) + coinsPerSecond);
                UpdateUI();
                Save();
            }
        }
    }

    private int GetClickCost()
    {
        return Mathf.RoundToInt(startClickCost * Mathf.Pow(clickCostGrowth, clickLevel - 1));
    }

    private int GetAutoCost()
    {
        return Mathf.RoundToInt(startAutoCost * Mathf.Pow(autoCostGrowth, autoLevel));
    }

    public void OnTap()
    {
        coins += coinsPerClick;

        int clicks = PlayerPrefs.GetInt("total_clicks", 0) + 1;
        PlayerPrefs.SetInt("total_clicks", clicks);

        int totalCoins = PlayerPrefs.GetInt("total_coins", 0) + coinsPerClick;
        PlayerPrefs.SetInt("total_coins", totalCoins);

        PlayerPrefs.Save();

        UpdateUI();
        Save();

        Debug.Log("Кликов: " + clicks);

        if (Achievements.Instance != null)
            Achievements.Instance.CheckAll();

    }

    public void OnUpgradeClick()
    {
        int cost = GetClickCost();
        if (coins < cost) return;

        coins -= cost;
        clickLevel++;
        coinsPerClick = clickLevel;

        PlayerPrefs.SetInt("upgrades_bought", PlayerPrefs.GetInt("upgrades_bought", 0) + 1);
        PlayerPrefs.Save();

        UpdateUI();
        Save();

        Debug.Log("Улучшение! Уровень: " + clickLevel + ", куплено всего: " + PlayerPrefs.GetInt("upgrades_bought", 0));

        if (Achievements.Instance == null)
        {
            Debug.LogError("!!! Achievements.Instance = NULL — объект Achievements не на сцене!");
            return;
        }

        Debug.Log("Вызываю CheckAll...");
        Achievements.Instance.CheckAll();
        Debug.Log("CheckAll вызван.");

        if (LevelSystem.Instance != null)
            LevelSystem.Instance.OnUpgradeBought();
    }

    public void OnUpgradeAuto()
    {
        int cost = GetAutoCost();
        if (coins < cost) return;

        coins -= cost;
        autoLevel++;
        coinsPerSecond = autoLevel * autoIncomePerLevel;

        PlayerPrefs.SetInt("upgrades_bought", PlayerPrefs.GetInt("upgrades_bought", 0) + 1);
        PlayerPrefs.Save();

        UpdateUI();
        Save();

        if (Achievements.Instance != null) Achievements.Instance.CheckAll();

        if (LevelSystem.Instance != null)
            LevelSystem.Instance.OnUpgradeBought();
    }

   public void OnReset()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        coins = 0;
        coinsPerClick = 1;
        coinsPerSecond = 0;
        clickLevel = 1;
        autoLevel = 0;

        UpdateUI();

        if (Achievements.Instance != null)
            Achievements.Instance.ResetAll();

        if (DailyReward.Instance != null)
            DailyReward.Instance.ResetAll();

        Debug.Log("[Reset] Прогресс полностью сброшен");
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        PlayerPrefs.SetInt(KEY_COINS, coins);
        PlayerPrefs.Save();
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinsText != null) coinsText.text = coins.ToString();
        if (perClickText != null) perClickText.text = "+" + coinsPerClick;
        if (perSecondText != null) perSecondText.text = "+" + coinsPerSecond + "/сек";

        if (clickLevelText != null) clickLevelText.text = "Ур. " + clickLevel;
        int clickCost = GetClickCost();
        if (clickUpgradeCostText != null) clickUpgradeCostText.text = clickCost.ToString();
        if (clickUpgradeButton != null) clickUpgradeButton.interactable = coins >= clickCost;

        if (autoLevelText != null) autoLevelText.text = "Ур. " + autoLevel;
        int autoCost = GetAutoCost();
        if (autoUpgradeCostText != null) autoUpgradeCostText.text = autoCost.ToString();
        if (autoUpgradeButton != null) autoUpgradeButton.interactable = coins >= autoCost;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(KEY_COINS, coins);
        PlayerPrefs.SetInt(KEY_PER_CLICK, coinsPerClick);
        PlayerPrefs.SetInt(KEY_PER_SEC, coinsPerSecond);
        PlayerPrefs.SetInt(KEY_CLICK_LEVEL, clickLevel);
        PlayerPrefs.SetInt(KEY_AUTO_LEVEL, autoLevel);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        coins = PlayerPrefs.GetInt(KEY_COINS, 0);
        coinsPerClick = PlayerPrefs.GetInt(KEY_PER_CLICK, 1);
        coinsPerSecond = PlayerPrefs.GetInt(KEY_PER_SEC, 0);
        clickLevel = PlayerPrefs.GetInt(KEY_CLICK_LEVEL, 1);
        autoLevel = PlayerPrefs.GetInt(KEY_AUTO_LEVEL, 0);

        if (clickLevel < 1) clickLevel = 1;
        coinsPerClick = clickLevel;

        Debug.Log("[Load] coins=" + coins + " perClick=" + coinsPerClick + " clickLevel=" + clickLevel);
    }
}