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
    private const int pityThreshold = 500;
    private bool isRolling = false;
    public LegendaryTextEffect legendaryTextController;
    public Transform gachaResultContent;
    public GameObject gachaText;
    public GameObject gachaResultItemPrefab;
    public Transform gachaResultItemContainer;
    public GameObject buttonGroup;
    public ChoseeButtonManagerPet petListManager;
    [SerializeField] private Transform itemPetContent;
    void Start()
    {
        pityFill.fillAmount = 0;
        pityText.text = $"0/{pityThreshold}";
        currentRollCount = 0;
        buttonGroup.SetActive(false);
        //gachaResultItemContainer.parent.gameObject.SetActive(false);
    }

    public void RollOnce()
    {
        if (!isRolling)
        {
            buttonGroup.SetActive(false);
            StartCoroutine(RollEffect(1));
        }
    }

    public void RollTen()
    {
        if (!isRolling)
        {
            buttonGroup.SetActive(false);
            StartCoroutine(RollEffect(10));
        }
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

        int fullRounds = Random.Range(2, 4);
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
            var data = slot.GetComponent<PetSlotData>();
            if (data == null)
                continue;

            Debug.Log("-> " + slot.name + " [" + data.rarity + "]");
            string resultText = $"You give: {slot.name} [{data.rarity}]";
            AddGachaResultToScrollView(resultText);
            //data.DisplayPetInfo(); // Hiển thị thông tin pet
            if (data.rarity == Rarity.Legendary)
            {
                legendaryTextController.PlayLegendaryAnimation();
                gotLegendary = true;
            }
            else if (data.rarity == Rarity.Rare)
            {
                legendaryTextController.PlayRareAnimation();
            }
            else if (data.rarity == Rarity.Skin)
            {
                legendaryTextController.PlaySkinAnimation();
            }
        }

        int tempRollCount = currentRollCount + rollCount;
        float currentFill = pityFill.fillAmount;
        float newFill = Mathf.Min((float)tempRollCount / pityThreshold, 1f);
        bool shouldReset = gotLegendary || tempRollCount >= pityThreshold;

        StartCoroutine(SmoothUpdatePityFill(currentFill, newFill, tempRollCount, shouldReset));
        currentRollCount = shouldReset ? 0 : tempRollCount;

        ShowPrefabResultList(resultList);
        buttonGroup.SetActive(true);
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

        return candidates.Count > 0
            ? candidates[Random.Range(0, candidates.Count)]
            : petSlots[Random.Range(0, petSlots.Count)];
    }

    Rarity RollByRate()
    {
        float rand = Random.value;
        if (rand < 0.005f) return Rarity.Legendary;       // 0.5%
        else if (rand < 0.03f) return Rarity.Rare;         // 2.5%
        else if (rand < 0.10f) return Rarity.Normal;       // 7%
        else if (rand < 0.15f) return Rarity.Weapons;      // 5%
        else if (rand < 0.16f) return Rarity.Skin;         // 1%
        else if (rand < 0.30f) return Rarity.Item;         // 15%
        else return Rarity.Coin;                            // 69.5%
    }
    // Rarity RollByRate()
    // {
    //     float rand = Random.value;
    //     if (rand < 0.00001f) return Rarity.Legendary;   // 0.000001% -> 0.5%
    //     else if (rand < 0.03001f) return Rarity.Rare;    // 2.5% + 0.00001f = 2.50001%
    //     else if (rand < 0.10001f) return Rarity.Normal;  // 7% + 0.03001f = 7.00001%
    //     else if (rand < 0.15001f) return Rarity.Weapons; // 5% + 0.10001f = 5.00001%
    //     else if (rand < 0.16001f) return Rarity.Skin;    // 1% + 0.15001f = 1.00001%
    //     else if (rand < 0.30001f) return Rarity.Item;    // 15% + 0.16001f = 15.00001%
    //     else return Rarity.Coin;                         // 69.5% + 0.30001f = 69.49999%
    // }

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

        pityFill.fillAmount = to;
        pityText.text = $"{targetCount}/{pityThreshold}";

        if (resetAfter)
        {
            yield return new WaitForSeconds(0.2f);
            pityFill.fillAmount = 0;
            pityText.text = $"0/{pityThreshold}";
        }
    }

    void ShowPrefabResultList(List<RectTransform> resultList)
    {
        // Xoa ket qua cu
        foreach (Transform child in gachaResultItemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var slot in resultList)
        {
            var data = slot.GetComponent<PetSlotData>();
            if (data == null) continue;

            GameObject resultGO = Instantiate(gachaResultItemPrefab, gachaResultItemContainer);
            GachaResultDisplay display = resultGO.GetComponent<GachaResultDisplay>();

            if (display != null)
            {
                display.Setup(data.icon, slot.name, data.rarity);
            }
            foreach (Transform pet in itemPetContent)
            {
                if (pet.name == slot.name)
                {
                    pet.gameObject.SetActive(true);
                    Debug.Log($"[Gacha] Unlocked pet: {pet.name}");
                }
            }
        }
    }
}
