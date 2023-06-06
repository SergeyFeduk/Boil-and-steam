using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInventoryObjectController : MonoBehaviour
{
    public static MouseInventoryObjectController inst { get; private set; }
    private void Awake()
    {

        if (inst != null && inst != this)
        {
            Destroy(this);
        }
        else
        {
            inst = this;
        }
    }
}
