using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public abstract class Weapon : Item
    {
		public abstract void Attack();  //武器攻击函数，由人物来调用进行攻击
    }
}