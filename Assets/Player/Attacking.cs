using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Attacking : MonoBehaviour
{
    private Player player;

    private enum attack {jab, slam, kick };

    private attack nextAttack;

    private Queue<attack> attacks;

    private bool canPress;

    private float delay;

    private bool slapOnCoolDown;

    [SerializeField]private float delayValue;


    #region Singleton
    public static Attacking Instance { get; private set; }

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

    private void NextAttack()
    {
        switch(nextAttack)
        {
            case attack.jab:nextAttack = attack.slam; break;
            
            case attack.slam:nextAttack = attack.kick; break;

            case attack.kick:nextAttack = attack.jab; break;
        }
    }
    private void Jab()
    {
        player.animator.ResetTrigger("slam");
        player.animator.ResetTrigger("kick");
        player.animator.SetTrigger("jab");
    }
    private void Slam()
    {
        player.animator.ResetTrigger("jab");
        player.animator.ResetTrigger("kick");
        player.animator.SetTrigger("slam");
    }
    private void Kick()
    {
        player.animator.ResetTrigger("jab");
        player.animator.ResetTrigger("slam");
        player.animator.SetTrigger("kick");
    }

    private void Slap()
    {
        if (!slapOnCoolDown && Input.GetKeyDown(KeyCode.F))
        {
            player.animator.SetTrigger("slap");
            StartCoroutine(SlapCoolDown());
        }
            
    }

    private IEnumerator SlapCoolDown()
    {
        slapOnCoolDown = true;
        yield return new WaitForSeconds(2);
        slapOnCoolDown = false;
    }

    private void InitAttack(attack val)
    {
            switch (val)
            {
                case attack.jab: { 
                        
                        Jab();
                        break; }

                case attack.slam: {
                        
                        Slam(); 
                        break; }
                        
                case attack.kick: { 
                        
                        Kick(); 
                        break; }

            }
    }
    bool AnimatorIsPlaying() { return (player.animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !player.animator.IsInTransition(0)); }
    
    bool AnimatorIsPlaying(string stateName)
    {
        return player.animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }
    bool AttackCheck()
    {
        return !(AnimatorIsPlaying("Jab")||AnimatorIsPlaying("Kick")||AnimatorIsPlaying("Slam"));
    }

    private void InputManager()
    {
            if (Input.GetMouseButtonDown(0)&&attacks.Count<2)
            {
                attacks.Enqueue(nextAttack);
                NextAttack();
            };
            if (AttackCheck()&&!player.animator.IsInTransition(0)&&attacks.Count>0)
            {
                    Debug.Log(attacks.Peek());
                    InitAttack(attacks.Dequeue()); 
                    canPress = true;
                    delay = delayValue;
            }
    }

    private void ResetCombination()
    {
        if (canPress == false&&attacks.Count==0)
            nextAttack = attack.jab;
    }
    //=====================BLOCK==========================================================================
    private void Block()
    {
        
        if (Input.GetMouseButtonDown(1))
        {
            StartBlocking();
        }
        if (Input.GetMouseButtonUp(1))
        {
            //|| Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0)
            Unblock();
        }
    }
    private void StartBlocking()
    {
        player.animator.SetBool("isBlocking", true);
    }
    private void StopBlocking()
    {
        player.animator.SetBool("isBlocking", false);
    }

    public void Unblock()
    {
        StopBlocking();
        player.animator.ResetTrigger("jab");
        player.animator.ResetTrigger("slap");
        player.animator.ResetTrigger("slam");
        player.animator.ResetTrigger("kick");
        nextAttack = attack.jab;
        attacks.Clear();
    }
    //=================================================================================================





    void Start()
    {
        delay = delayValue;
        attacks = new Queue<attack>();
        nextAttack = attack.jab;
        player = GetComponent<Player>();
    }




    // Update is called once per frame
    void Update()
    {
        if (player.isUIOpen)
            return; 
            
        //if (!player.animator.GetBool("isBlocking"))
        //{
            
        //}
        InputManager();
        Slap();

        Block();
        ResetCombination();
    }
    private void FixedUpdate()
    {
        if (canPress)
        {
            delay-=0.1f;
        }
        if (delay <=0)
        {
            canPress = false;
        }
    }
}
