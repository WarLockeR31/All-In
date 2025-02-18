using UnityEngine;

public class UIManager : MonoBehaviour
{
    public GameObject uiPanel;
    private Player player;

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
    }
}

