using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;

public class WaitingRoomUI : MonoBehaviourPunCallbacks
{
    public Transform playerCardParent;
    public GameObject playerCardPrefab;
    public Button startButton;
    public Button leaveButton;

    public ChatBox chatBox;

    [System.Serializable]
    public class HeroData
    {
        public string name;
        public Sprite avatar;
    }

    public List<HeroData> heroList = new List<HeroData>();

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        Debug.Log("Is Master Client: " + PhotonNetwork.IsMasterClient);

        startButton.onClick.AddListener(OnStartGameClicked);
        leaveButton.onClick.AddListener(OnLeaveRoomClicked);

        startButton.gameObject.SetActive(false);

        UpdatePlayerListUI();
        OnJoinedRoom();
    }

    public override void OnJoinedRoom()
    {
        UpdatePlayerListUI();

        if (PhotonNetwork.IsMasterClient)
            startButton.gameObject.SetActive(true);

        chatBox.JoinChat();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerListUI();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerListUI();
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
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

            if (player.CustomProperties.TryGetValue("SelectedHero", out object heroNameObj))
            {
                string heroName = heroNameObj as string;
                Sprite avatarSprite = GetHeroSpriteByName(heroName);
                if (avatarSprite != null)
                {
                    Transform avatarTransform = card.transform.Find("UserPicture/Avatar");
                    if (avatarTransform != null)
                    {
                        Image avatarImage = avatarTransform.GetComponent<Image>();
                        avatarImage.sprite = avatarSprite;
                    }
                }
            }
        }
    }

    private Sprite GetHeroSpriteByName(string heroName)
    {
        foreach (var hero in heroList)
        {
            if (hero.name == heroName)
                return hero.avatar;
        }
        return null;
    }

    public void OnStartGameClicked()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel("MainGame");
        }
    }

    public void OnLeaveRoomClicked()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("CreateRoom");
    }
}
