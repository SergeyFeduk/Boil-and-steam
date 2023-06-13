using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Item StartItem;
    [SerializeField] private GameObject panel;

    public Inventory inventory = new Inventory(9);

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            if(ContextCanvasManager.inst.CheckPanel(panel.name))
            {
                ContextCanvasManager.inst.DestroyPanel(panel.name);           
            }
            else
            {
                ContextCanvasManager.inst.CreatePanel(panel);
            }
        }
        if(Input.GetKeyDown(KeyCode.P)) Player.inst.inventory.inventory.TryAddItems(StartItem);
    }
}