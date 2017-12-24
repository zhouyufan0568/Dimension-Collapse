using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIManager : MonoBehaviour {
    private static SettingUIManager instance;
    public static SettingUIManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SettingUIManager>();
                if (instance == null)
                {
                    Debug.LogError("SettingUIManager is missing!");
                }
            }
            return instance;
        }
    }

    public Animator backButtonAnimator;
    public Animator contentAnimator;
    public GameObject[] fillPanels;
    public GameObject[] contentPanels;
    private Animator[] animators;
    private CanvasGroup[] canvasGroups;

    public int firstSelected = 0;
    private int curSelected = -1;

    private float lastClicked = -1f;
    private float clickInterval = 0.8f;

    private bool isOpen = false;

    private void Start()
    {
        animators = new Animator[contentPanels.Length];
        canvasGroups = new CanvasGroup[contentPanels.Length];

        for (int i = 0; i < contentPanels.Length; i++)
        {
            animators[i] = contentPanels[i].GetComponent<Animator>();
            canvasGroups[i] = contentPanels[i].GetComponent<CanvasGroup>();
        }

        if (firstSelected < 0 || firstSelected >= contentPanels.Length)
        {
            firstSelected = 0;
        }

        Close();
    }

    public void SetAsSelected(int index)
    {
        if (index < 0 || index >= contentPanels.Length || index == curSelected)
        {
            return;
        }

        if (curSelected != -1)
        {
            animators[curSelected].SetTrigger("Close");
            canvasGroups[curSelected].interactable = false;
            canvasGroups[curSelected].blocksRaycasts = false;
            fillPanels[curSelected].SetActive(false);
        }

        animators[index].SetTrigger("Open");
        canvasGroups[index].interactable = true;
        canvasGroups[index].blocksRaycasts = true;
        fillPanels[index].SetActive(true);

        curSelected = index;
    }

    private void Open()
    {
        if (!isOpen)
        {
            contentAnimator.SetTrigger("Open");
            backButtonAnimator.SetTrigger("Open");
        }

        isOpen = true;

        if (curSelected == -1 && firstSelected >= 0 && firstSelected < contentPanels.Length)
        {
            SetAsSelected(firstSelected);
        }
    }

    private void Close()
    {
        if (isOpen)
        {
            contentAnimator.SetTrigger("Close");
            backButtonAnimator.SetTrigger("Close");
            if (curSelected >= 0 && curSelected < animators.Length)
            {
                animators[curSelected].SetTrigger("Close");
            }
        }

        isOpen = false;

        for (int i = 0; i < contentPanels.Length; i++)
        {
            canvasGroups[i].interactable = false;
            canvasGroups[i].blocksRaycasts = false;
            canvasGroups[i].alpha = 0;
            fillPanels[i].SetActive(false);
        }

        curSelected = -1;
    }

    public void Reverse()
    {
        if (Time.time - lastClicked < clickInterval)
        {
            return;
        }

        if (isOpen)
        {
            Close();
        }
        else
        {
            Open();
        }

        lastClicked = Time.time;
    }
}
