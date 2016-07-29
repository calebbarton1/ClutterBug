using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ClutterMenu : MonoBehaviour {

    static private GameObject node;

    [MenuItem("ClutterBug/CreateNode")]
    static public void CreateNode()
    {
        node = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/ClutterBug/ClutterNode.prefab", typeof(GameObject));
        Object clone = Instantiate(node, Vector3.zero, Quaternion.identity);
        clone.name = node.name;
    }
}
