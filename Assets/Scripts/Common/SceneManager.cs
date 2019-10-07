using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private static SceneManager Instance = null;

    private SceneManager() {}

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

    public static SceneManager GetInstance()
    {
        if(Instance == null)
        {
            Debug.Log("[WARNING] Scene Manager is null.");
        }
        return Instance;
    }
}
