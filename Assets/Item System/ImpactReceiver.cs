using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    [RequireComponent(typeof(CharacterController))]
    public class ImpactReceiver : MonoBehaviour
    {
        public float mass = 1f;
        public float threshold = 0.2f;
        public float attenuaion = 5f;

        private Vector3 impact;
        private CharacterController controller;

        public void AddImpact(Vector3 direction, float force)
        {
            if (Mathf.Approximately(force, 0f))
            {
                return;
            }

            direction.Normalize();
            if (direction.y < 0)
            {
                direction.y *= -1;
            }

            impact += direction * (force / mass);
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
                impact = Vector3.Lerp(impact, Vector3.zero, attenuaion * Time.deltaTime);
            }
        }
    }
}
