using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    // ����� ��� ������� ����
    public GameObject settings;
    public GameObject help;
    public GameObject controls;
    public void PlayGame()
    {
        Time.timeScale = 1f;
        // �������� ��������� ����� �� ������� � Build Settings
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
    }

    // ����� ��� �������� � ���������
    public void OpenOptions()
    {
        settings.SetActive(true);
    }
    public void OpenHelp()
    {
        help.SetActive(true);
    }
    // ����� ��� ������ �� ����
    public void QuitGame()
    {
        Debug.Log("���� �������!");
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
