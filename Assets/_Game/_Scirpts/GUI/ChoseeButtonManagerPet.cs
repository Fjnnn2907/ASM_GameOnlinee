using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoseeButtonManagerPet : MonoBehaviour
{
    [SerializeField] private List<ChoseButtonPet> chosePets;
    private ChoseButtonPet currentSelectedPet;

    private void Start()
    {
        foreach (ChoseButtonPet button in chosePets)
        {
            button.SetManager(this);
        }

        PlayerPrefs.DeleteAll();
        string selectPet = PlayerPrefs.GetString("SelectedPet","");
        if (string.IsNullOrEmpty(selectPet))
        {
            return;
        }
        foreach (ChoseButtonPet button in chosePets)
        {
            if(button.pet.name == selectPet)
            {
                SelectPet(button);
                break;
            }
        }
    }
    public void UnlockPetByName(string petName)
    {
        foreach (ChoseButtonPet button in chosePets)
        {
            if (button.pet.name.Trim().ToLower() == petName.Trim().ToLower())
            {
                button.pet.gameObject.SetActive(true);
                Debug.Log($"Đã mở khóa pet: {petName}");
                break;
            }
        }
    }
    public void SelectPet(ChoseButtonPet newPet)
    {
        if (currentSelectedPet != null)
            currentSelectedPet.UnPressChose();

        currentSelectedPet = newPet;
        currentSelectedPet.PressChose();

        PlayerPrefs.SetString("SelectedPet", newPet.pet.name);
        PlayerPrefs.Save();
    }
    public List<ChoseButtonPet> GetAllPetButtons()
    {
        return chosePets;
    }
}
