using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Szenen-übergreifender Singleton, der
/// • Ziele (Goals) zählt,
/// • Level-Complete auslöst,
/// • Restart / Next-Level anstößt.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    int totalGoals;        // Gesamtzahl der Ziele in der aktuellen Szene
    int activatedGoals;    // Anzahl bereits aktivierter Ziele

    PauseMenuManager pauseUI;   // Cache für das UI in jeder Gameplay-Szene

    //─────────────────────────────────────────────────────────────
    #region Singleton + Scene-Hooks
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    /// <summary>Wird aufgerufen, sobald eine neue Szene fertig geladen ist.</summary>
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Zähler zurücksetzen (Goals registrieren sich danach in ihrem Start())
        totalGoals = 0;
        activatedGoals = 0;

        // Neues Pause-UI in dieser Szene suchen (kann in der MenuScene null sein)
        pauseUI = FindObjectOfType<PauseMenuManager>();
    }
    #endregion

    //─────────────────────────────────────────────────────────────
    #region Goal-Callbacks  (aufgerufen von Goal.cs)
    public void RegisterGoal() => totalGoals++;
    public void GoalActivated() { activatedGoals++; CheckGoals(); }
    public void GoalDeactivated() => activatedGoals--;

    void CheckGoals()
    {
        if (totalGoals > 0 && activatedGoals == totalGoals)
        {
            Debug.Log("Alle Ziele aktiviert → Level geschafft!");
            if (pauseUI) pauseUI.ShowLevelComplete();   // Level-Complete-UI
            else LoadNextLevel();               // Fallback (falls UI fehlt)
        }
    }
    #endregion

    //─────────────────────────────────────────────────────────────
    #region Öffentliche Flow-Methoden
    public void RestartLevel()
    {
        // Sicherheitshalber Zeitfaktor zurücksetzen
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1;
        int next = SceneManager.GetActiveScene().buildIndex + 1;

        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene("Menu");   // oder Credits-Szene
    }

    /// <summary>
    /// Kann von Spiellogik aufgerufen werden, wenn der Spieler stirbt.
    /// </summary>
    public void TriggerGameOver()
    {
        if (pauseUI) pauseUI.ShowGameOver();
    }
    #endregion
}
