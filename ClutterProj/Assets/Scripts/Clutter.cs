using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;
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

    //buttons for generating objects
    //credit to "zaikman" for the script

    [Space(10)]

    [InspectorButton("SpawnObjectsInArea")]
    public bool SpawnObjects;//makes a button with this bool

    [Space(10)]

    [InspectorButton("DeleteObject")]
    public bool DeleteObjects;

    [Space(10)]


    //initialise enum and colliders
    [Tooltip("The shape of the area where objects are placed")]
    public colliderMenu shape = colliderMenu.Box;
    Collider col;

    [Tooltip("Adds clutter per click instead of rerolling")]
    public bool Additive = false;

    [Tooltip("Objects to be created as clutter")]
    public List<GameObject> goList;//temp

    [Tooltip("Number of clutter created")]
    public int numberToSpawn;

    [HideInInspector]
    public List<Object> spawnedObjects;

    private GameObject nodeParent;

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


    public void ShapeColliders()//currently just tells me what the shape of the transform is in the editor
    {
        switch (shape)
        {
            case colliderMenu.Box:
                if (col.GetType() != typeof(BoxCollider))//checks what collider is currently on node
                {
                    DestroyImmediate(col);//destroys previous collider
                    col = gameObject.AddComponent<BoxCollider>();//makes new one
                }
                break;

            case colliderMenu.Sphere:
                if (col.GetType() != typeof(SphereCollider))
                {
                    DestroyImmediate(col);
                    col = gameObject.AddComponent<SphereCollider>();
                }
                break;

            case colliderMenu.Cylinder:
                if (col.GetType() != typeof(CapsuleCollider))
                {
                    DestroyImmediate(col);
                    col = gameObject.AddComponent<CapsuleCollider>();//not sure how to change defaults from code, as it looks like a circle
                }
                break;

            default:
                break;
        }
    }

   

    private void SpawnObjectsInArea()
    {
        if (!Additive)
            DeleteObject(); //Delete previously placed objects

        if (goList.Count != 0)
        {
            switch (shape)
            {
                case colliderMenu.Box:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));//random x,y,z
                        InstantiateObject(spawnPos);
                    }

                    break;

                case colliderMenu.Sphere:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                        InstantiateObject(spawnPos);
                    }


                    break;

                case colliderMenu.Cylinder:

                    Debug.Log("Not in program yet");

                    break;

                default:
                    break;
            }
            
        }

        else
        {
            Debug.LogWarning("No Objects in List!");
        }
    }
    

    private void DeleteObject()
    {
        //destroy all objects spawned then clear the list
       for (int index = spawnedObjects.Count; index > 0; --index)
        {
            DestroyImmediate(spawnedObjects[index - 1]);
        }

        spawnedObjects.Clear();
    }

    public GameObject RandomObject()//temp
    {
        GameObject go;
        int objIndex;

        objIndex = Random.Range(0, goList.Count);
        go = goList[objIndex];

        return go;        
    }

    public void InstantiateObject(Vector3 loc)//instantiates object with given location
    {
        GameObject tempObj;
        RaycastHit hit;
        int breakLimit = 0;

        loc = transform.TransformPoint(loc * .5f); //takes transform in world space and modifies it using random value


        //system will use the location, then raycast down to place the object
        while (!Physics.Linecast(loc, Vector3.down * 100))//will check if cast goes through floor, and keep moving the position up until a solid ground is found
        {
            ++loc.y;
            ++breakLimit;

            if (breakLimit > 25)//will break function if there is no ground
            {
                Debug.Log("No collider found. Object not instantiated.");
                return;
            }
        }


        if (Physics.Raycast(loc, Vector3.down, out hit))
        {
            GameObject toSpawn;
            toSpawn = RandomObject();

            tempObj = (GameObject)Instantiate(toSpawn, new Vector3(hit.point.x, hit.point.y + (toSpawn.transform.localScale.y * .5f), hit.point.z), Quaternion.identity);//instantiate objects on surface of raycast

            if (!nodeParent)
            {
                nodeParent = new GameObject("Clutter");
            }

            tempObj.name = toSpawn.name;
            tempObj.transform.parent = nodeParent.transform;
            
            spawnedObjects.Add(tempObj);            
        }
    }
}