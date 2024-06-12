using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathProgression : MonoBehaviour {
    [Header("Legend icons")]
    [SerializeField] Sprite[] icons;

    [Header("Path buttons")]
    [SerializeField] List<Button> enemyPathButtons = new();
    [SerializeField] List<Button> eventPathButtons = new();
    [SerializeField] List<Button> restPathButtons = new();

    [Header("Path line renderers")]
    [SerializeField] LineRenderer[] paths;

    [Header("Rest canvas")]
    [SerializeField] GameObject restCanvas;

    [Header("Other values")]
    [SerializeField][Tooltip("Time it takes at start for view to scroll from top to bottom")] float viewScrollTime;

    int pathPoint = 0; //Used to determine what point does path linerender draw for TravelledLine
    public int PathPoint {
        get { return pathPoint; }
        set { pathPoint = value; }
    }

    [Header("FindObjectOfType workaround")]
    [SerializeField] Player player;
    GameManager gM;

    void Start() {
        FindPathButtons();
        gM = FindObjectOfType<GameManager>();
        StartCoroutine(MoveContentView());
    }

    void FindPathButtons() {
        GameObject[] enemyButtons = GameObject.FindGameObjectsWithTag("PathButton_Enemy"); //Side note: Tags are assigned manually in inspector
        GameObject[] eventButtons = GameObject.FindGameObjectsWithTag("PathButton_Event");
        GameObject[] restButtons = GameObject.FindGameObjectsWithTag("PathButton_Rest");

        foreach (GameObject button in enemyButtons) enemyPathButtons.Add(button.GetComponent<Button>());
        foreach (GameObject button in eventButtons) eventPathButtons.Add(button.GetComponent<Button>());
        foreach (GameObject button in restButtons) restPathButtons.Add(button.GetComponent<Button>());
        
        AssignButtonIcons();
        AssignRestButtons();
    }

    void AssignButtonIcons() {
        foreach (Button button in enemyPathButtons) button.image.sprite = icons[0];
        foreach (Button button in eventPathButtons) button.image.sprite = icons[1];
        foreach (Button button in restPathButtons) button.image.sprite = icons[2];
    }

    void AssignRestButtons() {
        //foreach (Button button in restPathButtons) button.onClick.AddListener(delegate { Rest(); });
        foreach (Button button in restPathButtons) button.onClick.AddListener(delegate { ActivateRestView(); });
    }

    public void DrawPath(int pIndex) {
        //Add new empty point for TravelledLine linerender
        paths[3].positionCount += 1;

        //Select the lastly added point & draw line based on paths index & pathpoint
        paths[3].SetPosition(paths[3].positionCount - 1, paths[pIndex].GetPosition(pathPoint));
    }

    public void PlaceEnemiesOnPath() { //Not used, did not work as intended. All buttons had the same enemy in the end
        int randomIndex = 0;

        //Add button listener for every button in the paths, that assign a random enemy data to enemy
        foreach (Button button in enemyPathButtons) {
            randomIndex = Random.Range(0, gM.EnemyDatas.Length);
            Debug.Log($"Random index roll: {randomIndex} for {button.name}. {gM.EnemyDatas[randomIndex].enemyName}");
            button.onClick.AddListener(delegate { gM.Enemy.AssignDataValues(gM.EnemyDatas[randomIndex]); });
        }
    }

    #region Button functions
    public void SelectRandomEnemyData(Button button) { //Button press function. Used in PathButtons
        //int rand = Random.Range(0, gM.EnemyDatas.Length);
        int rand = 0;

        //After a button is pressed it also modifies the path point int. Therefore it can be used here to check what range of enemies can spawn on pressed button
        switch (pathPoint) {
            case < 5:
            Debug.Log($"PathPoint: {pathPoint} below below 5. Using GM enemy range between 0 to 2");
            rand = Random.Range(0, 3);
            break;

            case 5:
            Debug.Log($"PathPoint: {pathPoint}. MiniBoss time! Using GM enemy range between 3 to 3");
            rand = Random.Range(3, 4);
            break;

            case < 10:
            Debug.Log($"PathPoint: {pathPoint} below below 10. Using GM enemy range between 1 to 3");
            rand = Random.Range(1, 4);
            break;

            case 20:
            Debug.Log($"PathPoint: {pathPoint}. BOSS time! Selectin boss enemy from boss data");
            rand = 0; //Index 0 for now, because there is only one boss enemy atm.
            break;
        }

        //Second "same" switch to check data sheet to use at different points
        switch (pathPoint) {
            case 20:
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.BossEnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.BossEnemyDatas[rand]);
            break;

            default:
            Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.EnemyDatas[rand].enemyName}");
            gM.Enemy.AssignDataValues(gM.EnemyDatas[rand]);
            break;
        }
    }

    //----Used in Rest canvas buttons----
    public void Rest() {
        float restHeal = player.MaxHp * 0.3f;
        float healResult = player.Health + restHeal;

        if (healResult > player.MaxHp) {
            player.Health = player.MaxHp;
        }
        else {
            player.TakeHeal((int)restHeal);
        }
    }

    public void Pray() {
        player.MaxHp += 2;
    }
    #endregion

    void ActivateRestView() {
        gameObject.SetActive(false);
        restCanvas.SetActive(true);
    }

    //Automatic scrolling for the start, seeing the whole path and realize you can scroll it (hopefully).
    IEnumerator MoveContentView() {
        ScrollRect scrollRect = FindObjectOfType<ScrollRect>();

        //float scrollDuration = 10f;
        float timer = 0f;
        float startValue = scrollRect.verticalNormalizedPosition;
        float endValue = 0f;

        while (timer < viewScrollTime) {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / viewScrollTime);
            yield return scrollRect.verticalNormalizedPosition = Mathf.Lerp(startValue, endValue, t);
        }

        scrollRect.verticalNormalizedPosition = endValue;
    }
}
