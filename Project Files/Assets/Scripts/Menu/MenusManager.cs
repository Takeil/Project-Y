using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenusManager : MonoBehaviour
{
    public Text difficultyText;
    int diff = 0;

    private void Start()
    {
        diff = PlayerPrefs.GetInt("Difficulty",1);
        ShowDifficulty();
    }

    public void OnExitButton()
    {
        Application.Quit();
        Debug.Log("Exit");
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }

    public void IncrementDifficulty()
    {
        diff++;

        if (diff > 3)
        {
            diff = 1;
        }

        ShowDifficulty();
        SetDifficulty(diff);
    }

    void ShowDifficulty()
    {
        switch (diff)
        {
            case 1:
                difficultyText.text = "DIFFICULTY\n<b><color=#00ff00>FAIR</color></b>";
                break;
            case 2:
                difficultyText.text = "DIFFICULTY\n<b><color=#ffff00>UNFAIRLY FAIR</color></b>";
                break;
            case 3:
                difficultyText.text = "DIFFICULTY\n<b><color=#ff0000>UNFAIR</color></b>";
                break;
            default:
                difficultyText.text = "INVALID";
                Debug.Log("INVALID");
                break;
        }
    }

    public void SetDifficulty(int val)
    {
        PlayerPrefs.SetInt("Difficulty", val);
    }
}