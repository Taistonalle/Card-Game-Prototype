using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Component", menuName = "Scriptable Objects/Craft component", order = 1)]
public class CraftComponentData : ScriptableObject {
    [Header("Component values")]
    public int drawAmount;
    public int damage;
    public int healAmount;
    public int blockAmount;
    public int aPRecoverAmount;
    public int playCost;
    public int buffDuration;
    public int debuffDuration;

    [Header("What does component add?")]
    public bool draw;
    public bool dealDamage;
    public bool burnHeal;
    public bool heal;
    public bool block;
    public bool burnRecoverAp;
    public bool recoverAp;
    public bool buff;
    public BuffType buffType;
    public bool debuff;
    public DebuffType debuffType;
}