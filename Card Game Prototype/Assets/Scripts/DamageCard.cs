using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DamageCard : DragHandler {
    [Header("Card base info")]
    [SerializeField] DataDamageCard dmgCardData;

    Player player;
    Image cardImage;
    Vector3 slotPos;

    GameManager gM;

    void Start() {
        player = FindObjectOfType<Player>();
        cardImage = GetComponent<Image>();
        cardImage.sprite = dmgCardData.cardArt;
        slotPos = transform.position;

        gM = FindObjectOfType<GameManager>();
    }

    public override void OnEndDrag(PointerEventData eventData) {
        base.OnEndDrag(eventData);

        //Debug.Log($"Mouse pos: {eventData.position}");
        if (/*eventData.position.x >= 550 &&*/ eventData.position.y >= 200) {
            Debug.Log("Used card");
            DealDamage(FindObjectOfType<Enemy>()); //Placeholder way of handling damage to target
        }
        else {
            StartCoroutine(MoveCardBackToHand(5f));
        }
    }

    void DealDamage(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.ActionPoints < dmgCardData.playCost) {
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

    IEnumerator MoveCardBackToHand(float moveSpeed) {
        float timer = 0f;
        //Move card to back where it started. After timer reaches 1.5 seconds stop routine. 
        while (transform.position != slotPos && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            yield return transform.position = Vector2.Lerp(transform.position, slotPos, moveSpeed * Time.deltaTime);
        }
        Debug.Log("Move to hand ended");
    }
}
