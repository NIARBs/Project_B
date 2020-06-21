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
    protected CapsuleCollider2D capsuleCollider;

    protected EEnemyType enemyType = EEnemyType.None;

    [SerializeField] protected bool isAlive = true;
    [SerializeField] protected int hp;
    [SerializeField] protected int damage;

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float attackCycle;
    [SerializeField] protected int nextMove;

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

    public void OnDamaged()
    {
        //spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
        //spriteRenderer.flipY = true;
        capsuleCollider.enabled = false;
        rigid.AddForce(Vector2.up, ForceMode2D.Impulse);
        Invoke("Dead", 3.0f);
    }

    void Dead()
    {
        Destroy(gameObject);
    }

    public int GetDamage()
    {
        return damage;
    }
}
