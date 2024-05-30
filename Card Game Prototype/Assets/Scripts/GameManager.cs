using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameState { PlayerTurn, EnemyTurn, AfterCombat, Shop }
public class GameManager : MonoBehaviour {
    [SerializeField] GameState gameState; //Use this later to check what player can do. For example: Not play cards in enemy turn etc.
    public GameState GameState {
        get { return gameState; }
    }
    [SerializeField] float cardMoveRoutineMaxTime;
    public float CardMoveRoutineMaxTime {
        get { return cardMoveRoutineMaxTime; }
    }

    [Header("Card prefabs")]
    [SerializeField] GameObject[] dmgCards;

    [Header("End turn button")]

    [SerializeField] TextMeshProUGUI endTurnButtonTxt;
    public TextMeshProUGUI EndTurnButtonTxt {
        get { return endTurnButtonTxt; }
    }

    BoxCollider2D cardDropArea;
    public BoxCollider2D CardDropArea {
        get { return cardDropArea; }
    }
    Player player;
    PlayerHand hand;
    PlayerCardPile pCP;
    DiscardPile dP;

    void Start() {
        cardDropArea = GameObject.Find("CardDropArea").GetComponent<BoxCollider2D>();
        player = FindObjectOfType<Player>();
        hand = FindObjectOfType<PlayerHand>();
        pCP = FindObjectOfType<PlayerCardPile>();
        dP = FindObjectOfType<DiscardPile>();
        StartCoroutine(InstantiateCardCopy(dmgCards[0]));
    }

    IEnumerator InstantiateCardCopy(GameObject card) { //proto start initiation
        //Shuffle deck cards into PlayerCardPile. Deck part is still yet to be implemented!
        for (int i = 0; i < 10; i++) {
            yield return new WaitForSeconds(0.2f);
            GameObject newCard = Instantiate(card, pCP.transform, false);
            pCP.AddCardIntoCardPile(newCard);
            pCP.UpdateCounter();
        }

        //Remove all cards and add them into hand
        int revOrder = 0;
        for (int i = pCP.Cards.Count - 1; i >= 0; i--) {
            yield return new WaitForSeconds(0.2f);
            pCP.StartCoroutine(pCP.MoveCardToPlayerHand(pCP.Cards[i], 5f, revOrder));
            pCP.RemoveCard(pCP.Cards[i]);
            revOrder++;
        }
    }

    //Call this function from End turn button
    public void StartEndTurn() {
        switch (gameState) {
            case GameState.PlayerTurn:
            endTurnButtonTxt.text = "Enemy turn";
            StartCoroutine(EndTurn());
            break;
        }
    }

    public IEnumerator BeginNewTurn() {
        endTurnButtonTxt.text = "End turn";
        gameState = GameState.PlayerTurn;

        //Placeholder thing to do
        //Check if PlayerCardPile is empty. Yes -> empty discard pile into PlayerCardPile. NOTE no actual check yet, implement later.
        for (int i = dP.Cards.Count - 1; i >= 0; i--) {
            yield return new WaitForSeconds(0.2f);
            dP.StartCoroutine(dP.MoveCardToPlayerCardPile(dP.Cards[i], 5f));
        }

        yield return new WaitForSeconds(CardMoveRoutineMaxTime); //Wait for the cards to be moved before drawing
        player.ResetAP();
        hand.StartCoroutine(hand.DrawCards(player.DrawAmount));
    }

    IEnumerator EndTurn() {
        gameState = GameState.EnemyTurn;
        //Call enemy function stuff
        Enemy enemy = FindObjectOfType<Enemy>(); //Note to future self. This way works only for one enemy at a time.
        yield return enemy.StartCoroutine(enemy.DealDamage(enemy.EnemyData.damage));
    }
}
