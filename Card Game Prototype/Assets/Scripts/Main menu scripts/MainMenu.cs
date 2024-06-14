using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    [Header("Buttons")]
    [SerializeField] GameObject[] buttons;

    [Header("Card adding view")]
    [SerializeField] TextMeshProUGUI cardCounterTxt;
    [SerializeField] int viewedCardIndex;
    [SerializeField] List<CardData> cardDatas = new(); //Adjusted in inspector. Data needs to be in same order as PlayerDeck MainMenuCardPrefabs
    public List<CardData> CardDatas {
        get { return cardDatas; }
    }

    [Header("Preview card")]
    [SerializeField] CardPreview cardPreview;
    PlayerDeck pDeck;


    void Start() {
        pDeck = FindObjectOfType<PlayerDeck>();
        //cardPreview = GetComponentInChildren<CardPreview>();
        //ButtonSetup();
    }

    #region Button functions
    public void NewGame() { // Not used atm
        SceneManager.LoadScene(1);
    }

    public void Options() {
        Debug.Log("Yet to be implemented :)");
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void PreviousCard() {
        if (viewedCardIndex == 0) return;
        viewedCardIndex -= 1;
        DisplayViewedCard(viewedCardIndex);
    }

    public void NextCard() {
        if (viewedCardIndex >= pDeck.MaxStartCardAmount || viewedCardIndex >= pDeck.MainMenuCardPrefabs.Length - 1) return;
        viewedCardIndex += 1;
        DisplayViewedCard(viewedCardIndex);
    }

    public void AddToDeck() {
        pDeck.MenuAddCard(pDeck.MainMenuCardPrefabs[viewedCardIndex]);
        UpdateCounter();
        if (pDeck.CardCount >= pDeck.MaxStartCardAmount) {
            buttons[4].SetActive(true);
        }
    }

    public void Continue() {
        SceneManager.LoadScene(1);
        pDeck.OriginalDeck = new List<GameObject>(pDeck.Cards); //Copy the created card selection list
    }
    #endregion

    /*
    void ButtonSetup() {
        buttons[3].GetComponent<Button>().onClick.AddListener(delegate { pDeck.MenuAddCard(pDeck.MainMenuCardPrefabs[viewedCardIndex]); });
        buttons[3].GetComponent<Button>().onClick.AddListener(delegate { UpdateCounter(); });
    }
    */

    void UpdateCounter() {
        cardCounterTxt.text = $"Cards in deck: {pDeck.CardCount}/{pDeck.MaxStartCardAmount}";
    }

    void DisplayViewedCard(int dataIndex) {
        cardPreview.CardData = cardDatas[dataIndex];
        cardPreview.CardSetup();
    }
}
