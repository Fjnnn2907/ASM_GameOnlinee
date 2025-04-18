using Photon.Pun;
using UnityEngine;

public class PaddleController : MonoBehaviourPun
{
    public float minY = -4f;
    public float maxY = 4f;
    public float moveSpeed = 5f;

    void Update()
    {
        if (!photonView.IsMine) return;

        float move = Input.GetAxis("Vertical");
        transform.Translate(Vector3.up * move * moveSpeed * Time.deltaTime);

        // Giới hạn vị trí paddle không vượt ra khỏi trục Y
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }
}
