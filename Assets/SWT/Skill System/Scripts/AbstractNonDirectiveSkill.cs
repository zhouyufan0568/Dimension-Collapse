using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    /// <summary>
    /// The base class for making a new skill easier.What you need to do is just implementing CooldownTimeLeft, CooldownTimeTotal and CastCore.
    /// </summary>
    public abstract class AbstractNonDirectiveSkill : NondirectiveSkill
    {
        /// <summary>
        /// Cast skill towards the target.When there is cooldown time remaining, it will just return.
        /// </summary>
        public override void Cast()
        {
            if (!IsReady)
            {
                return;
            }

            CastCore();
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
        public abstract void CastCore();
    }
}
