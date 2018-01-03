using UnityEngine;
using DimensionCollapse;
namespace DimensionCollapse
{
    public class SkillItem : Item {

        public float coldTime;
        private float coldCount = 0;
        public bool isColdDown = true;
        //here is the icon of the skill
        //    //
        public string skillDescription;

        private void Update()
        {
            if (!isColdDown&&coldCount<coldTime)
            {
                coldCount += Time.deltaTime;
            }
            if (coldCount >= coldTime)
            {
                coldCount = 0;
                isColdDown = true;
            }
        }

        public void useSkill()
        {
            if (isColdDown)
            {
                skillEffect();
            }
        }
        public virtual void skillEffect()
        {

        }
    }
}

