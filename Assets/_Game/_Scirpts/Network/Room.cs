using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Room : MonoBehaviour
{
    public TextMeshProUGUI textRoom;
    public Button joinRoomButton;

    private void Start()
    {
        joinRoomButton.onClick.AddListener(JoinRoom);
    }
    private void JoinRoom()
    {
        GameObject.Find("RoomManager").GetComponent<RoomManager>().JoinRoomByName(textRoom.text);
    }
}
