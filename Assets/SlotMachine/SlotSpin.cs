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
    public Image image;                   //���������� ��������� 
    public RectTransform reelTransform;  // ��������� ��������
    public float spinSpeed = 1000f;       // �������� ��������
    public float spinTime = 2f;          // ����� ��������
    public bool isSpinning = false;

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
        float elapsedTime = 0f;

        while (elapsedTime < spinTime)
        {
            reelTransform.anchoredPosition -= new Vector2(0, spinSpeed * Time.deltaTime);
            if (reelTransform.anchoredPosition.y <= -200) // ���� ������� ����� �� �������
            {

                reelTransform.anchoredPosition += new Vector2(0, 400); // ���������� ������� �����
                image.sprite = slotSymbols[Random.Range(0, slotSymbols.Length)].symbolSprite;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        isSpinning = false;
        AlignToSymbol(); // ����������� � ���������� �������
    }

    void AlignToSymbol()
    {
        float roundedY = Mathf.Round(0f) * 100;
        reelTransform.anchoredPosition = new Vector2(reelTransform.anchoredPosition.x, roundedY);
    }
}
