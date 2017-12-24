namespace DimensionCollapse
{
    /// <summary>
    /// Base class of all skills in the game.
    /// </summary>
    public abstract class Skill : Item
    {
        /// <summary>
        /// How much time the player must wait for to cast this skill again.
        /// </summary>
        public abstract float CooldownTimeLeft { get; }

        /// <summary>
        /// The total cooldown time of this skill.
        /// </summary>
        public abstract float CooldownTimeTotal { get; }

        /// <summary>
        /// Whether the player can cast this skill now?
        /// </summary>
        public abstract bool IsReady { get; }
    }
}
