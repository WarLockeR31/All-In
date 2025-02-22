using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private AudioSource jab;
    [SerializeField] private AudioSource slam;
    [SerializeField] private AudioSource kick;
    [SerializeField] private AudioSource slap;

    [SerializeField] private AudioSource uiClick;

    public int money = 100;


    [SerializeField]
    private float damage = 10;
    public float Damage { get { return damage; } }

    public Animator animator;

    public bool isUIOpen = false;

    public enum abilities {dash, doublejump, walljump, hook}

    public Dictionary<int, bool> unlockables;

    public TextMeshProUGUI moneyShow;

    private bool isInvincible;
    public bool IsInvincible {  get { return isInvincible; } }

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

    public void UiClick()
    {
        uiClick.Play();
    }


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
        if (money <= 0 || isInvincible)
            return;

        money -= Mathf.RoundToInt(animator.GetBool("isBlocking") ? damage / 2 : damage);
        StartCoroutine(Invincibility());
        ScreenShake.Instance.takeDamageShake();
        TakeDamageVignette.Instance.InvokeDamageVignette();

        if (money <= 0)
        {
            Debug.LogWarning("PlayerDead");
        }
    }

    private IEnumerator Invincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(0.5f);
        isInvincible = false;
    }

    [Header("SlowMo")]
    public float slowmoTimescale = 0.2f;
    public float slowmoDuration = 0.1f;

    public void TriggerSlowmo()
    {
        StartCoroutine(DoSlowmo());
    }

    private IEnumerator DoSlowmo()
    {
        Time.timeScale = slowmoTimescale;
        //Time.fixedDeltaTime = Time.timeScale * 0.02f;

        float timer = 0;
        while (timer < slowmoDuration)
        {
            Time.timeScale = Mathf.Lerp(slowmoTimescale, 1, timer / slowmoDuration);
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        Time.timeScale = 1f;
        //Time.fixedDeltaTime = 0.02f;
    }

    public void PlayPunchSound()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jab"))
        {
            if (jab.isPlaying)
                return;
            jab.Play();
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slap"))
        {
            if (slap.isPlaying)
                return;
            slap.Play();
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Slam"))
        {
            if (slam.isPlaying)
                return;
            slam.Play();
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Kick"))
        {
            if (kick.isPlaying)
                return;
            kick.Play();
        }
    }
}
