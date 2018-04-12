using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimensionCollapse
{
    public class ScreenUIManager : MonoBehaviour
    {
        private ScreenUI currentScreen;
        private ScreenUI lastScreen;
        public ScreenUI firstScreen;
        public CanvasGroup fadePanel;

        private void Start()
        {
            foreach (ScreenUI screen in GetComponentsInChildren<ScreenUI>())
            {
                if (screen != firstScreen)
                {
                    CanvasGroup otherCanvasGroup = screen.GetComponent<CanvasGroup>();
                    otherCanvasGroup.alpha = 0f;
                    otherCanvasGroup.interactable = false;
                    otherCanvasGroup.blocksRaycasts = false;
                }
            }
            CanvasGroup canvasGroup = firstScreen.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            currentScreen = firstScreen;
        }

        public void SwitchTo(ScreenUI nextScreen)
        {
            if (nextScreen == currentScreen)
            {
                return;
            }

            currentScreen.Out();
            nextScreen.In();
            lastScreen = currentScreen;
            currentScreen = nextScreen;
        }

        public void SwitchBack()
        {
            SwitchTo(lastScreen);
        }
    }
}
