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

    [Header("Event Rewards")]
    [SerializeField] GameObject[] rCard;
    [SerializeField] GameObject[] rComponent;

    [Header("References")]
    [SerializeField] GameObject pathCanvas;
    [SerializeField] Player player;
    [SerializeField] PlayerDeck deck;
    [SerializeField] StatusBar statusBar;
    [SerializeField] CardCrafting cardCrafting;

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

            case "Polished statue":
            PolishedStatueButtons();
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

    void AddRewardComponent(GameObject component) { //Copied and slightly modified version from GameManager PickComponentButton function 
        //Clone the component
        GameObject clone = Instantiate(component, cardCrafting.CompParent.transform);
        int lastId = 0;

        //Is there already component added? Get last Id and move new component next to it. Else move the first component to left side of the scrollable view
        if (cardCrafting.StoredComponents.Count > 0) {
            lastId = cardCrafting.StoredComponents.IndexOf(cardCrafting.StoredComponents[^1]);
            clone.transform.localPosition = cardCrafting.StoredComponents[lastId].transform.localPosition + new Vector3(100f, 0f);
        }
        else clone.transform.localPosition = new Vector3(80f, -50f, 0f); //For some odd reason, in inspector this is really 80, 0, 0. Which is wanted btw.

        //Then add it to list & incrase slider range/size
        cardCrafting.StoredComponents.Add(clone);
        cardCrafting.IncreaseSliderSize();

        //Delegate the function
        clone.GetComponent<Button>().onClick.AddListener(delegate { cardCrafting.AddComponentValues(clone.GetComponent<CraftComponent>()); });
    }

    #region Event button assignments
    void GraveYardButtons() {
        int goodOrBad = Random.Range(0, 101);

        //Bad results
        switch (goodOrBad) { //In this case, choice one = statue, choice two = dig/shovel
            case <= 49:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneBad}\n\nYou lost 10 max health and gained a craft component!";
                AddRewardComponent(rComponent[0]);
                player.MaxHp -= 10;
                statusBar.UpdateHealthTxt();
                statusBar.UpdateDeckCountTxt();
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoBad}\n\nYou lost 5 health";
                player.Health -= 5;
                statusBar.UpdateHealthTxt();
                ToggleButtonsVisibility();
            });
            break;

            //Good results
            case >= 50:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneGood}\n\nYou gained a craft component!";
                AddRewardComponent(rComponent[1]);
                statusBar.UpdateDeckCountTxt();
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
                player.MaxHp -= 5;
                player.Health -= 10;
                statusBar.UpdateHealthTxt();
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoBad}\n\nYou lost 3 max health";
                player.MaxHp -= 3;
                statusBar.UpdateHealthTxt();
                ToggleButtonsVisibility();
            });
            break;

            //Good results
            case >= 50:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneGood}\n\nYou gained 5 max health and healed for 10 health!";
                player.MaxHp += 5;
                player.Health += 10;
                statusBar.UpdateHealthTxt();
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoGood}\n\nYou lost 1 health and gained a craft component!";
                AddRewardComponent(rComponent[2]);
                player.Health -= 1;
                statusBar.UpdateHealthTxt();
                statusBar.UpdateDeckCountTxt();
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
                //int randId = Random.Range(0, deck.CardCount);
                eDesc.text = $"{usedEvent.choiceOneBad}\n\nYou feel like you also lost something. Removed: {deck.Cards[^1].GetComponent<Card>().CardData.cardName}";
                deck.RemoveCard(deck.Cards[^1]);
                statusBar.UpdateDeckCountTxt();
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoBad}\n\nYou lost 40 health";
                player.Health -= 40;
                statusBar.UpdateHealthTxt();
                ToggleButtonsVisibility();
            });
            break;

            //Good results
            case >= 50:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneGood}\n\nYou gained a crafting component!";
                AddRewardComponent(rComponent[3]);
                statusBar.UpdateDeckCountTxt();
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoGood}\n\nYou gained 20 max health and 1 max action point!";
                player.MaxHp += 20;
                player.MaxAP += 1;
                statusBar.UpdateHealthTxt();
                ToggleButtonsVisibility();
            });
            break;
        }
    }

    void PolishedStatueButtons() {
        int goodOrBad = Random.Range(0, 101);

        //Bad results
        switch (goodOrBad) { //In this case, choice one = study, choice two = hide/wait
            case <= 49:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneBad}";
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoBad}";
                ToggleButtonsVisibility();
            });
            break;

            //Good results
            case >= 50:
            bChoiceOne.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceOneGood}\n\nYou gained a crafting component!";
                AddRewardComponent(rComponent[4]);
                statusBar.UpdateDeckCountTxt();
                ToggleButtonsVisibility();
            });
            bChoiceTwo.onClick.AddListener(delegate {
                eDesc.text = $"{usedEvent.choiceTwoGood}\n\nYou gained 5 max health and 2 max action points!";
                player.MaxHp += 5;
                player.MaxAP += 2;
                statusBar.UpdateHealthTxt();
                ToggleButtonsVisibility();
            });
            break;
        }
    }
    #endregion

    public void ActivatePathView() {
        gameObject.SetActive(true);
        pathCanvas.SetActive(false);

        ToggleButtonsVisibility();
        RemoveOldListeners();
        EventSetup();
        CheckEventName();
    }
}
