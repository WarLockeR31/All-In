using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject help;
    public GameObject controls;
    // Start is called before the first frame update
    public void OpenHelp()
    {
        help.SetActive(true);
    }
    // Метод для выхода из игры
    public void QuitGame()
    {
        Debug.Log("Игра закрыта!");
        Application.Quit();
    }

    public void ShowControls()
    {
        controls.SetActive(!controls.activeSelf);
    }
    public void OnBack()
    {
        help.SetActive(false);
        controls.SetActive(false);
    }
}
