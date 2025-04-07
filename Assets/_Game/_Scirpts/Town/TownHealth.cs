using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownHealth : MonoBehaviourPun, IPunObservable
{
    public int maxHealth = 1000;
    public float currentHealth;
    [SerializeField] private float smoothHealth;
    public float fillSpeed = 5f;
    private PhotonView photonView;

    [SerializeField] Slider slider;

    [SerializeField] private GameObject damageTextPrefab;
    [SerializeField] private Transform damageTextSpawnPoint;
    private Animator animator;
    private bool triggered75 = false;
    private bool triggered50 = false;
    private bool triggered25 = false;
    private bool isDead = false;


    void Start()
    {
        currentHealth = maxHealth;
        smoothHealth = currentHealth;
        photonView = GetComponent<PhotonView>();
        animator = GetComponent<Animator>();
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);
        }
        if (slider != null)
        {
            slider.maxValue = maxHealth;
            slider.value = currentHealth;
        }
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SyncHealth", RpcTarget.OthersBuffered, currentHealth);
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
        smoothHealth = syncedHealth;

        // if (slider != null)
        // {
        //     slider.maxValue = maxHealth;
        //     slider.value = currentHealth;
        // }
    }
    public void TakeDamage(int damage)
    {
        if (!photonView.IsMine) return;
        currentHealth -= damage;
        Debug.Log("Player took damage: " + damage);
        //targetSliderValue = currentHealth;
        ShowDamageText(damage);
        photonView.RPC("SyncHealth", RpcTarget.All, currentHealth);
        float healthPercent = currentHealth / maxHealth;

        if (!triggered75 && healthPercent <= 0.75f)
        {
            triggered75 = true;
            TriggerEventAt75Percent();
        }
        if (!triggered50 && healthPercent <= 0.5f)
        {
            triggered50 = true;
            TriggerEventAt50Percent();
        }
        if (!triggered25 && healthPercent <= 0.25f)
        {
            triggered25 = true;
            TriggerEventAt25Percent();
        }
        if (!isDead && currentHealth <= 0)
        {
            isDead = true;
            TriggerCollapseAnimation();
            StartCoroutine(CallHideSliderForAllClients(3f));
            //Destroy(gameObject, 3f);
        }
    }
    void TriggerEventAt75Percent()
    {
        photonView.RPC("RPC_TriggerAnimation", RpcTarget.All, "DamageState1");
    }

    void TriggerEventAt50Percent()
    {
        photonView.RPC("RPC_TriggerAnimation", RpcTarget.All, "DamageState2");
    }

    void TriggerEventAt25Percent()
    {
        photonView.RPC("RPC_TriggerAnimation", RpcTarget.All, "DamageState3");
    }

    void TriggerCollapseAnimation()
    {
        photonView.RPC("RPC_TriggerAnimation", RpcTarget.All, "Collapse");
    }
    [PunRPC]
    void RPC_TriggerAnimation(string animName)
    {
        animator?.Play(animName);
    }
    [PunRPC]
    void ShowDamageTextRPC(int damage, Vector3 position)
    {
        if (damageTextPrefab == null) return;

        GameObject dmgTextObj = Instantiate(damageTextPrefab, position, Quaternion.identity, damageTextSpawnPoint.parent);
        DamageText dmgText = dmgTextObj.GetComponent<DamageText>();
        if (dmgText != null)
        {
            dmgText.ShowDamage(damage);
        }
    }
    [PunRPC]
    public void RPC_TownDied()
    {
        currentHealth = 0;
    }
    [PunRPC]
    void RPC_HideSlider()
    {
        if (slider != null)
        {
            slider.gameObject.SetActive(false);
        }
    }
    void ShowDamageText(int damage)
    {
        photonView.RPC("ShowDamageTextRPC", RpcTarget.All, damage, damageTextSpawnPoint.position);
    }
    IEnumerator CallHideSliderForAllClients(float delay)
    {
        yield return new WaitForSeconds(delay);
        photonView.RPC("RPC_HideSlider", RpcTarget.All);
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
            currentHealth = (float)stream.ReceiveNext();
            smoothHealth = currentHealth;
        }
    }
}
