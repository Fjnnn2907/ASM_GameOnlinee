using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class BossControl : MonoBehaviour
{
    [SerializeField] float attackRange = 1.5f; // khoang cach tan cong
    public float updateRate = 1f; // tan suat cap nhap duong di
    [SerializeField] float chaseRange = 5f;
    //[SerializeField] int attackDamage = 10;
    [SerializeField] int minDamage = 5;
    [SerializeField] int maxDamage = 15;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float separationDistance = 0.5f; //khoang tach roi enemyenemy
    private float disableSeparationTime = 0; // thoi gian tat luc dayday
    private bool isSeparationDisabled = false; // kiem tra luc dayday
    private Vector2 randomOffset;
    private Transform lastTarget; // để biết khi nào đổi target
    private bool canAttack = true;
    public float moveSpeed = 2f;
    private Transform target;
    private Rigidbody2D rb;
    private Seeker seeker;
    private AIPath aiPath;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    protected EnemyState currentState = EnemyState.Idle;
    public EnemyStats enemyStats;
    private bool isDie = false;
    // private Vector2 initialPosition;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
        enemyStats = GetComponent<EnemyStats>();
        //rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        aiPath.maxSpeed = moveSpeed;
        // initialPosition = transform.position;
        randomOffset = new Vector2(UnityEngine.Random.Range(-separationDistance, separationDistance), UnityEngine.Random.Range(-separationDistance, separationDistance));
        InvokeRepeating(nameof(FindPlayer), 0f, updateRate); // kiem tra player moi giay
    }

    void Update()
    {
        if (enemyStats.CurrentHealth <= 0 && isDie == false) Die();

        if (isSeparationDisabled)
        {
            disableSeparationTime -= Time.deltaTime;
            if (disableSeparationTime <= 0f)
            {
                isSeparationDisabled = false;
            }
        }
        UpdateFindPlayerAndTown();

        if (!isSeparationDisabled)
        {
            ApplySeparation();
        }
        if (aiPath.reachedEndOfPath == false && aiPath.hasPath)
        {
            float deviation = Vector2.Distance(transform.position, aiPath.steeringTarget);
            if (deviation > 1f) //neu lech ra khoi path nhieunhieu
            {
                seeker.StartPath(transform.position, (Vector2)target.position + randomOffset, OnPathComplete);
            }
        }
        FlipSprite();
    }
    void UpdateFindPlayerAndTown()
    {
        if (target == null)
        {
            aiPath.canMove = false;
            aiPath.maxSpeed = 0;
            ChangeState(EnemyState.Idle);
            return;
        }
        TownHealth townHealth = target.GetComponent<TownHealth>();
        if (townHealth != null && townHealth.currentHealth <= 0)
        {
            // Neu town hien tai die, tim town gan nhat
            isSeparationDisabled = true;
            disableSeparationTime = 5f;
            aiPath.canMove = false;
            aiPath.maxSpeed = 0;
            ChangeState(EnemyState.Idle);

            // Tim town gan do
            GameObject[] towns = GameObject.FindGameObjectsWithTag("Town");
            float minDistance = Mathf.Infinity;
            Transform newTarget = null;

            foreach (var town in towns)
            {
                TownHealth th = town.GetComponent<TownHealth>();
                if (th != null && th.currentHealth > 0)
                {
                    float distanceT = Vector2.Distance(transform.position, town.transform.position);
                    if (distanceT < minDistance)
                    {
                        minDistance = distanceT;
                        newTarget = town.transform;
                    }
                }
            }

            // Neu tim thay town do, di chuyen toi
            if (newTarget != null)
            {
                target = newTarget;
                aiPath.canMove = true;
                aiPath.maxSpeed = moveSpeed;
                aiPath.destination = target.position;
                ChangeState(EnemyState.Run);
            }
            return;
        }

        float distance = Vector2.Distance(transform.position, target.position);
        Vector2 targetPositionWithOffset = target.position + (Vector3)randomOffset;
        if (distance > chaseRange)
        {
            FindPlayer();
            // target = null;
            // //aiPath.destination = initialPosition;
            // ChangeState(EnemyState.Idle);
            // aiPath.canMove = false;
            // aiPath.maxSpeed = 0f;
            return;
        }
        else if (distance <= attackRange)
        {
            if (target.CompareTag("Town") && transform.position.y > target.position.y)
            {
                aiPath.canMove = true;
                aiPath.maxSpeed = moveSpeed;
                aiPath.destination = targetPositionWithOffset;
                ChangeState(EnemyState.Run);
                return;
            }
            ChangeState(EnemyState.Attack);
            aiPath.canMove = false;
            aiPath.maxSpeed = 0f;
            FlipSprite();
            if (canAttack)
            {
                canAttack = false;
                Invoke(nameof(DealDamageToPlayer), attackCooldown); // delay dame 1s
            }
        }
        else
        {
            aiPath.canMove = true;
            aiPath.maxSpeed = moveSpeed;
            aiPath.destination = targetPositionWithOffset;  // cap nhap duong didi
            ChangeState(EnemyState.Run);
        }
    }
    void DealDamageToPlayer()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= attackRange)
        {
            var townHealth = target.GetComponent<TownHealth>();
            if (townHealth != null && townHealth.currentHealth > 0)
            {
                int randomDamage = UnityEngine.Random.Range(minDamage, maxDamage + 1); // max + 1 vi Random.Range(int, int) loai tru max
                townHealth.TakeDamage(randomDamage);
            }
        }

        canAttack = true;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject town = GameObject.FindGameObjectWithTag("Town");
        if (players.Length == 0 && town == null)
        {
            ChangeState(EnemyState.Idle); //
        }
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
    void ApplySeparation()
    {
        Vector2 separationForce = Vector2.zero;
        int neighborCount = 0;
        float separationRadius = separationDistance;

        Collider2D[] nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, separationRadius);
        foreach (Collider2D col in nearbyEnemies)
        {
            if (col.gameObject == this.gameObject || !col.CompareTag("Enemy")) continue;

            float distance = Vector2.Distance(transform.position, col.transform.position);
            if (distance > 0.01f)
            {
                Vector2 diff = (Vector2)(transform.position - col.transform.position);
                separationForce += diff.normalized / distance;
                neighborCount++;
            }
        }

        if (neighborCount > 0)
        {
            separationForce /= neighborCount;
            separationForce = separationForce.normalized * 0.05f;

            if (separationForce.magnitude > 0.05f)
            {
                separationForce = separationForce.normalized * 0.05f;
            }
            transform.position += (Vector3)separationForce;
        }
    }
    public void Die()
    {
        isDie = true;
        ChangeState(EnemyState.Die);
        aiPath.canMove = false;
        this.enabled = false;
        gameObject.SetActive(false);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error && aiPath != null && target != null)
        {
            aiPath.destination = (Vector2)target.position + randomOffset;
        }
    }
    private void ChangeState(EnemyState newState)
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
            case EnemyState.Hurt:
                PlayAnimation(Tag.HURT);
                break;
            case EnemyState.Die:
                PlayAnimation(Tag.DIE);
                break;
        }
    }
    protected void PlayAnimation(string animationName)
    {
        animator.Play(animationName);
    }
    void FlipSprite()
    {
        if (target == null) return;
        if (aiPath.desiredVelocity.x > 0.1f)
            spriteRenderer.flipX = false;
        else if (aiPath.desiredVelocity.x < -0.1f)
            spriteRenderer.flipX = true;
        if (target.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }
}
