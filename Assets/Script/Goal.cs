using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    private bool isTriggered = false;

    private void Start()
    {
        GameManager.Instance.RegisterGoal();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isTriggered)
            {
                isTriggered = true;
                Debug.Log($"{other.name} hat das Ziel aktiviert.");
                GameManager.Instance.GoalActivated();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (isTriggered)
            {
                isTriggered = false;
                Debug.Log($"{other.name} hat das Ziel verlassen.");
                GameManager.Instance.GoalDeactivated();
            }
        }
    }
}
