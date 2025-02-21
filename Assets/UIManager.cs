using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject uiPanel;
    private Player player;

    #region Singleton
    public static UIManager Instance { get; private set; }

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

    void Start()
    {
        player = GetComponent<Player>();
        uiPanel.SetActive(false);
    }

    public void ToggleUI()
    {
        bool isActive = !uiPanel.activeSelf;
        uiPanel.SetActive(isActive);
        player.SetUIActive(isActive);

        if (!isActive)
        {
            ArenaManager.Instance.SpawnWave();
        }
    }
}

