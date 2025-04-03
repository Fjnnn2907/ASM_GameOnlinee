using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotBarItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ItemSO itemSO;
    public int count = 1;

    public Image image;
    public TextMeshProUGUI countText;

    [HideInInspector]
    public Transform parentAfterDrag;

    private void Start()
    {
        if (itemSO != null)
            BeginItem(itemSO);
        RefreshCount();
    }

    public void BeginItem(ItemSO newItem)
    {
        if (newItem == null) return;

        itemSO = newItem;
        image.sprite = newItem.image;
    }

    public void RefreshCount()
    {
        countText.text = count > 1 ? count.ToString() : "";
        countText.gameObject.SetActive(count > 1);
    }

    #region Event
    public void OnBeginDrag(PointerEventData eventData)
    {
        image.raycastTarget = false;
        parentAfterDrag = transform.parent;
        transform.SetParent(transform.root);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;
        transform.SetParent(parentAfterDrag);
    }
    #endregion
}
