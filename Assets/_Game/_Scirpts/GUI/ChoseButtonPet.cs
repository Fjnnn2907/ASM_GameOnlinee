using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChoseButtonPet : MonoBehaviour
{
    [SerializeField] private Button chose;
    [SerializeField] private TextMeshProUGUI choseText;
    [SerializeField] private Image image;


    public GameObject pet;

    private ChoseeButtonManagerPet choseeButtonManager;
    private void Start()
    {
        chose.onClick.AddListener(() => choseeButtonManager.SelectPet(this));
    }

    public void SetManager(ChoseeButtonManagerPet choseeButtonManager)
    {
        this.choseeButtonManager = choseeButtonManager;
    }

    public void PressChose()
    {
        image.color = Color.green;
        choseText.text = "Using";
    }
    public void UnPressChose()
    {
        image.color = Color.white;
        choseText.text = "Chose";
    }
}
