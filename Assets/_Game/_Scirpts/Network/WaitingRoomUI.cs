using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomUI : MonoBehaviourPunCallbacks
{
    public Transform playerCardParent;
    public GameObject playerCardPrefab;
    public Button startButton;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Debug.Log("Is Master Client: " + PhotonNetwork.IsMasterClient);
        startButton.onClick.AddListener(OnStartGameClicked);
        startButton.gameObject.SetActive(false);
        UpdatePlayerListUI();

        OnJoinedRoom();
    }

    public override void OnJoinedRoom()
    {
       
        UpdatePlayerListUI();

        if (PhotonNetwork.IsMasterClient)
            startButton.gameObject.SetActive(true);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerListUI();
    }

    private void UpdatePlayerListUI()
    {
        foreach (Transform child in playerCardParent)
            Destroy(child.gameObject);

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject card = Instantiate(playerCardPrefab, playerCardParent);
            card.GetComponentInChildren<TextMeshProUGUI>().text = player.NickName;
        }
    }

    public void OnStartGameClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("MainGame"); 
        }
    }
}
