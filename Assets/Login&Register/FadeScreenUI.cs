using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class FadeScreenUI : ScreenUI
    {
        public float duration = 1f;
        private CanvasGroup canvasGroup;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

        public override void In()
        {
            StartCoroutine(CanvasGroupInCoroutine(duration));
        }

        public override void Out()
        {
            StartCoroutine(CanvasGroupOutCoroutine(duration));
        }

        private IEnumerator CanvasGroupInCoroutine(float duration)
        {
            if (canvasGroup.alpha >= 1f)
            {
                yield break;
            }

            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            float deltaPerSecond = (1 - canvasGroup.alpha) / duration;
            while (canvasGroup.alpha < 1f)
            {
                canvasGroup.alpha += deltaPerSecond * Time.deltaTime;
                yield return null;
            }
        }

        private IEnumerator CanvasGroupOutCoroutine(float duration)
        {
            if (canvasGroup.alpha <= 0f)
            {
                yield break;
            }

            float deltaPerSecond = (0 - canvasGroup.alpha) / duration;
            while (canvasGroup.alpha > 0f)
            {
                canvasGroup.alpha += deltaPerSecond * Time.deltaTime;
                yield return null;
            }
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }
}
