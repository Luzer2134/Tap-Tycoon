using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DailyReward : MonoBehaviour
{
    public static DailyReward Instance;

    [Header("UI")]
    public GameObject dailyPanel;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI streakText;
    public Button claimButton;

    [Header("Настройки")]
    public int baseReward = 100;
    public int streakBonus = 50;
    public int maxStreak = 7;

    //ЗАМЕНИТЬ НА 24 ПОТОМ!!!
    public float cooldownHours = 0.0005f;

    private const string KEY_LAST_CLAIM = "daily_last_claim";
    private const string KEY_STREAK = "daily_streak";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (claimButton != null)
            claimButton.onClick.AddListener(OnClaim);

        if (dailyPanel != null)
            dailyPanel.SetActive(false);

        CheckDaily();
    }

    private void CheckDaily()
    {
        if (CanClaim())
            OpenPanel();
    }

    private bool CanClaim()
    {
        string last = PlayerPrefs.GetString(KEY_LAST_CLAIM, "");
        if (string.IsNullOrEmpty(last)) return true;

        if (DateTime.TryParse(last, out DateTime lastDate))
            return (DateTime.Now - lastDate).TotalHours >= cooldownHours;

        return true;
    }

    private void OpenPanel()
    {
        if (dailyPanel == null) return;

        int streak = PlayerPrefs.GetInt(KEY_STREAK, 0);
        int dayInCycle = (streak % maxStreak) + 1;
        int reward = GetRewardForDay(dayInCycle);

        if (rewardText != null) rewardText.text = "+" + reward;
        if (streakText != null) streakText.text = "День " + dayInCycle + " / " + maxStreak;

        dailyPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private int GetRewardForDay(int day)
    {
        switch (day)
        {
            case 1: return 100;
            case 2: return 150;
            case 3: return 200;
            case 4: return 300;
            case 5: return 400;
            case 6: return 500;
            case 7: return 1000;
            default: return 100;
        }
    }

    public void OnClaim()
    {
        int streak = PlayerPrefs.GetInt(KEY_STREAK, 0);
        int dayInCycle = (streak % maxStreak) + 1;
        int reward = GetRewardForDay(dayInCycle);

        if (ClickerGame.Instance != null)
            ClickerGame.Instance.AddCoins(reward);

        PlayerPrefs.SetInt(KEY_STREAK, streak + 1);
        PlayerPrefs.SetString(KEY_LAST_CLAIM, DateTime.Now.ToString());
        PlayerPrefs.Save();

        if (dailyPanel != null) dailyPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteKey(KEY_LAST_CLAIM);
        PlayerPrefs.DeleteKey(KEY_STREAK);
        PlayerPrefs.Save();

        if (dailyPanel != null) dailyPanel.SetActive(false);

        Debug.Log("[DailyReward] Сброшено");
    }
}