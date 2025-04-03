using Photon.Pun;
using UnityEngine;

public class FinalStateMachine : MonoBehaviour
{
    [Header("State Machine")]
    [SerializeField] protected Animator anim;
    [SerializeField] protected Rigidbody2D rb;
    [SerializeField] protected PhotonView photonView;

    public State currentState = State.None;

    public enum State { None, Idle, Run, Attack, Die }

    [SerializeField] private GameObject flipGui;
    protected virtual void Init() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }

    private void Start()
    {
        Init();
        if(!photonView || !rb || !anim)
        {
            photonView = GetComponent<PhotonView>();
            rb = GetComponent<Rigidbody2D>();
            anim.GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        FSMUpdate();
    }

    private void FixedUpdate()
    {
        FSMFixedUpdate();
    }

    protected virtual void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;
    }

    protected void PlayAnimation(string animationName)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_PlayAnimation", RpcTarget.All, animationName);
        }
    }

    [PunRPC]
    protected void RPC_PlayAnimation(string animationName)
    {
        anim.Play(animationName);
    }

    protected virtual void SetVelocity(float x, float y, float speed)
    {
        Vector2 moveDir = new Vector2(x, y).normalized;
        rb.velocity = moveDir * speed;
        Flip(x);
        FlipGUI(x);
    }

    protected virtual void SetZeroVelocity()
    {
        rb.velocity = Vector2.zero;
    }

    protected virtual void SetDefautlState()
    {
        ChangeState(State.Idle);
    }

    protected virtual void Flip(float x)
    {
        if (Mathf.Abs(x) > Mathf.Epsilon)
        {
            transform.localScale = new Vector3(Mathf.Sign(x), 1, 1);
        }
    }
    private void FlipGUI(float x)
    {
        if (Mathf.Abs(x) > Mathf.Epsilon)
        {
            flipGui.transform.localScale = new Vector3(Mathf.Sign(x), 1, 1);
        }
    }
}
