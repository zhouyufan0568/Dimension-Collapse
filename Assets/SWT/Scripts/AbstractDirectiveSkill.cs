using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    /// <summary>
    /// The base class for making a new skill easier.What you need to do is just implementing CooldownTimeLeft, CooldownTimeTotal and CastCore.
    /// </summary>
    public abstract class AbstractDirectiveSkill : DirectiveSkill
    {
        /// <summary>
        /// Cast skill towards the target.When there is cooldown time remaining, it will just return.
        /// </summary>
        /// <param name="target">The target of this cast.</param>
        public override void Cast(GameObject target)
        {
            if (!IsReady)
            {
                return;
            }

            CastCore(target);
        }

        /// <summary>
        /// If CooldownTimeLeft == 0, we can know that player can cast this skill now.
        /// </summary>
        public override bool IsReady
        {
            get
            {
                return Mathf.Approximately(CooldownTimeLeft, 0f) ? true : false;
            }
        }

        /// <summary>
        /// The core of the skill which determines the concrete effect. 
        /// </summary>
        /// <param name="target">The target of this cast.</param>
        public abstract void CastCore(GameObject target);
    }
}