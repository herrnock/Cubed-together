using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    public GameObject startScreenCanvas;
    public GameObject mainMenuCanvas;
    public GameObject optionsCanvas;
    public GameObject settingsCanvas;
    public GameObject levelSelectionCanvas;
    public GameObject winCanvas;
    public GameObject pauseMenuCanvas; // Pausenmenü-Canvas

    private bool isGamePaused = false; // Status des Spiels (pausiert oder nicht)

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("MenuManager instantiated and set to DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Duplicate MenuManager instance destroyed");
        }
    }

    void Start()
    {
        ShowStartScreen();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                ShowPauseMenu();
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartLevel();
        }
    }

    public void ShowStartScreen()
    {
        HideAllMenus();
        if (startScreenCanvas != null)
        {
            startScreenCanvas.SetActive(true);
            Debug.Log("Start screen shown");
        }
    }

    public void ShowMainMenu()
    {
        HideAllMenus();
        if (mainMenuCanvas != null)
        {
            mainMenuCanvas.SetActive(true);
            Debug.Log("Main menu shown");
        }
    }

    public void ShowOptions()
    {
        HideAllMenus();
        if (optionsCanvas != null)
        {
            optionsCanvas.SetActive(true);
            Debug.Log("Options menu shown");
        }
    }

    public void ShowSettings()
    {
        HideAllMenus();
        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(true);
            Debug.Log("Settings menu shown");
        }
    }

    public void ShowLevelSelection()
    {
        HideAllMenus();
        if (levelSelectionCanvas != null)
        {
            levelSelectionCanvas.SetActive(true);
            Debug.Log("Level selection menu shown");
            // Hier kannst du zusätzliche Logik hinzufügen, um nur freigeschaltete Levels anzuzeigen
        }
    }

    public void ShowWinMenu()
    {
        HideAllMenus();
        if (winCanvas != null)
        {
            winCanvas.SetActive(true);
            Debug.Log("Win menu shown");
        }
        PauseGame();
    }

    public void ShowPauseMenu()
    {
        HideAllMenus();
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(true);
            Debug.Log("Pause menu shown");
        }
        PauseGame();
    }

    public void HideAllMenus()
    {
        if (startScreenCanvas != null) startScreenCanvas.SetActive(false);
        if (mainMenuCanvas != null) mainMenuCanvas.SetActive(false);
        if (optionsCanvas != null) optionsCanvas.SetActive(false);
        if (settingsCanvas != null) settingsCanvas.SetActive(false);
        if (levelSelectionCanvas != null) levelSelectionCanvas.SetActive(false);
        if (winCanvas != null) winCanvas.SetActive(false);
        if (pauseMenuCanvas != null) pauseMenuCanvas.SetActive(false);
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            ResumeGame();
            SceneManager.LoadScene(nextSceneIndex);
            Debug.Log("Loading next level: " + nextSceneIndex);
        }
        else
        {
            Debug.Log("No more levels to load!");
            ShowMainMenu();
        }
    }

    public void LoadMainMenu()
    {
        ResumeGame();
        ShowMainMenu();
        Debug.Log("Main menu loaded");
    }

    public void StartGame()
    {
        ResumeGame();
        SceneManager.LoadScene("Level1"); // Setze hier den Namen des ersten Levels ein
        Debug.Log("Starting game at Level1");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game!");
        Application.Quit();
    }

    public void PauseGame()
    {
        Time.timeScale = 0f; // Pausiere das Spiel
        isGamePaused = true;
        Debug.Log("Game paused");
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Setze das Spiel fort
        isGamePaused = false;
        HideAllMenus();
        Debug.Log("Game resumed");
    }

    public void RestartLevel()
    {
        ResumeGame();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Lade das aktuelle Level neu
        Debug.Log("Restarting level: " + SceneManager.GetActiveScene().name);
    }
}