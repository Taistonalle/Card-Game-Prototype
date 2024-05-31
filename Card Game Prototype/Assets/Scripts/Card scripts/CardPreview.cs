using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CardPreview : MonoBehaviour {
    [Header("Card info")]
    [SerializeField] DataDamageCard cardData;
    public DataDamageCard CardData {
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

    void Start() {
        CardSetup();
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
}
