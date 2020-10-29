using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
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
    
    public void NextStage()
    {

    }
}
