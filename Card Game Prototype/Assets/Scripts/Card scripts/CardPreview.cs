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
        bool heal = cardData.heal;
        bool block = cardData.block;
        bool recAp = cardData.recoverAp;
        bool buff = cardData.buff;
        bool debuff = cardData.debuff;

        //Triple checks
        if (heal && block && buff) {
            descriptionTxt.text = $"{cardData.description} Heal {cardData.healAmount}, block {cardData.blockAmount} and gain {cardData.buffType} for {cardData.buffDuration} turns";
        }

        //Double checks
        if (draw && dmg) descriptionTxt.text = $"{cardData.description} Draw {cardData.drawAmount} and deal {cardData.damage} damage";
        else if (draw && block) descriptionTxt.text = $"{cardData.description} Block {cardData.blockAmount} and draw {cardData.drawAmount}";
        else if (dmg && heal) descriptionTxt.text = $"{cardData.description} Deal {cardData.damage} and heal {cardData.healAmount}";
        else if (block && recAp) descriptionTxt.text = $"{cardData.description} Block {cardData.blockAmount} and gain {cardData.aPRecoverAmount} action points";

        //Single checks
        else if (draw) descriptionTxt.text = $"{cardData.description} {cardData.drawAmount}";
        else if (dmg) descriptionTxt.text = $"{cardData.description} {cardData.damage} damage";
        else if (heal) descriptionTxt.text = $"{cardData.description} {cardData.healAmount} health";
        else if (block) descriptionTxt.text = $"{cardData.description} {cardData.blockAmount} block";
        else if (recAp) descriptionTxt.text = $"{cardData.description} {cardData.aPRecoverAmount} action points";
        else if (buff) descriptionTxt.text = $"{cardData.description} Gain {cardData.buffType} for {cardData.buffDuration} turns";
    }

    public void AssingNewData() { //random for now
        int rand = Random.Range(0, cardDatas.Count);
        Debug.Log($"Card data assigned to {gameObject.name}: {cardDatas[rand]}");
        cardData = cardDatas[rand];
        CardSetup();
    }
}
