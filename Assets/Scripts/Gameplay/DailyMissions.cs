using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DailyMissions : MonoBehaviour
{
    public static DailyMissions Instance;

    [Header("UI")]
    public GameObject missionsPanel;
    public Button openButton;
    public Button closeButton;

    [Header("Миссия 1 — 100 кликов")]
    public TextMeshProUGUI m1Progress;
    public Button m1ClaimButton;
    public int m1Target = 100;
    public int m1Reward = 200;

    [Header("Миссия 2 — 500 монет")]
    public TextMeshProUGUI m2Progress;
    public Button m2ClaimButton;
    public int m2Target = 500;
    public int m2Reward = 300;

    [Header("Миссия 3 — купить 1 улучшение")]
    public TextMeshProUGUI m3Progress;
    public Button m3ClaimButton;
    public int m3Target = 1;
    public int m3Reward = 500;

    private const string KEY_DAY = "mission_day";
    private const string KEY_M1_CLAIMED = "m1_claimed";
    private const string KEY_M2_CLAIMED = "m2_claimed";
    private const string KEY_M3_CLAIMED = "m3_claimed";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        if (openButton != null) openButton.onClick.AddListener(OpenPanel);
        if (closeButton != null) closeButton.onClick.AddListener(ClosePanel);

        if (m1ClaimButton != null) m1ClaimButton.onClick.AddListener(() => Claim(1));
        if (m2ClaimButton != null) m2ClaimButton.onClick.AddListener(() => Claim(2));
        if (m3ClaimButton != null) m3ClaimButton.onClick.AddListener(() => Claim(3));

        CheckDay();
        if (missionsPanel != null) missionsPanel.SetActive(false);
        RefreshUI();
    }

    private void CheckDay()
    {
        string today = DateTime.Now.ToString("yyyy-MM-dd");
        string saved = PlayerPrefs.GetString(KEY_DAY, "");

        if (saved != today)
        {
            PlayerPrefs.SetString(KEY_DAY, today);
            PlayerPrefs.SetInt(KEY_M1_CLAIMED, 0);
            PlayerPrefs.SetInt(KEY_M2_CLAIMED, 0);
            PlayerPrefs.SetInt(KEY_M3_CLAIMED, 0);
            PlayerPrefs.Save();
            Debug.Log("[Missions] Новый день — миссии сброшены");
        }
    }

    public void OnGameplayUpdate() { RefreshUI(); }

    private void RefreshUI()
    {
        int clicks = PlayerPrefs.GetInt("total_clicks", 0);
        int coins = PlayerPrefs.GetInt("total_coins", 0);
        int upgrades = PlayerPrefs.GetInt("upgrades_bought", 0);

        if (m1Progress != null) m1Progress.text = Mathf.Min(clicks, m1Target) + " / " + m1Target;
        if (m1ClaimButton != null) m1ClaimButton.interactable = 
            PlayerPrefs.GetInt(KEY_M1_CLAIMED, 0) == 0 && clicks >= m1Target;

        if (m2Progress != null) m2Progress.text = Mathf.Min(coins, m2Target) + " / " + m2Target;
        if (m2ClaimButton != null) m2ClaimButton.interactable = 
            PlayerPrefs.GetInt(KEY_M2_CLAIMED, 0) == 0 && coins >= m2Target;

        if (m3Progress != null) m3Progress.text = Mathf.Min(upgrades, m3Target) + " / " + m3Target;
        if (m3ClaimButton != null) m3ClaimButton.interactable = 
            PlayerPrefs.GetInt(KEY_M3_CLAIMED, 0) == 0 && upgrades >= m3Target;
    }

    private void Claim(int missionIndex)
    {
        int reward = 0;

        if (missionIndex == 1 && PlayerPrefs.GetInt(KEY_M1_CLAIMED, 0) == 0)
        { PlayerPrefs.SetInt(KEY_M1_CLAIMED, 1); reward = m1Reward; }

        if (missionIndex == 2 && PlayerPrefs.GetInt(KEY_M2_CLAIMED, 0) == 0)
        { PlayerPrefs.SetInt(KEY_M2_CLAIMED, 1); reward = m2Reward; }

        if (missionIndex == 3 && PlayerPrefs.GetInt(KEY_M3_CLAIMED, 0) == 0)
        { PlayerPrefs.SetInt(KEY_M3_CLAIMED, 1); reward = m3Reward; }

        PlayerPrefs.Save();

        if (reward > 0 && ClickerGame.Instance != null)
            ClickerGame.Instance.AddCoins(reward);

        Debug.Log("[Mission] Награда: " + reward);
        RefreshUI();
    }

    public void OpenPanel()
    {
        RefreshUI();
        if (missionsPanel != null) missionsPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ClosePanel()
    {
        if (missionsPanel != null) missionsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void ResetAll()
    {
        PlayerPrefs.DeleteKey(KEY_DAY);
        PlayerPrefs.DeleteKey(KEY_M1_CLAIMED);
        PlayerPrefs.DeleteKey(KEY_M2_CLAIMED);
        PlayerPrefs.DeleteKey(KEY_M3_CLAIMED);
        PlayerPrefs.Save();
        RefreshUI();
    }
}