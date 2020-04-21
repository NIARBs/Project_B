using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEnemyType
{
    None,
    Mushroomy,
    Archer,
    Ghost,
    Medusa
}

public class Enemy : MonoBehaviour
{
    [Header("- 오브젝트 설정")]
    [SerializeField] protected GameObject player;
    protected Rigidbody2D rigid;
    protected SpriteRenderer spriteRenderer;

    protected EEnemyType enemyType = EEnemyType.None;

    [SerializeField] protected bool isAlive = true;
    [SerializeField] protected int hp;
    [SerializeField] protected int damage;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackCycle;
    [SerializeField] protected int nextMove;

    void Awake()
    {
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector3 moveDir = player.transform.position - transform.position;
        moveDir.Normalize();
        transform.position += moveDir * moveSpeed * Time.deltaTime;
        */
    }

    void InitMushroomy()
    {
        enemyType = EEnemyType.Mushroomy;
    }

    void InitArcher()
    {
        enemyType = EEnemyType.Archer;
    }

    void InitGhost()
    {
        enemyType = EEnemyType.Ghost;
    }

    void InitMedusa()
    {
        enemyType = EEnemyType.Medusa;
    }
    
   
    protected void CheckEnemyType(EEnemyType type)
    {
       switch(type)
        {
            case EEnemyType.Mushroomy:
                InitMushroomy();
                break;

            case EEnemyType.Archer:
                InitArcher();
                break;

            case EEnemyType.Ghost:
                InitGhost();
                break;

            case EEnemyType.Medusa:
                InitMedusa();
                break;

            default:
                Debug.Log("적의 종류가 선택되지 않았습니다.");
                break;
        }
    }

}
