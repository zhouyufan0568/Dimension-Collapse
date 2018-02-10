using UnityEngine;

namespace DimensionCollapse
{
    [RequireComponent(typeof(CharacterController))]
    public class ImpactReceiver : MonoBehaviour
    {
        [Tooltip("The mass of the object attached to.")]
        public float mass = 1f;
        [Tooltip("Force whose magnitude under it will be ignored.")]
        public float threshold = 0.2f;
        [Tooltip("The speed of the attenuation of the kinetic energy")]
        public float attenuation = 5f;

        private Vector3 impact;
        private CharacterController controller;

        public void AddImpact(Vector3 force)
        {
            impact += (force / mass);
        }

        public void AddImpact(Vector3 direction, float magnitude)
        {
            if (Mathf.Approximately(magnitude, 0f))
            {
                return;
            }

            direction.Normalize();
            //if (direction.y < 0)
            //{
            //    direction.y *= -1;
            //}

            impact += direction * (magnitude / mass);
        }

        private void Start()
        {
            impact = Vector3.zero;
            controller = GetComponent<CharacterController>();
        }

        private void Update()
        {
            if (impact.magnitude < threshold)
            {
                impact = Vector3.zero;
            }
            else
            {
                controller.Move(impact * Time.deltaTime);
                impact = Vector3.Lerp(impact, Vector3.zero, attenuation * Time.deltaTime);
            }
        }
    }
}
