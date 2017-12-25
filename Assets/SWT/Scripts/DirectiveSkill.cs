using UnityEngine;

namespace DimensionCollapse
{
    /// <summary>
    /// Base class of all directive skills in the game.
    /// </summary>
    public abstract class DirectiveSkill : Skill
    {
        /// <summary>
        /// Cast skill towards the target.User of the skill class should invoke this method directly.
        /// </summary>
        /// <param name="target">The target of this cast.</param>
        public abstract void Cast(GameObject target);
    }
}