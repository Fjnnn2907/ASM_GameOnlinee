using System.Collections;
using System.Collections.Generic;
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
        List<RectTransform> resultList = new List<RectTransform>();

        for (int i = 0; i < rollCount; i++)
        {
            // Chon slot ket qua theo ti le
            RectTransform selectedSlot = GetRandomSlotByRarity();

            // Quay hieu ung hightline
            int totalRolls = Random.Range(20, 30);
            float delay = startDelay;

            for (int j = 0; j < totalRolls; j++)
            {
                int index = j % petSlots.Count;
                highlightEffect.position = petSlots[index].position;
                yield return new WaitForSeconds(delay);
                if (j > totalRolls * 0.6f)
                    delay += slowDownRate;
            }

            // Dung o ket qua
            highlightEffect.position = selectedSlot.position;

            resultList.Add(selectedSlot);
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("Ket qua Gacha x" + rollCount + ":");
        foreach (var slot in resultList)
        {
            var rarity = slot.GetComponent<PetSlotData>().rarity;
            Debug.Log("-> " + slot.name + " [" + rarity + "]");
            if (rarity == Rarity.Legendary)
            {
                legendaryTextController.PlayLegendaryAnimation();
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

        if (rand < 0.001f) // 0.1%
            return Rarity.Legendary;
        else if (rand < 0.02f) // 1.5%
            return Rarity.Rare;
        else if (rand < 0.1f) // 5%
            return Rarity.Normal;
        else if (rand < 0.1f) // 5%
            return Rarity.Weapons;
        else if (rand < 0.05f) // 0.5%
            return Rarity.Skin;
        else if (rand < 0.2f) // 15%
            return Rarity.Item;
        else
            return Rarity.Coin; // 62.9%
    }
    void ShowResult(int index)
    {
    }
}
