using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPreview : MonoBehaviour {
    [Header("Card info")]
    [SerializeField] CardData cardData;
    public CardData CardData {
        get { return cardData; }
        set { cardData = value; }
    }

    [Header("Card parts")]
    [SerializeField] Image background;
    [SerializeField] Image[] borders;
    [SerializeField] Image cardImage;
    [SerializeField] TextMeshProUGUI playCostTxt;
    [SerializeField] TextMeshProUGUI nameTxt;
    [SerializeField] TextMeshProUGUI descriptionTxt;

    [Header("Card datas")]
    [SerializeField] List<CardData> cardDatas;


    void Awake() {
        CardSetup();

        //Load all assets in Card datas folder to get card datas
        cardDatas = new List<CardData>(Resources.LoadAll<CardData>("Card datas"));
    }

    public void CardSetup() {
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

    public void AssingNewData() { //random for now
        int rand = Random.Range(0, cardDatas.Count);
        Debug.Log($"Card data assigned to {gameObject.name}: {cardDatas[rand]}");
        cardData = cardDatas[rand];
        CardSetup();
    }
}
