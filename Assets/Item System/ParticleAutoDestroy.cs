using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class ParticleAutoDestroy : MonoBehaviour
    {
        private void Start()
        {
            Destroy(gameObject, GetComponent<ParticleSystem>().main.duration);
        }
    }
}
