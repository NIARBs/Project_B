using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherArrowController : MonoBehaviour
{

    [SerializeField] float arrowMoveSpeed;

    private Rigidbody2D myRigid;
    public Transform Target;
    public float moveAngle = 45.0f;
    public float gravity = 9.8f;

    private Transform myTransform;

    //private Vector3 prevPosition;
    // Start is called before the first frame update
    void Start()
    {
        myTransform = transform;
        myRigid = GetComponent<Rigidbody2D>();
        //StartCoroutine(SimulateArrow());
        //prevPosition = transform.position; //처음 위치 저장
        //GetComponent<Rigidbody2D>().AddForce(transform.forward * arrowMoveSpeed, ForceMode2D.Impulse);
    }

    private void Update()
    {
        //transform.rotation = Quaternion.LookRotation(myRigid.velocity);

        
    }

    private void FixedUpdate()
    {

        /*
        Vector3 deltaPos = transform.position - prevPosition; //변한 위치 - 처음 위치 = 방향
        float angle = Mathf.Atan2(deltaPos.y, deltaPos.x) * Mathf.Rad2Deg; //삼각함수로 각도 구함
        
        if(angle != 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, angle);
            prevPosition = transform.position;
            
        }
        */
    }

    IEnumerator SimulateArrow()
    {
        float targetDistance = Vector3.Distance(myTransform.position, Target.position);
        float arrowVelocity = targetDistance / (Mathf.Sin(2 * moveAngle * Mathf.Deg2Rad) / gravity);
        float Vx = Mathf.Sqrt(arrowVelocity) * Mathf.Cos(moveAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(arrowVelocity) * Mathf.Sin(moveAngle * Mathf.Deg2Rad);
        float moveDuration = targetDistance / Vx;

        myTransform.rotation = Quaternion.LookRotation(Target.position - myTransform.position);

        float elapseTime = 0;

        while(elapseTime < moveDuration)
        {
            myTransform.Translate(0, (Vy - (gravity * elapseTime)) * Time.deltaTime, Vx * Time.deltaTime);
            elapseTime += Time.deltaTime;
            yield return null;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") || other.CompareTag("Platform"))
        {
            Destroy(this);
        }
    }
}
