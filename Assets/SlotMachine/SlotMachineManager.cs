using UnityEngine;
using System.Collections;


public class SlotMachine : MonoBehaviour
{
    public SlotSpin[] reels; // Массив барабанов
    public float delayBetweenReels = 0.5f; // Задержка между стартами барабанов
    public Transform SlotMachineTransform;
    [SerializeField]private float GoofinessAmount = 2f;
    private bool big = false;
    private bool rotationCheck = false;

    public void StartSlotMachine()
    {
        if(!IsSpinnig())
            StartCoroutine(SpinAllReels());
    }

    IEnumerator SpinAllReels()
    {
        for (int i = 0; i < reels.Length; i++)
        {
            reels[i].StartSpin();
            yield return new WaitForSeconds(delayBetweenReels);
        }
    }
    private void Update()
    {
        GoofyAhhAnim();
    }


    private void GoofyAhhAnim()
    {
        Scale();
        Rotate();
    }
    
    private void Scale()
    {
        if (IsSpinnig())
        {
            if (SlotMachineTransform.localScale.y >= 1.05)
            {
                big = true;
            }
            if (SlotMachineTransform.localScale.y<=0.95)
                big=false;
            
            float scaleChange = GoofinessAmount * Time.deltaTime;
            
            if (big)
                SlotMachineTransform.localScale-=new Vector3(0f, scaleChange, 0f);
            else
                SlotMachineTransform.localScale+=new Vector3(0f, scaleChange, 0f);
        }
    }
    void Rotate()
    {
        if (IsSpinnig())
        {
            Vector3 realRotation = SlotMachineTransform.eulerAngles;
            realRotation.z = (realRotation.z > 180) ? realRotation.z - 360 : realRotation.z;
            if (realRotation.z >= 3f)
                rotationCheck = true;
            if (realRotation.z <= -3f)
                rotationCheck=false;

            float scaleChange = 10f* GoofinessAmount * Time.deltaTime;

            if (rotationCheck)
                SlotMachineTransform.Rotate(new Vector3(0f, 0f, -scaleChange));
            else
                SlotMachineTransform.Rotate(new Vector3(0f, 0f, scaleChange));
            Debug.Log(SlotMachineTransform.rotation.z);
        }
    }


    bool IsSpinnig()
    {
        for (int i = 0;i < reels.Length;i++)
        {
            if (reels[i].isSpinning) return true;
        }
        return false;
    }
}
