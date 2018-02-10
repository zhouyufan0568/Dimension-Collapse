using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class Molotov : AbstractMissile
    {
        public GameObject framePrefab;
        public int frameCount;
        public float damagePerSecond = 20f;

        [ReadOnlyInInspector]
        public HashSet<PlayerManager> victims;

        public override void ExplodeCore()
        {
            Emit(frameCount);
        }


        public void Emit(int count)
        {
            victims = new HashSet<PlayerManager>();

            for (int i = 0; i < count; i++)
            {
                float angleR = 2 * Mathf.PI / count * i;
                GameObject frameObj = Instantiate(framePrefab, transform);
                frameObj.transform.rotation = Quaternion.identity;
                frameObj.transform.position = transform.position;
                frameObj.GetComponent<Rigidbody>().AddForce(new Vector3(Mathf.Cos(angleR), 0, Mathf.Sin(angleR)) * 12f, ForceMode.VelocityChange);
                frameObj.GetComponentInChildren<FrameFragment>().molotov = this;
            }
        }

        private void Update()
        {
            if (victims == null)
            {
                return;
            }

            foreach (var victim in victims)
            {
                if (victim != null)
                {
                    victim.OnAttacked(Time.deltaTime * damagePerSecond);
                }
            }
        }

        private void OnDestroy()
        {
            if (victims != null)
            {
                victims.Clear();
                victims = null;
            }
        }
    }
}
