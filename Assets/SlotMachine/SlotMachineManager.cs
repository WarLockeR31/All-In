using UnityEngine;
using System.Collections;


public class SlotMachine : MonoBehaviour
{
    public SlotSpin[] reels; // ������ ���������
    public float delayBetweenReels = 0.5f; // �������� ����� �������� ���������
    

    public void StartSlotMachine()
    {
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
}
