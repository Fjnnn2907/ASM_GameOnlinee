using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class ChatBox : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    private string currentChannel;

    [Header("UI")]
    public TMP_InputField inputField;
    public TextMeshProUGUI chatContent;

    private void Start()
    {
        Application.runInBackground = true;

        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
                           "1.0",
                           new AuthenticationValues(PhotonNetwork.NickName));
    }
      
    private void Update()
    {
        if (chatClient != null)
            chatClient.Service();
    }

    public void JoinChat()
    {
        currentChannel = PhotonNetwork.CurrentRoom.Name;
        chatClient.Subscribe(new string[] { currentChannel });
    }

    public void OnConnected()
    {
        Debug.Log("Connected to Photon Chat");
        JoinChat();
    }

    public void OnDisconnected()
    {
        Debug.Log("Disconnected from Photon Chat");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log("Subscribed to channel: " + channels[0]);
        AddMessageToUI($"<color=grey>Tham gia kênh chat: {channels[0]}</color>");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            AddMessageToUI($"<b>{senders[i]}</b>: {messages[i]}");
        }
    }

    public void SendChatMessage()
    {
        string msg = inputField.text.Trim();
        if (!string.IsNullOrEmpty(msg) && chatClient != null && chatClient.CanChat)
        {
            chatClient.PublishMessage(currentChannel, msg);
            inputField.text = "";
        }
    }

    private void AddMessageToUI(string message)
    {
        chatContent.text += message + "\n";
    }

    public void DebugReturn(DebugLevel level, string message) { }
    public void OnChatStateChange(ChatState state) { }
    public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }
    public void OnUnsubscribed(string[] channels) { }
    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }
}
