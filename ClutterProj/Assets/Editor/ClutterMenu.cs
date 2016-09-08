using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

public class ClutterMenu : MonoBehaviour {

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
        base.OnInspectorGUI();
        Node nodeScript = (Node)target;

        //buttons
        if (GUILayout.Button("Spawn Clutter"))
            nodeScript.SpawnObjectsInArea();

        if (GUILayout.Button("Delete Clutter"))
            nodeScript.DeleteClutter();

        /*
        nodeScript.debug = EditorGUILayout.Toggle("Enable Debugging", nodeScript.debug);//debug bool
        nodeScript.shape = (Node.colliderMenu)EditorGUILayout.EnumPopup("Shape of Clutter Node", nodeScript.shape);//shape enum
        nodeScript.clutterMask = LayerMaskField("Clutter Masking", nodeScript.clutterMask);//layermask list

        //draw list
        var property = serializedObject.FindProperty("prefabList");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property, true);
        serializedObject.ApplyModifiedProperties();

        */
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
        base.OnInspectorGUI();
        
        Node2D nodeScript = (Node2D)target;

        //buttons
        if (GUILayout.Button("Spawn Clutter"))
            nodeScript.SpawnObjectsInArea();

        if (GUILayout.Button("Delete Clutter"))
            nodeScript.DeleteClutter();

        //WIP
        /*
        nodeScript.debug = EditorGUILayout.Toggle("Enable Debugging", nodeScript.debug);//debug bool
        nodeScript.shape = (Node2D.colliderMenu)EditorGUILayout.EnumPopup("Shape of Clutter Node", nodeScript.shape);//shape enum
        nodeScript.clutterMask = LayerMaskField("Clutter Masking", nodeScript.clutterMask);//layermask list

        //draw list
        var property = serializedObject.FindProperty("prefabList");
        serializedObject.Update();
        EditorGUILayout.PropertyField(property, true);
        serializedObject.ApplyModifiedProperties();
        */
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