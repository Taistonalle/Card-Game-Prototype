using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlannedAction { Attack, Debuff, BlockOrDodge }

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy", order = 1)]
public class DataEnemy : ScriptableObject {
    public PlannedAction action;
    public string enemyName;
    public int health;
    public int damage;
    public Sprite enemyArt;
    public bool canStun;
    public bool canDebuff;
    public bool bossEnemy;
}
