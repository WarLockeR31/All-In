using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CasinoManager : MonoBehaviour
{
    public Player player;

    public GetColorFromImage proccedColor;

    public RollTheBall ball;

    private bool canCheck;

    public TextMeshProUGUI Bet;

    private bool block;

    

    private int currentBet = 0;

    [SerializeField]private Animator[] ColorButtons;

    public enum colors{red, green, black, none }

    private colors currentColorChoice;

    public void incBet()
    {
        if (block) return;
        if (currentBet+20>player.money)
            currentBet=player.money;
        else
            currentBet+=20;
    }


    public void decBet()
    {
        if (block) return;
        if ( currentBet-20<0)
        {
            currentBet=0;
        }
        else
            currentBet-=20;
    }

    public void  PlayerChoiceRed()
    {
        if(block) return;
        currentColorChoice = colors.red;
        ColorButtons[1].SetBool("ispressed", false);
        ColorButtons[2].SetBool("ispressed", false);

    }
    public void PlayerChoiceGreen()
    {
        if(block) return;
        currentColorChoice = colors.green;
        ColorButtons[0].SetBool("ispressed", false);
        ColorButtons[2].SetBool("ispressed", false);

    }
    public void PlayerChoiceBlack()
    {
        if(block) return;
        currentColorChoice = colors.black;
        ColorButtons[0].SetBool("ispressed", false);
        ColorButtons[1].SetBool("ispressed", false);
    }


    private void Reset()
    {
        foreach(var item in ColorButtons)
        {
            item.SetBool("ispressed", false);
        }
        currentBet = 0;
        currentColorChoice= colors.none; 
    }
    private colors CheckColor()
    {
        if (proccedColor.color.r>0.7f)
        {
            return colors.red;
        }
        else
            if (proccedColor.color.g > 0.6)
                return colors.green;
            else
                return colors.black;
    }

    private void RouletteCheck()
    {
        if (CheckColor()==currentColorChoice)
        {
            if (CheckColor() == colors.green)
                player.money+=10*currentBet;
            else
                player.money+=2*currentBet;
        }
        Reset();
            
    }
    IEnumerator EBLoUTINOE()
    {
        yield return new WaitForSeconds(0.01f);

        while(!ball.stopped)
        {
            block = true;
            foreach(var item in ColorButtons)
            {
                item.GetComponentInParent<BetButton>().block = true;
            }
            yield return new WaitForSeconds(0.2f);
        }
        RouletteCheck();
        block = false;
        foreach (var item in ColorButtons)
        {
            item.GetComponentInParent<BetButton>().block = false;
        }
    }
    public void StartRoulette()
    {

        if (block||currentBet==0||(!ColorButtons[0].GetBool("ispressed")&&!ColorButtons[1].GetBool("ispressed")&&!ColorButtons[2].GetBool("ispressed")))
            return;
        ball.stopped = false;
        StartCoroutine(EBLoUTINOE());
        ball.StartingSpeed = UnityEngine.Random.Range(100, 201);
        ball.currentSpeed = ball.StartingSpeed;
        player.money-=currentBet;
    }
    private void Start()
    {
        currentColorChoice = colors.none;
        block = false;
        
    }

    private void Update()
    { 
        

        Bet.text = currentBet.ToString();
        Debug.Log(player.money);
    }
}
