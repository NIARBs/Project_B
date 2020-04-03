using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultMove : MonoBehaviour
{
    public float Speed = 5;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.D))
        {
            transform.Translate(Vector2.right * Speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(Vector2.left * Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector2.up * Speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector2.down * Speed * Time.deltaTime);
        }
    }
}
