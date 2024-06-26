using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiscardPile : MonoBehaviour {
    [SerializeField] List<GameObject> cards = new();
    public List<GameObject> Cards {
        get { return cards; }
    }
    [SerializeField] int cardCount;
    public int CardCount {
        get { return cardCount; }
    }
    [SerializeField] TextMeshProUGUI counterTxt;
    bool moveToPCPDone; //Used to check if routine is done, before continuing
    public bool MoveToPCPDone {
        get { return moveToPCPDone; }
    }
    bool moveToDpDone;
    public bool MoveToDpDone {
        get { return moveToDpDone; }
        set { moveToDpDone = value; }
    }

    GameManager gM;
    DiscardPile dP;
    PlayerCardPile pCP;

    void Start() {
        gM = FindObjectOfType<GameManager>();
        dP = FindObjectOfType<DiscardPile>();
        pCP = FindObjectOfType<PlayerCardPile>();
    }

    public void AddCardIntoDiscardPile(GameObject card) {
        cards.Add(card);
        UpdateCounter();
        StartCoroutine(MoveCardToDiscardPile(card, 5f));
    }

    public void RemoveCard(GameObject card) {
        cards.Remove(card);
        UpdateCounter();
    }

    void UpdateCounter() {
        cardCount = cards.Count;
        counterTxt.text = cardCount.ToString();
    }

    public void ClearDiscardPile() {
        foreach (GameObject card in cards) Destroy(card);
        cards.Clear();
        UpdateCounter();
    }

    IEnumerator MoveCardToDiscardPile(GameObject card, float moveSpeed) {
        moveToDpDone = false;

        float timer = 0f;
        while (card.transform.position != transform.position && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            //Scale card to be smaller as it moves to pile
            card.transform.localScale = new Vector3(card.transform.localScale.x - Time.deltaTime, card.transform.localScale.y - Time.deltaTime);
            yield return card.transform.position = Vector2.Lerp(card.transform.position, transform.position, moveSpeed * Time.deltaTime);
        }
        //Hide card and set discard pile tranform as it's parent
        card.SetActive(false);
        card.transform.SetParent(transform, true);
        //UpdateCounter();
        card.transform.localScale = new Vector3(1f, 1f);
        card.GetComponent<Card>().Clicked = false; //Set Card clicked bool back to false. -> Hovering "animations" work again

        moveToDpDone = true;
    }

    public IEnumerator MoveCardToPlayerCardPile(GameObject card, float moveSpeed) {
        moveToPCPDone = false;

        //Temporarily set card to be visible
        card.SetActive(true);

        //Remove card from the discard pile
        RemoveCard(card);

        float timer = 0f;
        while (Vector2.Distance(card.transform.position, pCP.transform.position) > 0.01f && timer <= gM.CardMoveRoutineMaxTime) { //Gpt while loop muutos
            timer += Time.deltaTime;
            // Scale card to be smaller as it moves to pile
            card.transform.localScale -= Vector3.one * Time.deltaTime;
            card.transform.position = Vector2.Lerp(card.transform.position, pCP.transform.position, moveSpeed * Time.deltaTime);
            yield return null;
        }
        /* While loop ennen gpt muutosta
        while (card.transform.position != pCP.transform.position && timer <= gM.CardMoveRoutineMaxTime) {
            timer += Time.deltaTime;
            //Scale card to be smaller as it moves to pile
            card.transform.localScale = new Vector3(card.transform.localScale.x - Time.deltaTime, card.transform.localScale.y - Time.deltaTime);
            yield return card.transform.position = Vector2.Lerp(card.transform.position, pCP.transform.position, moveSpeed * Time.deltaTime);
        }
        */

        //Hide card and set pile tranform as it's parent
        card.SetActive(false);
        card.transform.SetParent(pCP.transform, true);
        pCP.AddCardIntoCardPile(card);
        pCP.UpdateCounter();
        card.transform.localScale = new Vector3(1f, 1f);

        moveToPCPDone = true;
    }
}
