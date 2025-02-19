using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class SlotSpin : MonoBehaviour
{
    public SlotSymbol[] slotSymbols;
    public Image image;                   //Содержимое барабашки 
    public RectTransform reelTransform;  // Контейнер барабана
    public float spinSpeed = 1000f;       // Скорость кручения
    public float spinTime = 2f;          // Время кручения
    public bool isSpinning = false;
    public int SpinCount = 5;
    public int objIndex =0;
    public bool guaranteeDrop = false;

    

    public void StartSpin()
    {
        if (!isSpinning)
        {
            StartCoroutine(SpinReel());
        }
    }

    IEnumerator SpinReel()
    {
        isSpinning = true;
        int currSpin=0;
        while (true)
        {
            reelTransform.anchoredPosition -= new Vector2(0, spinSpeed * Time.deltaTime);
            if (reelTransform.anchoredPosition.y <= -200) // Если символы вышли за границу
            {
                reelTransform.anchoredPosition += new Vector2(0, 400);// Перемещаем обратно вверх
                currSpin++;
                if (currSpin==SpinCount&&guaranteeDrop)
                    image.sprite = slotSymbols[objIndex].symbolSprite;
                else
                if (currSpin==SpinCount&&!guaranteeDrop)
                    image.sprite = slotSymbols[objIndex].symbolSprite;
                else
                    image.sprite = slotSymbols[Random.Range(0, slotSymbols.Length)].symbolSprite;
               
            }
            if (currSpin==SpinCount)
                break;
            yield return null;
        }

        isSpinning = false;
        AlignToSymbol(); // Выравниваем к ближайшему символу
    }

    void AlignToSymbol()
    {
        float roundedY = Mathf.Round(0f) * 100;
        reelTransform.anchoredPosition = new Vector2(reelTransform.anchoredPosition.x, roundedY);
    }
}
