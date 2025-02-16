using UnityEngine;
using System.Collections;


public class SlotMachine : MonoBehaviour
{
    public SlotSpin[] reels; // Массив барабанов
    public float delayBetweenReels = 0.5f; // Задержка между стартами барабанов
    

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
