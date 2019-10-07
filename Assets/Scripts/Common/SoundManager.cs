using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager Instance = null;

    private SoundManager() {}

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

    public static SoundManager GetInstance()
    {
        if(Instance == null)
        {
            Debug.Log("[WARNING] Sound Manager is null.");
        }
        return Instance;
    }
}
