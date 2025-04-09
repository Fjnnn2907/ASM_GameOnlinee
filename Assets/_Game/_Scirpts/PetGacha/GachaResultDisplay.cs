using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GachaResultDisplay : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI nameText;
    public Image border;

    public void Setup(Sprite sprite, string petName, Rarity rarity)
    {
        icon.sprite = sprite;
        nameText.text = petName;

        switch (rarity)
        {
            case Rarity.Legendary:
                border.color = new Color(141f / 255f, 45f / 255f, 54f / 255f); ;
                break;
            case Rarity.Rare:
                border.color = new Color(190f / 255f, 89f / 255f, 34f / 255f);
                break;
            case Rarity.Skin:
                border.color = new Color(190f / 255f, 89f / 255f, 34f / 255f);
                break;
            default:
                border.color = new Color(49f / 255f, 44f / 255f, 34f / 255f);
                break;
        }
    }
}
