using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class DraggableItem : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private TMPro.TextMeshProUGUI text;

    private RectTransform rect;

    private bool inMouseControl = false;
    private bool stopFlag = false;
    public Item myItem;

    private RaycastHit hit;
    private Ray MyRay;

    public void Start()
    {
        rect = GetComponent<RectTransform>();
    }
    public void UpdateUI()
    {
        image.sprite = myItem.data.icon;
        text.text = myItem.count.ToString();
    }

    public void ClickOnObject(bool isSwapped = false)
    {
        if (Input.GetMouseButton(1) || Input.GetMouseButton(2)) return; //дл€ гор€чих клавиш помен€ть

        if (!inMouseControl)
        {
            Transform parent = transform.parent;
            transform.SetParent(MouseInventoryObjectController.inst.transform, false);
            if (!isSwapped)
            {
                parent.GetComponent<InventorySlot>().SetInventoryObject(null);
            }
            ControlPosition();
            inMouseControl = true;
        }
        else
        {
            GameObject obj = RayCastThroughObject();
            if (!BetterIsMouseOverUI())
            {
                stopFlag = true;
                WorldItemThrower.inst.ThrowItems(myItem);
                Destroy(gameObject);
            }
            else if (obj == null) return;
            else if (obj.GetComponent<InventorySlot>() != null)
            {
                inMouseControl = false;
                stopFlag = true;
                obj.GetComponent<InventorySlot>().SetInventoryObject(gameObject);
            }
            else if (obj.GetComponent<DraggableItem>() != null)
            {
                ObjectAndObjectInteraction(obj.GetComponentInParent<InventorySlot>());
            }
        }
    }
    private async void ControlPosition()
    {
        while (true)
        {
            if (stopFlag)
            {
                stopFlag = false;
                return;
            }
            rect.anchoredPosition = Input.mousePosition;
            await Task.Yield();
        }
    }
    private bool BetterIsMouseOverUI()
    {
        image.raycastTarget = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        bool b = ScreenUtils.IsMouseOverUI();
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        image.raycastTarget = true;
        return b;
    }
    private GameObject RayCastThroughObject()
    {
        image.raycastTarget = false;
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        Collider2D d = Physics2D.OverlapPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        gameObject.GetComponent<BoxCollider2D>().enabled = true;
        image.raycastTarget = true;
        if (d == null) return null;
        return d.gameObject;
    }
    private void ObjectAndObjectInteraction(InventorySlot slot)
    {

        if (slot.item.data == myItem.data && slot.item.data.maxStack != slot.item.count)
        {
            int remainder = slot.item.data.maxStack - slot.item.count;
            if (myItem.count <= remainder)
            {
                inMouseControl = false;
                stopFlag = true;
                slot.AddToItemCount(myItem.count);
                Destroy(gameObject);
            }
            else
            {
                int delta = remainder;
                myItem.count -= delta;
                UpdateUI();
                slot.AddToItemCount(delta);
                slot.UpdateUI();
            }
        }
        else
        {
            stopFlag = true;
            inMouseControl = false;
            slot.SwapObjects(gameObject).GetComponent<DraggableItem>().ClickOnObject(true);
        }
    }
    private void OnApplicationQuit() => stopFlag = true;
}
