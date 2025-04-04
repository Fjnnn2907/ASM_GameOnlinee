using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class NickName : MonoBehaviour
{
    public Button nickNameButton;
    public TextMeshProUGUI textName;
    public TMP_InputField playerNameInput;
    public int selectedCharacterIndex = 0;

    private void Start()
    {
        nickNameButton.onClick.AddListener(SetPlayerName);
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

    public void SelectCharacter(int index)
    {
        selectedCharacterIndex = index;
    }
    public void ChangeScene()
    {
        SceneManager.LoadScene("CreateRoom");
    }
}
