using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class NickName : MonoBehaviour
{
    public GameObject loginUI;
    public Button nickNameButton;
    public TextMeshProUGUI textName;
    public TMP_InputField playerNameInput;
    public int selectedCharacterIndex = 0;


    private void Start()
    {
        nickNameButton.onClick.AddListener(SetPlayerName);

        //if (PlayerPrefs.HasKey("NickName"))
        //{
        //    Debug.Log("a");
        //    string saveName = PlayerPrefs.GetString("NickName");
        //    PhotonNetwork.NickName = saveName;
        //    textName.text = saveName;
        //    Debug.Log(textName.text);
        //    loginUI.SetActive(false);
        //}
        //else
        //{
        //    PopUpLogin(); 
        //}

        PopUpLogin();
    }


    public void SetPlayerName()
    {
        if (!string.IsNullOrEmpty(playerNameInput.text))
        {
            PlayerPrefs.SetString("NickName", playerNameInput.text);

            PhotonNetwork.NickName = playerNameInput.text;
            textName.text = playerNameInput.text;
        }
    }

    public void PopUpLogin()
    {
        bool hasNickname = PhotonNetwork.NickName == "";
        textName.text = PhotonNetwork.NickName;
        loginUI.SetActive(hasNickname);
    }

    public void SelectCharacter(int index)
    {
        selectedCharacterIndex = index;
    }
    public void ChangeScene()
    {
        SceneManager.LoadScene("CreateRoom");
    }
}
