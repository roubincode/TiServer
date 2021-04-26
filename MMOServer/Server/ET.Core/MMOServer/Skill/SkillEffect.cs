// Base component for skill effects.
using UnityEngine;

namespace ETModel
{
    public abstract class SkillEffect : Sprite
    {
        public DamageType damageType;
        public Entity caster;

    }
}
