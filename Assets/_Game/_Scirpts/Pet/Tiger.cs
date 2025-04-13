using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Photon.Pun;
using UnityEngine;

public class Tiger : MonoBehaviourPun
{
    public Transform owner;
    public float followDistance = 3f;
    public float teleportDistance = 10f;
    public float roamRadius = 2f;
    public float idleMoveInterval = 4f;
    public float detectRange = 4f;
    public float giveUpChaseDistance = 6f;
    public LayerMask enemyLayer;

    private float lastIdleMoveTime;
    private Vector3 idleRoamTarget;

    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform currentTargetEnemy;

    public PetStats petData;
    private PetState currentState = PetState.Idle;

    [SerializeField] private GameObject iceSpikePrefab;
    [SerializeField] private float iceSpikeSpacing = 0.01f;
    [SerializeField] private float iceSpikeSpawnDelay = 0.1f;
    [SerializeField] private float skillCooldown = 5f;
    private float lastSkillTime;
    public float skillAttackRange = 10f;
    private bool isUsingSkill = false;

    [SerializeField] private GameObject knockbackSkillInstance;
    [SerializeField] private float knockbackSkillCooldown = 6f;
    private float lastKnockbackTime;
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
            float distanceToEnemy = Vector3.Distance(transform.position, currentTargetEnemy.position);
            float distanceToOwnerNow = Vector3.Distance(transform.position, owner.position);

            spriteRenderer.flipX = currentTargetEnemy.position.x < transform.position.x;

            if (!currentTargetEnemy.gameObject.activeInHierarchy || distanceToOwnerNow > giveUpChaseDistance)
            {
                ResetEnemyTarget();
                StartRoamingAroundPlayer();
                ChangeState(PetState.Run);
                return;
            }

            if (distanceToEnemy <= skillAttackRange)
            {
                aiPath.canMove = false;
                destinationSetter.target = null;
                TryUseSkill();
                return;
            }
        }
        else
        {
            Collider2D[] enemiesInSkillRange = Physics2D.OverlapCircleAll(transform.position, skillAttackRange, enemyLayer);
            if (enemiesInSkillRange.Length > 0)
            {
                aiPath.canMove = false;
                destinationSetter.target = null;
                TryUseSkill();
                return;
            }

            if (Time.time - lastIdleMoveTime > idleMoveInterval)
            {
                lastIdleMoveTime = Time.time;
                StartRoamingAroundPlayer();
            }
        }

        if (aiPath.velocity.magnitude < 0.01f)
        {
            ChangeState(PetState.Idle);
        }
        else
        {
            ChangeState(PetState.Run);
        }

        if (aiPath.desiredVelocity.x > 0.1f)
        {
            SetFlipX(false);
        }
        else if (aiPath.desiredVelocity.x < -0.1f)
        {
            SetFlipX(true);
        }
    }

    void TryUseSkill()
    {
        if (isUsingSkill) return;

        Collider2D[] enemiesInSkillRange = Physics2D.OverlapCircleAll(transform.position, skillAttackRange, enemyLayer);

        if (enemiesInSkillRange.Length > 0)
        {
            if (Time.time - lastKnockbackTime >= knockbackSkillCooldown)
            {
                lastKnockbackTime = Time.time;
                StartCoroutine(HandleSkillDelay(() =>
                {
                    ChangeState(PetState.Skill);
                    Vector3 skillPos = transform.position;
                    photonView.RPC("RPC_UseKnockbackSkill", RpcTarget.All, skillPos);
                }));
                return;
            }

            if (Time.time - lastSkillTime >= skillCooldown)
            {
                lastSkillTime = Time.time;
                StartCoroutine(HandleSkillDelay(() =>
                {
                    ChangeState(PetState.Skill);

                    foreach (Collider2D enemy in enemiesInSkillRange)
                    {
                        if (enemy != null && enemy.gameObject.activeInHierarchy)
                        {
                            Vector3 enemyPos = enemy.transform.position;
                            photonView.RPC("RPC_UseSkill", RpcTarget.All, enemyPos);
                        }
                    }
                }));
            }
        }
    }
    IEnumerator HandleSkillDelay(System.Action onSkillStart)
    {
        isUsingSkill = true;
        aiPath.canMove = false;
        destinationSetter.target = null;

        onSkillStart?.Invoke();

        yield return new WaitForSeconds(2f);

        ChangeState(PetState.Idle);
        aiPath.canMove = true;
        isUsingSkill = false;
    }
    [PunRPC]
    void RPC_UseKnockbackSkill(Vector3 spawnPos)
    {
        if (knockbackSkillInstance != null)
        {
            knockbackSkillInstance.SetActive(true);
            StartCoroutine(DisableAfter(knockbackSkillInstance, 4f));
        }
    }

    IEnumerator DisableAfter(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        obj.SetActive(false);
    }

    [PunRPC]
    void RPC_UseSkill(Vector3 targetPosition)
    {
        StartCoroutine(SpawnIceSpikeSequence(transform.position, targetPosition));
    }

    IEnumerator SpawnIceSpikeSequence(Vector2 startPos, Vector2 targetPos)
    {
        Vector2 dir = (targetPos - startPos).normalized;
        float distance = Vector2.Distance(startPos, targetPos);
        int spikeCount = Mathf.FloorToInt(distance / iceSpikeSpacing);

        for (int i = 0; i <= spikeCount; i++)
        {
            Vector2 spawnPos = startPos + dir * i * iceSpikeSpacing;

            GameObject spike = PhotonNetwork.Instantiate(iceSpikePrefab.name, spawnPos, Quaternion.identity);

            Collider2D hit = Physics2D.OverlapCircle(spawnPos, 0.3f, enemyLayer);
            if (hit != null)
            {
                if (hit.TryGetComponent(out EnemyStats enemyHP))
                {
                    enemyHP.TakeDamage((int)(petData.Attack * 1.2f));
                }
            }

            yield return new WaitForSeconds(iceSpikeSpawnDelay);
        }
    }

    void ResetEnemyTarget()
    {
        currentTargetEnemy = null;
        destinationSetter.target = null;
    }

    void ResumeMovement()
    {
        aiPath.canMove = true;
    }

    void StartRoamingAroundPlayer()
    {
        Vector2 roamOffset = Random.insideUnitCircle * roamRadius;
        idleRoamTarget = owner.position + (Vector3)roamOffset;
        destinationSetter.target = null;
        aiPath.canMove = true;
        aiPath.destination = idleRoamTarget;
        ChangeState(PetState.Run);
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
                animator.Play("Idle");
                break;
            case PetState.Run:
                animator.Play("Run");
                break;
            case PetState.Skill:
                animator.Play("Skill");
                break;
        }
    }

    void SetFlipX(bool value)
    {
        spriteRenderer.flipX = value;
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_SetFlipX", RpcTarget.Others, value);
        }
    }

    [PunRPC]
    void RPC_SetFlipX(bool value)
    {
        spriteRenderer.flipX = value;
    }
}
