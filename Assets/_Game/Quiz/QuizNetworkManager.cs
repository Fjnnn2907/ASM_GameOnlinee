using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class QuizNetworkManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    public GameObject loginPanel;
    public GameObject roomPanel;
    public GameObject chatPanel;
    public TMP_InputField playerNameInput;
    public TMP_InputField roomNameInput;
    public TMP_Text roomInfoText;
    public TMP_Text playerListText;
    public TMP_Text chatText;
    public TMP_InputField chatInput;
    public Button createRoomBtn;
    public Button joinRoomBtn;
    public Button startGameBtn;
    public Button sendChatBtn;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        // Gán sự kiện cho các nút
        createRoomBtn.onClick.AddListener(OnCreateRoom);
        joinRoomBtn.onClick.AddListener(OnJoinRoom);
        startGameBtn.onClick.AddListener(OnStartGame);
        sendChatBtn.onClick.AddListener(OnSendChat);

        startGameBtn.gameObject.SetActive(false); // Chỉ hiển thị cho Master Client
    }

    public void OnCreateRoom()
    {
        if (string.IsNullOrEmpty(playerNameInput.text) || string.IsNullOrEmpty(roomNameInput.text)) return;

        PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
        RoomOptions options = new RoomOptions { MaxPlayers = 4 };
        PhotonNetwork.CreateRoom(roomNameInput.text, options);
    }

    public void OnJoinRoom()
    {
        if (string.IsNullOrEmpty(playerNameInput.text) || string.IsNullOrEmpty(roomNameInput.text)) return;

        PhotonNetwork.LocalPlayer.NickName = playerNameInput.text;
        PhotonNetwork.JoinRoom(roomNameInput.text);
    }

    public void OnStartGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false; // Đóng phòng không cho ai vào nữa
            photonView.RPC("StartGame", RpcTarget.All);
        }
    }

    public void OnSendChat()
    {
        if (!string.IsNullOrEmpty(chatInput.text))
        {
            photonView.RPC("ReceiveChat", RpcTarget.All,
                PhotonNetwork.LocalPlayer.NickName,
                chatInput.text);
            chatInput.text = "";
        }
    }

    [PunRPC]
    void ReceiveChat(string sender, string message)
    {
        string formattedMessage = $"[{System.DateTime.Now.ToString("HH:mm")}] {sender}: {message}\n";
        chatText.text += formattedMessage;

        // Tự động cuộn xuống tin nhắn mới nhất
        Canvas.ForceUpdateCanvases();
        chatText.rectTransform.sizeDelta = new Vector2(
            chatText.rectTransform.sizeDelta.x,
            chatText.preferredHeight
        );
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        loginPanel.SetActive(true);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        UpdateCachedRoomList(roomList);
    }

    void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (var info in roomList)
        {
            // Xóa phòng không còn tồn tại
            if (!info.IsOpen || !info.IsVisible || info.RemovedFromList)
            {
                if (cachedRoomList.ContainsKey(info.Name))
                {
                    cachedRoomList.Remove(info.Name);
                }
            }
            else
            {
                // Cập nhật thông tin phòng
                cachedRoomList[info.Name] = info;
            }
        }
    }

    public override void OnJoinedRoom()
    {
        loginPanel.SetActive(false);
        roomPanel.SetActive(true);
        chatPanel.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            startGameBtn.gameObject.SetActive(true);
        }

        UpdateRoomInfo();
    }

    void UpdateRoomInfo()
    {
        roomInfoText.text = $"Phòng: {PhotonNetwork.CurrentRoom.Name}\n" +
                          $"Người chơi: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}";

        playerListText.text = "Danh sách người chơi:\n";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            playerListText.text += $"- {player.NickName}\n";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateRoomInfo();

        // Gửi thông báo có người mới vào phòng
        photonView.RPC("ReceiveChat", RpcTarget.All,
            "Hệ thống",
            $"{newPlayer.NickName} đã tham gia phòng!");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateRoomInfo();

        // Gửi thông báo có người rời phòng
        photonView.RPC("ReceiveChat", RpcTarget.All,
            "Hệ thống",
            $"{otherPlayer.NickName} đã rời phòng!");
    }

    [PunRPC]
    void StartGame()
    {
        PhotonNetwork.LoadLevel("QuizGameScene");
    }
}