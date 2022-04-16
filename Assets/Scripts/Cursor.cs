using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : MonoBehaviour
{
    private ISelectable hoveredItem;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(collision.name);
        hoveredItem = collision.gameObject.GetComponent<ISelectable>();
        Debug.Log(hoveredItem);
    }

    public void OnCursorClick()
    {
        hoveredItem?.OnSelected();
    }
}
