using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Photon.Pun;
using UnityEngine;

public class PetFollowPlayer : MonoBehaviourPun
{
    public Transform owner;
    public float followDistance = 3f;
    public float teleportDistance = 10f;
    public float roamRadius = 2f;
    public float idleMoveInterval = 2f;

    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private float lastIdleMoveTime;
    private Vector3 idleRoamTarget;
    private SpriteRenderer spriteRenderer;
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (!photonView.IsMine) return;

        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            owner = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Không tìm thấy player!");
        }

        lastIdleMoveTime = Time.time;
    }

    void Update()
    {
        if (!photonView.IsMine || owner == null) return;

        float distance = Vector3.Distance(transform.position, owner.position);

        if (distance > teleportDistance)
        {
            transform.position = owner.position + Random.insideUnitSphere * 1.5f;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            aiPath.canMove = false;
            Invoke("ResumeMovement", 0.5f);
        }
        else if (distance > followDistance)
        {
            destinationSetter.target = owner;
        }
        else
        {
            if (Time.time - lastIdleMoveTime > idleMoveInterval)
            {
                idleRoamTarget = owner.position + (Vector3)Random.insideUnitCircle * roamRadius;
                GameObject tempTarget = new GameObject("PetRoamTarget");
                tempTarget.transform.position = idleRoamTarget;
                destinationSetter.target = tempTarget.transform;

                Destroy(tempTarget, idleMoveInterval);
                lastIdleMoveTime = Time.time;
            }
        }
        if (aiPath.desiredVelocity.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (aiPath.desiredVelocity.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
    }
    void ResumeMovement()
    {
        aiPath.canMove = true;
    }
    
    
}
