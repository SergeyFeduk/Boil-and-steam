using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContextCanvasManager : MonoBehaviour
{
    public static ContextCanvasManager inst { get; private set; }
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
    public bool CheckPanel(string name)
    {
        name = name + "(Clone)";
        for (int i = 0; i < transform.childCount;i++)
        {
            if (transform.GetChild(i).name == name) return true;
        }
        return false;
    }
    public void CreatePanel(GameObject obj)
    {
        GameObject o = Instantiate(obj,transform);
        o.transform.SetParent(transform);
        o.transform.localScale = Vector3.one;
    }
    public void DestroyPanel(string name)
    {
        name = name + "(Clone)";
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == name)
            {
                Destroy(transform.GetChild(i).gameObject);
                break;
            }
        }
    }
}
