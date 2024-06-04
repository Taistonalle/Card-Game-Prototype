using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum GameState { PathView, PlayerTurn, EnemyTurn, AfterCombat, Shop, GameOver }
public class GameManager : MonoBehaviour {
    [SerializeField] GameState gameState; //Use this to check what player can do. For example: Not play cards in enemy turn etc.
    public GameState GameState {
        get { return gameState; }
    }
    [SerializeField] float cardMoveRoutineMaxTime;
    public float CardMoveRoutineMaxTime {
        get { return cardMoveRoutineMaxTime; }
    }
    [SerializeField] List<GameObject> deckCopy;

    [Header("Card prefabs")]
    [SerializeField] GameObject[] dmgCards; // Remove later not used atm

    [Header("End turn button")]
    [SerializeField] TextMeshProUGUI endTurnButtonTxt;
    public TextMeshProUGUI EndTurnButtonTxt {
        get { return endTurnButtonTxt; }
    }

    [Header("Game over box")]
    [SerializeField] GameObject gOBox;

    [Header("Card drop area")]
    [SerializeField] BoxCollider2D cardDropArea;
    public BoxCollider2D CardDropArea {
        get { return cardDropArea; }
    }

    [Header("Enemy related")]
    [SerializeField] Enemy enemy;
    public Enemy Enemy {
        get { return enemy; }
    }
    [SerializeField] DataEnemy[] enemyDatas;
    public DataEnemy[] EnemyDatas {
        get { return enemyDatas; }
    }
    [SerializeField] DataEnemy[] bossEnemyDatas;
    public DataEnemy[] BossEnemyDatas {
        get { return bossEnemyDatas; }
    }

    [Header("Player related")]
    [SerializeField] Player player;
    [SerializeField] PlayerHand hand;
    [SerializeField] PlayerCardPile pCP;
    [SerializeField] DiscardPile dP;
    PlayerDeck deck;


    void Start() {
        deck = FindObjectOfType<PlayerDeck>();
        //StartCoroutine(InstantiateCardCopy(dmgCards[0]));
        //StartCoroutine(InstantiateDeckCards());
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

    IEnumerator InstantiateDeckCards() {
        //Add copied deck cards as instiated cards to PlayerCardPile
        foreach (GameObject card in deckCopy) {
            GameObject copiedCard = Instantiate(card, pCP.transform, false);
            pCP.AddCardIntoCardPile(copiedCard);
            pCP.UpdateCounter();
        }

        //Add fixed amount of cards to hand. Hardcoded amount atm: i >= 5
        int revOrder = 0;
        for (int i = pCP.CardCount -1; i >= 5; i--) {
            yield return new WaitForSeconds(0.2f);
            pCP.StartCoroutine(pCP.MoveCardToPlayerHand(pCP.Cards[i], 5f, revOrder));
            pCP.RemoveCard(pCP.Cards[i]);
            revOrder++;
        }
    }

    #region Button functions
    public void StartEndTurn() {
        switch (gameState) {
            case GameState.PlayerTurn:
            endTurnButtonTxt.text = "Enemy turn";
            StartCoroutine(EndTurn());
            break;
        }
    }

    public void Retry() {
        SceneManager.LoadScene(1);
    }

    public void ReturnToMenu() {
        deck.ResetDeck();
        SceneManager.LoadScene(0);
    }

    public void StartCombat() {
        gameState = GameState.PlayerTurn;
        //FindEverything();

        hand.ClearHand();
        pCP.ClearPlayerCardPile();
        dP.ClearDiscardPile();
        //enemy.AssignDataValues();
        player.ResetAP();
        CopyDeckForUsage();
        StartCoroutine(InstantiateDeckCards());

    }

    public void PickRewardButton(Button button) {
        //Get data from the picked card
        var cardData = button.gameObject.GetComponentInParent<CardPreview>().CardData;

        //Loop through card prefabs to compare data. If same -> Add prefab to deck
        for (int i = 0; i < deck.CardPrefabs.Length; i++) {
            if (deck.CardPrefabs[i].GetComponent<Card>().CardData == cardData) {
                Debug.Log($"{deck.CardPrefabs[i].name} matched with {cardData}. Adding {deck.CardPrefabs[i].name} to deck");
                deck.RewardAddCard(deck.CardPrefabs[i]);
                break;
            }
            else {
                Debug.Log($"No match found for {cardData}!");
            }
        }
        enemy.CombatCanvas.SetActive(false);
        enemy.RewardCanvas.SetActive(false);
        enemy.PathCanvas.SetActive(true);
    }

    public void SkipRewardButton() {
        enemy.CombatCanvas.SetActive(false);
        enemy.RewardCanvas.SetActive(false);
        enemy.PathCanvas.SetActive(true);
    }
    #endregion

    //void FindEverything() {
    //    player = FindObjectOfType<Player>();
    //    hand = FindObjectOfType<PlayerHand>();
    //    deck = FindObjectOfType<PlayerDeck>();
    //    pCP = FindObjectOfType<PlayerCardPile>();
    //    dP = FindObjectOfType<DiscardPile>();
    //}

    void CopyDeckForUsage() {
        deckCopy = deck.Cards;
    }

    public IEnumerator BeginNewTurn() {
        endTurnButtonTxt.text = "End turn";
        gameState = GameState.PlayerTurn;

        //Check if PlayerCardPile is empty. Yes -> empty discard pile into PlayerCardPile.
        if (pCP.CardCount == 0) {
            for (int i = dP.Cards.Count - 1; i >= 0; i--) {
                yield return new WaitForSeconds(0.2f);
                dP.StartCoroutine(dP.MoveCardToPlayerCardPile(dP.Cards[i], 5f));
            }
        }

        yield return new WaitForSeconds(CardMoveRoutineMaxTime); //Wait for the cards to be moved before drawing
        player.ResetAP();
        //Check if player can draw a card
        if (hand.CardCount < 10 && pCP.Cards.Count > 0) {
            Debug.Log("Drawing card(s)");
            hand.StartCoroutine(hand.DrawCards(player.DrawAmount));
        }
    }

    IEnumerator EndTurn() {
        gameState = GameState.EnemyTurn;
        //Call enemy function stuff
        //Enemy enemy = FindObjectOfType<Enemy>(); //Note to future self. This way works only for one enemy at a time.
        yield return enemy.StartCoroutine(enemy.DealDamage(enemy.EnemyData.damage));
    }

    public IEnumerator GameOver() {
        yield return gameState = GameState.GameOver;

        //Activate/make game over message box visible. Add fancier fade in effect later?
        gOBox.SetActive(true);
    }
}
