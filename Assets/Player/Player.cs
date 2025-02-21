using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int money = 100;

    private int playerLevel = 0;

    [SerializeField]
    private float damage = 10;
    public float Damage { get { return damage; } }

    public Animator animator;

    public bool isUIOpen = false;

    public enum abilities {dash, doublejump, walljump, hook}

    public Dictionary<int, bool> unlockables;

    public TextMeshProUGUI moneyShow;


    #region Singleton
    public static Player Instance { get; private set; }

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

    public void SetUIActive(bool state)
    {
        isUIOpen = state;
        // Останавливаем управление камерой
        Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = state;
    }

   

    void Start()
    {
        unlockables = new Dictionary<int, bool>();
        unlockables.Add(0, false);
        unlockables.Add(1, false);
        unlockables.Add(2, false);
        unlockables.Add(3, false);
    }

    void Update()
    {

        moneyShow.text = money.ToString();
        //foreach(var kvp in unlockables)
        //{
        //    //Debug.Log($"{kvp.Key} -> {kvp.Value}\n");
        //}
    }

    

    public void TakeDamage(float damage)
    {
        if (money <= 0)
            return;

        money -= Mathf.RoundToInt(animator.GetBool("isBlocking") ? damage / 2 : damage);

        if (money <= 0)
        {
            Debug.LogWarning("PlayerDead");
        }
    }
}
