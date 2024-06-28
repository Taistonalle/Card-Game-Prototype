using TMPro;
using UnityEngine.UI;

public struct CardStruct {
    readonly public void CardSetup(Image bg, Image topBorder, Image botBorder, Image cardImg, TextMeshProUGUI cost, TextMeshProUGUI name, TextMeshProUGUI desc, CardData data) {
        bg.sprite = data.cardBackground;
        bg.color = data.backgroundColor;
        topBorder.sprite = data.cardBorders[0];
        botBorder.sprite = data.cardBorders[1];
        cardImg.sprite = data.cardImage;
        cost.text = data.playCost.ToString();
        name.text = data.cardName;

        //Checks for bools in data, to know what to write in description
        //Single bools
        bool draw = data.draw;
        bool dmg = data.dealDamage;
        bool burnHeal = data.burnHeal;
        bool heal = data.heal;
        bool block = data.block;
        bool burnRecAp = data.burnRecoverAp;
        bool recAp = data.recoverAp;
        bool buff = data.buff;
        bool debuff = data.debuff;

        //Triple checks
        if (draw && dmg && heal) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Deal {data.damage} damage & Heal {data.healAmount}";
        }
        else if (dmg && heal && block) {
            desc.text = $"{data.description} Deal {data.damage} damage, Heal {data.healAmount} & Block {data.blockAmount}";
        }
        else if (heal && block && buff) {
            desc.text = $"{data.description} Heal {data.healAmount}, Block {data.blockAmount} & Gain {data.buffType} for {data.buffDuration} turns";
        }
        else if (draw && heal && block) {
            desc.text = $"{data.description} Draw {data.drawAmount},Heal {data.healAmount} & Block {data.blockAmount}";
        }
        else if (dmg && block && buff) {
            desc.text = $"{data.description} Deal {data.damage} damage, Block {data.blockAmount} & Gain {data.buffType} for {data.buffDuration} turns";
        }
        else if (draw && block && buff) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Block {data.blockAmount} & Gain {data.buffType} for {data.buffDuration} turns";
        }
        else if (draw && dmg && block) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Deal {data.damage} damage & Block {data.blockAmount}";
        }
        else if (dmg && heal && buff) {
            desc.text = $"{data.description} Deal {data.damage} damage, Heal {data.healAmount} & Gain {data.buffType} for {data.buffDuration} turns";
        }
        else if (draw && dmg && buff) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Deal {data.damage} damage & Gain {data.buffType} for {data.buffDuration} turns";
        }
        else if (draw && heal && buff) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Heal {data.healAmount} & Gain {data.buffType} for {data.buffDuration} turns";
        }
        else if (heal && block && recAp) {
            desc.text = $"{data.description} Heal {data.healAmount}, Block {data.blockAmount} & Gain {data.aPRecoverAmount} action points";
        }
        else if (dmg && block && recAp) {
            desc.text = $"{data.description} Deal {data.damage} damage, Block {data.blockAmount} & Gain {data.aPRecoverAmount} action points";
        }
        else if (draw && dmg && recAp) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Deal {data.damage} damage & Gain {data.aPRecoverAmount} action points";
        }
        else if (draw && heal && recAp) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Heal {data.healAmount} & Gain {data.aPRecoverAmount} action points";
        }
        else if (draw && block && recAp) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Block {data.blockAmount} & Gain {data.aPRecoverAmount} action points";
        }
        else if (draw && dmg && debuff) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Deal {data.damage} & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (draw && heal && debuff) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Heal {data.healAmount} & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (draw && block && debuff) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Block {data.blockAmount} & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (draw && recAp && debuff) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Gain {data.aPRecoverAmount} action points & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (draw && buff && debuff) {
            desc.text = $"{data.description} Draw {data.drawAmount}, Gain {data.buffType} for {data.buffDuration} & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (dmg && heal && debuff) {
            desc.text = $"{data.description} Deal {data.damage} damage, Heal {data.healAmount} & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (dmg && block && debuff) {
            desc.text = $"{data.description} Deal {data.damage} damage, Block {data.blockAmount} & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (dmg && buff && debuff) {
            desc.text = $"{data.description} Deal {data.damage} damage, Gain {data.buffType} for {data.buffDuration} turns & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (block && buff && debuff) {
            desc.text = $"{data.description} Block {data.blockAmount}, Gain {data.buffType} for {data.buffDuration} turns & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (heal && block && debuff) {
            desc.text = $"{data.description} Heal {data.healAmount}, Block {data.blockAmount} & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (heal && recAp && debuff) {
            desc.text = $"{data.description} Heal {data.healAmount}, Gain {data.recoverAp} action points & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (heal && buff && debuff) {
            desc.text = $"{data.description} Heal {data.healAmount}, Gain {data.buffType} for {data.buffDuration} turns & {data.debuffType} for {data.debuffDuration} turns";
        }
        else if (recAp && buff && debuff) {
            desc.text = $"{data.description} Gain {data.aPRecoverAmount} action points, Gain {data.buffType} for {data.buffDuration} turns & {data.debuffType} for {data.debuffDuration} turns";
        }

        //Double checks
        else if (draw && dmg) desc.text = $"{data.description} Draw {data.drawAmount} & Deal {data.damage} damage";
        else if (dmg && heal) desc.text = $"{data.description} Deal {data.damage} damage & Heal {data.healAmount}";
        else if (heal && block) desc.text = $"{data.description} Heal {data.healAmount} & Block {data.blockAmount}";
        else if (block && recAp) desc.text = $"{data.description} Block {data.blockAmount} & Gain {data.aPRecoverAmount} action points";
        else if (block && buff) desc.text = $"{data.description} Block {data.blockAmount} & Gain {data.buffType} for {data.buffDuration} turns";
        else if (draw && heal) desc.text = $"{data.description} Draw {data.drawAmount} & Heal {data.healAmount}";
        else if (dmg && block) desc.text = $"{data.description} Deal {data.damage} damage & Block {data.blockAmount}";
        else if (dmg && recAp) desc.text = $"{data.description} Deal {data.damage} damage & Gain {data.aPRecoverAmount} action points";
        else if (heal && buff) desc.text = $"{data.description} Heal {data.healAmount} & Gain {data.buffType} for {data.buffDuration} turns";
        else if (draw && block) desc.text = $"{data.description} Draw {data.drawAmount} & Block {data.blockAmount}";
        else if (dmg && buff) desc.text = $"{data.description} Deal {data.damage} damage & Gain {data.buffType} for {data.buffDuration} turns";
        else if (draw && buff) desc.text = $"{data.description} Draw {data.drawAmount} & Gain {data.buffType} for {data.buffDuration} turns";
        else if (heal && recAp) desc.text = $"{data.description} Heal {data.healAmount} & Gain {data.aPRecoverAmount} action points";
        else if (draw && recAp) desc.text = $"{data.description} Draw {data.drawAmount} & Gain {data.aPRecoverAmount} action points";
        else if (dmg && debuff) desc.text = $"{data.description} Deal {data.damage} & {data.debuffType} for {data.debuffDuration}";
        else if (block && debuff) desc.text = $"{data.description} Block {data.blockAmount} & {data.debuffType} for {data.debuffDuration}";
        else if (heal && debuff) desc.text = $"{data.description} Heal {data.healAmount} & {data.debuffType} for {data.debuffDuration}";
        else if (draw && debuff) desc.text = $"{data.description} Draw {data.drawAmount} & {data.debuffType} for {data.debuffDuration}";
        else if (debuff && recAp) desc.text = $"{data.description} {data.debuffType} for {data.debuffDuration} turns & Gain {data.aPRecoverAmount} action points";
        else if (buff && recAp) desc.text = $"{data.description} Gain {data.buffType} for {data.buffDuration} turns & Gain {data.aPRecoverAmount} action points";
        else if (buff && debuff) desc.text = $"{data.description} Gain {data.buffType} for {data.buffDuration} turns & {data.debuffType} for {data.debuffDuration} turns";

        //Single checks
        else if (draw) desc.text = $"{data.description} Draw {data.drawAmount}";
        else if (dmg) desc.text = $"{data.description} Deal {data.damage} damage";
        else if (burnHeal) desc.text = $"{data.description} Heal {data.healAmount}";
        else if (heal) desc.text = $"{data.description} Heal {data.healAmount}";
        else if (block) desc.text = $"{data.description} Block {data.blockAmount}";
        else if (burnRecAp) desc.text = $"{data.description} Gain {data.aPRecoverAmount} action points";
        else if (recAp) desc.text = $"{data.description} Gain {data.aPRecoverAmount} action points";
        else if (buff) desc.text = $"{data.description} Gain {data.buffType} for {data.buffDuration} turns";
        else if (debuff) desc.text = $"{data.description} {data.debuffType} for {data.debuffDuration} turns";

        else desc.text = "";
    }
}