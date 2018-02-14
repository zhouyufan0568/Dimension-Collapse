using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class ParticleAutoFadeOut : MonoBehaviour
    {
        private static int tintColorID;
        static ParticleAutoFadeOut()
        {
            tintColorID = Shader.PropertyToID("_TintColor");
        }

        public float fadeOutDuration = 0.5f;
        private bool started;

        private List<Material> mats;

        private void Awake()
        {
            mats = new List<Material>();
            foreach (var renderer in GetComponentsInChildren<ParticleSystemRenderer>())
            {
                if (renderer.material.HasProperty(tintColorID))
                {
                    Material mat = new Material(renderer.material);
                    renderer.material = mat;
                    mats.Add(mat);
                }
            }
        }

        private void OnEnable()
        {
            started = false;

            foreach (var mat in mats)
            {
                Color color = mat.GetColor(tintColorID);
                color.a = 1f;
                mat.SetColor(tintColorID, color);
            }
        }

        public void FadeOut()
        {
            if (!started)
            {
                started = true;
                StartCoroutine(FadeOutCoroutine());
            }
        }

        private IEnumerator FadeOutCoroutine()
        {
            float lifetime = 0f;
            while (lifetime <= fadeOutDuration)
            {
                foreach (var mat in mats)
                {
                    Color color = mat.GetColor(tintColorID);
                    color.a = Mathf.Lerp(1f, 0f, lifetime / fadeOutDuration);
                    mat.SetColor(tintColorID, color);
                }
                lifetime += Time.deltaTime;
                yield return null;
            }
            gameObject.SetActive(false);
        }
    }
}
