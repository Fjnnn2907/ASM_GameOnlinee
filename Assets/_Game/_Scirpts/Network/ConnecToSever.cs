using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class ConnecToSever : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject LoadingGUI;
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        LoadingGUI.SetActive(true);
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        LoadingGUI.SetActive(false);
    }
}
