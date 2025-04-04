using System;
using Pathfinding;
using Photon.Pun;
using UnityEngine;

public class EnemyFollowSimple : MonoBehaviourPun
{
    [SerializeField] float attackRange = 1.5f; // khoang cach tan cong
    public float updateRate = 1f; // tan suat cap nhap duong di
    [SerializeField] float chaseRange = 5f;
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
        InvokeRepeating(nameof(FindPlayer), 0f, updateRate); // kiem tra player moi giay
    }

    void Update()
    {
        if (target == null)
        {   
            aiPath.canMove = false;
            aiPath.maxSpeed = 0;
            return;
        }
        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > chaseRange)
        {
            target = null;
            //aiPath.destination = initialPosition;
            ChangeState(EnemyState.Idle);
            aiPath.canMove = false;
            aiPath.maxSpeed = 0f;
        }
        else if (distance <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            aiPath.canMove = false;
            aiPath.maxSpeed = 0f;
            FlipSprite();
        }
        else
        {
            aiPath.canMove = true;
            aiPath.maxSpeed = moveSpeed;
            aiPath.destination = target.position; // cap nhap duong didi
            ChangeState(EnemyState.Run);
        }

        FlipSprite();
    }

    void FindPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length == 0) return;
        GameObject closestPlayer = null; // tim player gan nhatnhat
        float minDistance = Mathf.Infinity;

        foreach (var player in players)
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance < minDistance && distance <= chaseRange)
            {
                minDistance = distance;
                closestPlayer = player;
            }
        }

        if (closestPlayer != null)
        {
            target = closestPlayer.transform;
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
