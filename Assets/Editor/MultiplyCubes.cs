using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class MultiplyCubes : EditorWindow {

    private enum ShapeOption
    {
        STAIRS,
        MASONRY,
        DOOR
    }

    private Vector2 scrollPos = Vector2.zero;

    private bool showTranslate = false;

    private bool showClone = true;
    private float multiplyBy = 1f;
    private int offsetX = 1;
    private int offsetY = 0;
    private int offsetZ = 1;
    private bool needTofollow = true;
    private bool addIntoParent = true;
    //private bool autoBind = true;

    private bool showReplace = false;
    private Material materialForReplace;

    private ShapeOption shapeOption = ShapeOption.STAIRS;
    private bool showShape = false;
    private GameObject prefab;

    private int stepNumber = 5;
    private int stepWidth = 5;
    private int stepDepth = 1;
    private int stepHeight = 1;

    private int masonryWidth = 5;
    private int masonryDepth = 2;
    private int masonryHeight = 10;

    private int doorTotalWidth = 3;
    private int doorTotalHeight = 2;
    private int doorDepth = 1;
    private int doorHeight = 1;
    private int doorLegWidth = 1;

    private bool showBind = false;
    private float breakForce = Mathf.Infinity;
    private bool bindX = false;
    private bool bindY = false;
    private bool bindZ = false;

    private bool showHollow = false;

    private bool showAddAndRemove = false;
    private Rigidbody rigidBodyTemplate;

    private bool showMerge = false;

    private bool showTools = false;

    private bool showWriteIntoFiles;
    public int chunkSize = 10;

    private bool showReadFromFiles;
    public GameObject cubePrefab;

	private bool createNoiseChunk;
	private int numOfChunkX=30;
	private int numOfChunkZ=30;
	private int sizeOfChunk=100;
	private int indexOfChunkX;
	private int indexOfChunkZ;

    [MenuItem("BlackHole Tools/MultiplyCubes")]
    static void Init()
    {
        GetWindow(typeof(MultiplyCubes));
    }

    private void OnGUI()
    {
        Transform[] selected = Selection.transforms;
        Object[] created = null;
        int undoGroupId = Undo.GetCurrentGroup();

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        EditorGUILayout.HelpBox("coordinate system:\n → z\n↓\nx", MessageType.Info);

        showTranslate = EditorGUILayout.Foldout(showTranslate, "Translate");
        if (showTranslate)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Up", GUILayout.ExpandWidth(true)))
            {
                TranslateSelected(Vector3.up);
            }

            if (GUILayout.Button("Down", GUILayout.ExpandWidth(true))){
                TranslateSelected(Vector3.down);
            }
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Left", GUILayout.ExpandWidth(true)))
            {
                TranslateSelected(Vector3.left);
            }

            if (GUILayout.Button("Right", GUILayout.ExpandWidth(true)))
            {
                TranslateSelected(Vector3.right);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Forward", GUILayout.ExpandWidth(true)))
            {
                TranslateSelected(Vector3.forward);
            }

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(true)))
            {
                TranslateSelected(Vector3.back);
            }
            EditorGUILayout.EndHorizontal();
        }

        showClone = EditorGUILayout.Foldout(showClone, "Clone");
        if (showClone)
        {
            needTofollow = GUILayout.Toggle(needTofollow, "Follow The Newest");

            addIntoParent = GUILayout.Toggle(addIntoParent, "Add Into Parent");

            multiplyBy = EditorGUILayout.FloatField("Multiply By", multiplyBy);
            if (multiplyBy <= 0)
            {
                EditorGUILayout.HelpBox("Usage of a non-positive multiply factor " +
                    "is not recommended.", MessageType.Warning);
            }

            offsetX = EditorGUILayout.IntSlider("Offset in X", offsetX, 0, 10);
            offsetY = EditorGUILayout.IntSlider("Offset in Y", offsetY, -10, 10);
            offsetZ = EditorGUILayout.IntSlider("Offset in Z", offsetZ, 0, 10);
            if(offsetX == 0 || offsetZ == 0)
            {
                EditorGUILayout.HelpBox("Usage of a zero offsetX or a zero " +
                    "offsetZ is not recommended.", MessageType.Warning);
            }
            float actualOffsetX = offsetX * multiplyBy;
            float actualOffsetY = offsetY * multiplyBy;
            float actualOffsetZ = offsetZ * multiplyBy;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("↖", GUILayout.ExpandWidth(true)))
            {
                created = CloneCubes(selected, -actualOffsetX, actualOffsetY, -actualOffsetZ);
            }
            if (GUILayout.Button("↑", GUILayout.ExpandWidth(true)))
            {
                created = CloneCubes(selected, -actualOffsetX, actualOffsetY, 0);
            }
            if (GUILayout.Button("↗", GUILayout.ExpandWidth(true)))
            {
                created = CloneCubes(selected, -actualOffsetX, actualOffsetY, actualOffsetZ);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("←", GUILayout.ExpandWidth(true)))
            {
                created = CloneCubes(selected, 0, actualOffsetY, -actualOffsetZ);
            }
            if (GUILayout.Button("y axis", GUILayout.ExpandWidth(true)))
            {
                if (actualOffsetY != 0)
                {
                    created = CloneCubes(selected, 0, actualOffsetY, 0);
                }
            }
            if (GUILayout.Button("→", GUILayout.ExpandWidth(true)))
            {
                created = CloneCubes(selected, 0, actualOffsetY, actualOffsetZ);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("↙", GUILayout.ExpandWidth(true)))
            {
                created = CloneCubes(selected, actualOffsetX, actualOffsetY, -actualOffsetZ);
            }
            if (GUILayout.Button("↓", GUILayout.ExpandWidth(true)))
            {
                created = CloneCubes(selected, actualOffsetX, actualOffsetY, 0);
            }
            if (GUILayout.Button("↘", GUILayout.ExpandWidth(true)))
            {
                created = CloneCubes(selected, actualOffsetX, actualOffsetY, actualOffsetZ);
            }
            GUILayout.EndHorizontal();

            if (created != null && needTofollow)
            {
                Selection.objects = created;
                Undo.CollapseUndoOperations(undoGroupId);
            }
        }

        showReplace = EditorGUILayout.Foldout(showReplace, "Replace");
        if (showReplace)
        {
            materialForReplace = (Material)EditorGUILayout.ObjectField(
                "Material", materialForReplace, typeof(Material), true);
            if (GUILayout.Button("Replace") && materialForReplace)
            {
                ReplaceMaterialOfSelected();
            }
        }

        showShape = EditorGUILayout.Foldout(showShape, "Shape");
        if (showShape)
        {
            //That true above tells the editor property to allow also scene objects. 
            //Set to false if you only want assets to be allowed. 
            prefab = (GameObject)EditorGUILayout.ObjectField(
                "Prefab", prefab, typeof(GameObject), true);
            shapeOption = (ShapeOption)EditorGUILayout.EnumPopup("Shape to Create", shapeOption);
            switch (shapeOption)
            {
                case ShapeOption.STAIRS:
                    stepNumber = EditorGUILayout.IntSlider("Step Number", stepNumber, 1, 100);
                    stepWidth = EditorGUILayout.IntSlider("Step Width", stepWidth, 1, 50);
                    stepDepth = EditorGUILayout.IntSlider("Step Depth", stepDepth, 1, 50);
                    stepHeight = EditorGUILayout.IntSlider("Step Height", stepHeight, 1, 50);
                    if (GUILayout.Button("Create Stairs", GUILayout.ExpandWidth(true)) && prefab)
                    {
                        GameObject stairs = CreateStairs(
                            prefab, stepWidth, stepDepth, stepHeight, stepNumber);
                        Undo.RegisterCreatedObjectUndo(stairs, "Create Stairs");
                    }
                    break;
                case ShapeOption.MASONRY:
                    masonryWidth = EditorGUILayout.IntSlider("Masonry Width", masonryWidth, 1, 500);
                    masonryDepth = EditorGUILayout.IntSlider("Masonry Depth", masonryDepth, 1, 500);
                    masonryHeight = EditorGUILayout.IntSlider("Masonry Height", masonryHeight, 1, 100);
                    if (GUILayout.Button("Create Masonry Structure") && prefab)
                    {
                        GameObject masonryStructure = CreateMasonryStructure(
                            prefab, masonryWidth, masonryDepth, masonryHeight);
                        Undo.RegisterCreatedObjectUndo(masonryStructure, "Create Masonry Structure");
                    }
                    break;
                case ShapeOption.DOOR:
                    doorTotalWidth = EditorGUILayout.IntSlider("Door Total Width", doorTotalWidth, 3, 100);
                    doorTotalHeight = EditorGUILayout.IntSlider("Door Total Height", doorTotalHeight, 2, 100);
                    doorDepth = EditorGUILayout.IntSlider("Door Depth", doorDepth, 1, 100);
                    doorHeight = EditorGUILayout.IntSlider("Door Height", doorHeight, 1, 100);
                    doorLegWidth = EditorGUILayout.IntSlider("Door Leg Width", doorLegWidth, 1, 100);
                    if (GUILayout.Button("Create Door") && prefab)
                    {
                        GameObject door = CreateDoor(prefab, doorTotalWidth, doorTotalHeight, doorDepth, doorHeight, doorLegWidth);
                        Undo.RegisterCreatedObjectUndo(door, "Create Door");
                    }
                    break;
            }
        }

        showBind = EditorGUILayout.Foldout(showBind, "Bind");
        if (showBind)
        {
            breakForce = EditorGUILayout.FloatField(
                new GUIContent("Break Force", "Maximun force the joint can withstand before breaking.[0.001, infinity]"), breakForce);

            bindX = EditorGUILayout.Toggle(new GUIContent("Bind X", "Make cubes connected to each other along X."), bindX);
            bindY = EditorGUILayout.Toggle(new GUIContent("Bind Y", "Make cubes connected to each other along Y."), bindY);
            bindZ = EditorGUILayout.Toggle(new GUIContent("Bind Z", "Make cubes connected to each other along Z."), bindZ);

            if (GUILayout.Button(new GUIContent("Bind", "Make cubes connected to each other along the selected axis."), GUILayout.ExpandWidth(true)))
            {
                BindSelected(bindX, bindY, bindZ);
                Undo.CollapseUndoOperations(undoGroupId);
            }

            if (GUILayout.Button(new GUIContent("Bind All Axis", "Make cubes connected to each other along all axes."), GUILayout.ExpandWidth(true)))
            {
                BindSelected(true, true, true);
                Undo.CollapseUndoOperations(undoGroupId);
            }

            if (GUILayout.Button(new GUIContent("Unbind All Axis", "Make cubes disconnected to each other along all axes."), GUILayout.ExpandWidth(true)))
            {
                UnbindSelected();
                Undo.CollapseUndoOperations(undoGroupId);
            }
        }

        showHollow = EditorGUILayout.Foldout(showHollow, "Hollow");
        if (showHollow)
        {
            if (GUILayout.Button("Hollow", GUILayout.ExpandWidth(true))){
                HollowSelected();
            }
        }

        showAddAndRemove = EditorGUILayout.Foldout(showAddAndRemove, "Add&Remove");
        if (showAddAndRemove)
        {
            rigidBodyTemplate = (Rigidbody)EditorGUILayout.ObjectField(
                "Template", rigidBodyTemplate, typeof(Rigidbody), true);
            if (GUILayout.Button("Copy Rigidbody", GUILayout.ExpandWidth(true)) && rigidBodyTemplate)
            {
                AddRigidbodyToSelected();
            }
            if (GUILayout.Button("Remove Rigidbody", GUILayout.ExpandWidth(true)))
            {
                RemoveRigidbodyFromSelected();
            }
        }

        showMerge = EditorGUILayout.Foldout(showMerge, "Merge");
        if (showMerge)
        {
            if (GUILayout.Button("Merge Selected", GUILayout.ExpandWidth(true)))
            {
                MergeSelected();
            }
        }

        showTools = EditorGUILayout.Foldout(showTools, "Tools");
        if (showTools)
        {
            if (GUILayout.Button("Show GameObject Count", GUILayout.ExpandWidth(true))){
                ShowSelectedChildCount();
            }

            if (GUILayout.Button("Wrap Selected", GUILayout.ExpandWidth(true)))
            {
                WrapSelected();
            }

            if (GUILayout.Button("Move To Origin", GUILayout.ExpandWidth(true)))
            {
                MoveSelectedToOrigin();
            }

            //if (GUILayout.Button("Remove Rotations", GUILayout.ExpandWidth(true)))
            //{
            //    RemoveRotationOfSelected();
            //}
        }

        showWriteIntoFiles = EditorGUILayout.Foldout(showWriteIntoFiles, "Write Into Files");
        if (showWriteIntoFiles)
        {
            if (GUILayout.Button("Start", GUILayout.ExpandWidth(true)))
            {
                MapToFiles();
            }
        }

        showReadFromFiles = EditorGUILayout.Foldout(showReadFromFiles, "Read From Files");
        if (showReadFromFiles)
        {
            cubePrefab = (GameObject)EditorGUILayout.ObjectField(
               "Prefab", cubePrefab, typeof(GameObject), true);
            if (GUILayout.Button("Start", GUILayout.ExpandWidth(true)))
            {
                if (cubePrefab != null)
                {
                    FilesToMap();
                }
            }
        }

		createNoiseChunk = EditorGUILayout.Foldout(createNoiseChunk, "Create Noise Chunk");
		if (createNoiseChunk)
		{
			numOfChunkX = EditorGUILayout.IntField("Num of Chunk X", numOfChunkX);
			numOfChunkZ = EditorGUILayout.IntField("Num of Chunk Z", numOfChunkZ);
			sizeOfChunk = EditorGUILayout.IntField("Size Of Chunk", sizeOfChunk);
			indexOfChunkX = EditorGUILayout.IntField("Index Of Chunk X", indexOfChunkX);
			indexOfChunkZ = EditorGUILayout.IntField("Index Of Chunk Z", indexOfChunkZ);

			if (GUILayout.Button("Start", GUILayout.ExpandWidth(true)))
			{
				CreateNoiseChunk ();
			}
		}

        EditorGUILayout.EndScrollView();
    }

    private GameObject[] CloneCubes(Transform[] selected, float offsetX, float offsetY, float offsetZ)
    {
        GameObject[] created = new GameObject[selected.Length];
    
        for (int i = 0; i < selected.Length; i++)
        {
            Transform selectedOne = selected[i];
            GameObject newCube =
                Instantiate(
                    selectedOne.gameObject,
                    selectedOne.transform.position + new Vector3(offsetX, offsetY, offsetZ),
                    selectedOne.transform.rotation);
            newCube.name = selectedOne.name;
            created[i] = newCube;
            if (addIntoParent)
            {
                newCube.transform.parent = selectedOne.transform.parent;
            }
            Undo.RegisterCreatedObjectUndo(newCube, "new Cube");
        }
        return created;
    }

    private GameObject CreateStairs(GameObject prefab, int width, int depth, int height, int layerNumber)
    {
        GameObject stairs = new GameObject();
        for (int curStep = 0; curStep < layerNumber; curStep++)
        {
            for (int curLayer = 0; curLayer <= curStep; curLayer++)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < depth; j++)
                    {
                        for (int k = 0; k < height; k++)
                        {
                            GameObject oneCube =
                                Instantiate(prefab,
                                new Vector3(i, k + curLayer * height, j + curStep * depth),
                                Quaternion.identity);
                            oneCube.transform.parent = stairs.transform;
                        }
                    }
                }
            }
        }

        stairs.name = string.Format("Stair({0}×{1}×{2}×{3})", width, depth, height, layerNumber);
        return stairs;
    }

    private GameObject CreateMasonryStructure(GameObject prefab, int width, int depth, int height)
    {
        GameObject masonryStructure = new GameObject();
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < depth; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    GameObject oneCube = Instantiate(prefab,
                        new Vector3(i, k, j), Quaternion.identity);
                    oneCube.transform.parent = masonryStructure.transform;
                }
            }
        }

        masonryStructure.name = string.Format("MasonryStructure({0}×{1}×{2})", width, depth, height);
        return masonryStructure;
    }

    private GameObject CreateDoor(GameObject prefab, int totalWidth, int totalHeight,
        int depth, int doorHeight, int legWidth)
    {
        GameObject door = new GameObject();
        for (int i = 0; i < depth; i++)
        {
            for (int j = 0; j < totalWidth; j++)
            {
                for (int k = 0; k < totalHeight; k++)
                {
                    if (!(j >= legWidth && j < totalWidth - legWidth && k < totalHeight - doorHeight))
                    {
                        GameObject oneCube = Instantiate(prefab,
                            new Vector3(i, k, j), Quaternion.identity);
                        oneCube.transform.parent = door.transform;
                    }
                }
            }
        }

        door.name = "Door";
        return door;
    }

    private void BindStructure(GameObject structure, bool bindX, bool bindY, bool bindZ)
    {
        GameObject[] childs = new GameObject[structure.transform.childCount];
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i] = structure.transform.GetChild(i).gameObject;
        }
        BindStructure(childs, bindX, bindY, bindZ);
    }

    private void BindStructure(GameObject[] structure, bool bindX, bool bindY, bool bindZ)
    {
        //step 1: if no diminsion need to be binded,just return.
        if (!bindX && !bindY && !bindZ)
        {
            return;
        }

        //step 2: move the origin to (0, 0, 0) temporarily.
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        for (int i = 0; i < structure.Length; i++)
        {
            if (structure[i] == null)
            {
                continue;
            }

            Vector3 position = structure[i].transform.position;
            if (position.x < minX)
            {
                minX = position.x;
            } else if (position.x > maxX)
            {
                maxX = position.x;
            }
            if (position.y < minY)
            {
                minY = position.y;
            } else if (position.y > maxY)
            {
                maxY = position.y;
            }
            if (position.z < minZ)
            {
                minZ = position.z;
            } else if (position.z > maxZ)
            {
                maxZ = position.z;
            }
        }
        Vector3 origin = new Vector3(minX, minY, minZ);
        for (int i = 0; i < structure.Length; i++)
        {
            if (structure[i] == null)
            {
                continue;
            }

            structure[i].transform.position -= origin;
        }

        //step 3: create a three-dimensional array to place all the cubes.
        int lengthX = (int)(maxX - minX) + 1;
        int lengthY = (int)(maxY - minY) + 1;
        int lengthZ = (int)(maxZ - minZ) + 1;
        if (lengthX <= 0 || lengthY <= 0 || lengthZ <= 0)
        {
            return;
        }
        Rigidbody[,,] cubeSpace = new Rigidbody[lengthX, lengthY, lengthZ];
        for (int i = 0; i < structure.Length; i++)
        {
            if (structure[i] == null)
            {
                continue;
            }

            GameObject oneCube = structure[i];
            Rigidbody rigidbody = oneCube.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = oneCube.AddComponent<Rigidbody>();
            }
            Vector3 position = oneCube.transform.position;
            int x = (int)position.x;
            int y = (int)position.y;
            int z = (int)position.z;
            if (cubeSpace[x, y, z] == null)
            {
                cubeSpace[x, y, z] = rigidbody;
            }
        }

        //step 4: create fixed joints.
        for (int i = 0; i < lengthX; i++)
        {
            for (int j = 0; j < lengthY; j++)
            {
                for (int k = 0; k < lengthZ; k++)
                {
                    if (cubeSpace[i, j, k] == null)
                    {
                        continue;
                    }

                    Rigidbody rigidbody = cubeSpace[i, j, k];
                    GameObject curCube = rigidbody.gameObject;
                    if (bindX)
                    {
                        if (i - 1 >= 0 && cubeSpace[i - 1, j, k] != null)
                        {
                            FixedJoint fixedJoint = curCube.AddComponent<FixedJoint>();
                            fixedJoint.connectedBody = cubeSpace[i - 1, j, k];
                            Undo.RegisterCreatedObjectUndo(fixedJoint, "new Fixed Joint");
                        }
                        if (i + 1 < lengthX && cubeSpace[i + 1, j, k] != null)
                        {
                            FixedJoint fixedJoint = curCube.AddComponent<FixedJoint>();
                            fixedJoint.connectedBody = cubeSpace[i + 1, j, k];
                            Undo.RegisterCreatedObjectUndo(fixedJoint, "new Fixed Joint");
                        }
                    }
                    if (bindY)
                    {
                        if (j - 1 >= 0 && cubeSpace[i, j - 1, k] != null)
                        {
                            FixedJoint fixedJoint = curCube.AddComponent<FixedJoint>();
                            fixedJoint.connectedBody = cubeSpace[i, j - 1, k];
                            Undo.RegisterCreatedObjectUndo(fixedJoint, "new Fixed Joint");
                        }
                        if (j + 1 < lengthY && cubeSpace[i, j + 1, k] != null)
                        {
                            FixedJoint fixedJoint = curCube.AddComponent<FixedJoint>();
                            fixedJoint.connectedBody = cubeSpace[i, j + 1, k];
                            Undo.RegisterCreatedObjectUndo(fixedJoint, "new Fixed Joint");
                        }
                    }
                    if (bindZ)
                    {
                        if (k - 1 >= 0 && cubeSpace[i, j, k - 1] != null)
                        {
                            FixedJoint fixedJoint = curCube.AddComponent<FixedJoint>();
                            fixedJoint.connectedBody = cubeSpace[i, j, k - 1];
                            Undo.RegisterCreatedObjectUndo(fixedJoint, "new Fixed Joint");
                        }
                        if (k + 1 < lengthZ && cubeSpace[i, j, k + 1] != null)
                        {
                            FixedJoint fixedJoint = curCube.AddComponent<FixedJoint>();
                            fixedJoint.connectedBody = cubeSpace[i, j, k + 1];
                            Undo.RegisterCreatedObjectUndo(fixedJoint, "new Fixed Joint");
                        }
                    }
                }
            }
        }

        //step 5: move the origin back.
        for (int i = 0; i < structure.Length; i++)
        {
            if (structure[i] == null)
            {
                continue;
            }

            structure[i].transform.position += origin;
        }
    }

    private void BindSelected(bool bindX, bool bindY, bool bindZ)
    {
        if (Selection.transforms.Length == 1)
        {
            BindStructure(Selection.activeGameObject, bindX, bindY, bindZ);
        }
        else if (Selection.transforms.Length > 1)
        {
            GameObject[] cubes = new GameObject[Selection.transforms.Length];
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                cubes[i] = Selection.transforms[i].gameObject;
            }
            BindStructure(cubes, bindX, bindY, bindZ);
        }
    }

    private void UnbindStructure(GameObject structure)
    {
        GameObject[] childs = new GameObject[structure.transform.childCount];
        for (int i = 0; i < childs.Length; i++)
        {
            childs[i] = structure.transform.GetChild(i).gameObject;
        }
        UnbindStructure(childs);
    }

    private void UnbindStructure(GameObject[] structure)
    {
        for (int i = 0; i < structure.Length; i++)
        {
            if (structure[i] == null)
            {
                continue;
            }

            FixedJoint[] fixedJoints = structure[i].GetComponents<FixedJoint>();
            for (int j = 0; j < fixedJoints.Length; j++)
            {
                Undo.DestroyObjectImmediate(fixedJoints[j]);
            }
        }
    }

    private void UnbindSelected()
    {
        if (Selection.transforms.Length == 1)
        {
            UnbindStructure(Selection.activeGameObject);
        }
        else if (Selection.transforms.Length > 1)
        {
            GameObject[] cubes = new GameObject[Selection.transforms.Length];
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                cubes[i] = Selection.transforms[i].gameObject;
            }
            UnbindStructure(cubes);
        }
    }

    private void HollowStructure(LinkedList<Transform> structure)
    {
        Vector3 origin;
        Transform[] cubes;
        Transform[,,] cubeSpace = CreateCubeSpace(structure, out origin, out cubes);

        int lengthX = cubeSpace.GetLength(0);
        int lengthY = cubeSpace.GetLength(1);
        int lengthZ = cubeSpace.GetLength(2);

        bool[,,] needToRemoves = new bool[lengthX, lengthY, lengthZ];
        for (int i = 0; i < lengthX; i++)
        {
            for (int j = 0; j < lengthY; j++)
            {
                for (int k = 0; k < lengthZ; k++)
                {
                    if (cubeSpace[i, j, k] == null)
                    {
                        continue;
                    }

                    bool needToRemove = true;

                    if (i == 0 || i == lengthX - 1 || cubeSpace[i - 1, j, k] == null || cubeSpace[i + 1, j, k] == null)
                    {
                        needToRemove = false;
                    }

                    if (j == 0 || j == lengthY - 1 || cubeSpace[i, j - 1, k] == null || cubeSpace[i, j + 1, k] == null)
                    {
                        needToRemove = false;
                    }

                    if (k == 0 || k == lengthZ - 1 || cubeSpace[i, j, k - 1] == null || cubeSpace[i, j, k + 1] == null)
                    {
                        needToRemove = false;
                    }

                    needToRemoves[i, j, k] = needToRemove;
                }
            }
        }
        for (int i = 0; i < lengthX; i++)
        {
            for (int j = 0; j < lengthY; j++)
            {
                for (int k = 0; k < lengthZ; k++)
                {
                    if (needToRemoves[i, j, k])
                    {
                        DestroyImmediate(cubeSpace[i, j, k].gameObject);
                    }
                }
            }
        }

        //move the origin back.
        for (int i = 0; i < cubes.Length; i++)
        {
            if (cubes[i] == null)
            {
                continue;
            }

            cubes[i].position += origin;
        }
    }

    private void GetAllChildren(Transform parent, LinkedList<Transform> children)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            if (child.childCount == 0)
            {
                children.AddLast(child);
            }
            else
            {
                GetAllChildren(child, children);
            }
        }
    }

    private LinkedList<Transform> GetAllChildrenFromSelected()
    {
        LinkedList<Transform> structure = new LinkedList<Transform>();
        if (Selection.transforms.Length > 0)
        {
            for (int i = 0; i < Selection.transforms.Length; i++)
            {
                if (Selection.transforms[i].childCount == 0)
                {
                    structure.AddLast(Selection.transforms[i]);
                }
                else
                {
                    GetAllChildren(Selection.transforms[i], structure);
                }
            }
        }
        return structure;
    }

    private void HollowSelected()
    {
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        if (structure.Count != 0)
        {
            HollowStructure(structure);
        }
    }

    private void AddRigidbodyToSelected()
    {
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        if (structure.Count != 0)
        {
            foreach (Transform transform in structure)
            {
                Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    DestroyImmediate(rigidbody);
                }
                rigidbody = transform.gameObject.AddComponent<Rigidbody>();
                UnityEditorInternal.ComponentUtility.CopyComponent(rigidBodyTemplate);
                UnityEditorInternal.ComponentUtility.PasteComponentValues(rigidbody);
            }
        }
    }

    private void RemoveRigidbodyFromSelected()
    {
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        if (structure.Count != 0)
        {
            foreach (Transform transform in structure)
            {
                Rigidbody rigidbody = transform.GetComponent<Rigidbody>();
                if (rigidbody != null)
                {
                    DestroyImmediate(rigidbody);
                }
            }
        }
    }

    private void ShowSelectedChildCount()
    {
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        Debug.Log(structure.Count);
    }

    private void TranslateSelected(Vector3 toTranslate)
    {
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        foreach (Transform transform in structure)
        {
            transform.position += toTranslate;
        }
    }

    private void WrapSelected()
    {
        if (Selection.transforms.Length == 0)
        {
            return;
        }
        GameObject gameObject = new GameObject();
        gameObject.transform.parent = Selection.transforms[0].parent;
        foreach (Transform transform in Selection.transforms)
        {
            transform.parent = gameObject.transform;
        }
    }

    private void ReplaceMaterialOfSelected()
    {
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        foreach (Transform transform in structure)
        {
            MeshRenderer meshRenderer = transform.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.material = materialForReplace;
            }
        }
    }

    private Transform[,,] CreateCubeSpace(LinkedList<Transform> structure, out Vector3 origin, out Transform[] cubes)
    {
        cubes = new Transform[structure.Count];
        int index = 0;
        foreach (Transform cube in structure)
        {
            cubes[index++] = cube;
        }
        Transform[,,] cubeSpace = CreateCubeSpace(cubes, out origin);
        return cubeSpace;
    }

    private Transform[,,] CreateCubeSpace(Transform[] cubes, out Vector3 origin)
    {
        //step 1: move the origin to (0, 0, 0) temporarily.
        float minX = float.MaxValue;
        float maxX = float.MinValue;
        float minY = float.MaxValue;
        float maxY = float.MinValue;
        float minZ = float.MaxValue;
        float maxZ = float.MinValue;
        Debug.Log(cubes.Length);
        for (int i = 0; i < cubes.Length; i++)
        {
            Vector3 position = cubes[i].position;
            if (position.x < minX)
            {
                minX = position.x;
            }
            if (position.x > maxX)
            {
                maxX = position.x;
            }
            if (position.y < minY)
            {
                minY = position.y;
            }
            if (position.y > maxY)
            {
                maxY = position.y;
            }
            if (position.z < minZ)
            {
                minZ = position.z;
            }
            if (position.z > maxZ)
            {
                maxZ = position.z;
            }
        }
        origin = new Vector3(minX, minY, minZ);
        for (int i = 0; i < cubes.Length; i++)
        {
            cubes[i].position -= origin;
        }

        //step 2: create a three-dimensional array to place all the cubes.
        int lengthX = (int)(maxX - minX) + 1;
        int lengthY = (int)(maxY - minY) + 1;
        int lengthZ = (int)(maxZ - minZ) + 1;
        Debug.Log(lengthX);
        Debug.Log(lengthY);
        Debug.Log(lengthZ);
        if (lengthX <= 0 || lengthY <= 0 || lengthZ <= 0)
        {
            return null;
        }
        Transform[,,] cubeSpace = new Transform[lengthX, lengthY, lengthZ];
        for (int i = 0; i < cubes.Length; i++)
        {
            Transform oneCube = cubes[i];
            Vector3 position = oneCube.position;
            int x = (int)position.x;
            int y = (int)position.y;
            int z = (int)position.z;
            if (cubeSpace[x, y, z] == null)
            {
                cubeSpace[x, y, z] = oneCube;
            }
        }

        return cubeSpace;
    }

    private void MergeSelected()
    {
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        if (structure.Count > 0)
        {
            MergeCubes(structure);
        }
    }

    private void MergeCubes(LinkedList<Transform> structure)
    {
        Vector3 origin;
        Transform[] cubes;
        Transform[,,] cubeSpace = CreateCubeSpace(structure, out origin, out cubes);

        int lengthX = cubeSpace.GetLength(0);
        int lengthY = cubeSpace.GetLength(1);
        int lengthZ = cubeSpace.GetLength(2);

        if (lengthX > 0 && lengthY > 0 && lengthZ > 0)
        {
            if (lengthX == 1 || lengthY == 1 || lengthZ == 1)
            {
                int count = 0;
                GameObject prefab = null;
                for (int i = 0; i < lengthX; i++)
                {
                    for (int j = 0; j < lengthY; j++)
                    {
                        for (int k = 0; k < lengthZ; k++)
                        {
                            if (cubeSpace[i, j, k] != null)
                            {
                                count++;
                                if (prefab == null)
                                {
                                    prefab = cubeSpace[i, j, k].gameObject;
                                }
                            }
                        }
                    }
                }

                if (count == lengthX * lengthY * lengthZ)
                {
                    GameObject bigCube = Instantiate(
                        prefab,
                        origin + new Vector3((float)lengthX / 2 - 0.5f, (float)lengthY / 2 - 0.5f, (float)lengthZ / 2 - 0.5f),
                        Quaternion.identity);
                    bigCube.transform.localScale = new Vector3(lengthX, lengthY, lengthZ);
                    bigCube.transform.parent = prefab.transform.parent;

                    for (int i = 0; i < lengthX; i++)
                    {
                        for (int j = 0; j < lengthY; j++)
                        {
                            for (int k = 0; k < lengthZ; k++)
                            {
                                if (cubeSpace[i, j, k] != null)
                                {
                                    DestroyImmediate(cubeSpace[i, j, k].gameObject);
                                }
                            }
                        }
                    }

                    return;
                }
            }
        }

        foreach (Transform cube in cubes)
        {
            if (cube == null)
            {
                continue;
            }
            cube.position += origin;
        }
    }

    private void MoveSelectedToOrigin()
    {
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        if (structure.Count != 0)
        {
            Vector3 origin;
            Transform[] cubes;
            Transform[,,] cubespace = CreateCubeSpace(structure, out origin, out cubes);
        }
    }

    private void RemoveRotationOfSelected()
    {
        if (Selection.transforms.Length > 0)
        {
            foreach(Transform root in Selection.transforms)
            {
                Dictionary<Transform, Vector3> cubeToPos = new Dictionary<Transform, Vector3>();
                AddIntoDic(root, cubeToPos);
                RemoveRotation(root, cubeToPos);
            }
        }
    }

    private void AddIntoDic(Transform root, Dictionary<Transform, Vector3> cubeToPos)
    {
        if (root == null)
        {
            return;
        }

        cubeToPos.Add(root, root.position);
        for (int i = 0; i < root.childCount; i++)
        {
            AddIntoDic(root.GetChild(i), cubeToPos);
        }
    }

    //unfinished
    private void RemoveRotation(Transform root, Dictionary<Transform, Vector3> cubeToPos)
    {
        if (root == null)
        {
            return;
        }

        root.SetPositionAndRotation(cubeToPos[root], Quaternion.identity);
        for (int i = 0; i < root.childCount; i++)
        {
            RemoveRotation(root.GetChild(i), cubeToPos);
        }
    }

    private void MapToFiles()
    {
        //step 1: create a cube space.
        LinkedList<Transform> structure = GetAllChildrenFromSelected();
        if (structure.Count <= 0)
        {
            return;
        }
        Vector3 origin;
        Transform[] cubes;
        Transform[,,] cubeSpace = CreateCubeSpace(structure, out origin, out cubes);
        Debug.Log(origin);
        int spaceSize_X_From = (int)origin.x;
        int spaceSize_Y_From = (int)origin.y;
        int spaceSize_Z_From = (int)origin.z;
        int spaceSize_X_To = (int)origin.x + cubeSpace.GetLength(0);
        int spaceSize_Z_To = (int)origin.z + cubeSpace.GetLength(2);
        int cubeSpace_X = cubeSpace.GetLength(0);
        int cubeSpace_Y = cubeSpace.GetLength(1);
        int cubeSpace_Z = cubeSpace.GetLength(2);

        //step 2: determine where chunks exist.
        int chunk_X_From = Mathf.FloorToInt((float)spaceSize_X_From / chunkSize);
        int chunk_Z_From = Mathf.FloorToInt((float)spaceSize_Z_From / chunkSize);
        int chunk_X_To = Mathf.CeilToInt((float)spaceSize_X_To / chunkSize);
        int chunk_Z_To = Mathf.CeilToInt((float)spaceSize_Z_To / chunkSize);
        Debug.Log("chunk_X_From: " + chunk_X_From);
        Debug.Log("chunk_Z_From: " + chunk_Z_From);
        Debug.Log("chunk_X_To: " + chunk_X_To);
        Debug.Log("chunk_Z_To: " + chunk_Z_To);
        bool[,] isExist = new bool[chunk_X_To, chunk_Z_To];
        for (int i = chunk_X_From; i < chunk_X_To; i++)
        {
            for (int j = chunk_Z_From; j < chunk_Z_To; j++)
            {
                int minX = i * chunkSize;
                int minZ = j * chunkSize;
                int maxX = (i + 1) * chunkSize - 1;
                int maxZ = (j + 1) * chunkSize - 1;
                int minY = spaceSize_Y_From;
                int maxY = spaceSize_Y_From + cubeSpace_Y - 1;
                bool hasSth = false;
                for (int x = minX; x <= maxX; x++)
                {
                    for (int z = minZ; z <= maxZ; z++)
                    {
                        for (int y = minY; y <= maxY; y++)
                        {
                            int indexX = x - spaceSize_X_From;
                            int indexY = y - spaceSize_Y_From ;
                            int indexZ = z - spaceSize_Z_From;
                            if (indexX < 0 || indexY < 0 || indexZ < 0
                                || indexX >= cubeSpace_X || indexY >= cubeSpace_Y || indexZ >= cubeSpace_Z)
                            {
                                continue;
                            }
                            if (cubeSpace[indexX, indexY, indexZ] != null)
                            {
                                hasSth = true;
                            }
                        }
                    }
                }
                if (hasSth)
                {
                    isExist[i, j] = true;
                }
            }
        }

        //step 3: create the directory structure of the map in StreamingAssets/Map/MapFragments.
        string rootPath = Application.streamingAssetsPath + "/Map/MapFragments";
        Debug.Log("rootPath: " + rootPath);
        DirectoryInfo rootPathInfo = new DirectoryInfo(rootPath);
        if (!rootPathInfo.Exists)
        {
            DirectoryInfo parent = rootPathInfo.Parent;
            if (!parent.Exists)
            {
                parent.Create();
                Debug.Log("Created: " + parent);
            }
            rootPathInfo.Create();
            Debug.Log("Created: " + rootPathInfo);
        }
        for (int i = chunk_X_From; i < chunk_X_To; i++)
        {
            string path = rootPath + '/' + i;
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                dir.Create();
                Debug.Log("Created: " + dir);
            }
        }

        //step 4: write/modify the info of the whole map.
        string mapInfoPath = rootPath + "/mapInfo.bytes";
        FileInfo mapInfoInfo = new FileInfo(mapInfoPath);
        int last_chunk_X_To = -1;
        int last_chunk_Z_To = -1;
        int last_chunkSize = -1;
        if (mapInfoInfo.Exists)
        {
            using (BinaryReader br = new BinaryReader(new FileStream(mapInfoPath, FileMode.Open)))
            {
                last_chunk_X_To = br.ReadInt32();
                last_chunk_Z_To = br.ReadInt32();
                last_chunkSize = br.ReadInt32();
            }
        }

        using(BinaryWriter bw = new BinaryWriter(new FileStream(mapInfoPath, FileMode.Create)))
        {
            int toBeWrtten_chunk_X_To = chunk_X_To > last_chunk_X_To ? chunk_X_To : last_chunk_X_To;
            int toBeWrtten_chunk_Z_To = chunk_Z_To > last_chunk_Z_To ? chunk_Z_To : last_chunk_Z_To;

            bw.Write(toBeWrtten_chunk_X_To);
            bw.Write(toBeWrtten_chunk_Z_To);
            bw.Write(chunkSize);
        }

        //step 5: find the lowest cube in every(x, z)
        int[,] lowestCubeY = new int[cubeSpace_X, cubeSpace_Z];
        for (int x = 0; x < cubeSpace_X; x++)
        {
            for (int z = 0; z < cubeSpace_Z; z++)
            {
                lowestCubeY[x, z] = -1;
            }
        }
        for (int x = 0; x < cubeSpace_X; x++)
        {
            for (int z = 0; z < cubeSpace_Z; z++)
            {
                for (int y = 0; y < cubeSpace_Y; y++)
                {
                    if (cubeSpace[x, y, z] != null)
                    {
                        lowestCubeY[x, z] = y;
                        break;
                    }
                }
            }
        }

        int[] offsetX = new int[] {-1,1,0,0,0,0};
        int[] offsetY = new int[] {0,0,-1,1,0,0};
        int[] offsetZ = new int[] {0,0,0,0,-1,1};
        //step 6: write every chunk into a file. 
        //sub step 1: initialize the dictionary used for counting the number of the materials used in one chunk.
        MaterialManager materialManager = MaterialManager.INSTANCE;
        Dictionary<byte, LinkedList<Transform>> materialIdToCubes = new Dictionary<byte, LinkedList<Transform>>();
        byte[] Ids = materialManager.GetAllIds();
        foreach (byte Id in Ids)
        {
            materialIdToCubes.Add(Id, new LinkedList<Transform>());
        }
        Debug.Log("chunk_X_To: " + chunk_X_To);
        Debug.Log("chunk_Z_To: " + chunk_Z_To);
        for (int i = chunk_X_From; i < chunk_X_To; i++)
        {
            for (int j = chunk_Z_From; j < chunk_Z_To; j++)
            {
                if (!isExist[i, j])
                {
                    continue;
                }
                
                //sub step 2: create a new .bytes file.
                string path = rootPath + '/' + i + '/' + j + ".bytes";
                using (BinaryWriter bw = new BinaryWriter(new FileStream(path, FileMode.Create)))
                {
                    //sub step 3: count how many cubes there are in this chunk and how many materials these cubes use.
                    int cubeCount = 0;
                    int materialCount = 0;

                    foreach (byte Id in Ids)
                    {
                        materialIdToCubes[Id].Clear();
                    }

                int minX = i * chunkSize;
                int minZ = j * chunkSize;
                int maxX = (i + 1) * chunkSize - 1;
                int maxZ = (j + 1) * chunkSize - 1;
                int minY = spaceSize_Y_From;
                int maxY = spaceSize_Y_From + cubeSpace_Y - 1;
                    for (int x = minX; x <= maxX; x++)
                    {
                        for (int z = minZ; z <= maxZ; z++)
                        {
                            for (int y = minY; y <= maxY; y++)
                            {
                                int indexX = x - spaceSize_X_From;
                                int indexY = y - spaceSize_Y_From;
                                int indexZ = z - spaceSize_Z_From;
                                if (indexX < 0 || indexY < 0 || indexZ < 0
                                    || indexX >= cubeSpace_X || indexY >= cubeSpace_Y || indexZ >= cubeSpace_Z)
                                {
                                    continue;
                                }
                                Transform cube = cubeSpace[indexX, indexY, indexZ];
                                if (cube != null)
                                {
                                    cubeCount++;

                                    byte materialId = 0;
                                    MeshRenderer renderer = cube.gameObject.GetComponent<MeshRenderer>();
                                    if (renderer != null)
                                    {
                                        materialId = materialManager.MaterialNameToId(renderer.sharedMaterial.name);
                                    }
                                    if (materialIdToCubes[materialId].Count == 0)
                                    {
                                        materialCount++;
                                    }
                                    materialIdToCubes[materialId].AddLast(cube);
                                }
                            }
                        }
                    }

                    //sub step 4: write the first line, format(ignore '|'): cubeCount|materialCount
                    bw.Write(cubeCount);
                    bw.Write(materialCount);

                    //sub step 5: write all the cubes in this chunk into file.
                    int visiblePlaneCount = cubeCount * 6;
                    foreach(byte Id in Ids)
                    {
                        //sub sub step 1: determine if current chunk uses this material.
                        if (materialIdToCubes[Id].Count <= 0)
                        {
                            continue;
                        }

                        //sub sub step 2: write the line which tells which material is used and how much cube use this material
                        bw.Write(Id);
                        bw.Write(materialIdToCubes[Id].Count);
                        
                        foreach (Transform cube in materialIdToCubes[Id])
                        {
                            //sub sub step 3: write the encoded xyz into the file
                            ushort x = (ushort)(cube.transform.position.x + spaceSize_X_From - minX);
                            ushort y = (ushort)(cube.transform.position.y + spaceSize_Y_From - 0);
                            ushort z = (ushort)(cube.transform.position.z + spaceSize_Z_From - minZ);
                            //Debug.Log("x: " + x + " y: " + y + " z:" + z);
                            ushort encodedXYZ = MapChunkIO.EncodeXYZ(x, y, z);
                            bw.Write(encodedXYZ);

                            //sub sub step 4: determine the plane that is visible
                            int indexXInCubeSpace = (int)(cube.transform.position.x);
                            int indexYInCubeSpace = (int)(cube.transform.position.y);
                            int indexZInCubeSpace = (int)(cube.transform.position.z);
                            //up, down, left, right, forward ,backward.
                            bool[] isVisible = new bool[6];
                            for (int direction = 0; direction < 6; direction++)
                            {
                                isVisible[direction] = true;
                                int adjacentX = indexXInCubeSpace + offsetX[direction];
                                int adjacentY = indexYInCubeSpace + offsetY[direction];
                                int adjacentZ = indexZInCubeSpace + offsetZ[direction];
                                if (adjacentX >= 0 && adjacentX < cubeSpace_X 
                                    && adjacentY >= 0 && adjacentY < cubeSpace_Y
                                    && adjacentZ >= 0 && adjacentZ < cubeSpace_Z
                                    && cubeSpace[adjacentX, adjacentY, adjacentZ] != null)
                                {
                                    isVisible[direction] = false;
                                    visiblePlaneCount--;
                                }
                                if (direction == 2 && lowestCubeY[indexXInCubeSpace, indexZInCubeSpace] == indexYInCubeSpace)
                                {
                                    isVisible[direction] = false;
                                    visiblePlaneCount--;
                                }
                            }
                            byte encodedVisible = MapChunkIO.EncodeVisible(isVisible);
                            bw.Write(encodedVisible);
                        }
                    }
                    bw.Write(visiblePlaneCount);
                }
            }
        }

        //last step: move back all the cubes.
        foreach (Transform cube in cubes)
        {
            cube.position += origin;
        }
    }

    private void FilesToMap()
    {
        //step 1: get the single instance of the MaterialManager. 
        MaterialManager materialManager = MaterialManager.INSTANCE;

        //step 2: read the mapInfo.bytes.
        string rootPath = Application.streamingAssetsPath + "/Map/MapFragments";
        if (!ValidateDirExists(rootPath))
        {
            Debug.LogError("Map is broken.");
            return;
        }
        string mapInfoPath = rootPath + "/mapInfo.bytes";
        if (!ValidateFileExists(mapInfoPath))
        {
            Debug.LogError("Map is broken.");
            return;
        }
        int Chunk_X_Max;
        int Chunk_Z_Max;
        int ChunkSize;
        using (BinaryReader br = new BinaryReader(new FileStream(mapInfoPath, FileMode.Open)))
        {
            Chunk_X_Max = br.ReadInt32();
            Chunk_Z_Max = br.ReadInt32();
            ChunkSize = br.ReadInt32();
        }

        //step 3: read all chunks
        GameObject map = new GameObject
        {
            name = "MapFromFiles"
        };
        for (int chunkX = 0; chunkX < Chunk_X_Max; chunkX++)
        {
            string dirPath = rootPath + '/' + chunkX;
            if (!ValidateDirExists(dirPath))
            {
                continue;
            }

            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            FileInfo[] fileInfos = dirInfo.GetFiles();
            foreach (FileInfo fileInfo in fileInfos)
            {
                if (!fileInfo.FullName.EndsWith(".bytes"))
                {
                    continue;
                }

                string strChunkZ = fileInfo.Name.Split('.')[0];
                int chunkZ;
                if (int.TryParse(strChunkZ, out chunkZ))
                {
                    GameObject chunk = new GameObject
                    {
                        name = "chunk(" + chunkX + "," + chunkZ + ")"
                    };
                    chunk.transform.parent = map.transform;
                    using (BinaryReader br = new BinaryReader(new FileStream(fileInfo.FullName, FileMode.Open)))
                    {
                        int cubeCount = br.ReadInt32();
                        int materialCount = br.ReadInt32();
                        for (int i = 0; i < materialCount; i++)
                        {
                            byte materialId = br.ReadByte();
                            int count = br.ReadInt32();
                            while (count-- > 0)
                            {
                                ushort encodedXYZ = br.ReadUInt16();
                                byte encodedVisible = br.ReadByte();
                                byte x, y, z;
                                MapChunkIO.DecodeXYZ(encodedXYZ, out x, out y, out z);
                                float actualX = (float)x + chunkX * ChunkSize;
                                float actualY = (float)y;
                                float actualZ = (float)z + chunkZ * ChunkSize;
                                GameObject cube = Instantiate(cubePrefab, new Vector3(actualX, actualY, actualZ), Quaternion.identity, chunk.transform);
                                MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
                                if (renderer == null)
                                {
                                    renderer = cube.AddComponent<MeshRenderer>();
                                }
                                renderer.sharedMaterial = materialManager.IdToMaterial(materialId);
                            }
                        }
                        int visiblePlaneCount;
                        visiblePlaneCount = br.ReadInt32();
                    }
                }
            }
        }
    }

    //used in FilesToMap()
    private bool ValidateFileExists(string path)
    {
        return new FileInfo(path).Exists;
    }

    //used in FilesToMap()
    private bool ValidateDirExists(string path)
    {
        return new DirectoryInfo(path).Exists;
    }

	private void CreateNoiseChunk(){
		NoiseFuction noise = new NoiseFuction ();

		int terrainHeight = 10;

		int worldX = indexOfChunkX * sizeOfChunk;
		int worldZ = indexOfChunkZ * sizeOfChunk;
		float terrainSizeX = numOfChunkX * sizeOfChunk;
		float terrainSizeZ = numOfChunkZ * sizeOfChunk;

		GameObject obj = new GameObject ("Chunk["+indexOfChunkX+","+indexOfChunkZ+"]");
		obj.SetActive (false);

		int worldY;
		for (int x = 0; x < sizeOfChunk; x++) {
			for (int z = 0; z < sizeOfChunk; z++) {
				//worldY = Mathf.FloorToInt(terrainHeight * noise.PerlinNoise ((worldX + x)/(float)ChunkSize, (worldZ + z)/(float)ChunkSize));
				worldY = Mathf.FloorToInt(terrainHeight * noise.PerlinNoise ((worldX + x)/(float)terrainSizeX, (worldZ + z)/(float)terrainSizeZ));
				GameObject cube = GameObject.CreatePrimitive (PrimitiveType.Cube);
				if (worldY <= 5) {
					cube.GetComponent<MeshRenderer> ().material = (Material) Resources.Load ("Materials/(26)sea");
				} else {
					cube.GetComponent<MeshRenderer> ().material = (Material) Resources.Load ("Materials/(25)grass");
				}
				cube.transform.position = new Vector3 (worldX + x, worldY, worldZ + z);
				cube.transform.SetParent (obj.transform);
			}
		}
	}
}
