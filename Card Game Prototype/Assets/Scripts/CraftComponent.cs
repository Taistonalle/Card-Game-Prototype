using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CraftComponent : MonoBehaviour {
    [Header("Component values")]
    [SerializeField] int drawAmount;
    public int DrawAmount { get { return drawAmount; } }
    [SerializeField] int damage;
    public int Damage { get { return damage; } }
    [SerializeField] int healAmount;
    public int HealAmount { get { return healAmount; } }
    [SerializeField] int blockAmount;
    public int BlockAmount { get { return blockAmount; } }
    //[SerializeField] int aPRecoverAmount;
    //public int APRecoverAmount { get { return aPRecoverAmount; } }
    [SerializeField] int playCost;
    public int PlayCost { get { return playCost; } }
    [SerializeField] int buffDuration;
    public int BuffDuration { get { return buffDuration; } }
    [SerializeField] int debuffDuration;
    public int DebuffDuration { get { return debuffDuration; } }

    [Header("Max & Min values for randomizer")]
    [SerializeField] int minDrawAmount;
    [SerializeField] int maxDrawAmount;
    [SerializeField] int minDmg;
    [SerializeField] int maxDmg;
    [SerializeField] int minHeal;
    [SerializeField] int maxHeal;
    [SerializeField] int minBlock;
    [SerializeField] int maxBlock;
    //[SerializeField] int minApRec;
    //[SerializeField] int maxApRec;
    [SerializeField] int minPlayCost;
    [SerializeField] int maxPlayCost;

    [Header("What does component add?")]
    [SerializeField] bool draw;
    public bool Draw { get { return draw; } }
    [SerializeField] bool dealDamage;
    public bool DealDamage { get { return dealDamage; } }
    //[SerializeField] bool burnHeal;
    //public bool BurnHeal { get { return burnHeal; } }
    [SerializeField] bool heal;
    public bool Heal { get { return heal; } }
    [SerializeField] bool block;
    public bool Block { get { return block; } }
    //[SerializeField] bool burnRecoverAp;
    //public bool BurnRecoverAp { get { return burnRecoverAp; } }
    //[SerializeField] bool recoverAp;
    //public bool RecoverAp { get { return recoverAp; } }
    [SerializeField] bool buff;
    public bool Buff { get { return buff; } }
    [SerializeField] BuffType buffType;
    public BuffType BuffType { get { return buffType; } }
    [SerializeField] bool debuff;
    public bool Debuff { get { return debuff; } }
    [SerializeField] DebuffType debuffType;
    public DebuffType DebuffType { get { return debuffType; } }

    readonly bool[] componentBools = new bool[6];

    [Header("Things to reference from inspector")]
    [SerializeField] TextMeshProUGUI componentValuesTxt;

    void Start() {
        UpdateDetailsTxt();
    }

    void SetCompBoolsToBeUsed() {
        int randId = Random.Range(0, componentBools.Length);
        componentBools[randId] = true;

        draw = componentBools[0];
        dealDamage = componentBools[1];
        heal = componentBools[2];
        block = componentBools[3];
        //recoverAp = componentBools[4]; //Removed/commented this, because noticed if i got this, I placed it on everything and played in brain dead mode. Takes thinking part away
        buff = componentBools[4];
        debuff = componentBools[5];
    }

    public void RandomizeComponent() {
        SetCompBoolsToBeUsed();

        int randDraw = 0;
        int randDmg = 0;
        int randHeal = 0;
        int randBlock = 0;
        //int randApRec = 0;
        int randPlayCost = 0;
        int randBuffIndex = 0;
        int randBuffDur = 0;
        int randDebuffIndex = 0;
        int randDebuffDur = 0;

        randPlayCost = Random.Range(minPlayCost, maxPlayCost + 1); //Randomize play cost, with exeptions


        if (draw) randDraw = Random.Range(minDrawAmount, maxDrawAmount + 1);
        if (dealDamage) randDmg = Random.Range(minDmg, maxDmg + 1);
        if (heal) randHeal = Random.Range(minHeal, maxHeal + 1);
        if (block) randBlock = Random.Range(minBlock, maxBlock + 1);
        //if (recoverAp) { 
        //    randApRec = Random.Range(minApRec, maxApRec + 1);
        //    randPlayCost = 0; //Always want this kind of component to cost 0
        //}
        if (buff) {
            randBuffIndex = 1/*Random.Range(0, 1 + 1)*/; //Modify this later if more buffs are added
            randBuffDur = Random.Range(1, 3 + 1);
            randPlayCost = Random.Range(2, maxPlayCost + 1); //One cost buff felt bit too powerful. 
        }
        if (debuff) {
            randDebuffIndex = 1/*Random.Range(0, System.Enum.GetNames(typeof(DebuffType)).Length - 1)*/; //Modify this later if more debuffs are added
            //randDebuffDur = Random.Range(1, 3 + 1);
        }

        //Assign the random values to actual values
        drawAmount = randDraw;
        damage = randDmg;
        healAmount = randHeal;
        blockAmount = randBlock;
        //aPRecoverAmount = randApRec;
        switch (randBuffIndex) {
            case 0:
            buffType = BuffType.None;
            break;

            case 1:
            buffType = BuffType.Strenght;
            break;
        }
        buffDuration = randBuffDur;

        switch (randDebuffIndex) {
            case 0:
            debuffType = DebuffType.None;
            break;

            case 1:
            debuffType = DebuffType.Stun;
            randDebuffDur = 1;
            randPlayCost = Random.Range(5, 6 + 1); //Felt like stunning with one cost is too OP. 
            break;
        }
        debuffDuration = randDebuffDur;

        playCost = randPlayCost;
        UpdateDetailsTxt();
    }

    void UpdateDetailsTxt() {
        if (draw) componentValuesTxt.text = $"Draw: {drawAmount}\nPlay cost: {playCost}";
        else if (dealDamage) componentValuesTxt.text = $"Damage: {damage}\nPlay cost: {playCost}";
        else if (heal) componentValuesTxt.text = $"Heal: {healAmount}\nPlay cost: {playCost}";
        else if (block) componentValuesTxt.text = $"Block: {blockAmount}\nPlay cost: {playCost}";
        //else if (recoverAp) componentValuesTxt.text = $"AP recover: {aPRecoverAmount}\nPlay cost: {playCost}";
        else if (buff) componentValuesTxt.text = $"Buff: {buffType} ({buffDuration})\nPlay cost: {playCost}";
        else if (debuff) componentValuesTxt.text = $"Debuff: {debuffType} ({debuffDuration})\nPlay cost: {playCost}";
    }

    public void ResetComponentValues() {
        draw = false;
        dealDamage = false;
        heal = false;
        block = false;
        //recoverAp = false;
        buff = false;
        debuff = false;
        for (int i = 0; i < componentBools.Length; i++) componentBools[i] = false;

        drawAmount = 0;
        damage = 0;
        healAmount = 0;
        blockAmount = 0;
        //aPRecoverAmount = 0;
        buffType = BuffType.None;
        debuffType = DebuffType.None;
    }
}