using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScaleWizard : EditorWindow {

    [MenuItem("BlackHole Tools/Scale Wizard")]
    static void Init()
    {
        GetWindow(typeof(ScaleWizard));
    }

    private float scaleFactor = 1f;

    private void OnGUI()
    {
        scaleFactor = EditorGUILayout.FloatField(scaleFactor);
        if (GUILayout.Button("Scale", GUILayout.ExpandWidth(true)))
        {
            GameObject parent = Selection.activeGameObject;
            if (parent == null)
            {
                return;
            }

            Collider collider = parent.GetComponent<Collider>();
            if (collider is BoxCollider)
            {
                BoxCollider boxCollider = (BoxCollider)collider;
                boxCollider.center *= scaleFactor;
                boxCollider.size *= scaleFactor;
            }
            else if (collider is CapsuleCollider)
            {
                CapsuleCollider capsuleCollider = (CapsuleCollider)collider;
                capsuleCollider.center *= scaleFactor;
                capsuleCollider.radius *= scaleFactor;
                capsuleCollider.height *= scaleFactor;
            }

            GameObject child = parent.transform.GetChild(0).gameObject;
            child.transform.localScale *= scaleFactor;
            child.transform.localPosition *= scaleFactor;
        }
    }
}
