using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSystem : MonoBehaviour
{
    public static LevelSystem Instance;

    [Header("UI")]
    public Image backgroundImage;      
    public Image characterImage;       
    public Image progressFill;            
    public TextMeshProUGUI progressText;  
    public TextMeshProUGUI levelText;     

    [Header("Уровни")]
    public LevelData[] levels;

    private int currentLevel = 0; 

    [System.Serializable]
    public class LevelData
    {
        public string name;         
        public Sprite background;   
        public Sprite character;    
        public int requiredUpgrades; 
    }

    private const string KEY_LEVEL = "player_level";

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt(KEY_LEVEL, 0);
        ApplyLevel();
        UpdateUI();
    }

    public void OnUpgradeBought()
    {
        int upgrades = PlayerPrefs.GetInt("upgrades_bought", 0);
        int nextRequired = GetNextRequiredUpgrades();

        if (nextRequired > 0 && upgrades >= nextRequired)
        {
            LevelUp();
        }

        UpdateUI();
    }

    private int GetNextRequiredUpgrades()
    {
        int nextIndex = currentLevel + 1;
        if (nextIndex >= levels.Length) return -1; 
        return levels[nextIndex].requiredUpgrades;
    }

    private void LevelUp()
    {
        currentLevel++;
        PlayerPrefs.SetInt(KEY_LEVEL, currentLevel);
        PlayerPrefs.Save();

        ApplyLevel();
        Debug.Log("[Level] Новый уровень: " + levels[currentLevel].name);
    }

    private void ApplyLevel()
    {
        if (currentLevel >= levels.Length) currentLevel = levels.Length - 1;

        var lvl = levels[currentLevel];

        if (backgroundImage != null && lvl.background != null)
            backgroundImage.sprite = lvl.background;

        if (characterImage != null && lvl.character != null)
            characterImage.sprite = lvl.character;

        if (levelText != null)
            levelText.text = lvl.name;
    }

    private void UpdateUI()
    {
        int upgrades = PlayerPrefs.GetInt("upgrades_bought", 0);
        int nextRequired = GetNextRequiredUpgrades();

        if (nextRequired == -1)
        {
            // Максимум
            if (progressFill != null) progressFill.fillAmount = 1f;
            if (progressText != null) progressText.text = "MAX";
            return;
        }

        int prevRequired = currentLevel == 0 ? 0 : levels[currentLevel].requiredUpgrades;
        int progressInLevel = upgrades - prevRequired;
        int neededForNext = nextRequired - prevRequired;

        float percent = Mathf.Clamp01((float)progressInLevel / neededForNext);

        if (progressFill != null) progressFill.fillAmount = percent;
        if (progressText != null) progressText.text = progressInLevel + " / " + neededForNext;
    }
}