using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonManagement : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 0;
    }

    public void LoadScene(string name)
    {
        Application.LoadLevel(name);
    }

    public void PauseButton()
    {
        Time.timeScale = 0;
    }

    public void StartButton()
    {
        Time.timeScale = 1;
    }
}
