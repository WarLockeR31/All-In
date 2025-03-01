using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CloseButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;
    private RectTransform buttonRect;
    public bool block;
    private Vector2 originalSize;
    public float scaleFactor = 0.9f; // ��������� ��������� ������ (��������, 0.9 = 90%)
    public float animationSpeed = 0.1f; // �������� ��������
    private void Start()
    {
        animator = GetComponent<Animator>();
        buttonRect = GetComponent<RectTransform>();
        originalSize = buttonRect.sizeDelta;
    }

    void Update()
    {
        block = CasinoManager.Instance.Block;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (block || SlotMachine.Instance.IsSpinnig()) { return; }
        animator.SetBool("selected", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (block || SlotMachine.Instance.IsSpinnig()) { return; }
        animator.SetBool("selected", false);
    }


    public void OnButtonClicked()
    {
        if (block || SlotMachine.Instance.IsSpinnig()) { return; }
        Player.Instance.UiClick();
        StopAllCoroutines();
        StartCoroutine(GoofyAhhButton());
    }
    IEnumerator GoofyAhhButton()
    {
        Vector2 targetSize = originalSize * (2 - scaleFactor); // ��������� ������

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

        UIManager.Instance.ToggleUI();
    }
}
