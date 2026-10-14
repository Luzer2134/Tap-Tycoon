using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour
{
    [Header("Кнопки в панели настроек")]
    public Button mainMenuButton;
    public Button leaveButton;

    [Header("Окно подтверждения выхода")]
    public GameObject leaveConfirmPanel;
    public Button confirmLeaveButton;
    public Button cancelLeaveButton;

    private void Start()
    {
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        if (leaveButton != null)
            leaveButton.onClick.AddListener(ShowLeaveConfirm);

        if (confirmLeaveButton != null)
            confirmLeaveButton.onClick.AddListener(ConfirmLeave);

        if (cancelLeaveButton != null)
            cancelLeaveButton.onClick.AddListener(CancelLeave);

        if (leaveConfirmPanel != null)
            leaveConfirmPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowLeaveConfirm()
    {
        if (leaveConfirmPanel != null)
            leaveConfirmPanel.SetActive(true);
    }

    public void ConfirmLeave()
    {
        Debug.Log("[Leave] Выход из игры");
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void CancelLeave()
    {
        if (leaveConfirmPanel != null)
            leaveConfirmPanel.SetActive(false);
    }
}