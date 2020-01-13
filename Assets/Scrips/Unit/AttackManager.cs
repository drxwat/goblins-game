using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager
{
    float baseHitChance;

    System.Random random = new System.Random();

    public AttackManager(float _baseHitChance = 0.8f)
    {
        baseHitChance = _baseHitChance;
    }

    public void PerformAttack(Unit attacker, Unit defender, Vector3 attackDirection)
    {
        int atkVal = attacker.getStats().attack.value;
        int defVal = defender.getStats().deffence.value;

        bool isHits = IsAttackHits(atkVal, defVal);

        attacker.Attack(attackDirection);
        if (isHits)
        {
            int damage = GetBinomDamage(attacker.getStats().damage.value);
            Debug.Log("Hits enemy with " + damage + " damage");
            defender.BeAttacked(attackDirection);
            defender.TakeDamage(damage);
        } else
        {
            Debug.Log("Misses target");
        }
    }

    bool IsAttackHits(int attackValue, int defenceValue)
    {
        // https://www.desmos.com/calculator/auubsajefh 
        int diff = Mathf.Abs(attackValue - defenceValue); 

        float HCDiff = (Mathf.Log(diff, 2) + 5f) * 1.4f; // limit 20
        HCDiff = HCDiff / 100;
        HCDiff = Mathf.Clamp(HCDiff, 0, 0.95f - baseHitChance);

        float HC = attackValue > defenceValue ? baseHitChance + HCDiff : baseHitChance - HCDiff;

        double randomDrop = random.NextDouble();
        Debug.Log("Atk Dice " + (float)randomDrop + "Hit Chance " + HC);

        return HC > randomDrop;
    }

    int GetBinomDamage(int baseDamage)
    {
        int damage = baseDamage;
        double randomDrop = random.NextDouble();

        float[] distrib = new float[5] { 0.0625f, 0.25f, 0.375f, 0.25f, 0.0625f };
        int damageMin = -2;
        float probThreshold = 0f;

        for(int i = 0; i < distrib.Length; i++) {
            probThreshold += distrib[i];
            if (randomDrop < probThreshold)
            {
                damage = damage + damageMin + i;
                break;
            }
        }
        return Mathf.Max(1, damage);
    }
}
