using UnityEditor;
using UnityEngine;

public class CombineMeshWizard : EditorWindow
{
    [MenuItem("BlackHole Tools/CombineMeshWizard")]
    static void CreateWizard()
    {
        GetWindow(typeof(CombineMeshWizard));
    }

    private Material material;

    private void OnGUI()
    {
        EditorGUILayout.HelpBox("Please make all objects to be combined children of an object and select this one only.", MessageType.Info);
        material = (Material)EditorGUILayout.ObjectField("Material", material, typeof(Material), true);
        if (GUILayout.Button("Combine", GUILayout.ExpandWidth(true)))
        {
            OnCombine();
        }
    }

    private void OnCombine()
    {
        GameObject selected = Selection.activeGameObject;
        if (selected == null)
        {
            return;
        }
        MeshFilter[] meshFilters = selected.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        for (int i = 0; i < meshFilters.Length; i++)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
        }

        MeshFilter resFilter = selected.GetComponent<MeshFilter>();
        if (resFilter == null)
        {
            resFilter = selected.AddComponent<MeshFilter>();
        }
        Mesh mesh = new Mesh();
        mesh.name = selected.name;
        mesh.CombineMeshes(combine, true);
        resFilter.sharedMesh = mesh;

        MeshRenderer resRenderer = selected.GetComponent<MeshRenderer>();
        if (resRenderer == null)
        {
            resRenderer = selected.AddComponent<MeshRenderer>();
        }
        resRenderer.sharedMaterial = material;
        selected.SetActive(true);

        MeshSaverEditor.SaveMesh(mesh, mesh.name, true, true);
    }
}
