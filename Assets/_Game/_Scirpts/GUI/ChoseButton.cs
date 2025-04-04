using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class ChoseButton : MonoBehaviour
{
    [SerializeField] private Button chose;
    [SerializeField] private TextMeshProUGUI choseText;
    [SerializeField] private Image image;

    public GameObject hero;

    private ChoseeButtonManager choseeButtonManager;
    private void Start()
    {
        chose.onClick.AddListener(() => choseeButtonManager.SelectHero(this));
    }

    public void SetManager(ChoseeButtonManager choseeButtonManager)
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
