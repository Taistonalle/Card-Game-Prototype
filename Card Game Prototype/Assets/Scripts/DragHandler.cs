using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler {
    //Inherit this for other scripts that need drag

    public virtual void OnDrag(PointerEventData eventData) {
        Cursor.visible = false;
        transform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData) {
        Cursor.visible = true;
        Debug.Log($"Drag ended on {gameObject.name}");
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        Debug.Log($"{gameObject.name} clicked");
    }
}
