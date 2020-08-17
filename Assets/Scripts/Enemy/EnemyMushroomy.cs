using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class EnemyMushroomy : Enemy
{
    [SerializeField] private Vector3 nextFrontVec;

    [SerializeField] private float minNextMoveStateTime = 2.0f;
    [SerializeField] private float maxNextMoveStateTime = 5.0f;
    private float nextMoveStateTime;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Start()
    {
        CheckEnemyType(EEnemyType.Mushroomy);

        //StartCoroutine("StartEnemyAI");
        GetComponent<Animator>().SetBool("Move", true);
        nextMove = -1;
    }

    private void FixedUpdate()
    {
        //움직임
        rigid.velocity = new Vector2(nextMove * moveSpeed, rigid.velocity.y);

        //플랫폼 확인
        Vector2 downVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(downVec, Vector3.down, new Color(0, 1, 0));
        Debug.DrawRay(frontVec, nextFrontVec, new Color(0, 1, 0));

        RaycastHit2D downRayHit = Physics2D.Raycast(downVec, Vector3.down, 1, LayerMask.GetMask("Ground"));
        RaycastHit2D frontRayHit = Physics2D.Raycast(frontVec, nextFrontVec, 0.4f, LayerMask.GetMask("Ground"));
        if(downRayHit.collider == null || frontRayHit.collider != null)
        {
            //Debug.Log("더이상 플랫폼이 없어요!!!");
            Turn();
        }
    }

    private void NextFrontVec()
    {
        if(nextMove == 1)
        {
            nextFrontVec = Vector3.right;
        }
        else if(nextMove == -1)
        {
            nextFrontVec = Vector3.left;
        }
    }

    private void Turn()
    {
        nextMove *= -1;

        spriteRenderer.flipX = nextMove == 1;

        NextFrontVec();
        //StopCoroutine("StartEnemyAI");
        //StartCoroutine("StartEnemyAI");
    }

    private IEnumerator StartEnemyAI()
    {
        while(true)
        {
            nextMove = Random.Range(-1, 2);

            NextFrontVec();
            
            if(nextMove != 0)
            {
                spriteRenderer.flipX = nextMove == 1;
            }

            nextMoveStateTime = Random.Range(minNextMoveStateTime, maxNextMoveStateTime);
            yield return new WaitForSeconds(nextMoveStateTime);
        }
    }
}
