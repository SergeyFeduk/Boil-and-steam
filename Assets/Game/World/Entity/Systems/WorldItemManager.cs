using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldItemManager
{
    private List<Entity> worldItems = new List<Entity>();

    private readonly float radiusToMove = 5;
    private readonly float radiusToTake = 1;

    private readonly float speed = 0.2f;
    public void Update()
    {
        for(int i = 0; i < worldItems.Count; i++)
        {
            Entity entity = worldItems[i];
            if(entity != null && entity.GetComponent<CWorldItem>().readyToMove && Player.inst.inventory.inventory.CanFitItems(new Item(entity.GetComponent<CWorldItem>().itemData, 1)))
            {
                Vector2 vector = new Vector2(Player.inst.transform.position.x, Player.inst.transform.position.y) - entity.position;
                if (radiusToTake < vector.magnitude && vector.magnitude <= radiusToMove)
                {
                    Vector2 offset = vector.normalized * speed;
                    entity.position += offset;
                }
                else if (vector.magnitude < radiusToTake)
                {
                    Player.inst.inventory.inventory.TryAddItems(new Item(entity.GetComponent<CWorldItem>().itemData, 1));
                    entity.Decompose();
                }
            }   
        }
    }
    public void AddWorldItem(Entity entity)
    {
        worldItems.Add(entity);
    }
    public void RemoveWorldItem(Entity entity)
    {
        worldItems.Remove(entity);
    }
}
