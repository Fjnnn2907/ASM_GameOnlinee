using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Pathfinding;

public class SlimzBlue : MonoBehaviourPun
{
    public Transform owner;
    public float followDistance = 3f;
    public float teleportDistance = 10f;
    public float roamRadius = 2f;
    public float idleMoveInterval = 2f;
    public float attackRange = 1f;
    public float detectRange = 4f;
    public float attackCooldown = 1f;
    public int damage = 10;
    public float giveUpChaseDistance = 6f; // neu enemy xa hon khoang nay, pet bo truy duoiduoi
    public float stopAttackDistance = 0.3f; // trong pham vi nay tan congcong
    public LayerMask enemyLayer;

    private float lastAttackTime;
    private float lastIdleMoveTime;
    private Vector3 idleRoamTarget;

    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform currentTargetEnemy;

    public PetStats petData;
    private PetState currentState = PetState.Idle;
    [SerializeField] private Collider2D attackCollider;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        if (!photonView.IsMine) return;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            owner = playerObj.transform;
        }

        lastIdleMoveTime = Time.time;
        lastAttackTime = Time.time;
    }

    void Update()
    {
        if (!photonView.IsMine || owner == null) return;

        float distanceToOwner = Vector3.Distance(transform.position, owner.position);

        if (distanceToOwner > teleportDistance)
        {
            transform.position = owner.position + Random.insideUnitSphere * 1.5f;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            aiPath.canMove = false;
            Invoke("ResumeMovement", 0.5f);
        }
        if (currentTargetEnemy != null)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, currentTargetEnemy.GetComponent<Collider2D>().ClosestPoint(transform.position));
            float distanceToOwnerNow = Vector3.Distance(transform.position, owner.position);

            spriteRenderer.flipX = currentTargetEnemy.position.x < transform.position.x;
            if (!currentTargetEnemy.gameObject.activeInHierarchy || distanceToOwnerNow > giveUpChaseDistance)
            {
                ResetEnemyTarget();
                StartRoamingAroundPlayer();
                ChangeState(PetState.Run);
                return;
            }
            else if (distanceToEnemy <= stopAttackDistance)
            {
                aiPath.canMove = false;

                destinationSetter.target = null;
                spriteRenderer.flipX = currentTargetEnemy.position.x < transform.position.x;
                AttackEnemy();
            }
            else if (distanceToEnemy > stopAttackDistance)
            {
                destinationSetter.target = currentTargetEnemy;
                aiPath.canMove = true;
                ChangeState(PetState.Run);
            }

            return;
        }
        else
        {
            // tim enemy moi neu khong co targettarget
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);
            if (enemiesInRange.Length > 0)
            {
                // Chon enemy gan nhat vao vung dodo
                Transform nearestEnemy = enemiesInRange[0].transform;
                float minDistance = Vector3.Distance(transform.position, nearestEnemy.position);

                foreach (Collider2D col in enemiesInRange)
                {
                    float dist = Vector3.Distance(transform.position, col.transform.position);
                    if (dist < minDistance)
                    {
                        nearestEnemy = col.transform;
                        minDistance = dist;
                    }
                }

                currentTargetEnemy = nearestEnemy;
                destinationSetter.target = currentTargetEnemy;
                aiPath.canMove = true;
                ChangeState(PetState.Run);
                return;
            }
            else
            {
                if (Time.time - lastIdleMoveTime > idleMoveInterval)
                {
                    lastIdleMoveTime = Time.time;
                    StartRoamingAroundPlayer();
                }
            }
        }

        if (distanceToOwner > teleportDistance)
        {
            transform.position = owner.position + (Vector3)Random.insideUnitCircle * 1.5f;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);

            ResetEnemyTarget();
            Invoke("StartRoamingAroundPlayer", 0.5f);
            ChangeState(PetState.Run);
            return;
        }

        if (currentTargetEnemy != null)
        {
            spriteRenderer.flipX = currentTargetEnemy.position.x < transform.position.x;
        }
        else
        {
            if (aiPath.desiredVelocity.x > 0.1f)
            {
                spriteRenderer.flipX = false;
            }
            else if (aiPath.desiredVelocity.x < -0.1f)
            {
                spriteRenderer.flipX = true;
            }
        }
    }

    void AttackEnemy()
    {
        if (currentTargetEnemy == null) return;

        float distanceToEnemy = Vector3.Distance(transform.position, currentTargetEnemy.GetComponent<Collider2D>().ClosestPoint(transform.position));
        if (distanceToEnemy > stopAttackDistance)
        {
            return;
        }
        if (Time.time - lastAttackTime >= (1f / petData.AttackSpeed))
        {
            spriteRenderer.flipX = currentTargetEnemy.position.x < transform.position.x;
            ChangeState(PetState.Attack);
            lastAttackTime = Time.time;
            attackCollider.enabled = true;
            Invoke("DisableAttackCollider", 0.2f);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!attackCollider.enabled) return;

        if (((1 << other.gameObject.layer) & enemyLayer) != 0)
        {
            if (other.TryGetComponent(out EnemyStats enemyHP))
            {
                float baseDamage = petData.Attack;

                // Crit?
                bool isCrit = Random.value <= petData.CritChance;
                if (isCrit)
                {
                    baseDamage *= petData.CritDamage;
                }

                enemyHP.TakeDamage((int)baseDamage);
            }
        }
    }
    void DisableAttackCollider()
    {
        attackCollider.enabled = false;
    }
    void ResumeMovement()
    {
        aiPath.canMove = true;
    }

    private void ChangeState(PetState newState)
    {
        if (currentState == newState) return;

        currentState = newState;
        switch (currentState)
        {
            case PetState.Idle:
                animator.Play(Tag.IDLE);
                break;
            case PetState.Run:
                animator.Play(Tag.RUN);
                break;
            case PetState.Attack:
                animator.Play(Tag.ATTACK);
                break;
        }
    }
    void StartRoamingAroundPlayer()
    {
        Vector3 roamTargetPos = owner.position + (Vector3)Random.insideUnitCircle * roamRadius;
        GameObject roamTarget = new GameObject("PetRoamTarget");
        roamTarget.transform.position = roamTargetPos;
        destinationSetter.target = roamTarget.transform;
        Destroy(roamTarget, idleMoveInterval);

        aiPath.canMove = true;
        ChangeState(PetState.Run);
    }
    void ResetEnemyTarget()
    {
        currentTargetEnemy = null;
        destinationSetter.target = null;
        aiPath.canMove = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, stopAttackDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}
