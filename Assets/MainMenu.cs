using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // Метод для запуска игры
    public GameObject settings;
    public GameObject help;
    public void PlayGame()
    {
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

    public void OnBack()
    {
        settings.SetActive(false);
        help.SetActive(false);
    }
}
