using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;

public class ConnecToSever : MonoBehaviourPunCallbacks
{
    private static ConnecToSever instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
     
    [SerializeField] private GameObject LoadingGUI;
    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
            LoadingGUI.SetActive(true);
        }       
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
