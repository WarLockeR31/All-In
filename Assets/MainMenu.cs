using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Метод для запуска игры
    public GameObject settings;
    public GameObject help;
    public GameObject controls;
    public void PlayGame()
    {
        Time.timeScale = 1f;
        // Загрузка следующей сцены по порядку в Build Settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    // Метод для перехода в настройки
    public void OpenOptions()
    {
        settings.SetActive(true);
    }
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
        settings.SetActive(false);
        help.SetActive(false);
        controls.SetActive(false);
    }
}
