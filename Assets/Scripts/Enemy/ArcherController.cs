using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherController : MonoBehaviour
{

    public enum ENEMY_TYPE
    {
        GOOMBA,
        ARCHER,
        GHOST,
        MEDUSA
    }

    [SerializeField] public ENEMY_TYPE enemyType;

    [SerializeField] public bool isAlive = true;
    [SerializeField] public float moveSpeed;
    [SerializeField] public bool attackPoss = false;
    [SerializeField] public int attackDamage;
    [SerializeField] public GameObject weapon;
    [SerializeField] public GameObject attackPosObject;
    [SerializeField] public int attackX;
    [SerializeField] public int attackY;
    //[SerializeField] public float weaponMoveSpeed;
    //[SerializeField] public float weaponAttackDamage;
    //[SerializeField] public float weaponGravity;
    [SerializeField] public int nextMove;
    [SerializeField] public GameObject player;


    private Rigidbody2D rigid;
    private bool dirLeft = false;
    private Vector3 attackPosition;
    private GameObject newWeapon;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        TypeCheck(enemyType); // 적 타입 체크
    }

    // Start is called before the first frame update
    void Start()
    {
        //attackPosition = attackPosObject.GetComponent<Transform>().position;
        StartCoroutine("AttackToPlayer");
    }

    // Update is called once per frame
    void Update()
    {
        /*
        // 공격가능한 상태면 화살 움직임
        if(attackPoss == true)
        {
            
            
            Vector3 targetPosition = player.transform.position;
            targetPosition.y = 0f;

            newWeapon.transform.LookAt(targetPosition);
            Vector3 newPosition = Vector3.MoveTowards(newWeapon.transform.position, targetPosition, 
                                                        weaponMoveSpeed * Time.deltaTime);
            newWeapon.transform.position = newPosition;
            
        }
        */

    }

    private void FixedUpdate()
    {
        //움직임
        rigid.velocity = new Vector2(nextMove * moveSpeed, rigid.velocity.y);

        //플랫폼 확인
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 3, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Debug.Log("더이상 플랫폼이 없어요!!!");
            nextMove *= -1;
            Flip();
        }
    }

    void EnemyGoomba()
    {
        //Debug.Log("나는 Goomba");

    }

    void EnemyArcher()
    {
        Debug.Log("나는 Archer");
    }

    void EnemyGhost()
    {
        //Debug.Log("나는 Ghost");
    }

    void EnemyMedusa()
    {
        //Debug.Log("나는 Medusa");
    }


    void TypeCheck(ENEMY_TYPE type)
    {
        switch (type)
        {
            case ENEMY_TYPE.GOOMBA:
                EnemyGoomba();
                break;

            case ENEMY_TYPE.ARCHER:
                EnemyArcher();
                break;

            case ENEMY_TYPE.GHOST:
                EnemyGhost();
                break;

            case ENEMY_TYPE.MEDUSA:
                break;
        }
    }

    void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    IEnumerator AttackToPlayer()
    {
        if (attackPoss == true)
        {
            Debug.Log("--공격시작--");
            yield return new WaitForSeconds(1f);

            newWeapon = GameObject.Instantiate(weapon,Vector3.zero, Quaternion.identity);
            newWeapon.transform.SetParent(attackPosObject.transform);
            newWeapon.transform.position = new Vector2(0, 0);

            Rigidbody2D arrowRigid = weapon.GetComponent<Rigidbody2D>();
            //arrowRigid.AddForce(new Vector2(attackX, attackY), ForceMode2D.Impulse);


            yield return new WaitForSeconds(1f);
        }

    }

    
}

