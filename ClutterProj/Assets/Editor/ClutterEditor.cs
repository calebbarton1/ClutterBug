// Copyright (C) 2016 Caleb Barton (caleb.barton@hotmail.com)
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


[CustomEditor(typeof(Node))]
public class NodeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        Node nodeScript = (Node)target;

        EditorGUILayout.Separator();

        //buttons
        if (GUILayout.Button("Spawn Clutter"))
            nodeScript.SpawnObjectsInArea();

        EditorGUILayout.Separator();

        if (GUILayout.Button("Delete Clutter"))
            nodeScript.DeleteClutter();

        EditorGUILayout.Separator();


        nodeScript.debug = EditorGUILayout.Toggle("Enable Debugging", nodeScript.debug);//debug bool
        nodeScript.shape = (Node.colliderMenu)EditorGUILayout.EnumPopup("Shape of Clutter Node", nodeScript.shape);//shape enum
        nodeScript.clutterMask = LayerMaskField("Clutter Mask", nodeScript.clutterMask);//layermask list
        nodeScript.numberToSpawn = EditorGUILayout.IntField("Number of Clutter", nodeScript.numberToSpawn);

        EditorGUILayout.Separator();

        //drawing out custom list for the prefabs and their random weighting
        //TODO: ALLOW DRAG AND DROP TO REPLACE ELEMENTS
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

            serializedObject.Update();
            SerializedProperty list2 = serializedObject.FindProperty("prefabWeights");//now get the list with the weights           

            if (list1.isExpanded && Event.current.type != EventType.DragPerform)//doesn't like to begin horizontals when the drag is updating, so we wait until next frame
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
        nodeScript.faceNormal = EditorGUILayout.Toggle("Rotate to Surface Normal", nodeScript.faceNormal);
        nodeScript.angleLimit = EditorGUILayout.Slider("Surface Angle Limit", nodeScript.angleLimit, 0, 89);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        nodeScript.lockX = EditorGUILayout.Toggle("Lock X Postion", nodeScript.lockX);
        nodeScript.lockZ = EditorGUILayout.Toggle("Lock Z Postion", nodeScript.lockZ);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        nodeScript.useMesh = EditorGUILayout.Toggle("Use Mesh Scaling", nodeScript.useMesh);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.rotationOverride == Vector3.zero && Event.current.type != EventType.DragPerform)
        {
            EditorGUILayout.LabelField("Random Rotation");

            //Showing the vector2 values as min/max
            //X
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotX.x = EditorGUILayout.FloatField(nodeScript.rotX.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotX.y = EditorGUILayout.FloatField(nodeScript.rotX.y);
            EditorGUILayout.EndHorizontal();

            //Y
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotY.x = EditorGUILayout.FloatField(nodeScript.rotY.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotY.y = EditorGUILayout.FloatField(nodeScript.rotY.y);
            EditorGUILayout.EndHorizontal();

            //Z
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Z Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotZ.x = EditorGUILayout.FloatField(nodeScript.rotZ.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotZ.y = EditorGUILayout.FloatField(nodeScript.rotZ.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.rotationOverride = EditorGUILayout.Vector3Field("Rotation Override", nodeScript.rotationOverride);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.scaleOverride == Vector3.zero && Event.current.type != EventType.DragPerform)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Random Scale");
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.randomScale.x = EditorGUILayout.FloatField(nodeScript.randomScale.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.randomScale.y = EditorGUILayout.FloatField(nodeScript.randomScale.y);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Separator();
        }
        nodeScript.scaleOverride = EditorGUILayout.Vector3Field("Scale Override", nodeScript.scaleOverride);
    }

    //Getting the Layers to draw into the inspector
    //TODO: MAKE THIS ITS OWN CLASS
    static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<int> layerNumbers = new List<int>();

        var layers = InternalEditorUtility.layers;

        layerNumbers.Clear();

        for (int i = 0; i < layers.Length; i++)
            layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        layerMask.value = mask;

        return layerMask;
    }
}

[CustomEditor(typeof(NodeChild))]
public class NodeChildInspector : Editor
{
    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        NodeChild nodeScript = (NodeChild)target;

        nodeScript.debug = EditorGUILayout.Toggle("Enable Debugging", nodeScript.debug);//debug bool
        nodeScript.clutterMask = LayerMaskField("Clutter Mask", nodeScript.clutterMask);//layermask list
        nodeScript.numberToSpawn = EditorGUILayout.IntField("Number of Clutter", nodeScript.numberToSpawn);
        nodeScript.distance = EditorGUILayout.FloatField("Distance from Parent", nodeScript.distance);

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
        nodeScript.faceNormal = EditorGUILayout.Toggle("Rotate to Surface Normal", nodeScript.faceNormal);
        nodeScript.angleLimit = EditorGUILayout.Slider("Surface Angle Limit", nodeScript.angleLimit, 0, 89);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        nodeScript.lockX = EditorGUILayout.Toggle("Lock X Postion", nodeScript.lockX);
        nodeScript.lockZ = EditorGUILayout.Toggle("Lock Z Postion", nodeScript.lockZ);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.rotationOverride == Vector3.zero)
        {
            EditorGUILayout.LabelField("Random Rotation");

            //Showing the vector2 values as min/max
            //X
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("X Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotX.x = EditorGUILayout.FloatField(nodeScript.rotX.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotX.y = EditorGUILayout.FloatField(nodeScript.rotX.y);
            EditorGUILayout.EndHorizontal();

            //Y
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Y Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotY.x = EditorGUILayout.FloatField(nodeScript.rotY.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotY.y = EditorGUILayout.FloatField(nodeScript.rotY.y);
            EditorGUILayout.EndHorizontal();

            //Z
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Z Rotation", GUILayout.Width(80));
            EditorGUILayout.LabelField("Min", GUILayout.Width(30));
            nodeScript.rotZ.x = EditorGUILayout.FloatField(nodeScript.rotZ.x);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            nodeScript.rotZ.y = EditorGUILayout.FloatField(nodeScript.rotZ.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Separator();
        }

        nodeScript.rotationOverride = EditorGUILayout.Vector3Field("Rotation Override", nodeScript.rotationOverride);

        EditorGUILayout.Separator();
        EditorGUILayout.Separator();

        if (nodeScript.scaleOverride == Vector3.zero)
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


    //Getting the Layers to draw into the inspector
    static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<int> layerNumbers = new List<int>();

        var layers = InternalEditorUtility.layers;

        layerNumbers.Clear();

        for (int i = 0; i < layers.Length; i++)
            layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        layerMask.value = mask;

        return layerMask;
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
        nodeScript.clutterMask = LayerMaskField("Clutter Mask", nodeScript.clutterMask);//layermask list
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


    //Getting the Layers to draw into the inspector
    static LayerMask LayerMaskField(string label, LayerMask layerMask)
    {
        List<int> layerNumbers = new List<int>();

        var layers = InternalEditorUtility.layers;

        layerNumbers.Clear();

        for (int i = 0; i < layers.Length; i++)
            layerNumbers.Add(LayerMask.NameToLayer(layers[i]));

        int maskWithoutEmpty = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if (((1 << layerNumbers[i]) & layerMask.value) > 0)
                maskWithoutEmpty |= (1 << i);
        }

        maskWithoutEmpty = EditorGUILayout.MaskField(label, maskWithoutEmpty, layers);

        int mask = 0;
        for (int i = 0; i < layerNumbers.Count; i++)
        {
            if ((maskWithoutEmpty & (1 << i)) > 0)
                mask |= (1 << layerNumbers[i]);
        }
        layerMask.value = mask;

        return layerMask;
    }
}