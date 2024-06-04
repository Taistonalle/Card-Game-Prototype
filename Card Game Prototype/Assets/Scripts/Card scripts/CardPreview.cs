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


    void Start() {
        CardSetup();

        //Load all assets in Card datas/Damage folder to get card datas for damage cards
        cardDatas = new List<CardData>(Resources.LoadAll<CardData>("Card datas/Damage"));
    }

    public void CardSetup() {
        background.sprite = cardData.cardBackground;
        background.color = cardData.backgroundColor;
        borders[0].sprite = cardData.cardBorders[0];
        borders[1].sprite = cardData.cardBorders[1];
        cardImage.sprite = cardData.cardImage;
        playCostTxt.text = cardData.playCost.ToString();
        nameTxt.text = cardData.cardName;
        descriptionTxt.text = $"{cardData.description} {cardData.damage} damage";
    }

    public void AssingNewData() { //random for now
        int rand = Random.Range(0, cardDatas.Count);
        Debug.Log($"Card data assigned to {gameObject.name}: {cardDatas[rand]}");
        cardData = cardDatas[rand];
        CardSetup();
    }
}
