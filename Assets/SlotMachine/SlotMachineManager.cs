using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class SlotMachine : MonoBehaviour
{
    public SlotSpin[] reels; // Массив барабанов
    public float delayBetweenReels = 0.5f; // Задержка между стартами барабанов
    public RectTransform SlotMachineTransform;
    [SerializeField] private float GoofinessAmount = 2f;
    private bool big = false;
    private bool rotationCheck = false;
    private int dropRate = 20;
    public Player player;
    private int[] reelcounter;
    private List<int> exclusions = new List<int>();
    [SerializeField] private int cost = 50;


    int RandomWithList(int min, int max, List<int> exclusions)
    { 
        List<int> possibleNumbers = new List<int>();
        if (exclusions.Count > 0)
            for (int i = min; i < max; i++)
            {
                if (!exclusions.Contains(i)) possibleNumbers.Add(i);
            }
        else
            return Random.Range(min, max);

        return possibleNumbers[Random.Range(0, possibleNumbers.Count)];
    }
    int RandomWithExclusion(int min, int max, int excluded)
    {
        int random;
        do
        {
            random = Random.Range(min, max);
        } while (random == excluded);

        return random;
    }
    public void StartSlotMachine()
    {
        if (IsSpinnig()||(player.money < cost))
            return;
            StartCoroutine(SpinAllReels());
        player.money = player.money - cost;
    }

        IEnumerator SpinAllReels()
    {
        int chance = Random.Range(1, 101);
        if (chance <= dropRate&&exclusions.Count!=player.unlockables.Count)
        {
            int unlockable = RandomWithList(0, 4, exclusions);
            player.unlockables[unlockable] = true;
            exclusions.Add(unlockable);
            foreach (var reel in reels)
            {
                reel.objIndex = unlockable; 
            }
            dropRate = 20;
        }
        else
        {
            reelcounter = new int[] { 2, 2, 2, 2 };
            foreach (var item in reels)
            {
                int randobj = Random.Range(0, 4);
                reelcounter[randobj]--;
                if (reelcounter[randobj]>0)
                    item.objIndex = randobj;
                else
                    item.objIndex = reelcounter[RandomWithExclusion(0, 4, randobj)];
            }
            if (dropRate<100)
                dropRate+=15;
        }
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

    private void Start()
    {
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
        else
            SlotMachineTransform.localScale = new Vector3 (1f, 1f, 0f);
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
            {
                SlotMachineTransform.Rotate(new Vector3(0f, 0f, -scaleChange));
            }
            else
            {
                SlotMachineTransform.Rotate(new Vector3(0f, 0f, scaleChange));
            }
                
            Debug.Log(SlotMachineTransform.rotation.z);
        }
        else
        {
            SlotMachineTransform.localEulerAngles = Vector3.zero;
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

    private void OnEnable()
    {
        SlotMachineTransform.localEulerAngles = Vector3.zero;
    }
}
