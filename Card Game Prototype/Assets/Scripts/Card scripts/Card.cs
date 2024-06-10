using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : DragAndPointerHandler {
    [Header("Card base info from scriptable object")]
    [SerializeField] CardData cardData;
    public CardData CardData { 
        get { return cardData; }
    }

    [Header("Card parts")]
    [SerializeField] Image background;
    [SerializeField] Image[] borders;
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI playCostTxt;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI descriptionTxt;

    Player player;
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
        CardSetup();
    }

    #region Functions
    void CardSetup() {
        background.sprite = cardData.cardBackground;
        background.color = cardData.backgroundColor;
        borders[0].sprite = cardData.cardBorders[0];
        borders[1].sprite = cardData.cardBorders[1];
        cardImage.sprite = cardData.cardImage;
        playCostTxt.text = cardData.playCost.ToString();
        nameTxt.text = cardData.cardName;
        //descriptionTxt.text = $"{cardData.description} {cardData.damage} damage";

        //Checks for bools in data, to know what to write in description

        //Single bools
        bool draw = cardData.draw;
        bool dmg = cardData.dealDamage;
        bool heal = cardData.heal;
        bool block = cardData.block;
        bool recAp = cardData.recoverAp;
        bool buff = cardData.buff;
        bool debuff = cardData.debuff;

        //Double checks
        if (draw && dmg) descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount} and deal {cardData.damage} damage";

        //Single checks
        else if (draw) descriptionTxt.text = $"{cardData.description} {cardData.drawAmount}";
        else if (dmg) descriptionTxt.text = $"{cardData.description} {cardData.damage} damage";
        else if (heal) descriptionTxt.text = $"{cardData.description} {cardData.healAmount} health";

    }

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

    #region Card actions
    void Draw() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            StopCoroutine(nameof(Draw));
        }
        else {
            DiscardPile dP = FindObjectOfType<DiscardPile>();
            PlayerHand hand = FindObjectOfType<PlayerHand>();

            //Action point calculations
            player.ReduceAP(cardData.playCost);

            //Add card to discard pile and draw
            dP.AddCardIntoDiscardPile(gameObject);
            hand.RemoveCardFromHand(gameObject);
            hand.RearrangeHand();
            hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        }
    }

    void DealDamage(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }
        else {
            DiscardPile dP = FindObjectOfType<DiscardPile>();
            PlayerHand hand = FindObjectOfType<PlayerHand>();

            //Damage and action point calculations
            target.TakeDamage(cardData.damage);
            player.ReduceAP(cardData.playCost);

            //Add card to discard pile
            dP.AddCardIntoDiscardPile(gameObject);
            hand.RemoveCardFromHand(gameObject);
            hand.RearrangeHand();
        }
    }

    void Heal() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        //DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();
        PlayerDeck deck = FindObjectOfType<PlayerDeck>();

        //Health and action point calculations
        player.TakeHeal(cardData.healAmount);
        player.ReduceAP(cardData.playCost);

        //Remove card from hand list and "burn/destroy/delete" card
        hand.RemoveCardFromHand(gameObject);
        deck.RemoveCard(gameObject);
        Destroy(gameObject); //Maybe add fancier way to remove card later
        hand.RearrangeHand();

        /* Before burning heal card idea came to mind
        //Add card do discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
        */
    }

    void DrawAndDealDamage(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            StopCoroutine(nameof(DrawAndDealDamage));
        }
        else {
            DiscardPile dP = FindObjectOfType<DiscardPile>();
            PlayerHand hand = FindObjectOfType<PlayerHand>();

            //Damage and action point calculations
            target.TakeDamage(cardData.damage);
            player.ReduceAP(cardData.playCost);

            //Add card to discard pile and draw
            dP.AddCardIntoDiscardPile(gameObject);
            hand.RemoveCardFromHand(gameObject);
            hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
            //yield return new WaitUntil(() => !hand.Drawing);
            hand.RearrangeHand();
        }
    }
    #endregion

    void CheckCardPos() {
        //Check if card is dropped on top of drop are or not
        ColliderDistance2D colDist = GetComponent<BoxCollider2D>().Distance(gM.CardDropArea);

        //Below zero == colliders overlap --> Card dropped on drop area
        //if (colDist.distance <= 0) DealDamage(FindObjectOfType<Enemy>()); //Placeholder way of handling damage to target
        if (colDist.distance <= 0) CheckCardDetails();
        else StartCoroutine(MoveCardBackToHand(5f));
    }

    //Function to check what the card can do
    void CheckCardDetails() {
        Enemy target = FindObjectOfType<Enemy>();
        //Checks for bools in data, to know what to do

        //Single bools
        bool draw = cardData.draw;
        bool dmg = cardData.dealDamage;
        bool heal = cardData.heal;
        bool block = cardData.block;
        bool recAp = cardData.recoverAp;
        bool buff = cardData.buff;
        bool debuff = cardData.debuff;

        //Double checks
        if (draw && dmg) DrawAndDealDamage(target);

        //Single checks
        else if (draw) Draw();
        else if (dmg) DealDamage(target);
        else if (heal) Heal();
        else if (block) Debug.Log("Implement block card");
        else if (recAp) Debug.Log("Implement AP recovery card");
        else if (buff) Debug.Log("Implement buff card");
        else if (debuff) Debug.Log("Implement debuff card");

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
