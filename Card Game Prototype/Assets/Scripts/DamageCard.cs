using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DamageCard : DragAndPointerHandler {
    [Header("Card base info")]
    [SerializeField] DataDamageCard dmgCardData;

    Player player;
    Image cardImage;
    Vector3 slotPos;
    public Vector3 SlotPos {
        get { return slotPos; }
        set { slotPos = value; }
    }
    int slotIndex; //Used for to activate/deactivate slotInUse in PlayerHand.cs struct
    public int SlotIndex {
        get { return slotIndex; }
        set { slotIndex = value; }
    }

    bool clicked; //Helps to prevent weird behaviour after dragging
    public bool Clicked { 
        get { return clicked; }
        set {  clicked = value; }
    }

    void Start() {
        player = FindObjectOfType<Player>();
        cardImage = GetComponent<Image>();
        cardImage.sprite = dmgCardData.cardArt;
        transform.localScale = new Vector2(1f, 1f);
    }

    #region Functions
    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);
        switch (gM.GameState) {
            case GameState.PlayerTurn:
            CheckCardPos();
            break;

            default:
            Debug.Log("Drag failed, not player turn");
            break;
        }
    }

    public override void OnPointerEnter(PointerEventData eventData) {
        switch (clicked) {
            case false:
            base.OnPointerEnter(eventData);
            MouseHovered(30f);
            break;
        }
    }

    public override void OnPointerExit(PointerEventData eventData) {
        switch (clicked) {
            case false:
            base.OnPointerExit(eventData);
            transform.position = slotPos;
            break;
        }
    }

    public override void OnPointerDown(PointerEventData eventData) {
        switch (gM.GameState) {
            case GameState.PlayerTurn:
            base.OnPointerDown(eventData);
            clicked = true;
            break;
        }
    }

    void MouseHovered(float posAmount) {
        transform.position = new Vector2(transform.position.x, transform.position.y + posAmount);
    }

    void DealDamage(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < dmgCardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }
        else {
            DiscardPile dP = FindObjectOfType<DiscardPile>();
            PlayerHand hand = FindObjectOfType<PlayerHand>();

            //Damage and action point calculations
            target.TakeDamage(dmgCardData.damage);
            player.ReduceAP(dmgCardData.playCost);

            //Add card to discard pile
            dP.AddCardIntoDiscardPile(gameObject);
            hand.RemoveCardFromHand(gameObject);
        }
    }

    void CheckCardPos() {
        //Check if card is dropped on top of drop are or not
        ColliderDistance2D colDist = GetComponent<BoxCollider2D>().Distance(gM.CardDropArea);

        //Below zero == colliders overlap --> Card dropped on drop area
        if (colDist.distance <= 0) DealDamage(FindObjectOfType<Enemy>()); //Placeholder way of handling damage to target
        else StartCoroutine(MoveCardBackToHand(5f));
    }
    #endregion

    #region IEnumerators
    IEnumerator MoveCardBackToHand(float moveSpeed) {
        float timer = 0f;
        //Move card to back where it started. After timer reaches max routine time stop routine. 
        while (transform.position != slotPos && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            yield return transform.position = Vector2.Lerp(transform.position, slotPos, moveSpeed * Time.deltaTime);
        }
        clicked = false; //Started dragging card, but decided to not play card instead.
        Debug.Log("Move to hand ended");
    }

    /* Stops dragging, not good
    IEnumerator MouseHovered(float moveSpeed, float moveAmount) {
        float timer = 0f;
        while (transform.position.y != slotPos.y + 5f) {
            timer += Time.deltaTime;
            yield return transform.position = Vector2.Lerp(transform.position, new Vector2(slotPos.x, slotPos.y + moveAmount), moveSpeed * Time.deltaTime);
        }
    }
    */
    #endregion
}
