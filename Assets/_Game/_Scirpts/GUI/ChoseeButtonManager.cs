using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

public class ChoseeButtonManager : MonoBehaviour
{
    [SerializeField] private List<ChoseButton> choseHeroes;
    private ChoseButton currentSelectedHero;

    [SerializeField] private Image avatar;

    private void Start()
    {
        foreach (ChoseButton button in choseHeroes)
        {
            button.SetManager(this);
        }

        PlayerPrefs.DeleteAll();
        string selectHero = PlayerPrefs.GetString("SelectedHero", "Player_1");

        foreach (ChoseButton button in choseHeroes)
        {
            if (button.hero.name == selectHero)
            {
                SelectHero(button);
                break;
            }
        }
    }

    public void SelectHero(ChoseButton newHero)
    {
        if (currentSelectedHero != null)
            currentSelectedHero.UnPressChose();

        currentSelectedHero = newHero;
        currentSelectedHero.PressChose();
        ChangeAvatar(newHero);

        PlayerPrefs.SetString("SelectedHero", newHero.hero.name);
        PlayerPrefs.Save();

        // Đồng bộ thông tin với Photon
        Hashtable props = new Hashtable
        {
            { "SelectedHero", newHero.hero.name }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    public void ChangeAvatar(ChoseButton newAvatar)
    {
        avatar.sprite = newAvatar.icon;
    }
}
