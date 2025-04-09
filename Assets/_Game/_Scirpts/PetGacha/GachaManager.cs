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
    public Image pityFill;
    public TextMeshProUGUI pityText;
    private int currentRollCount = 0;
    private const int pityThreshold = 100;
    private bool isRolling = false;
    public LegendaryTextEffect legendaryTextController;
    public Transform gachaResultContent;
    public GameObject gachaText;

    void Start()
    {
        pityFill.fillAmount = 0;
        pityText.text = $"0/{pityThreshold}";
        currentRollCount = 0;
    }
    public void RollOnce()
    {
        if (!isRolling)
            StartCoroutine(RollEffect(1));
    }

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
            RectTransform selectedSlot = GetRandomSlotByRarity();
            resultList.Add(selectedSlot);
        }

        bool alreadyHasLegendary = resultList.Exists(slot => slot.GetComponent<PetSlotData>().rarity == Rarity.Legendary);
        if (currentRollCount + rollCount >= pityThreshold && !alreadyHasLegendary)
        {
            int replaceIndex = Random.Range(0, resultList.Count);
            RectTransform legendarySlot = GetRandomSlotBySpecificRarity(Rarity.Legendary);
            resultList[replaceIndex] = legendarySlot;
        }

        int showIndex = Random.Range(0, resultList.Count);
        RectTransform showSlot = resultList[showIndex];
        int targetIndex = petSlots.IndexOf(showSlot);

        // Tinh tong so buoc highlight
        int fullRounds = Random.Range(3, 5);
        int totalSteps = fullRounds * petSlots.Count + targetIndex;
        float delay = startDelay;

        for (int step = 0; step <= totalSteps; step++)
        {
            int currentIndex = step % petSlots.Count;
            highlightEffect.position = petSlots[currentIndex].position;

            yield return new WaitForSeconds(delay);

            if (step > totalSteps * 0.6f)
                delay += slowDownRate;
        }

        Debug.Log("Kết quả Gacha x" + rollCount + ":");
        bool gotLegendary = false;

        foreach (var slot in resultList)
        {
            var rarity = slot.GetComponent<PetSlotData>().rarity;
            Debug.Log("-> " + slot.name + " [" + rarity + "]");
            string resultText = $"You give: {slot.name} [{rarity}]";
            AddGachaResultToScrollView(resultText);

            if (rarity == Rarity.Legendary)
            {
                legendaryTextController.PlayLegendaryAnimation();
                gotLegendary = true;
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

        int tempRollCount = currentRollCount + rollCount;
        float currentFill = pityFill.fillAmount;
        float newFill = Mathf.Min((float)tempRollCount / pityThreshold, 1f);
        bool shouldReset = gotLegendary || tempRollCount >= pityThreshold;
        StartCoroutine(SmoothUpdatePityFill(currentFill, newFill, tempRollCount, shouldReset));
        if (gotLegendary || tempRollCount >= pityThreshold)
        {
            currentRollCount = 0;
        }
        else
        {
            currentRollCount = tempRollCount;
        }

        isRolling = false;
    }

    RectTransform GetRandomSlotByRarity()
    {
        Rarity selectedRarity = RollByRate();
        return GetRandomSlotBySpecificRarity(selectedRarity);
    }

    RectTransform GetRandomSlotBySpecificRarity(Rarity rarity)
    {
        List<RectTransform> candidates = new List<RectTransform>();
        foreach (var slot in petSlots)
        {
            var data = slot.GetComponent<PetSlotData>();
            if (data != null && data.rarity == rarity)
                candidates.Add(slot);
        }

        if (candidates.Count == 0)
        {
            return petSlots[Random.Range(0, petSlots.Count)];
        }

        return candidates[Random.Range(0, candidates.Count)];
    }

    Rarity RollByRate()
    {
        float rand = Random.value;

        if (rand < 0.001f) return Rarity.Legendary;       // 0.1%
        else if (rand < 0.016f) return Rarity.Rare;     // 1.5%
        else if (rand < 0.066f) return Rarity.Normal;   // 5%
        else if (rand < 0.116f) return Rarity.Weapons;  // 5%
        else if (rand < 0.121f) return Rarity.Skin;     // 0.5%
        else if (rand < 0.271f) return Rarity.Item; // 15%
        else return Rarity.Coin;                        // 72.9%
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
    IEnumerator SmoothUpdatePityFill(float from, float to, int targetCount, bool resetAfter)
    {
        float duration = 0.5f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float currentFill = Mathf.Lerp(from, to, t);
            pityFill.fillAmount = currentFill;

            int currentCount = Mathf.RoundToInt(Mathf.Lerp(from * pityThreshold, targetCount, t));
            pityText.text = $"{currentCount}/{pityThreshold}";

            yield return null;
        }

        // Set gia tri cuoi cung
        pityFill.fillAmount = to;
        pityText.text = $"{targetCount}/{pityThreshold}";

        if (resetAfter)
        {
            yield return new WaitForSeconds(0.2f);
            pityFill.fillAmount = 0;
            pityText.text = $"0/{pityThreshold}";
        }
    }
    void ShowResult(int index) { }
}
