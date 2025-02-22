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
        help.SetActive(false);
        controls.SetActive(false);
    }
}
