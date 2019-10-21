using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGate : MonoBehaviour
{
    [SerializeField] GameObject door;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("플레이어 게이트 통과");
        }
    }
}
