using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField InputCreateRoom;
    [SerializeField] private TMP_InputField InputJoinRoom;

    [SerializeField] private Button createRoomBtn;
    [SerializeField] private Button joinRoomBtn;

    [SerializeField] private GameObject roomPrefab;
    [SerializeField] private Transform roomParent;
    private Dictionary<string, GameObject> currentRoomUIs = new();
    private void Start()
    {
        createRoomBtn.onClick.AddListener(CreateRoom);
        joinRoomBtn.onClick.AddListener(JoinRoom);
    }
    private void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(InputCreateRoom.text, roomOptions);
    }
    private void JoinRoom()
    {
        PhotonNetwork.JoinRoom(InputJoinRoom.text);
    }
    public void JoinRoomByName(string nameRoom)
    {
        PhotonNetwork.JoinRoom(nameRoom);
    }
    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("JoinRoom");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform child in roomParent.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.IsOpen && room.IsVisible && room.PlayerCount > 0)
            {
                GameObject newRoom = Instantiate(roomPrefab, roomParent.transform);
                newRoom.GetComponentInChildren<TextMeshProUGUI>().text = room.Name;
            }
        }
    }
}
