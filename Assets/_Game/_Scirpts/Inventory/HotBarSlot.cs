using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HotBarSlot : MonoBehaviour, IDropHandler
{
    public Image image;
    public Color selectedColor, notselectedColor;

    private void Start()
    {
        DeSelect();
    }
    public void Select()
    {
        image.color = selectedColor;
    }
    public void DeSelect()
    {
        image.color = notselectedColor;
    }
    public void OnDrop(PointerEventData eventData)
    {
        HotBarItem draggedItem = eventData.pointerDrag.GetComponent<HotBarItem>();
        if (draggedItem == null) return;

        if (transform.childCount == 0)
            SetNewParent(draggedItem);
        else
        {
            HotBarItem targetItem = transform.GetChild(0).GetComponent<HotBarItem>();

            if (CanStack(targetItem, draggedItem))
                StackItems(targetItem, draggedItem);
            else
                SwapItems(targetItem, draggedItem);
        }
    }

    private void SetNewParent(HotBarItem item)
    {
        item.parentAfterDrag = transform;
    }

    private bool CanStack(HotBarItem target, HotBarItem dragged)
    {
        return target != null && dragged.itemSO == target.itemSO &&
        target.itemSO.stackable && target.count + dragged.count <= 99;
    }

    private void StackItems(HotBarItem target, HotBarItem dragged)
    {
        target.count += dragged.count;
        target.RefreshCount();
        Destroy(dragged.gameObject);
    }

    private void SwapItems(HotBarItem target, HotBarItem dragged)
    {
        Transform oldParent = dragged.parentAfterDrag;
        dragged.parentAfterDrag = transform;
        target.transform.SetParent(oldParent);
        target.transform.localPosition = Vector3.zero;
    }
}