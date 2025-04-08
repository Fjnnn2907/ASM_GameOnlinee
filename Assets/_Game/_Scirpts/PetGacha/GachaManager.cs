using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GachaManager : MonoBehaviour
{
    public List<RectTransform> petSlots;
    public RectTransform highlightEffect;
    public float startDelay = 0.05f;
    public float slowDownRate = 0.01f;

    private bool isRolling = false;
    public LegendaryTextEffect legendaryTextController;
    public Transform gachaResultContent; // Content trong ScrollView
    public GameObject gachaText; // Prefab text dòng kết quả

    // Gacha x1
    public void RollOnce()
    {
        if (!isRolling)
            StartCoroutine(RollEffect(1));
    }

    // Gacha x10
    public void RollTen()
    {
        if (!isRolling)
            StartCoroutine(RollEffect(10));
    }

    IEnumerator RollEffect(int rollCount)
    {
        isRolling = true;

        // 1. Roll ra 10 ket qua truosc
        List<RectTransform> resultList = new List<RectTransform>();
        for (int i = 0; i < rollCount; i++)
        {
            RectTransform selectedSlot = GetRandomSlotByRarity();
            resultList.Add(selectedSlot);
        }

        // 2. Chọn 1 ket qua bat ki trong so do de dichuyen hieu ung den
        int showIndex = Random.Range(0, resultList.Count);
        RectTransform showSlot = resultList[showIndex];
        int targetIndex = petSlots.IndexOf(showSlot);

        // 3. Tinh toan so buoc quay
        int fullRounds = Random.Range(3, 5);
        int totalSteps = fullRounds * petSlots.Count + targetIndex;
        float delay = startDelay;

        // 4. Quay highlight
        for (int step = 0; step <= totalSteps; step++)
        {
            int currentIndex = step % petSlots.Count;
            highlightEffect.position = petSlots[currentIndex].position;

            yield return new WaitForSeconds(delay);

            if (step > totalSteps * 0.6f)
                delay += slowDownRate;
        }

        //5
        Debug.Log("Kết quả Gacha x" + rollCount + ":");
        foreach (var slot in resultList)
        {
            var rarity = slot.GetComponent<PetSlotData>().rarity;
            Debug.Log("-> " + slot.name + " [" + rarity + "]");
            string resultText = $"You give: {slot.name} [{rarity}]";
            AddGachaResultToScrollView(resultText);
            if (rarity == Rarity.Legendary)
            {
                legendaryTextController.PlayLegendaryAnimation();
            }
            else if (rarity == Rarity.Rare)
            {
                legendaryTextController.PlayRareAnimation();
            }
            else if (rarity == Rarity.Skin)
            {
                legendaryTextController.PlaySkinAnimation();
            }
        }

        isRolling = false;
    }
    RectTransform GetRandomSlotByRarity()
    {
        // 1. Roll ra loai do hiem theo ti le
        Rarity selectedRarity = RollByRate();

        // 2. Loc tat ca cac slot co dung do hiem
        List<RectTransform> candidates = new List<RectTransform>();
        foreach (var slot in petSlots)
        {
            var data = slot.GetComponent<PetSlotData>();
            if (data != null && data.rarity == selectedRarity)
                candidates.Add(slot);
        }

        // 3. Chon ngau nhien 1 trong so do
        if (candidates.Count == 0)
        {
            return petSlots[Random.Range(0, petSlots.Count)];
        }

        return candidates[Random.Range(0, candidates.Count)];
    }
    Rarity RollByRate()
    {
        float rand = Random.value;

        if (rand < 0.001f)  // 0.1%
            return Rarity.Legendary;
        else if (rand < 0.016f) // 1.5%
            return Rarity.Rare;
        else if (rand < 0.066f) // 5%
            return Rarity.Normal;
        else if (rand < 0.116f) // 5%
            return Rarity.Weapons;
        else if (rand < 0.121f) // 0.5%
            return Rarity.Skin;
        else if (rand < 0.271f) // 15%
            return Rarity.Item;
        else                    // 72.9%
            return Rarity.Coin;
    }
    void AddGachaResultToScrollView(string resultText)
    {
        GameObject newTextGO = Instantiate(gachaText, gachaResultContent);
        TextMeshProUGUI textComponent = newTextGO.GetComponent<TextMeshProUGUI>();
        if (textComponent != null)
        {
            textComponent.text = resultText;
        }
    }
    void ShowResult(int index)
    {
    }
}
