using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public bool isPaused = false;
    public GameObject pause;

    #region Singleton
    public static PauseManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion
    void Update()
    {
        // ������� ������� Escape ������/������� �����
        if (Input.GetKeyDown(KeyCode.Escape)&&!Player.Instance.isDead)
        {
            if (isPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Attacking.Instance.enabled = false;
        Player.Instance.GetComponent<FirstPersonController>().enabled = false;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        pause.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        
        Cursor.lockState = UIManager.Instance.uiPanel.activeSelf ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = UIManager.Instance.uiPanel.activeSelf;
        Time.timeScale = 1;

        
        pause.SetActive(false);

        isPaused = false;
        Player.Instance.GetComponent<FirstPersonController>().enabled = true;
        Attacking.Instance.enabled = true;

    }
}
