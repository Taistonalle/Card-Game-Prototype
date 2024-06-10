using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragAndPointerHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler {
    //Inherit this for other scripts that need drag
    protected GameManager gM;

    private void Awake() {
        gM = FindObjectOfType<GameManager>();
    }

    public virtual void OnDrag(PointerEventData eventData) {
        switch (gM.GameState) {
            case GameState.PlayerTurn:
            Cursor.visible = false;
            transform.position = eventData.position;
            break;
        }
    }

    public virtual void OnEndDrag(PointerEventData eventData) {
        Cursor.visible = true;
        //Debug.Log($"Drag ended on {gameObject.name}");
    }

    public virtual void OnPointerDown(PointerEventData eventData) {
        //Debug.Log($"{gameObject.name} clicked");
    }

    public virtual void OnPointerEnter(PointerEventData eventData) {
        //Debug.Log($"Mouse started hovering on {gameObject.name}");
    }

    public virtual void OnPointerExit(PointerEventData eventData) {
        //Debug.Log($"Mouse exited hovering on {gameObject.name}");
    }
}
