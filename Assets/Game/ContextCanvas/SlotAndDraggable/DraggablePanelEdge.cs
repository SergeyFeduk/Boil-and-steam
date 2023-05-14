using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
public class DraggablePanelEdge : MonoBehaviour
{
    private RectTransform panel;
    private bool stopFlag = false;
    Vector2 offset;
    public void Start()
    {
        panel = transform.parent.GetComponentInParent<RectTransform>();
    }
    public void StartMove()
    {
        panel.SetAsLastSibling();
        offset = (Vector2)panel.InverseTransformPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        MovePanel();
    }
    public void StopMove()
    {
        stopFlag = true;
    }
    private async void MovePanel()
    {
        while (true)
        {
            if (stopFlag)
            {
                stopFlag = false;
                return;
            }
            
            panel.anchoredPosition = (Vector2)Input.mousePosition - new Vector2(Screen.width/2,Screen.height/2) - offset;
            await Task.Yield();
        }
    }
    private void OnDestroy()
    {
        stopFlag = false;
    }
}
