using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorGate : MonoBehaviour
{
    [Header ("- 오브젝트 설정")][Tooltip ("문 오브젝트를 넣어주세요.")]
    [SerializeField] GameObject door;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Debug.Log("플레이어 게이트 통과");
        }
    }
}
