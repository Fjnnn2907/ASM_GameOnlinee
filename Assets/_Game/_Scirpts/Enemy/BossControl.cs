using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class BossControl : MonoBehaviour
{
    [SerializeField] float attackRange = 1.5f;
    [SerializeField] float chaseRange = 5f;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] int minDamage = 5;
    [SerializeField] int maxDamage = 15;
    [SerializeField] float separationDistance = 0.5f;
    public float updateRate = 1f;
    public float moveSpeed = 2f;

    private float disableSeparationTime = 0f;
    private bool isSeparationDisabled = false;
    private Vector2 randomOffset;
    private Transform target;
    private Transform lastTarget;
    private bool canAttack = true;
    private bool isDie = false;

    private Seeker seeker;
    private AIPath aiPath;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    protected EnemyState currentState = EnemyState.Idle;
    public EnemyStats enemyStats;
    public GameObject fireRainPrefab;
    public float spawnRadius = 10f;
    public float spawnInterval = 3f;
    //private bool isSpawning = true;
    [SerializeField] float roarChance = 0.1f; // 10% dung gam thetthet
    [SerializeField] float roarDuration = 3f;
    private bool isRoaring = false;
    [SerializeField] float roarHealthThreshold = 0.5f; // %mau de kich hoat gam thetthet
    [SerializeField] float fireRainDelay = 0f; // Thoi gian cho truoc khi spawn firnainfirnain

    private bool hasRoared = false; // Kiem tra gam thetthet

    void Start()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyStats = GetComponent<EnemyStats>();

        aiPath.maxSpeed = moveSpeed;
        randomOffset = Random.insideUnitCircle.normalized * separationDistance;
        //StartCoroutine(SpawnFireRain());
        InvokeRepeating(nameof(FindPlayer), 0f, updateRate);
    }

    void Update()
    {
        if (enemyStats.CurrentHealth <= 0 && !isDie)
        {
            Die();
            return;
        }
        if (enemyStats.CurrentHealth <= enemyStats.MaxHealth * roarHealthThreshold && !hasRoared)
        {
            StartCoroutine(PerformRoar());
            hasRoared = true;
        }
        if (isSeparationDisabled)
        {
            disableSeparationTime -= Time.deltaTime;
            if (disableSeparationTime <= 0f)
                isSeparationDisabled = false;
        }

        if (!isSeparationDisabled)
        {
            ApplySeparation();
        }

        UpdateTargetBehavior();

        if (aiPath.hasPath && !aiPath.reachedEndOfPath)
        {
            float deviation = Vector2.Distance(transform.position, aiPath.steeringTarget);
            if (deviation > 1f && target != null)
            {
                seeker.StartPath(transform.position, (Vector2)target.position + randomOffset, OnPathComplete);
            }
        }

        FlipSprite();
    }

    void FindPlayer()
    {
        //randomOffset = new Vector2(UnityEngine.Random.Range(-separationDistance, separationDistance), UnityEngine.Random.Range(-separationDistance, separationDistance));
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject town = GameObject.FindGameObjectWithTag("Town");
        Transform newTarget = null;
        if (players.Length > 0) // Kiểm tra nếu có player gần
        {
            float minDistancee = Mathf.Infinity;
            foreach (var player in players)
            {
                float distance = Vector2.Distance(transform.position, player.transform.position);
                if (distance < minDistancee && distance <= chaseRange)
                {
                    minDistancee = distance;
                    newTarget = player.transform;
                }
            }
        }
        if (newTarget == null && town != null)
        {
            newTarget = town.transform; // Nếu ko co player chuyen sang town
            seeker.StartPath(transform.position, target.position, OnPathComplete);
            return;
        }
        GameObject closestTarget = null; // tim player gan nhatnhat
        float minDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < minDistance && distance <= chaseRange)
            {
                minDistance = distance;
                closestTarget = player;
            }
        }
        if (town != null)
        {
            float distanceToTown = Vector2.Distance(transform.position, town.transform.position);
            if (distanceToTown < minDistance && distanceToTown <= chaseRange)
            {
                minDistance = distanceToTown;
                closestTarget = town;
            }
        }
        if (newTarget != null && newTarget != lastTarget)
        {
            target = newTarget;
            lastTarget = target;

            // tạo offset cố định
            randomOffset = UnityEngine.Random.insideUnitCircle.normalized * separationDistance;
        }
        // if (closestTarget != null)
        // {
        //     target = closestTarget.transform;
        //     seeker.StartPath(transform.position, target.position, OnPathComplete);
        // }
        if (closestTarget != null)
        {
            newTarget = closestTarget.transform;

            if (newTarget != lastTarget)
            {
                // Chỉ tạo offset mới khi đổi target
                randomOffset = UnityEngine.Random.insideUnitCircle.normalized * separationDistance;
                lastTarget = newTarget;
            }

            target = newTarget;
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }

    void UpdateTargetBehavior()
    {
        if (isRoaring)
        {
            aiPath.canMove = false;
            aiPath.maxSpeed = 0f;
            ChangeState(EnemyState.Roar);
            return;
        }
        if (target == null)
        {
            aiPath.canMove = false;
            aiPath.maxSpeed = 0f;
            ChangeState(EnemyState.Idle);
            return;
        }

        float dist = Vector2.Distance(transform.position, target.position);
        Vector2 destination = (Vector2)target.position + randomOffset;

        TownHealth townHealth = target.GetComponent<TownHealth>();
        if (townHealth != null && townHealth.currentHealth <= 0)
        {
            aiPath.canMove = false;
            aiPath.maxSpeed = 0f;
            ChangeState(EnemyState.Idle);
            isSeparationDisabled = true;
            disableSeparationTime = 5f;
            target = null;
            return;
        }

        if (dist > chaseRange)
        {
            target = null;
            aiPath.canMove = false;
            aiPath.maxSpeed = 0f;
            ChangeState(EnemyState.Idle);
            return;
        }
        else if (dist <= attackRange)
        {
            aiPath.canMove = false;
            aiPath.maxSpeed = 0f;
            ChangeState(EnemyState.Attack);

            if (canAttack)
            {
                canAttack = false;
                Invoke(nameof(DealDamage), attackCooldown);
            }
        }
        else
        {
            aiPath.canMove = true;
            aiPath.maxSpeed = moveSpeed;
            aiPath.destination = destination;
            ChangeState(EnemyState.Run);
        }
    }

    void DealDamage()
    {
        if (target == null || isRoaring) return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= attackRange)
        {
            // Random danh thuong va gam thetthet
            float roll = Random.value;
            if (roll <= roarChance)
            {
                // dung gam thetthet
                StartCoroutine(PerformRoar());
            }
            else
            {
                // danh thuongthuong
                int damage = Random.Range(minDamage, maxDamage + 1);

                var playerHealth = target.GetComponent<PlayerStats>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                else
                {
                    var townHealth = target.GetComponent<TownHealth>();
                    if (townHealth != null && townHealth.currentHealth > 0)
                    {
                        townHealth.TakeDamage(damage);
                    }
                }
            }
        }

        canAttack = true;
    }

    void ApplySeparation()
    {
        Vector2 separationForce = Vector2.zero;
        int count = 0;

        Collider2D[] nearby = Physics2D.OverlapCircleAll(transform.position, separationDistance);
        foreach (var col in nearby)
        {
            if (col.gameObject == this.gameObject || !col.CompareTag("Enemy")) continue;

            float dist = Vector2.Distance(transform.position, col.transform.position);
            if (dist > 0.01f)
            {
                Vector2 diff = (Vector2)(transform.position - col.transform.position);
                separationForce += diff.normalized / dist;
                count++;
            }
        }

        if (count > 0)
        {
            separationForce /= count;
            separationForce = separationForce.normalized * 0.05f;
            transform.position += (Vector3)separationForce;
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error && aiPath != null && target != null)
        {
            aiPath.destination = (Vector2)target.position + randomOffset;
        }
    }

    void FlipSprite()
    {
        if (aiPath.desiredVelocity.x >= 0.01f)
            spriteRenderer.flipX = false;
        else if (aiPath.desiredVelocity.x <= -0.01f)
            spriteRenderer.flipX = true;
    }

    public void Die()
    {
        isDie = true;
        ChangeState(EnemyState.Die);
        aiPath.canMove = false;
        this.enabled = false;
        gameObject.SetActive(false);
    }

    void ChangeState(EnemyState newState)
    {
        if (currentState == EnemyState.Hurt && newState != EnemyState.Die) return;
        currentState = newState;
        switch (currentState)
        {
            case EnemyState.Idle:
                PlayAnimation(Tag.IDLE);
                break;
            case EnemyState.Run:
                PlayAnimation(Tag.RUN);
                break;
            case EnemyState.Attack:
                PlayAnimation(Tag.ATTACK);
                break;
            case EnemyState.Roar:
                PlayAnimation(Tag.ROAR);
                break;
            case EnemyState.Die:
                PlayAnimation(Tag.DIE);
                break;
        }
    }

    void PlayAnimation(string animationName)
    {
        if (animator != null)
        {
            animator.Play(animationName);
        }
    }
    private IEnumerator PerformRoar()
    {
        isRoaring = true;
        ChangeState(EnemyState.Roar);

        float roarDuration = 10f;
        float timer = 0f;
        while (timer < roarDuration)
        {
            Vector2 randomPos = (Vector2)transform.position + Random.insideUnitCircle * spawnRadius;
            if (fireRainPrefab != null)
            {
                GameObject rain = Instantiate(fireRainPrefab, randomPos, Quaternion.identity);
                Destroy(rain, 2f);
            }

            yield return new WaitForSeconds(1f);
            timer += 1f;
        }

        isRoaring = false;
        hasRoared = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
