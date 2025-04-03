using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCtrl : FinalStateMachine
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private PlayerStats playerStats;
    private Vector2 moveInput;

    private bool isAttack = false;
    private bool isDie = false;

    //private List<string> tools = new List<string> {Tag.ATTACK ,Tag.AXE, Tag.WATERING };
    //private int currentToolIndex = 0;
    protected override void Init()
    {
        if (!photonView.IsMine) return;
        SetDefautlState();
    }

    protected override void FSMUpdate()
    {
        if (!photonView.IsMine) return;

        if (playerStats.CurrentHealth <= 0 && !isDie)
            ChangeState(State.Die);

        CheckInput();
    }
    private void CheckInput()
    {
        moveInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Mouse0) && !isAttack)
            ChangeState(State.Attack);
    }
    protected override void FSMFixedUpdate()
    {
        if (!photonView.IsMine) return;

        switch (currentState)
        {
            case State.Idle:
                IdleState();
                break;
            case State.Run:
                RunState();
                break;
            case State.Attack:
                UseToolState();
                break;
            case State.Die:
                DieState();
                break;
        }
    }

    private void IdleState()
    {
        if (isDie) return;
        PlayAnimation(Tag.IDLE);

        if (Mathf.Abs(moveInput.x) > Mathf.Epsilon || Mathf.Abs(moveInput.y) > Mathf.Epsilon)
            ChangeState(State.Run);
    }

    private void RunState()
    {
        if (isDie) return;
        PlayAnimation(Tag.RUN);
        SetVelocity(moveInput.x, moveInput.y, speed);

        if (Mathf.Abs(moveInput.x) <= Mathf.Epsilon && Mathf.Abs(moveInput.y) <= Mathf.Epsilon)
            ChangeState(State.Idle);

    }
    private void UseToolState()
    {
        if (isDie) return;

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        ItemSO selectedItem = HotBarManager.instance.GetSelectedItem();
        if (selectedItem == null)
        {
            ChangeState(State.Idle);
            return;
        }

        string newTool = selectedItem.action.ToString();

        if (!stateInfo.IsName(newTool))
        {
            PlayAnimation(newTool);
            SetZeroVelocity();
        }

        if (stateInfo.normalizedTime >= 1f && !stateInfo.loop)
        {
            ChangeState(State.Idle);
            SetVelocity(moveInput.x, moveInput.y, speed);
        }
    }

    private void DieState()
    {
        if (!isDie)
            StartCoroutine(ReSpawner());
    }
    IEnumerator ReSpawner()
    {
        isDie = true;
        PlayAnimation(Tag.DIE);
        yield return new WaitForSeconds(1);
        playerStats.AddHealth(30);
        yield return new WaitForSeconds(1);
        playerStats.AddHealth(30);
        PlayAnimation(Tag.DESPAWN);
        yield return new WaitForSeconds(1);
        playerStats.AddHealth(40);
        ChangeState(State.Idle);
        isDie = false;
    }
}
