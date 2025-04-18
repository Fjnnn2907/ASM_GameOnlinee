using Photon.Pun;
using UnityEngine;

public class BallController : MonoBehaviourPun
{
    public float initialSpeed = 5f;
    public float speedIncreasePerHit = 0.5f;

    private Rigidbody2D rb;
    public GameObject ballPrefab;
    public float cloneOffsetAngle = 10f; // lệch nhẹ
    public int maxBallCount = 20;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        LaunchBall();
    }

    void LaunchBall()
    {
        Vector2 direction = new Vector2(Random.value > 0.5f ? 1 : -1, Random.Range(-0.5f, 0.5f)).normalized;
        rb.velocity = direction * initialSpeed;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Paddle"))
        {
            rb.velocity = rb.velocity.normalized * (rb.velocity.magnitude + speedIncreasePerHit);

            if (PhotonNetwork.IsMasterClient && GameObject.FindGameObjectsWithTag("Ball").Length < maxBallCount)
            {
                Vector2 currentDir = rb.velocity.normalized;
                Vector2 newDir = Quaternion.Euler(0, 0, cloneOffsetAngle) * currentDir;

                GameObject newBall = PhotonNetwork.Instantiate(ballPrefab.name, transform.position, Quaternion.identity);
                Rigidbody2D newRb = newBall.GetComponent<Rigidbody2D>();
                newRb.velocity = newDir * rb.velocity.magnitude;
            }
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            // Bóng nảy lại theo hướng Y
            Vector2 velocity = rb.velocity;
            velocity.y = -velocity.y;
            rb.velocity = velocity;
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!photonView.IsMine) return; // Chỉ xử lý trên bóng của người chơi hiện tại

        if (collision.CompareTag("LeftWall"))
        {
            // Gọi RPC để đồng bộ điểm số
            photonView.RPC("AddScore", RpcTarget.All, false); // Right player scores
            PhotonNetwork.Destroy(gameObject);
        }
        else if (collision.CompareTag("RightWall"))
        {
            photonView.RPC("AddScore", RpcTarget.All, true); // Left player scores
            PhotonNetwork.Destroy(gameObject);
        }
    }

}
