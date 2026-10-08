using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance;

    [Header("Панели")]
    public GameObject settingsPanel;
    public GameObject shopPanel;
    public GameObject pausePanel;
    public GameObject achievementsPanel;

    [Header("Кнопки открытия")]
    public Button optionButton;
    public Button shopButton;
    public Button pauseButton;

    [Header("Кнопки закрытия")]
    public Button settingsCloseButton;
    public Button shopCloseButton;
    public Button pauseCloseButton;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        CloseAll();

        if (optionButton != null) optionButton.onClick.AddListener(OpenSettings);
        if (shopButton != null) shopButton.onClick.AddListener(OpenShop);
        if (pauseButton != null) pauseButton.onClick.AddListener(OpenPause);

        if (settingsCloseButton != null) settingsCloseButton.onClick.AddListener(Close);
        if (shopCloseButton != null) shopCloseButton.onClick.AddListener(Close);
        if (pauseCloseButton != null) pauseCloseButton.onClick.AddListener(Close);
    }

    public void OpenPanel(GameObject panel)
    {
        CloseAll();
        if (panel != null)
        {
            panel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void CloseAll()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (shopPanel != null) shopPanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (achievementsPanel != null) achievementsPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    public void OpenSettings() { OpenPanel(settingsPanel); }
    public void OpenShop() { OpenPanel(shopPanel); }
    public void OpenPause() { OpenPanel(pausePanel); }
    public void OpenAchievements() { OpenPanel(achievementsPanel); }
    public void Close() { CloseAll(); }
}