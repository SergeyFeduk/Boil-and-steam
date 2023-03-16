using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour
{
    //public virtual void Init(Cell cell) { this.cell = cell; }
    public List<GameObject> lootObjects = new List<GameObject>();
    public List<int> lootCount = new List<int>(); 
    public virtual void Destroy() {
        for (int i = 0; i < lootObjects.Count; i++) {
            for (int k = 0; k < lootCount[i]; k++) {
                Instantiate(lootObjects[i], transform.position, Quaternion.identity);
            }
        }
        //World.inst.entityManager.RemoveEntity(this); 
        Destroy(gameObject); 
    }

    //public Cell cell { get; private set; }

    public virtual void Tick() { }
    public virtual void MediumTick() { }
    public virtual void LongTick() { }
}
