using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BetButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private AudioSource betSound;

    private Animator animator;
    private RectTransform buttonRect;

    public bool block;

    private Vector2 originalSize;
    public float scaleFactor = 0.9f; // ��������� ��������� ������ (��������, 0.9 = 90%)
    public float animationSpeed = 0.1f; // �������� ��������
    private void Start()
    {
        betSound = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        buttonRect = GetComponent<RectTransform>();
        originalSize = buttonRect.sizeDelta;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (block) return;
        animator.SetBool("selected", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (block) return;
        animator.SetBool("selected", false);
    }


    public void OnButtonClicked()
    {
        if (block) return;

        betSound.Play();
        animator.SetBool("ispressed", !animator.GetBool("ispressed"));
        StopAllCoroutines();
        StartCoroutine(GoofyAhhButton());
    }

    IEnumerator GoofyAhhButton()
    {
        Vector2 targetSize = originalSize * (2-scaleFactor); // ��������� ������

        float elapsedTime = 0;
        while (elapsedTime < animationSpeed)
        {
            buttonRect.sizeDelta = Vector2.Lerp(buttonRect.sizeDelta, targetSize, elapsedTime / animationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonRect.sizeDelta = targetSize;

        yield return new WaitForSeconds(0.05f); // ��������� �����

        // ���������� �������
        elapsedTime = 0;
        while (elapsedTime < animationSpeed)
        {
            buttonRect.sizeDelta = Vector2.Lerp(buttonRect.sizeDelta, originalSize, elapsedTime / animationSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        buttonRect.sizeDelta = originalSize;
    }
}

