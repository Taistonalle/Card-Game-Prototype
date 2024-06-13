using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PathEvent : MonoBehaviour {
    [SerializeField] List<EventData> events = new();
    [SerializeField] EventData usedEvent;

    [Header("Event parts")]
    [SerializeField] TextMeshProUGUI eName;
    [SerializeField] TextMeshProUGUI eDesc;
    [SerializeField] Image eImg;
    [SerializeField] TextMeshProUGUI cHeader;
    [SerializeField] TextMeshProUGUI cOneHeader;
    [SerializeField] TextMeshProUGUI cOneDesc;
    [SerializeField] TextMeshProUGUI cTwoHeader;
    [SerializeField] TextMeshProUGUI cTwoDesc;
    [SerializeField] TextMeshProUGUI cThreeHeader;
    [SerializeField] TextMeshProUGUI cThreeDesc;

    [Header("Buttons")]
    [SerializeField] Button bChoiceOne;
    [SerializeField] Button bChoiceTwo;
    [SerializeField] Button bChoiceThree;
    [SerializeField] Button continueBtn;

    [Header("Reward cards")]
    [SerializeField] GameObject[] rCard;

    [Header("References")]
    [SerializeField] GameObject pathCanvas;
    [SerializeField] Player player;
    [SerializeField] PlayerDeck deck;

    void Awake() {
        deck = FindObjectOfType<PlayerDeck>();
        events = new List<EventData>(Resources.LoadAll<EventData>("Event Datas"));

        bChoiceThree.onClick.AddListener(delegate { ToggleButtonsVisibility(); });
    }

    void PickRandomEventData() {
        int dataIndex = Random.Range(0, events.Count);
        usedEvent = events[dataIndex];

        //Then remove the used event from the list
        events.Remove(usedEvent);
    }

    void EventSetup() {
        PickRandomEventData();

        eName.text = usedEvent.eventName;
        eDesc.text = usedEvent.eventDescription;
        eImg.sprite = usedEvent.eventImgSprite;

        //Choice texts
        cHeader.text = usedEvent.choiceHeader;
        cOneHeader.text = usedEvent.choiceOneHeaderTxt;
        cOneDesc.text = usedEvent.choiceOneDescriptionTxt;
        cTwoHeader.text = usedEvent.choiceTwoHeaderTxt;
        cTwoDesc.text = usedEvent.choiceTwoDescriptionTxt;
        cThreeHeader.text = usedEvent.choiceThreeHeaderTxt;
        cThreeDesc.text = usedEvent.choiceThreeDescriptionTxt;
    }

    void CheckEventName() {
        //Check event names from the scriptable object itself and add more here as more get added
        switch (usedEvent.eventName) {
            case "Makeshift graveyard":
            GraveYardButtons();
            break;

            case "Fountain":
            FountainButtons();
            break;

            case "Worshippers":
            WorshipperButtons();
            break;

            default:
            Debug.Log($"Event name: {usedEvent.eventName} not in PathEvents -> CheckEventName -> switch");
            break;
        }
    }

    void ButtonVisibilty(Button button) {
        switch (button.gameObject.activeSelf) {
            case true:
            button.gameObject.SetActive(false);
            break;

            case false:
            button.gameObject.SetActive(true);
            break;
        }
    }

    void ToggleButtonsVisibility() {
        ButtonVisibilty(bChoiceOne);
        ButtonVisibilty(bChoiceTwo);
        ButtonVisibilty(bChoiceThree);
        ButtonVisibilty(continueBtn);

        //Choice header
        if (cHeader.gameObject.activeSelf) cHeader.gameObject.SetActive(false);
        else cHeader.gameObject.SetActive(true);
    }

    void RemoveOldListeners() {
        bChoiceOne.onClick.RemoveAllListeners();
        bChoiceTwo.onClick.RemoveAllListeners();
    }

    #region Event button assignments
    void GraveYardButtons() {
        int goodOrBad = Random.Range(0, 101);

        //Bad results
        switch (goodOrBad) { //In this case, choice one = statue, choice two = dig/shovel
            case <= 49:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneBad}\n\nYou lost 10 max health and gained a {rCard[0].GetComponent<Card>().CardData.cardName} card!";
                deck.RewardAddCard(rCard[0]);
                player.MaxHp -= 10;
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoBad}\n\nYou lost 5 health";
                player.Health -= 5;
                ToggleButtonsVisibility();
            });
            break;

            //Good results
            case >= 50:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneGood}\n\nYou gained a {rCard[1].GetComponent<Card>().CardData.cardName} card!";
                deck.RewardAddCard(rCard[1]);
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoGood}";
                ToggleButtonsVisibility();
            });
            break;
        }
    }

    void FountainButtons() {
        int goodOrBad = Random.Range(0, 101);

        //Bad results
        switch (goodOrBad) { //In this case, choice one = fountain, choice two = leaf
            case <= 49:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneBad}\n\nYou lost 5 max health and lost 10 health";
                deck.RewardAddCard(rCard[0]);
                player.MaxHp -= 5;
                player.Health -= 10;
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoBad}\n\nYou lost 3 max health";
                player.MaxHp -= 3;
                ToggleButtonsVisibility();
            });
            break;

            //Good results
            case >= 50:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneGood}\n\nYou gained 5 max health and healed for 10 health!";
                player.MaxHp += 5;
                player.Health += 10;
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoGood}\n\nYou lost 1 health and gained a {rCard[2].GetComponent<Card>().CardData.cardName} card!";
                deck.RewardAddCard(rCard[2]);
                player.Health -= 1;
                ToggleButtonsVisibility();
            });
            break;
        }
    }

    void WorshipperButtons() {
        int goodOrBad = Random.Range(0, 101);

        //Bad results
        switch (goodOrBad) { //In this case, choice one = join, choice two = kill
            case <= 49:
            bChoiceOne.onClick.AddListener(delegate {
                int randId = Random.Range(0, deck.CardCount); 
                eDesc.text = $"{usedEvent.choiceOneBad}\n\nYou feel like you also lost something. Removed: {deck.Cards[randId].GetComponent<Card>().CardData.cardName}";
                deck.RemoveCard(deck.Cards[randId]);
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoBad}\n\nYou lost 40 health";
                player.Health -= 40;
                ToggleButtonsVisibility();
            });
            break;

            //Good results
            case >= 50:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneGood}\n\nYou gained a {rCard[3].GetComponent<Card>().CardData.cardName} card!";
                deck.RewardAddCard(rCard[3]);
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoGood}\n\nYou gained 20 max health and 1 max action point!";
                player.MaxHp += 20;
                player.MaxAP += 1;
                ToggleButtonsVisibility();
            });
            break;
        }
    }
    #endregion

    #region Button functions for path
    public void ActivatePathView() {
        gameObject.SetActive(true);
        pathCanvas.SetActive(false);

        ToggleButtonsVisibility();
        RemoveOldListeners();
        EventSetup();
        CheckEventName();
    }

    #endregion
}
