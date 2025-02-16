using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SlotMachineHandle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;
    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Selected", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Selected", false);
    }


    public void OnHandleClicked()
    {
        animator.SetTrigger("Pull");
    }
}
