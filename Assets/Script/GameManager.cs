using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private int totalGoals = 0;
    private int activatedGoals = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameManager initialisiert");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RegisterGoal()
    {
        totalGoals++;
        Debug.Log("Total Goals: " + totalGoals);
    }

    public void GoalActivated()
    {
        activatedGoals++;
        Debug.Log($"Ziel aktiviert. Aktivierte Ziele: {activatedGoals}");
        CheckAllGoalsActivated();
    }

    public void GoalDeactivated()
    {
        activatedGoals--;
        Debug.Log($"Ziel deaktiviert. Aktivierte Ziele: {activatedGoals}");
    }

    private void CheckAllGoalsActivated()
    {
        Debug.Log($"Überprüfen, ob alle Ziele aktiviert sind: {activatedGoals} / {totalGoals}");
        if (activatedGoals == totalGoals)
        {
            Debug.Log("Alle Ziele aktiviert! Level abgeschlossen!");
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            Debug.Log("Nächstes Level wird geladen: " + nextSceneIndex);
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("Keine weiteren Level zum Laden!");
        }
    }

    public void RestartLevel()
    {
        Debug.Log("Level wird neu gestartet: " + SceneManager.GetActiveScene().name);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
