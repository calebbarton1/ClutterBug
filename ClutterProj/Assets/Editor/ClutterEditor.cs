// Copyright (C) 2018 Caleb Barton (caleb.barton@hotmail.com)
//github.com/calebbarton1
//Released under MIT License
//https://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif



public class ClutterMenu : MonoBehaviour
{
    static private GameObject node;

    [MenuItem("ClutterBug/Create3DNode")]
    static public void Create3DNode()
    {
        //gets prefab path
        node = AssetDatabase.LoadAssetAtPath("Assets/ClutterBug/ClutterNode.prefab", typeof(GameObject)) as GameObject;
        Object clone = Instantiate(node, Vector3.zero, Quaternion.identity);
        //removes the (clone) in name
        clone.name = node.name;
    }

    [MenuItem("ClutterBug/Create2DNode")]
    static public void Create2DNode()
    {
        //gets prefab path
        node = AssetDatabase.LoadAssetAtPath("Assets/ClutterBug/ClutterNode2D.prefab", typeof(GameObject)) as GameObject;
        Object clone = Instantiate(node, Vector2.zero, Quaternion.identity);
        //removes the (clone) in name
        clone.name = node.name;
    }
}

public class LayerMasker : Editor
{
    /// <summary>
    /// Allows the editor to display all the layermasks in an enum like dropdown
    /// </summary>
    /// <param name="_label">The title of the layermask</param>
    /// <param name="_tooltip">provided tooltip</param>
    /// <param name="_layerMask">Layermask provided to be displayed</param>
    /// <returns></returns>
    public static LayerMask LayerMaskField(string _label, string _tooltip, LayerMask _layerMask)
    {
        List<int> layerNumbers = new List<int>();

        var layers = InternalEditorUtility.layers;

        layerNumbers.Clear();

        for (int i = 0; i < layers.Length; i++)
            layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if (((1 << layerNumbers[i]) & _layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(new GUIContent(_label, _tooltip), maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        _layerMask.value = mask;

        return _layerMask;
    }
}


[CustomEditor(typeof(Node)), CanEditMultipleObjects]
public class NodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        #region Buttons
        Node nodeScript = (Node)target;

        EditorGUILayout.Separator();

        if (!nodeScript.m_IsChild)
        {
            if (GUILayout.Button("Spawn Clutter"))
                nodeScript.StartClutterSpawn(false);

            EditorGUILayout.Separator();

            if (GUILayout.Button("Delete Clutter"))
                nodeScript.DeleteClutter();
        }

        #endregion

        #region Base Variables
        EditorGUILayout.Separator();

        nodeScript.debug = EditorGUILayout.Toggle(new GUIContent("Enable Debugging", "Toggles Clutterbug DebugLogs. Useful for checking why clutter isn't placing properly. \nWARNING! SLOW PERFORMANCE WHILE ENABLED"), nodeScript.debug);//debug bool
        if (!nodeScript.m_IsChild)
            nodeScript.shape = (Node.colliderMenu)EditorGUILayout.EnumPopup(new GUIContent("Shape of Clutter Node"), nodeScript.shape);//shape enum       
        nodeScript.clutterMask = LayerMasker.LayerMaskField("Clutter Mask", "Selected Layermasks will be ignored by Clutterbugs Raycasting.", nodeScript.clutterMask);//layermask list
        nodeScript.numberToSpawn = EditorGUILayout.IntField("Number of Clutter", nodeScript.numberToSpawn);
        nodeScript.distance = EditorGUILayout.FloatField(new GUIContent("Distance from Parent", "Children of the prefab will use this value to move away from the parent. The parent doesn't use this value for anything."), nodeScript.distance);

        EditorGUILayout.Separator();
        #endregion        

        #region Prefab List
        //initialise lists if they haven't been
        if (nodeScript.prefabList == null)
            nodeScript.prefabList = new List<GameObject>();
        if (nodeScript.prefabWeights == null)
            nodeScript.prefabWeights = new List<float>();

        serializedObject.Update();
        SerializedProperty prefabList = serializedObject.FindProperty("prefabList");//get prefab list
        EditorGUILayout.PropertyField(prefabList);//list label
        SerializedProperty size = prefabList.FindPropertyRelative("Array.size");
        EditorGUILayout.PropertyField(size);//list size
        serializedObject.ApplyModifiedProperties();

        //this is to keep the weights list the same size as the prefab list at all times.
        while (nodeScript.prefabWeights.Count < nodeScript.prefabList.Count)
            nodeScript.prefabWeights.Add(1f);
        while (nodeScript.prefabWeights.Count > nodeScript.prefabList.Count)
            nodeScript.prefabWeights.RemoveAt(nodeScript.prefabWeights.Count - 1);

        serializedObject.ApplyModifiedProperties();

        serializedObject.Update();
        SerializedProperty weightList = serializedObject.FindProperty("prefabWeights");//now get the list with the weights           

        if (size.hasMultipleDifferentValues)
            EditorGUILayout.HelpBox("Not showing lists with  different sizes.", MessageType.Warning);

        if (prefabList.isExpanded && !size.hasMultipleDifferentValues)
        {
            for (int index = 0; index < prefabList.arraySize; ++index)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(prefabList.GetArrayElementAtIndex(index), GUIContent.none, GUILayout.Width(150));//the prefab
                EditorGUILayout.LabelField("Weight", GUILayout.Width(60));
                EditorGUILayout.PropertyField(weightList.GetArrayElementAtIndex(index), GUIContent.none, GUILayout.Width(30));//the weight
                EditorGUILayout.EndHorizontal();
            }
        }

        serializedObject.ApplyModifiedProperties();
        #endregion

        #region Clutter Variables
        EditorGUILayout.Separator();

        nodeScript.allowOverlap = EditorGUILayout.Toggle(new GUIContent("Clutter Overlap", "Allows clutter to spawn inside other clutter."), nodeScript.allowOverlap);
        if (!nodeScript.m_IsChild)
            nodeScript.additive = EditorGUILayout.Toggle(new GUIContent("Additive Clutter", "On Clutter nodes only, enalbing this won't deleting previously created clutter."), nodeScript.additive);
        nodeScript.faceNormal = EditorGUILayout.Toggle(new GUIContent("Rotate to Normal", "Spawned clutter will turn to face the surface normal of the raycast point."), nodeScript.faceNormal);
        nodeScript.angleLimit = EditorGUILayout.Slider(new GUIContent("Surface Angle Limit","The maximum angel (in degrees) that an object can spawn on."), nodeScript.angleLimit, 0, 89);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        #endregion

        #region Transform and Position Options
        nodeScript.lockX = EditorGUILayout.Toggle(new GUIContent("Lock X Postion", "Limits clutter to 0 on the X axis."), nodeScript.lockX);
        nodeScript.lockZ = EditorGUILayout.Toggle(new GUIContent("Lock Z Postion","Limits clutter to 0 on the Z axis."), nodeScript.lockZ);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        nodeScript.offsetPos = EditorGUILayout.Toggle(new GUIContent("Offset Position", "If your clutter is spawning inside the ground, this option will move the object up by half its scale value."), nodeScript.offsetPos);
        if (nodeScript.offsetPos)
        {
            nodeScript.useMesh = EditorGUILayout.Toggle(new GUIContent("Use Mesh Scaling", "Toggle this to use the mesh scaling value instead of transform scaling."), nodeScript.useMesh);
        }

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        #region Rotation Fields
        if (nodeScript.rotationOverride == Vector3.zero)
        {
            EditorGUILayout.LabelField(new GUIContent("Random Rotation","Clutter will randomly rotate between Min and Max range when spawning."));

            //Showing the vector2 values as min/max
            //X
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotX.x = EditorGUILayout.FloatField(nodeScript.rotX.x, GUILayout.Width(50));
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotX.y = EditorGUILayout.FloatField(nodeScript.rotX.y, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            //Y
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotY.x = EditorGUILayout.FloatField(nodeScript.rotY.x, GUILayout.Width(50));
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotY.y = EditorGUILayout.FloatField(nodeScript.rotY.y, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            //Z
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Z Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotZ.x = EditorGUILayout.FloatField(nodeScript.rotZ.x, GUILayout.Width(50));
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotZ.y = EditorGUILayout.FloatField(nodeScript.rotZ.y, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.rotationOverride = EditorGUILayout.Vector3Field(new GUIContent("Rotation Override", "Filling data into this field will override the above random rotations, and will set all spawned clutter to this value."), nodeScript.rotationOverride);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();
        #endregion

        #region Scale Fields
        if (nodeScript.scaleOverride == Vector3.zero)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Random Scale","Will randomly scale clutter between Min and Max."), GUILayout.Width(90));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.randomScale.x = EditorGUILayout.FloatField(nodeScript.randomScale.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.randomScale.y = EditorGUILayout.FloatField(nodeScript.randomScale.y);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
        nodeScript.scaleOverride = EditorGUILayout.Vector3Field(new GUIContent("Scale Override", "Filling Data into this field will override the avoce random scale, and set all spawned clutter to this scale."), nodeScript.scaleOverride);
        #endregion
        #endregion
        serializedObject.ApplyModifiedProperties();
    }    
}

[CustomEditor(typeof(Node2D))]
public class Node2DInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        Node2D nodeScript = (Node2D)target;

        EditorGUILayout.Separator();

        //buttons
        if (GUILayout.Button("Spawn Clutter"))
            nodeScript.SpawnObjectsInArea();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Delete Clutter"))
            nodeScript.DeleteClutter();

        EditorGUILayout.Separator();


        nodeScript.debug = EditorGUILayout.Toggle("Enable Debugging", nodeScript.debug);//debug bool
        nodeScript.shape = (Node2D.colliderMenu)EditorGUILayout.EnumPopup("Shape of Clutter Node", nodeScript.shape);//shape enum
        nodeScript.clutterMask = LayerMasker.LayerMaskField("Clutter Mask", "", nodeScript.clutterMask);//layermask list
        nodeScript.numberToSpawn = EditorGUILayout.IntField("Number of Clutter", nodeScript.numberToSpawn);

        EditorGUILayout.Separator();

        {
            serializedObject.Update();
            SerializedProperty list1 = serializedObject.FindProperty("prefabList");//get prefab list
            EditorGUILayout.PropertyField(list1);//list label
            EditorGUILayout.PropertyField(list1.FindPropertyRelative("Array.size"));//list size
            serializedObject.ApplyModifiedProperties();

            //this is to keep the weights list the same size as the prefab list at all times.
            while (nodeScript.prefabWeights.Count < nodeScript.prefabList.Count)
                nodeScript.prefabWeights.Add(1f);
            while (nodeScript.prefabWeights.Count > nodeScript.prefabList.Count)
                nodeScript.prefabWeights.RemoveAt(nodeScript.prefabWeights.Count - 1);

            SerializedProperty list2 = serializedObject.FindProperty("prefabWeights");//now get the list with the weights

            serializedObject.Update();

            if (list1.isExpanded && Event.current.type != EventType.DragPerform)
            {
                //EditorGUILayout.PropertyField(list1.FindPropertyRelative("Array.size"));
                for (int index = 0; index < list1.arraySize; ++index)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(list1.GetArrayElementAtIndex(index), GUIContent.none, GUILayout.Width(150));//the prefab
                    EditorGUILayout.LabelField("Weight", GUILayout.Width(60));
                    EditorGUILayout.PropertyField(list2.GetArrayElementAtIndex(index), GUIContent.none, GUILayout.Width(30));//the weight
                    EditorGUILayout.EndHorizontal();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Separator();

        nodeScript.allowOverlap = EditorGUILayout.Toggle("Enable Clutter Overlap", nodeScript.allowOverlap);
        nodeScript.additive = EditorGUILayout.Toggle("Enable Additive Clutter", nodeScript.additive);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.orderLayerOverride == 0)
        {
            EditorGUILayout.LabelField("Random Order in Layer");

            //Showing the vector2 values as min/max
            //X
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.randomOrderLayer.x = EditorGUILayout.FloatField(nodeScript.randomOrderLayer.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.randomOrderLayer.y = EditorGUILayout.FloatField(nodeScript.randomOrderLayer.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.orderLayerOverride = (int)EditorGUILayout.FloatField("Order Layer", nodeScript.orderLayerOverride);


        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        nodeScript.lockX = EditorGUILayout.Toggle("Lock X Postion", nodeScript.lockX);
        nodeScript.lockY = EditorGUILayout.Toggle("Lock Y Postion", nodeScript.lockY);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();


        if (nodeScript.rotationOverride == 0)
        {
            //Showing the vector2 values as min/max
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Random Rotation", GUILayout.Width(110));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotZ.x = EditorGUILayout.FloatField(nodeScript.rotZ.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotZ.y = EditorGUILayout.FloatField(nodeScript.rotZ.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.rotationOverride = EditorGUILayout.FloatField("Rotation Override", nodeScript.rotationOverride);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.scaleOverride == Vector2.zero)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Random Scale", GUILayout.Width(100));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.randomScale.x = EditorGUILayout.FloatField(nodeScript.randomScale.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.randomScale.y = EditorGUILayout.FloatField(nodeScript.randomScale.y);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
        nodeScript.scaleOverride = EditorGUILayout.Vector3Field("Scale Override", nodeScript.scaleOverride);
    }
}