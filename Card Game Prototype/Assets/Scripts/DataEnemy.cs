using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlannedAction { Attack, Block, AttackAndBlock, Debuff }
public enum ActionsEnemyCanDo { Atk, AtkAndBlock, All }

[CreateAssetMenu(fileName = "Enemy", menuName = "Scriptable Objects/Enemy", order = 1)]
public class DataEnemy : ScriptableObject {
    public PlannedAction action;
    public string enemyName;
    public int health;
    public int minDamage;
    public int maxDamage;
    public int minBlock;
    public int maxBlock;
    public Sprite enemyArt;
    [Header("What can the enemy do?")]
    //[Tooltip("0  to debuff. So Attack, block, damageAndBlock, debuff = 3")][Range(0, 3)] public int actionRange;
    public ActionsEnemyCanDo enemyCanDo;
    public DebuffType debuffType;
    public int atksBeforeOtherAction;

    [Header("Multiplier values for attack & block combo")]
    [Range(0f, 1f)] public float atkMultiplier;
    [Range(0f, 1f)] public float blockMultiplier;

    [Header("Boss enemy or not?")]
    public bool bossEnemy;
    public bool miniBoss;
}
