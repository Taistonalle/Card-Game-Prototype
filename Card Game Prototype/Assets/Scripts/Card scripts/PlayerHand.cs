using CustomAudioManager;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CardSlots {
    public GameObject[] slots; //Make sure these are same size
    public bool[] slotsInUse;
}

public class PlayerHand : MonoBehaviour {
    [SerializeField] List<GameObject> cards = new(); //Used to manage card removal, adding etc
    public List<GameObject> Cards {
        get { return cards; }
    }
    [SerializeField] int cardCount;
    public int CardCount {
        get { return cardCount; }
    }
    [SerializeField] CardSlots cardSlots; //Related to visual side, where cards are seen
    public CardSlots CardSlots {
        get { return cardSlots; }
    }

    bool drawing;
    public bool Drawing {
        get { return drawing; }
    }

    //bool cardPlayed;
    //public bool CardPlayed {
    //    get { return cardPlayed; }
    //    set { cardPlayed = value; }
    //}

    GameManager gM;
    DiscardPile dP;

    private void Start() {
        gM = FindObjectOfType<GameManager>();
        dP = FindObjectOfType<DiscardPile>();
    }

    //private void Update() { //Debug testing purposes only
    //    if (Input.GetKeyDown(KeyCode.Space)) { // Trigger for testing
    //        dP.MoveToDpDone = true;
    //        Debug.Log("Manually setting MoveToDpDone to true for testing");
    //    }
    //}

    public void AddCardIntoHand(GameObject card) {
        cards.Add(card);
        UpdateCounter();
        AssignCardSlot(card);
    }

    public void RemoveCardFromHand(GameObject card) {
        cardSlots.slotsInUse[card.GetComponent<Card>().SlotIndex] = false; //Deactivate slot, so a new card can be assigned to that slot. 
        cards.Remove(card);
        UpdateCounter();
    }

    void UpdateCounter() {
        cardCount = cards.Count;
    }

    public void ClearHand() {
        foreach (GameObject card in cards) Destroy(card);
        for (int i = 0; i < cardSlots.slotsInUse.Length; i++) cardSlots.slotsInUse[i] = false;
        cards.Clear();
        UpdateCounter();
    }

    void AssignCardSlot(GameObject card) {
        //Search slots for empty slot
        for (int i = 0; i < cardSlots.slots.Length; i++) {
            //Found slot, assing card pos and exit loop
            if (!cardSlots.slotsInUse[i]) {
                //Think how to make next to lines better, instead of using specifically just DamageCard
                card.GetComponent<Card>().SlotPos = cardSlots.slots[i].transform.position;
                card.GetComponent<Card>().SlotIndex = i;
                StartCoroutine(MoveCardToCorrectSlot(card, i, 5f));
                cardSlots.slotsInUse[i] = true;
                break;
            }
        }
    }

    public void RearrangeHand() {
        //First check if hand card count is more than zero to continue
        if (cardCount <= 0) return;

        //Look at each current card in hand and assingn slot again if needed
        foreach (GameObject card in cards) {
            int currentCardIndex = card.GetComponent<Card>().SlotIndex;
            //Search for unused slot and check that current card index is not the same as before
            for (int i = 0; i < cardSlots.slotsInUse.Length; i++) {
                if (currentCardIndex > i && !cardSlots.slotsInUse[i]) {
                    //Mark old slot as unused, assing a new slot & parent then move it to new slot
                    cardSlots.slotsInUse[currentCardIndex] = false;
                    card.GetComponent<Card>().SlotPos = cardSlots.slots[i].transform.position;
                    card.GetComponent<Card>().SlotIndex = i;
                    card.transform.SetParent(cardSlots.slots[i].transform, true);
                    cardSlots.slotsInUse[i] = true;
                    StartCoroutine(MoveCardToCorrectSlot(card, i, 10f));
                    break;
                }

                /* Older way, loops through all -> causes to rearrange every card in hand not just necesessary ones
                if (!cardSlots.slotsInUse[i] && currentCardIndex != i) {
                    //Mark old slot as unused, assing a new slot & parent then move it to new slot
                    cardSlots.slotsInUse[currentCardIndex] = false;
                    card.GetComponent<DamageCard>().SlotPos = cardSlots.slots[i].transform.position;
                    card.GetComponent<DamageCard>().SlotIndex = i;
                    card.transform.SetParent(cardSlots.slots[i].transform, true);
                    cardSlots.slotsInUse[i] = true;
                    StartCoroutine(MoveCardToCorrectSlot(card, i, 10f));
                    break;
                }
                */
            }
        }
    }

    public IEnumerator DrawCards(int amount) {
        drawing = true;
        PlayerCardPile cardPile = FindObjectOfType<PlayerCardPile>();
        
        for (int i = 0; i < amount; i++) {
            //First check if card pile still has cards left. No - empty discard pile to player card pile & continue. Yes - Continue
            if (cardPile.CardCount == 0) {
                for (int j = dP.CardCount - 1; j >= 0; j--) {
                    Debug.Log($"Inside the dPC loop, dPC : {j}");
                    //yield return new WaitForSeconds(0.2f); //gpt muutos poisti t�m�n
                    //yield return dP.StartCoroutine(dP.MoveCardToPlayerCardPile(dP.Cards[j], 5f));
                    yield return new WaitForSeconds(0.2f);
                    AudioManager.PlayCardSound();
                    dP.StartCoroutine(dP.MoveCardToPlayerCardPile(dP.Cards[j], 5f));
                }
                yield return new WaitUntil(() => dP.CardCount == 0); //Dont continue until coroutine(s) are done
                cardPile.ShufflePile();
            }

            if (cardPile.CardCount > 0) {
                int slotId = 0;
                //Look for unused slot and use that slot index for coroutine
                for (int k = 0; k < cardSlots.slotsInUse.Length; k++) {
                    if (!cardSlots.slotsInUse[k]) {
                        slotId = k;
                        break;
                    }
                }
                yield return cardPile.StartCoroutine(cardPile.MoveCardToPlayerHand(cardPile.Cards[0], slotId)); //Draw top card & move it to unused slot
                cardPile.RemoveCard(cardPile.Cards[0]);
                //Debug.Log($"Loop: {i}, slotId: {slotId}, slotUsed: {cardSlots.slotsInUse[slotId]}"); // Debugging tool
                AudioManager.PlayCardSound();
                yield return new WaitForSeconds(0.2f);
            }
            else {
                //No more cards
                break;
            }
        }
        drawing = false;
        //cardPlayed = false;
    }

    IEnumerator MoveCardToCorrectSlot(GameObject card, int slotIndex, float moveSpeed) {
        float timer = 0f;
        while (card.transform.position != cardSlots.slots[slotIndex].transform.position && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            yield return card.transform.position = Vector2.Lerp(card.transform.position, cardSlots.slots[slotIndex].transform.position, moveSpeed * Time.deltaTime);
        }
    }
}
