using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    [SerializeField] protected PhotonView photonView;
    [SerializeField] protected float maxHealth = 100f;
    [SerializeField] protected float currentHealth;
    [SerializeField] protected Image fill;

    public float MaxHealth => maxHealth;
    public float CurrentHealth => currentHealth;

    protected virtual void Start()
    { 
        currentHealth = maxHealth;
        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        if (!photonView.IsMine) return;

        if (PhotonNetwork.IsMasterClient)
            photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.AllBuffered, damage);
        else
            photonView.RPC(nameof(RequestTakeDamage), RpcTarget.MasterClient, damage);
    }

    [PunRPC]
    public void RequestTakeDamage(float damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_TakeDamage), RpcTarget.AllBuffered, damage);
        }
    }

    [PunRPC]
    public void RPC_TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        currentHealth = Mathf.Max(0, currentHealth - damage);
        UpdateHealthBar();
    }

    public void AddHealth(float health)
    {
        if (!photonView.IsMine) return;

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_AddHealth), RpcTarget.AllBuffered, health);
        }
        else
        {
            photonView.RPC(nameof(RequestAddHealth), RpcTarget.MasterClient, health);
        }
    }

    [PunRPC]
    public void RequestAddHealth(float health)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC(nameof(RPC_AddHealth), RpcTarget.AllBuffered, health);
        }
    }

    [PunRPC]
    public void RPC_AddHealth(float health)
    {
        if (currentHealth >= maxHealth) return;
        currentHealth = Mathf.Min(maxHealth, currentHealth + health);
        UpdateHealthBar();
    }

    #region GUI
    protected void UpdateHealthBar()
    {
        float healthRatio = currentHealth / maxHealth;
        StartCoroutine(SmoothHealthBar(healthRatio));

        fill.color = (healthRatio < 0.3f)
            ? Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 2, 1))
            : Color.green;
    }

    protected IEnumerator SmoothHealthBar(float targetFill)
    {
        float startFill = fill.fillAmount;
        float elapsedTime = 0f;
        float duration = 0.5f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            fill.fillAmount = Mathf.Lerp(startFill, targetFill, elapsedTime / duration);
            yield return null;
        }

        fill.fillAmount = targetFill;
    }
    #endregion
}
