using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanicSystem : MonoBehaviour
{
    private int panicLv;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPanicLv(int lv)
    {
        panicLv = lv;
    }

    public void RefreshPanicSystem()
    {
        switch(panicLv)
        {
            case 0:
                break;
            case 1:
                break;
            case 2:
                break;
            case 3:
                break;
            default:
                break;
        }
    }

 
}
