using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TownHealth : MonoBehaviourPun, IPunObservable
{
    public int maxHealth = 1000;
    private float currentHealth;
    [SerializeField] private float smoothHealth;
    public float fillSpeed = 5f;
    private PhotonView photonView;

    [SerializeField] Slider slider;

    void Start()
    {
        currentHealth = maxHealth;
        smoothHealth = currentHealth;
        photonView = GetComponent<PhotonView>();
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);
        }
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }
    }
    void Update()
    {
        if (slider != null)
        {
            smoothHealth = Mathf.Lerp(smoothHealth, currentHealth, Time.deltaTime * fillSpeed);
            slider.value = smoothHealth;
        }
    }
    [PunRPC]
    void SyncHealth(int syncedHealth)
    {
        currentHealth = syncedHealth;

        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }
    }
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;
        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage);
        //targetSliderValue = currentHealth;
        if (currentHealth <= 0)
        {
            Debug.Log("Player Died");
            Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // Gui du lieu len mangmang
            stream.SendNext(currentHealth);
        }
        else
        {
            // Nhan du lieu tu mang
            currentHealth = (int)stream.ReceiveNext();

            if (slider != null)
            {
                slider.value = currentHealth;
            }
        }
    }
}
