using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private const int MAX_HP = 3;
    private const int MAX_MENTAL = 100;

    [Header("속성")]
    [Range(0, 3)]
    [SerializeField] private int hp;
    [Range(0, 300)]
    [SerializeField] private int mental;

    private PlayerMovement playerMovement;
    private PanicSystem panicSystem;

    // Start is called before the first frame update
    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        panicSystem = GetComponent<PanicSystem>();
    }

    // Update is called once per frame
    private void Update()
    {
        
    }

    public void IncreaseHP(int incHp)
    {
        int nextHp = hp + incHp;
        if(nextHp >= hp)
        {
            hp = MAX_HP;
        }
        else
        {
            hp = nextHp;
        }

        HUDManager.GetInstance().RefreshHUD();
    }

    public void DecreaseHP(int decHp)
    {
        int nextHp = hp - decHp;
        if(nextHp <= 0)
        {
            hp = 0;
        }
        else
        {
            hp = nextHp;
        }

        HUDManager.GetInstance().RefreshHUD();
    }

    public void IncreaseMental(int incMental)
    {
        int nextMental = mental + incMental;
        if(nextMental >= mental)
        {
            mental = MAX_MENTAL;
        }
        else
        {
            mental = nextMental;
        }

        UpdatePanicLv();
    }

    public void DecreaseMental(int decMental)
    {
        int nextMental = mental - decMental;
        if(nextMental <= 0)
        {
            mental = 0;
        }
        else
        {
            mental = nextMental;
        }

        UpdatePanicLv();
    }

    private void UpdatePanicLv()
    {
        if(mental < 120)
        {
            panicSystem.SetPanicLv(0);
        }
        else if(mental < 180)
        {
            panicSystem.SetPanicLv(1);
        }
        else if(mental < 260)
        {
            panicSystem.SetPanicLv(2);
            playerMovement.JumpPower = playerMovement.JumpPower - 2.0f;
        }
        else
        {
            panicSystem.SetPanicLv(3);
        }
    }
}
