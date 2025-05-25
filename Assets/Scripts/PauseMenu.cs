using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseScreen;       // PauseScreen Panel’i
    public Button continueButton;
    public Button settingsButton;
    public Button quitButton;

    private bool _isPaused;

    void Awake()
    {
        // Başlangıçta menüyü gizle
        pauseScreen.SetActive(false);

        // Buton dinleyicileri
        continueButton.onClick.AddListener(ResumeGame);
        settingsButton.onClick.AddListener(OpenSettings);
        quitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isPaused) ResumeGame();
            else PauseGame();
        }
    }

    void PauseGame()
    {
        _isPaused = true;
        pauseScreen.SetActive(true);
        Time.timeScale = 0f;            // Oyun durur
        AudioListener.pause = true;      // Sesleri de durdurmak istersen
    }

    void ResumeGame()
    {
        _isPaused = false;
        pauseScreen.SetActive(false);
        Time.timeScale = 1f;            // Oyun devam eder
        AudioListener.pause = false;
    }

    void OpenSettings()
    {
        // Ayarlar menüsü açılacaksa buraya ekle
        Debug.Log("Ayarlar menüsü açılabilir.");
    }

    void QuitGame()
    {
        SceneManager.LoadScene("MainMenu");
        ResumeGame();
    }
}