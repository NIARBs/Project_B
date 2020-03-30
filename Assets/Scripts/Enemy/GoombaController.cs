using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoombaController : MonoBehaviour
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
    //[SerializeField] public float attackCycle;
    //[SerializeField] public float weaponMoveSpeed;
    //[SerializeField] public float weaponAttackDamage;
    //[SerializeField] public float weaponGravity;
    [SerializeField] public int nextMove;

    private Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        TypeCheck(enemyType); // 적 타입 체크
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
   
    }

    private void FixedUpdate()
    {
        //움직임
        rigid.velocity = new Vector2(nextMove*moveSpeed, rigid.velocity.y);

        //플랫폼 확인
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Debug.Log("더이상 플랫폼이 없어요!!!");
            nextMove *= -1;
           
        } 
    }

    void EnemyGoomba()
    {
        Debug.Log("나는 Goomba");
       
    }

    void EnemyArcher()
    {
        //Debug.Log("나는 Archer");
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
       switch(type)
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

}
