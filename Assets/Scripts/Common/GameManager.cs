using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const int MAX_HP = 3;
    private static GameManager Instance = null;

    [SerializeField] int curHp = 3;
    [SerializeField] float curPanic = 0.0f;

    private GameManager() {}

    void Start()
    {
        if(Instance == null)
        {
            GameObject.DontDestroyOnLoad(this);
            Instance = this;
        }
        else
        {
            GameObject.Destroy(this);
        }
    }

    void Update()
    {
        
    }

    public static GameManager GetInstance()
    {
        if(Instance == null)
        {
            Debug.Log("[WARNING] Game Manager is null.");
        }
        return Instance;
    }

    public void IncreaseHP(int hp)
    {
        int nextHp = curHp + hp;
        if(nextHp >= curHp)
        {
            curHp = MAX_HP;
        }
        else
        {
            curHp = nextHp;
        }

        HUDManager.GetInstance().RefreshHUD();
    }

    public void DecreaseHP(int hp)
    {
        int nextHp = curHp - hp;
        if(nextHp <= 0)
        {
            curHp = 0;
        }
        else
        {
            curHp = nextHp;
        }

        HUDManager.GetInstance().RefreshHUD();
    }

    public void setPanic(float panic)
    {
        if(panic >= 1.0f)
        {
            curPanic = 1.0f;
        }
        else if(panic <= 0.0f)
        {
            curPanic = 0.0f;
        }
        else
        {
            curPanic = panic;
        }

        HUDManager.GetInstance().RefreshHUD();
    }

    public int getHP()
    {
        return curHp;
    }

    public float getPanic()
    {
        return curPanic;
    }
}
