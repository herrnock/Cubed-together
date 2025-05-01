using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panel‑Prefabs (Assets)")]
    public GameObject mainMenuPrefab;
    public GameObject levelSelectPrefab;
    public GameObject settingsPrefab;

    [Header("Spiel‑Szene")]
    public string gameplaySceneName = "Game";

    // Laufzeit‑Instanzen
    GameObject mainMenuPanel, levelSelectPanel, settingsPanel;

    const string MASTER_KEY = "VolumeMaster";
    const string SFX_KEY = "VolumeSFX";

    #region Unity
    void Awake()
    {
        // Panels erzeugen
        if (mainMenuPrefab)
        {
            mainMenuPanel = Instantiate(mainMenuPrefab, transform);
            BindMainMenuButtons();          // <— Listener anhängen
        }
        if (levelSelectPrefab)
        {
            levelSelectPanel = Instantiate(levelSelectPrefab, transform);
            levelSelectPanel.SetActive(false);
            BindLevelSelectButtons();       // <— Listener anhängen
        }
        if (settingsPrefab)
        {
            settingsPanel = Instantiate(settingsPrefab, transform);
            settingsPanel.SetActive(false);
            BindSettingsButtons();          // <— Listener anhängen
        }
    }

    void Start()
    {
        LoadVolumes();
        ShowPanel(mainMenuPanel);
    }
    #endregion

    #region Button‑Binding
    void BindMainMenuButtons()
    {
        Button playBtn = mainMenuPanel.transform.Find("PlayButton")?.GetComponent<Button>();
        Button lvlBtn = mainMenuPanel.transform.Find("LevelSelectButton")?.GetComponent<Button>();
        Button setBtn = mainMenuPanel.transform.Find("MainSettingButton")?.GetComponent<Button>();
        Button quitBtn = mainMenuPanel.transform.Find("QuitButton")?.GetComponent<Button>();

        if (playBtn) playBtn.onClick.AddListener(PlayGame);
        if (lvlBtn) lvlBtn.onClick.AddListener(OpenLevelSelect);
        if (setBtn) setBtn.onClick.AddListener(OpenMainSettings);
        if (quitBtn) quitBtn.onClick.AddListener(QuitGame);
    }

    void BindLevelSelectButtons()
    {
        Button backBtn = levelSelectPanel.transform.Find("MainMenuButton")?.GetComponent<Button>();
        if (backBtn) backBtn.onClick.AddListener(BackToMainMenu);

        // Beispiel‑Level‑Button („Level1Button“)
        Button level1 = levelSelectPanel.transform.Find("Level1Button")?.GetComponent<Button>();
        if (level1) level1.onClick.AddListener(() => LoadLevel("Level1")); // passt den Szenen‑Namen an
    }

    void BindSettingsButtons()
    {
        // Zurück‑Button
        Button backBtn = settingsPanel.transform.Find("MainMenuButton")?.GetComponent<Button>();
        if (backBtn) backBtn.onClick.AddListener(BackToMainMenu);

        // Slider
        Slider master = settingsPanel.transform.Find("VolumeSlider")?.GetComponent<Slider>();
        Slider sfx = settingsPanel.transform.Find("SFXSlider")?.GetComponent<Slider>();

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
    }
    #endregion

    #region Panel‑Umschalten
    void ShowPanel(GameObject target)
    {
        if (mainMenuPanel) mainMenuPanel.SetActive(target == mainMenuPanel);
        if (levelSelectPanel) levelSelectPanel.SetActive(target == levelSelectPanel);
        if (settingsPanel) settingsPanel.SetActive(target == settingsPanel);
    }
    #endregion

    #region Öffentliche Callbacks
    public void PlayGame() => SceneManager.LoadScene(gameplaySceneName);
    public void OpenLevelSelect() => ShowPanel(levelSelectPanel);
    public void OpenMainSettings() => ShowPanel(settingsPanel);
    public void BackToMainMenu() => ShowPanel(mainMenuPanel);
    public void QuitGame() => Application.Quit();

    // Optional mehrere Levels
    public void LoadLevel(string scene) => SceneManager.LoadScene(scene);
    #endregion

    #region Lautstärke
    public void SetMaster(float v) { PlayerPrefs.SetFloat(MASTER_KEY, v); }
    public void SetSFX(float v) { PlayerPrefs.SetFloat(SFX_KEY, v); }
    void LoadVolumes() { /* Werte werden schon im Slider‑Binding gesetzt */ }
    #endregion
}
