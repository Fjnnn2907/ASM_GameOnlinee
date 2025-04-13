using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Photon.Pun;
using UnityEngine;

public class SlimzPurl : MonoBehaviourPun
{
    public Transform owner;
    public float followDistance = 3f;
    public float teleportDistance = 10f;
    public float roamRadius = 2f;
    public float idleMoveInterval = 4f;
    public float attackRange = 1f;
    public float detectRange = 4f;
    public float attackCooldown = 1f;
    public int damage = 10;
    public float giveUpChaseDistance = 6f;
    public float stopAttackDistance = 0.3f;
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
    // public GameObject fireRainPrefab;
    public float skillCooldown = 5f;
    private float lastSkillTime;

    private Vector3 dashStartPos;
    private bool isDashing = false;
    private bool isReturning = false;
    public float dashSpeed = 6f;
    public float dashDistance = 1.5f;
    private Quaternion originalRotation;

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
        if (isDashing || isReturning) return;

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
                //TryUseSkill();
                AttackEnemy();
            }
            else if (distanceToEnemy > stopAttackDistance)
            {
                destinationSetter.target = currentTargetEnemy;
                aiPath.canMove = true;
                aiPath.maxSpeed = 3f;
                ChangeState(PetState.Run);
            }

            return;
        }
        else
        {
            Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectRange, enemyLayer);
            if (enemiesInRange.Length > 0)
            {
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

        if (currentTargetEnemy == null)
        {
            if (aiPath.velocity.magnitude < 0.01f)
            {
                ChangeState(PetState.Idle);
            }
            else
            {
                ChangeState(PetState.Run);
            }
        }

        if (currentTargetEnemy != null)
        {
            bool flip = currentTargetEnemy.position.x < transform.position.x;
            SetFlipX(flip);
        }
        else
        {
            if (aiPath.desiredVelocity.x > 0.1f) SetFlipX(false);
            else if (aiPath.desiredVelocity.x < -0.1f) SetFlipX(true);
        }
    }

    void AttackEnemy()
    {
        if (currentTargetEnemy == null || isDashing || isReturning) return;

        float distanceToEnemy = Vector3.Distance(transform.position, currentTargetEnemy.position);
        if (distanceToEnemy > stopAttackDistance) return;

        if (Time.time - lastAttackTime >= 0.1f)
        {
            lastAttackTime = Time.time;
            dashStartPos = transform.position;

            originalRotation = transform.rotation;

            Vector3 direction = (currentTargetEnemy.position - transform.position).normalized;
            transform.right = direction;

            Vector3 dashTargetPos = transform.position + direction * dashDistance;

            StartCoroutine(DashToEnemyAndReturn(dashTargetPos));
        }
    }

    IEnumerator DashToEnemyAndReturn(Vector3 targetPos)
    {
        isDashing = true;
        aiPath.canMove = false;
        destinationSetter.target = null;
        ChangeState(PetState.Attack);

        while (Vector3.Distance(transform.position, targetPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, dashSpeed * Time.deltaTime);
            yield return null;
        }

        attackCollider.enabled = true;
        DealDamageAoE(4f);
        yield return new WaitForSeconds(0.02f);
        attackCollider.enabled = false;

        isReturning = true;

        while (Vector3.Distance(transform.position, dashStartPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, dashStartPos, dashSpeed * Time.deltaTime);
            yield return null;
        }

        transform.rotation = originalRotation;

        isDashing = false;
        isReturning = false;

        aiPath.canMove = true;
        ChangeState(PetState.Idle);
    }

    // private void OnTriggerEnter2D(Collider2D other)
    // {
    //     if (!attackCollider.enabled) return;

    //     if (((1 << other.gameObject.layer) & enemyLayer) != 0)
    //     {
    //         if (other.TryGetComponent(out EnemyStats enemyHP))
    //         {
    //             float baseDamage = petData.Attack;

    //             bool isCrit = Random.value <= petData.CritChance;
    //             if (isCrit) baseDamage *= petData.CritDamage;

    //             enemyHP.TakeDamage((int)baseDamage);
    //         }
    //     }
    // }
    // void DealDamageDirect()
    // {
    //     if (currentTargetEnemy == null) return;

    //     if (currentTargetEnemy.TryGetComponent(out EnemyStats enemy))
    //     {
    //         float baseDamage = petData.Attack;
    //         if (Random.value <= petData.CritChance) baseDamage *= petData.CritDamage;

    //         enemy.TakeDamage((int)baseDamage);
    //     }
    // }
    void DealDamageAoE(float radius)
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, radius, enemyLayer);

        foreach (Collider2D enemyCol in enemies)
        {
            if (enemyCol.TryGetComponent(out EnemyStats enemy))
            {
                float baseDamage = petData.Attack;
                if (Random.value <= petData.CritChance)
                    baseDamage *= petData.CritDamage;

                enemy.TakeDamage((int)baseDamage);
            }
        }
    }

    // [PunRPC]
    // void RPC_UseSkill(Vector3 targetPosition)
    // {
    //     Vector3 spawnPos = targetPosition + new Vector3(0, 2f, 0);
    //     var skill = PhotonNetwork.Instantiate(fireRainPrefab.name, spawnPos, Quaternion.identity);
    //     Destroy(skill, 4f);
    // }

    // void TryUseSkill()
    // {
    //     if (Time.time - lastSkillTime >= skillCooldown && currentTargetEnemy != null)
    //     {
    //         lastSkillTime = Time.time;
    //         ChangeState(PetState.Skill);
    //         photonView.RPC("RPC_UseSkill", RpcTarget.All, currentTargetEnemy.position);
    //     }
    // }

    void ResumeMovement()
    {
        aiPath.canMove = true;
    }

    private void ChangeState(PetState newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        if (photonView.IsMine)
        {
            photonView.RPC("RPC_ChangeState", RpcTarget.Others, (int)newState);
        }

        PlayAnimation(newState);
    }

    [PunRPC]
    void RPC_ChangeState(int state)
    {
        currentState = (PetState)state;
        PlayAnimation(currentState);
    }

    void PlayAnimation(PetState state)
    {
        switch (state)
        {
            case PetState.Idle:
                animator.Play(Tag.IDLE); break;
            case PetState.Run:
                animator.Play(Tag.RUN); break;
            case PetState.Attack:
                animator.SetTrigger(Tag.ATTACK); break;
            case PetState.Skill:
                animator.SetTrigger(Tag.SKILL); break;
        }
    }

    [PunRPC]
    void RPC_SetFlipX(bool flipX)
    {
        spriteRenderer.flipX = flipX;
    }

    void SetFlipX(bool flip)
    {
        spriteRenderer.flipX = flip;
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_SetFlipX", RpcTarget.Others, flip);
        }
    }

    void ResetEnemyTarget()
    {
        currentTargetEnemy = null;
        destinationSetter.target = null;
    }

    void StartRoamingAroundPlayer()
    {
        Vector2 randomOffset = Random.insideUnitCircle * roamRadius;
        Vector3 targetPos = owner.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        destinationSetter.target = null;
        aiPath.destination = targetPos;
        aiPath.canMove = true;
        ChangeState(PetState.Run);
    }
}