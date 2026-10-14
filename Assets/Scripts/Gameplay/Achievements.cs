using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

[Serializable]
public class Achievement
{
    public string id;
    public string title;
    public string description;
    public int targetValue;
    public int reward;
    [HideInInspector] public bool unlocked;
}

public class Achievements : MonoBehaviour
{
    public static Achievements Instance;

    [Header("UI")]
    public GameObject achievementsPanel;
    public Button openButton;
    public Button closeButton;

    [Header("Список достижений")]
    public Achievement[] achievements;

    [Header("UI строки достижений")]
    public TextMeshProUGUI[] titleTexts;
    public TextMeshProUGUI[] statusTexts;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (openButton != null) openButton.onClick.AddListener(OpenPanel);
        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);

        foreach (var a in achievements)
            a.unlocked = PlayerPrefs.GetInt("ach_" + a.id, 0) == 1;

        if (achievementsPanel != null) achievementsPanel.SetActive(false);

        RefreshUI();
    }

    public void CheckAll()
    {
        int totalClicks = PlayerPrefs.GetInt("total_clicks", 0);
        int totalCoins = PlayerPrefs.GetInt("total_coins", 0);
        int upgrades = PlayerPrefs.GetInt("upgrades_bought", 0);

        Debug.Log($"=== CheckAll START: clicks={totalClicks}, coins={totalCoins}, upgrades={upgrades}, achievements.Length={achievements.Length} ===");

        for (int i = 0; i < achievements.Length; i++)
        {
            var a = achievements[i];
            bool isUnlocked = PlayerPrefs.GetInt("ach_" + a.id, 0) == 1;

            Debug.Log($"  [{i}] id={a.id}, target={a.targetValue}, reward={a.reward}, isUnlocked={isUnlocked}");

            if (isUnlocked) continue;

            int progress = GetProgress(a.id, totalClicks, totalCoins, upgrades);
            Debug.Log($"     progress={progress}, target={a.targetValue}");

            if (progress >= a.targetValue)
            {
                PlayerPrefs.SetInt("ach_" + a.id, 1);
                PlayerPrefs.Save();

                a.unlocked = true;
                achievements[i] = a;

                if (ClickerGame.Instance != null)
                    ClickerGame.Instance.AddCoins(a.reward);

                Debug.Log("[Achievement] Открыто: " + a.title + " (+" + a.reward + ")");
            }
        }
        RefreshUI();
    }

    private int GetProgress(string id, int clicks, int coins, int upgrades)
    {
        id = id.Trim().ToLower(); 

        if (id == "first_upgrade") return upgrades;
        if (id == "clicks_10") return clicks;
        if (id == "clicks_100") return clicks;
        if (id == "coins_1000") return coins;
        if (id == "clicks_1000") return clicks;

        Debug.LogWarning("Неизвестный id ачивки: '" + id + "'");
        return 0;
    }

    private void RefreshUI()
    {
        if (achievements == null) return;

        for (int i = 0; i < achievements.Length && i < titleTexts.Length; i++)
        {
            if (titleTexts[i] != null)
                titleTexts[i].text = achievements[i].title;

            if (statusTexts[i] != null)
                statusTexts[i].text = achievements[i].unlocked ? "получено" : "не открыто";
        }
    }

    public void OpenPanel()
    {
        if (achievementsPanel != null) achievementsPanel.SetActive(true);
        Time.timeScale = 0f;
        RefreshUI();
    }

    public void ClosePanel()
    {
        if (achievementsPanel != null) achievementsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

        public void ResetAll()
    {
        PlayerPrefs.DeleteKey("ach_first_upgrade");
        PlayerPrefs.DeleteKey("ach_clicks_10");
        PlayerPrefs.DeleteKey("ach_clicks_100");
        PlayerPrefs.DeleteKey("ach_coins_1000");
        PlayerPrefs.DeleteKey("ach_clicks_1000");
        PlayerPrefs.Save();

        RefreshUI();
        Debug.Log("[Achievements] Сброшено");
    }
}