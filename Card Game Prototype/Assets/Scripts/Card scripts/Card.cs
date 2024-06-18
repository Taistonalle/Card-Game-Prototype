using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

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
        set { clicked = value; }
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
        bool burnHeal = cardData.burnHeal;
        bool heal = cardData.heal;
        bool block = cardData.block;
        bool recAp = cardData.recoverAp;
        bool buff = cardData.buff;
        bool debuff = cardData.debuff;

        //Triple checks
        if (draw && dmg && heal) {
            descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount}, Deal {cardData.damage} damage & Heal {cardData.healAmount}";
        }
        else if (dmg && heal && block) {
            descriptionTxt.text = $"{cardData.description} Deal {cardData.damage} damage, Heal {cardData.healAmount} & Block {cardData.blockAmount}";
        }
        else if (heal && block && buff) {
            descriptionTxt.text = $"{cardData.description} Heal {cardData.healAmount}, Block {cardData.blockAmount} & Gain {cardData.buffType} for {cardData.buffDuration} turns";
        }
        else if (draw && heal && block) {
            descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount},Heal {cardData.healAmount} & Block {cardData.blockAmount}";
        }
        else if (dmg && block && buff) {
            descriptionTxt.text = $"{cardData.description} Deal {cardData.damage}, Block {cardData.blockAmount} & Gain {cardData.buffType} for {cardData.buffDuration} turns";
        }
        else if (draw && block && buff) {
            descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount}, Block {cardData.blockAmount} & Gain {cardData.buffType} for {cardData.buffDuration} turns";
        }
        else if (dmg && heal && buff) {
            descriptionTxt.text = $"{cardData.description} Deal {cardData.damage}, Heal {cardData.healAmount} & Gain {cardData.buffType} for {cardData.buffDuration} turns";
        }
        else if (draw && dmg && buff) {
            descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount}, Deal {cardData.damage} & Gain {cardData.buffType} for {cardData.buffDuration} turns";
        }

        //Double checks
        else if (draw && dmg) descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount} & Deal {cardData.damage} damage";
        else if (dmg && heal) descriptionTxt.text = $"{cardData.description} Deal {cardData.damage} damage & Heal {cardData.healAmount}";
        else if (heal && block) descriptionTxt.text = $"{cardData.description} Heal {cardData.healAmount} & Block {cardData.blockAmount}";
        else if (block && recAp) descriptionTxt.text = $"{cardData.description} Block {cardData.blockAmount} & Gain {cardData.aPRecoverAmount} action points";
        else if (block && buff) descriptionTxt.text = $"{cardData.description} Block {cardData.blockAmount} & Gain {cardData.buffType} for {cardData.buffDuration} turns";
        else if (draw && heal) descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount} & Heal {cardData.healAmount}";
        else if (dmg && block) descriptionTxt.text = $"{cardData.description} Deal {cardData.damage} damage & Block {cardData.blockAmount}";
        else if (heal && buff) descriptionTxt.text = $"{cardData.description} Heal {cardData.healAmount} & Gain {cardData.buffType} for {cardData.buffDuration} turns";
        else if (draw && block) descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount} & Block {cardData.blockAmount}";
        else if (dmg && buff) descriptionTxt.text = $"{cardData.description} Deal {cardData.damage} damage & Gain {cardData.buffType} for {cardData.buffDuration} turns";
        else if (draw && buff) descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount} & Gain {cardData.buffType} for {cardData.buffDuration} turns";

        //Single checks
        else if (draw) descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount}";
        else if (dmg) descriptionTxt.text = $"{cardData.description} Deal {cardData.damage} damage";
        else if (burnHeal) descriptionTxt.text = $"{cardData.description} Heal {cardData.healAmount}";
        else if (heal) descriptionTxt.text = $"{cardData.description} Heal {cardData.healAmount}";
        else if (block) descriptionTxt.text = $"{cardData.description} Block {cardData.blockAmount}";
        else if (recAp) descriptionTxt.text = $"{cardData.description} Gain {cardData.aPRecoverAmount} action points";
        else if (buff) descriptionTxt.text = $"{cardData.description} Gain {cardData.buffType} for {cardData.buffDuration} turns";

        else descriptionTxt.text = "";
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
            //Does player have strenght buff?
            switch (player.Buffs[0]) {
                case BuffEffect.Strenght:
                int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
                Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
                target.TakeDamage(totalDmg);
                break;

                default:
                target.TakeDamage(cardData.damage);
                break;
            }
            player.ReduceAP(cardData.playCost);

            //Add card to discard pile
            dP.AddCardIntoDiscardPile(gameObject);
            hand.RemoveCardFromHand(gameObject);
            hand.RearrangeHand();
        }
    }

    void BurnHeal() {
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

    void Block() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Block and cost calculations
        player.GainBlock(cardData.blockAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void RecoverAp() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Ap recovery and cost calculations
        player.RecoverAP(cardData.aPRecoverAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void Buff() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Buff check & cost calculations
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void DrawAndDealDamage(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
            //StopCoroutine(nameof(DrawAndDealDamage));
        }
        else {
            DiscardPile dP = FindObjectOfType<DiscardPile>();
            PlayerHand hand = FindObjectOfType<PlayerHand>();

            //Damage and action point calculations
            //Does player have strenght buff?
            switch (player.Buffs[0]) {
                case BuffEffect.Strenght:
                int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
                Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
                target.TakeDamage(totalDmg);
                break;

                default:
                target.TakeDamage(cardData.damage);
                break;
            }
            player.ReduceAP(cardData.playCost);

            //Add card to discard pile and draw
            dP.AddCardIntoDiscardPile(gameObject);
            hand.RemoveCardFromHand(gameObject);
            hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
            //yield return new WaitUntil(() => !hand.Drawing);
            hand.RearrangeHand();
        }
    }

    void BlockAndDraw() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Block and cost calculations
        player.GainBlock(cardData.blockAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile and draw
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        hand.RearrangeHand();
    }

    void DealDamageAndHeal(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Damage, Heal and cost calculations
        //Does player have strenght buff?
        switch (player.Buffs[0]) {
            case BuffEffect.Strenght:
            int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
            Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
            target.TakeDamage(totalDmg);
            break;

            default:
            target.TakeDamage(cardData.damage);
            break;
        }
        player.TakeHeal(cardData.healAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void HealAndBlock() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Heal, block and cost calculations
        player.TakeHeal(cardData.healAmount);
        player.GainBlock(cardData.blockAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void BlockAndRecoverAp() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Block, Ap recovery and cost calculations
        player.RecoverAP(cardData.aPRecoverAmount);
        player.GainBlock(cardData.blockAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void BlockAndBuff() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Block, buff check and cost calculations
        player.GainBlock(cardData.blockAmount);
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void DrawAndHeal() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Heal and cost calculations
        player.TakeHeal(cardData.healAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile and draw
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        hand.RearrangeHand();
    }

    void DealDamageAndBlock(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Damage, Block and cost calculations
        //Does player have strenght buff?
        switch (player.Buffs[0]) {
            case BuffEffect.Strenght:
            int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
            Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
            target.TakeDamage(totalDmg);
            break;

            default:
            target.TakeDamage(cardData.damage);
            break;
        }
        player.GainBlock(cardData.blockAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void HealAndBuff() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Heal, buff check and cost calculations
        player.TakeHeal(cardData.healAmount);
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void DealDamageAndBuff(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Damage, buff check and cost calculations
        //Does player have strenght buff?
        switch (player.Buffs[0]) {
            case BuffEffect.Strenght:
            int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
            Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
            target.TakeDamage(totalDmg);
            break;

            default:
            target.TakeDamage(cardData.damage);
            break;
        }
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void DrawAndBuff() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Buff check and cost calculations
        player.GainBuff(cardData.buffType, cardData.blockAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile and draw
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        hand.RearrangeHand();
    }

    void DrawDealDamageAndHeal(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Damage, Heal and cost calculations
        //Does player have strenght buff?
        switch (player.Buffs[0]) {
            case BuffEffect.Strenght:
            int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
            Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
            target.TakeDamage(totalDmg);
            break;

            default:
            target.TakeDamage(cardData.damage);
            break;
        }
        player.TakeHeal(cardData.healAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile and draw
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        hand.RearrangeHand();
    }

    void DealDamageHealAndBlock(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Damage, Heal, Block and cost calculations
        //Does player have strenght buff?
        switch (player.Buffs[0]) {
            case BuffEffect.Strenght:
            int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
            Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
            target.TakeDamage(totalDmg);
            break;

            default:
            target.TakeDamage(cardData.damage);
            break;
        }
        player.TakeHeal(cardData.healAmount);
        player.GainBlock(cardData.blockAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void HealBlockAndBuff() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Heal, Block, buff check and cost calculations
        player.TakeHeal(cardData.healAmount);
        player.GainBlock(cardData.blockAmount);
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void DrawHealAndBlock() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Heal, Block and cost calculations
        player.TakeHeal(cardData.healAmount);
        player.GainBlock(cardData.blockAmount);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile and draw
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        hand.RearrangeHand();
    }

    void DealDamageBlockAndBuff(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Damage, Block, buff check and cost calculations
        //Does player have strenght buff?
        switch (player.Buffs[0]) {
            case BuffEffect.Strenght:
            int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
            Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
            target.TakeDamage(totalDmg);
            break;

            default:
            target.TakeDamage(cardData.damage);
            break;
        }
        player.GainBlock(cardData.blockAmount);
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void DrawBlockAndBuff() {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Block, buff check and cost calculations
        player.GainBlock(cardData.blockAmount);
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile and draw
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        hand.RearrangeHand();
    }

    void DealDamageHealAndBuff(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Damage, Heal, buff check and cost calculations
        //Does player have strenght buff?
        switch (player.Buffs[0]) {
            case BuffEffect.Strenght:
            int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
            Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
            target.TakeDamage(totalDmg);
            break;

            default:
            target.TakeDamage(cardData.damage);
            break;
        }
        player.TakeHeal(cardData.healAmount);
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.RearrangeHand();
    }

    void DrawDealDamageAndBuff(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Damage, buff check and cost calculations
        //Does player have strenght buff?
        switch (player.Buffs[0]) {
            case BuffEffect.Strenght:
            int totalDmg = Mathf.RoundToInt(player.StrDmgMultiplier * cardData.damage);
            Debug.Log($"Toltal dmg after multiplier: {totalDmg}");
            target.TakeDamage(totalDmg);
            break;

            default:
            target.TakeDamage(cardData.damage);
            break;
        }
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile and draw
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        hand.RearrangeHand();
    }

    void DrawHealAndBuff(Enemy target) {
        //Fist check if player has enouch action points to use this card. Yes -> Continue. No -> Jump out of function and give indication for error
        if (player.AP < cardData.playCost) {
            Debug.Log("Not enough AP to play this card");
            StartCoroutine(MoveCardBackToHand(5f));
            return;
        }

        DiscardPile dP = FindObjectOfType<DiscardPile>();
        PlayerHand hand = FindObjectOfType<PlayerHand>();

        //Heal, buff check and cost calculations
        player.TakeHeal(cardData.healAmount);
        player.GainBuff(cardData.buffType, cardData.buffDuration);
        player.ReduceAP(cardData.playCost);

        //Add card to discard pile and draw
        dP.AddCardIntoDiscardPile(gameObject);
        hand.RemoveCardFromHand(gameObject);
        hand.StartCoroutine(hand.DrawCards(cardData.drawAmount));
        hand.RearrangeHand();
    }
    #endregion

    void CheckCardPos() {
        //Check if card is dropped on top of drop are or not
        ColliderDistance2D colDist = GetComponent<BoxCollider2D>().Distance(gM.CardDropArea);

        //Below zero == colliders overlap --> Card dropped on drop area
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
        bool burnHeal = cardData.burnHeal;
        bool heal = cardData.heal;
        bool block = cardData.block;
        bool recAp = cardData.recoverAp;
        bool buff = cardData.buff;
        bool debuff = cardData.debuff;

        //Triple checks
        if (draw && dmg && heal) DrawDealDamageAndHeal(target);
        else if (dmg && heal && block) DealDamageHealAndBlock(target);
        else if (heal && block && buff) HealBlockAndBuff();
        else if (draw && heal && block) DrawHealAndBlock();
        else if (dmg && block && buff) DealDamageBlockAndBuff(target);
        else if (draw && block && buff) DrawBlockAndBuff();
        else if (dmg && heal && buff) DealDamageHealAndBuff(target);
        else if (draw && dmg && buff) DrawDealDamageAndBuff(target);
        else if (draw && heal && buff) DrawHealAndBuff(target);

        /* 
        //Triple checks old
        if (heal && block && buff) HealBlockAndBuff();
        */

        //Double checks
        else if (draw && dmg) DrawAndDealDamage(target);
        else if (dmg && heal) DealDamageAndHeal(target);
        else if (heal && block) HealAndBlock();
        else if (block && recAp) BlockAndRecoverAp();
        else if (block && buff) BlockAndBuff();
        else if (draw && heal) DrawAndHeal();
        else if (dmg && block) DealDamageAndBlock(target);
        else if (heal && buff) HealAndBuff();
        else if (draw && block) BlockAndDraw();
        else if (dmg && buff) DealDamageAndBuff(target);
        else if (draw && buff) DrawAndBuff();
        /*
        //Double checks old
        else if (draw && dmg) DrawAndDealDamage(target);
        else if (draw && block) BlockAndDraw();
        else if (dmg && heal) DealDamageAndHeal(target);
        else if (block && recAp) BlockAndRecoverAp();
        */

        //Single checks
        else if (draw) Draw();
        else if (dmg) DealDamage(target);
        else if (burnHeal) BurnHeal();
        else if (block) Block();
        else if (recAp) RecoverAp();
        else if (buff) Buff();
        else if (debuff) Debug.Log("Implement debuff card");

        //Not in the checks yet
        else Debug.Log("Card functions not implement yet");
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
    #endregion
}
