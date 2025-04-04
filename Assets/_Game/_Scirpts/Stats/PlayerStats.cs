using UnityEngine;
using TMPro;
using Photon.Pun;
public class PlayerStats : Stats
{
    [SerializeField] private TextMeshProUGUI nameText;
    protected override void Start()
    {
        base.Start();
        SetNickName();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            TakeDamage(10);
        }
    }
    private void SetNickName()
    {
        nameText.text = photonView.Owner.NickName;
    }
}
