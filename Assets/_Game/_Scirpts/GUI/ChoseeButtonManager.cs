using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChoseeButtonManager : MonoBehaviour
{
    [SerializeField] private List<ChoseButton> choseHeroes;
    private ChoseButton currentSelectedHero;

    private void Start()
    {
        foreach (ChoseButton button in choseHeroes)
        {
            button.SetManager(this);
        }

        string selectHero = PlayerPrefs.GetString("SelectedHero");
        foreach(ChoseButton button in choseHeroes)
        {
            if(button.hero.name == selectHero)
            {
                SelectHero(button);
                break;
            }
        }
    }
    public void SelectHero(ChoseButton newHero)
    {
        if (currentSelectedHero != null)
        {
            currentSelectedHero.UnPressChose();
        }

        currentSelectedHero = newHero;
        currentSelectedHero.PressChose();

        PlayerPrefs.SetString("SelectedHero", newHero.hero.name);
        PlayerPrefs.Save();
    }
}
