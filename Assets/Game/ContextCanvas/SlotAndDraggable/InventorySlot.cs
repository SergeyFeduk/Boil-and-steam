using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class InventorySlot : MonoBehaviour
{
    
    [SerializeField] private GameObject prefab;
    private GameObject myObject;

    public UnityAction SlotItemChanged;
    private Vector3 zeroPos = new Vector3(0, 0, -10);
   
    public Item item = Item.itemNull;
    private void Start() => UpdateUI();
    public void UpdateUI()
    {
        if(item != Item.itemNull)
        {
            if(myObject == null) 
            {
                myObject = Instantiate(prefab);
                myObject.transform.SetParent(transform, false);
                myObject.transform.localPosition = zeroPos;
            }
            DraggableItem obj = myObject.GetComponent<DraggableItem>();
            obj.myItem = item;
            obj.UpdateUI();
        }
        else if(myObject != null) Destroy(myObject);
    }
    public void PanelUpdateItem(Item aitem)
    {
        this.item = aitem;
        UpdateUI();
    }
    public void ChangeItemWithoutUIUpdate(Item item)
    {
        this.item = item;
        SlotItemChanged.Invoke();
    }
    public void SetInventoryObject(GameObject obj)
    {
        if (obj == null) item = Item.itemNull;
        myObject = obj;
        if (obj != null)
        {
            item = obj.GetComponent<DraggableItem>().myItem;
            obj.transform.SetParent(transform);
            obj.transform.localPosition = zeroPos;
        }
        SlotItemChanged.Invoke();
        UpdateUI();
    }
    public void AddToItemCount(int count)
    {
        item.count += count;
        if (item.data.maxStack - item.count < 0) print("Кто-то добавил слишком много в стак! >:(");
        SlotItemChanged.Invoke();
        UpdateUI();
    }
    public GameObject SwapObjects(GameObject obj)
    {
        GameObject o = myObject;
        SlotItemChanged.Invoke();
        SetInventoryObject(obj);
        return o;
    }
}
