using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager Instance = null;
    
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
}
