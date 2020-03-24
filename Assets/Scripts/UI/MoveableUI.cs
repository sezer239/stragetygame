using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveableUI : EventTrigger
{
    public bool enableDrag = true;
    private bool dragging;
    private Vector2 firstContactDifference;
    private bool firstContactChecked = false;

    public void Update()
    {
        if (!enableDrag) return;
        if (dragging)
        {
            if (!firstContactChecked)
            {
                firstContactChecked = true;
                firstContactDifference = Input.mousePosition - transform.position;
            }
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - firstContactDifference;
        }
        else
        {
            firstContactChecked = false;
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        dragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        dragging = false;
    }
}
