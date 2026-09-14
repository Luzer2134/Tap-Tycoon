using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ClickerGame : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI perClickText;
    public TextMeshProUGUI upgradeCostText;
    public Button upgradeButton;

    [Header("Настройки")]
    public int coinsPerClick = 1;
    public int upgradeCost = 10;
    public int upgradeMultiplier = 2;

    private int coins = 0;

    private const string KEY_COINS = "coins";
    private const string KEY_PER_CLICK = "per_click";
    private const string KEY_UPGRADE_COST = "upgrade_cost";

    private void Start()
    {
        coins = PlayerPrefs.GetInt(KEY_COINS, 0);
        coinsPerClick = PlayerPrefs.GetInt(KEY_PER_CLICK, 1);
        upgradeCost = PlayerPrefs.GetInt(KEY_UPGRADE_COST, 10);

        UpdateUI();
    }

    public void OnTap()
    {
        coins += coinsPerClick;
        UpdateUI();
        Save();
    }

    public void OnUpgrade()
    {
        if (coins < upgradeCost) return;

        coins -= upgradeCost;
        coinsPerClick *= upgradeMultiplier;
        upgradeCost *= upgradeMultiplier;

        UpdateUI();
        Save();
    }

    public void OnReset()
    {
        PlayerPrefs.DeleteAll();
        coins = 0;
        coinsPerClick = 1;
        upgradeCost = 10;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (coinsText != null)
            coinsText.text = "Монеты: " + coins;

        if (perClickText != null)
            perClickText.text = "За клик: " + coinsPerClick;

        if (upgradeCostText != null)
            upgradeCostText.text = "Улучшение: " + upgradeCost;

        if (upgradeButton != null)
            upgradeButton.interactable = coins >= upgradeCost;
    }

    private void Save()
    {
        PlayerPrefs.SetInt(KEY_COINS, coins);
        PlayerPrefs.SetInt(KEY_PER_CLICK, coinsPerClick);
        PlayerPrefs.SetInt(KEY_UPGRADE_COST, upgradeCost);
        PlayerPrefs.Save();
    }
}