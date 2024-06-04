using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathProgression : MonoBehaviour {
    [Header("Legend icons")]
    [SerializeField] Sprite[] icons;

    [Header("Path buttons")]
    [SerializeField] List<Button> pathButtons = new();

    [Header("Path line renderers")]
    [SerializeField] LineRenderer[] paths;

    int pathPoint = 0;
    public int PathPoint {
        get { return pathPoint; }
        set { pathPoint = value; }
    }

    GameManager gM;

    void Start() {
        AssignButtonIcons();
        gM = FindObjectOfType<GameManager>();
        //PlaceEnemiesOnPath(); did not work as first expected
    }

    void AssignButtonIcons() {
        foreach (Button button in pathButtons) {
            button.image.sprite = icons[0];
            //Debug.Log($"Icon assing loop: {button}, icon sprite: {button.image.sprite}");
        }
    }

    public void DrawPath(int pIndex) {
        paths[3].positionCount += 1;

        //Lastly added path position
        //paths[3].SetPosition(paths[pIndex].positionCount - 1, new Vector2(0f, 0f));
        paths[3].SetPosition(paths[3].positionCount - 1, paths[pIndex].GetPosition(pathPoint));
    }

    public void PlaceEnemiesOnPath() {
        int randomIndex = 0;

        //Add button listener for every button in the paths, that assign a random enemy data to enemy
        foreach (Button button in pathButtons) {
            randomIndex = Random.Range(0, gM.EnemyDatas.Length);
            Debug.Log($"Random index roll: {randomIndex} for {button.name}. {gM.EnemyDatas[randomIndex].enemyName}");
            button.onClick.AddListener(delegate { gM.Enemy.AssignDataValues(gM.EnemyDatas[randomIndex]); });
        }
    }

    public void SelectRandomEnemyData(Button button) { //Button press function
        int rand = Random.Range(0, gM.EnemyDatas.Length);

        Debug.Log($"Rand index  roll: {rand} for {button.name}. Enemy: {gM.EnemyDatas[rand].enemyName}");
        gM.Enemy.AssignDataValues(gM.EnemyDatas[rand]);
    }
}
