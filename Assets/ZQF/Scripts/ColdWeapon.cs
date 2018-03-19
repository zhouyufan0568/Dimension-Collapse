using UnityEngine;
using DimensionCollapse;

public class ColdWeapon : Weapon
{
    float Damage { get; set; }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override bool CanAttack()
    {
        throw new System.NotImplementedException();
    }

    public override void Attack(bool b){
        return;
    }
}
