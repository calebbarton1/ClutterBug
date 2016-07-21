using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Reflection;

[ExecuteInEditMode]
public class Clutter : MonoBehaviour {

    //enums for collider selection in inspector
    public enum colliderMenu
    {
        Box,
        Sphere,
        Cylinder
    }

    //initialise enum and colliders
    public colliderMenu shape = colliderMenu.Box;
    Collider col;

    public GameObject go;

    public void Awake()
    {
        col = gameObject.GetComponent<Collider>();
    }


    void Start() {

    }

    // Update is called once per frame
    void Update()
    {
        ShapeColliders();
    }


    public void ShapeColliders()
    {
        switch (shape)
        {
            case colliderMenu.Box:
                if (col.GetType() != typeof(BoxCollider))//checks what collider is currently on node
                {
                    Destroy(col);//destroys previous collider
                    col = gameObject.AddComponent<BoxCollider>();//makes new one
                }
                break;

            case colliderMenu.Sphere:
                if (col.GetType() != typeof(SphereCollider))
                {
                    Destroy(col);
                    col = gameObject.AddComponent<SphereCollider>();
                }
                break;

            case colliderMenu.Cylinder:
                if (col.GetType() != typeof(CapsuleCollider))
                {
                    Destroy(col);
                    col = gameObject.AddComponent<CapsuleCollider>();
                }
                break;

            default:
                break;
        }
    }

    //button for generating objects
    //credit to "zaikman" for the script
    [Space(10)]

    [InspectorButton("OnButtonClicked")]
    public bool SpawnObjects;//makes a button with this bool

    [Space(10)]

    [InspectorButton("DeleteObject")]
    public bool DeleteObjects;

    Object testGo;

    private void OnButtonClicked()
    {
        Debug.Log("shit worked yo");
        testGo = Instantiate(go, gameObject.transform.position, Quaternion.identity);//placeholder spawn
    }

    private void DeleteObject()
    {
        DestroyImmediate(testGo);//placeholder
    }
    
}