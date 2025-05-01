using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Steuert Pause-, PauseSettings-, GameOver- und LevelComplete-Menüs
/// in jeder Gameplay-Szene.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    //─────────────────────────────────────────────────────────────
    #region Inspector-Felder
    [Header("Panel-Prefabs (Assets)")]
    public GameObject pauseMenuPrefab;        // Resume • Restart • Settings • MainMenu
    public GameObject pauseSettingsPrefab;    // Volume-/SFX-Slider • Zurück
    public GameObject gameOverPrefab;         // Retry • MainMenu
    public GameObject levelCompletePrefab;    // NextLevel • MainMenu

    [Header("Szenen-Namen")]
    public string menuSceneName = "Menu";

    [Header("Pause-Hotkey")]
    public KeyCode pauseKey = KeyCode.Escape;
    #endregion

    //─────────────────────────────────────────────────────────────
    #region Private Variablen
    GameObject pauseMenuPanel, pauseSettingsPanel, gameOverPanel, levelCompletePanel;
    bool isPaused;

    const string MASTER_KEY = "VolumeMaster";
    const string SFX_KEY = "VolumeSFX";
    #endregion

    //─────────────────────────────────────────────────────────────
    #region Unity-Lifecycle
    void Awake()
    {
        // Pause-Panel
        if (pauseMenuPrefab)
        {
            pauseMenuPanel = Instantiate(pauseMenuPrefab, transform);
            BindPauseMenuButtons();
            pauseMenuPanel.SetActive(false);
        }
        else Debug.LogWarning("[PauseMenuManager] PauseMenu-Prefab fehlt!");

        // Pause-Settings
        if (pauseSettingsPrefab)
        {
            pauseSettingsPanel = Instantiate(pauseSettingsPrefab, transform);
            BindPauseSettingsButtons();
            pauseSettingsPanel.SetActive(false);
        }
        else Debug.LogWarning("[PauseMenuManager] PauseSettings-Prefab fehlt!");

        // Game-Over
        if (gameOverPrefab)
        {
            gameOverPanel = Instantiate(gameOverPrefab, transform);
            BindGameOverButtons();
            gameOverPanel.SetActive(false);
        }
        else Debug.LogWarning("[PauseMenuManager] GameOver-Prefab fehlt!");

        // Level-Complete
        if (levelCompletePrefab)
        {
            levelCompletePanel = Instantiate(levelCompletePrefab, transform);
            BindLevelCompleteButtons();
            levelCompletePanel.SetActive(false);
        }
        else Debug.LogWarning("[PauseMenuManager] LevelComplete-Prefab fehlt!");
    }

    void Update()
    {
        // Esc nur zulassen, wenn kein GameOver/LevelComplete angezeigt wird
        if (Input.GetKeyDown(pauseKey) &&
            (gameOverPanel == null || !gameOverPanel.activeSelf) &&
            (levelCompletePanel == null || !levelCompletePanel.activeSelf))
        {
            if (isPaused) Resume(); else Pause();
        }
    }
    #endregion

    //─────────────────────────────────────────────────────────────
    #region Button-Binding
    void BindPauseMenuButtons()
    {
        Button resume = pauseMenuPanel.transform.Find("ResumeButton")?.GetComponent<Button>();
        Button restart = pauseMenuPanel.transform.Find("RestartButton")?.GetComponent<Button>();
        Button settings = pauseMenuPanel.transform.Find("PauseSettingButton")?.GetComponent<Button>();
        Button mainMenu = pauseMenuPanel.transform.Find("MainMenuButton")?.GetComponent<Button>();

        if (resume) resume.onClick.AddListener(Resume);
        if (restart) restart.onClick.AddListener(Restart);
        if (settings) settings.onClick.AddListener(OpenPauseSettings);
        if (mainMenu) mainMenu.onClick.AddListener(BackToMainMenu);
    }

    void BindPauseSettingsButtons()
    {
        // Slider
        Slider master = pauseSettingsPanel.transform.Find("VolumeSlider")?.GetComponent<Slider>();
        Slider sfx = pauseSettingsPanel.transform.Find("SFXSlider")?.GetComponent<Slider>();

        if (master)
        {
            master.value = PlayerPrefs.GetFloat(MASTER_KEY, 0.8f);
            master.onValueChanged.AddListener(SetMaster);
        }
        if (sfx)
        {
            sfx.value = PlayerPrefs.GetFloat(SFX_KEY, 0.8f);
            sfx.onValueChanged.AddListener(SetSFX);
        }

        // Zurück-Button
        Button back = pauseSettingsPanel.transform.Find("PauseMenuButton")?.GetComponent<Button>();
        if (back) back.onClick.AddListener(OpenPauseMenu);
    }

    void BindGameOverButtons()
    {
        Button retry = gameOverPanel.transform.Find("RetryButton")?.GetComponent<Button>();
        Button main = gameOverPanel.transform.Find("MainMenuButton")?.GetComponent<Button>();

        if (retry) retry.onClick.AddListener(Restart);
        if (main) main.onClick.AddListener(BackToMainMenu);
    }

    void BindLevelCompleteButtons()
    {
        Button next = levelCompletePanel.transform.Find("NextLevelButton")?.GetComponent<Button>();
        Button main = levelCompletePanel.transform.Find("MainMenuButton")?.GetComponent<Button>();

        if (next) next.onClick.AddListener(NextLevel);
        if (main) main.onClick.AddListener(BackToMainMenu);
    }
    #endregion

    //─────────────────────────────────────────────────────────────
    #region Pause-/Game-Flow
    public void Pause()
    {
        isPaused = true;
        Time.timeScale = 0;
        ShowPanel(pauseMenuPanel);
    }

    public void Resume()
    {
        isPaused = false;
        Time.timeScale = 1;
        HideAllPanels();
    }

    public void Restart()
    {
        isPaused = false;
        Time.timeScale = 1;
        HideAllPanels();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void BackToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(menuSceneName);
    }

    public void OpenPauseSettings() => ShowPanel(pauseSettingsPanel);
    public void OpenPauseMenu() => ShowPanel(pauseMenuPanel);

    public void ShowGameOver()
    {
        isPaused = true;
        Time.timeScale = 0;
        ShowPanel(gameOverPanel);
    }

    public void ShowLevelComplete()
    {
        isPaused = true;
        Time.timeScale = 0;
        ShowPanel(levelCompletePanel);
    }

    public void NextLevel()
    {
        Time.timeScale = 1;
        int next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            BackToMainMenu(); // oder Credits-Szene laden
    }
    #endregion

    //─────────────────────────────────────────────────────────────
    #region Panel-Helper
    void ShowPanel(GameObject target)
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(target == pauseMenuPanel);
        if (pauseSettingsPanel) pauseSettingsPanel.SetActive(target == pauseSettingsPanel);
        if (gameOverPanel) gameOverPanel.SetActive(target == gameOverPanel);
        if (levelCompletePanel) levelCompletePanel.SetActive(target == levelCompletePanel);
    }

    void HideAllPanels()
    {
        if (pauseMenuPanel) pauseMenuPanel.SetActive(false);
        if (pauseSettingsPanel) pauseSettingsPanel.SetActive(false);
        if (gameOverPanel) gameOverPanel.SetActive(false);
        if (levelCompletePanel) levelCompletePanel.SetActive(false);
    }
    #endregion

    //─────────────────────────────────────────────────────────────
    #region Lautstärke
    public void SetMaster(float v) { PlayerPrefs.SetFloat(MASTER_KEY, v); }
    public void SetSFX(float v) { PlayerPrefs.SetFloat(SFX_KEY, v); }
    #endregion
}
