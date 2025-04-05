using System;
using Pathfinding;
using Photon.Pun;
using UnityEngine;

public class EnemyFollowSimple : MonoBehaviourPun
{
    [SerializeField] float attackRange = 1.5f; // khoang cach tan cong
    public float updateRate = 1f; // tan suat cap nhap duong di
    [SerializeField] float chaseRange = 5f;
    [SerializeField] int attackDamage = 10;
    [SerializeField] float attackCooldown = 1.5f;
    [SerializeField] float separationDistance = 0.5f; //khoang tach roi enemyenemy
    private Vector2 randomOffset;
    private bool canAttack = true;
    public float moveSpeed = 2f;
    private Transform target;
    private Rigidbody2D rb;
    private Seeker seeker;
    private AIPath aiPath;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    protected EnemyState currentState = EnemyState.Idle;
    // private Vector2 initialPosition;
    void Start()
    {
        seeker = GetComponent<Seeker>();
        aiPath = GetComponent<AIPath>();
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
        if (target == null)
        {   
            aiPath.canMove = false;
            aiPath.maxSpeed = 0;
            ChangeState(EnemyState.Idle);
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

        FlipSprite();
    }
    void DealDamageToPlayer()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);
        if (distance <= attackRange)
        {
            var townHealth = target.GetComponent<TownHealth>();
            if (townHealth != null)
            {
                townHealth.TakeDamage(attackDamage);
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
        randomOffset = new Vector2(UnityEngine.Random.Range(-separationDistance, separationDistance), UnityEngine.Random.Range(-separationDistance, separationDistance));
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject town = GameObject.FindGameObjectWithTag("Town");
        if (players.Length == 0 && town != null)
        {
            target = town.transform; // Nếu ko co player chuyen sang town
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
        if (closestTarget != null)
        {
            target = closestTarget.transform;
            seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }
    void OnPathComplete(Path p)
    {
        if (!p.error && aiPath != null)
        {
            aiPath.destination = target.position;
        }
    }
    // void MoveToPlayer()
    // {
    //     Vector2 direction = (target.position - transform.position).normalized;
    //     rb.velocity = direction * moveSpeed;
    // }
    void TakeDamage()
    {

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
