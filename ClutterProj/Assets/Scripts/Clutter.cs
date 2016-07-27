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

    [Tooltip("Adds clutter per click instead of rerolling")]
    public bool Additive = false;

    [Tooltip("Number of clutter created")]
    public int numberToSpawn;

    [Tooltip("The angle of limit.\nIf a slope is less than or equal this value, clutter isn't spawned.")]
    public int degreeLimit = 45;

    [Space(5)]

    [Tooltip("Objects to be created as clutter")]
    public List<GameObject> goList;//temp
    

    private GameObject nodeParent;

    void Start() {

    }

    // Update is called once per frame
   // void Update()
   // {
   //     ShapeColliders();
   // }


    public void OnDrawGizmos()//currently just tells me what the shape of the transform is in the editor
    {
        Gizmos.color = new Color(0.50f, 1.0f, 1.0f, 0.5f);
        switch (shape)
        {
            case colliderMenu.Box:
                Gizmos.DrawCube(transform.position, transform.localScale);
                Gizmos.color = new Color(0, 0, 0, .75f);
                Gizmos.DrawWireCube(transform.position, transform.localScale);
                break;

            case colliderMenu.Sphere:
                Gizmos.DrawSphere(transform.position, transform.localScale.x * .5f);
                Gizmos.color = new Color(0, 0, 0, .75f);
                Gizmos.DrawWireSphere(transform.position, transform.localScale.x * .5f);
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
                        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));//random x,y,z in a box
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
        DestroyImmediate(nodeParent);
    }

    public GameObject RandomObject()//temp
    {
        GameObject go;
        int objIndex;

        objIndex = Random.Range(0, goList.Count);
        go = goList[objIndex];

        return go;        
    }

    public void InstantiateObject(Vector3 _loc)//instantiates object with given location
    {        
        int breakLimit = 0;

        //gets random object
        GameObject toSpawn;
        toSpawn = RandomObject();

        Renderer objSize = toSpawn.GetComponent<Renderer>();//caching render of prefab we want to spawn

        _loc = transform.TransformPoint(_loc * .45f); //takes transform in world space and modifies it using random value


        //system will use the location, then raycast down to place the object
        while (!Physics.Raycast(_loc, Vector3.down))//will check if cast goes through floor, and keep moving the position up until a solid ground is found
        {
            ++_loc.y;
            ++breakLimit;

            if (breakLimit > transform.localScale.y)//will break function if there is no ground
            {
                Debug.Log("No collider found. Object not instantiated.");
                return;
            }
        }


        RaycastHit hit;

        if (Physics.Raycast(_loc, Vector3.down, out hit))
        {
            if (Vector3.Angle(Vector3.down,hit.normal) <= (180 - degreeLimit))
            {
                Debug.Log("Angle too sharp. Object not instantiated");
                return;
            }

            RaycastHit hit2;
            if (Physics.SphereCast(_loc, objSize.bounds.size.x * .75f, Vector3.down, out hit2, transform.localScale.y))//lots of casting, but it works
            {
                if (hit2.collider.tag == "Clutter")
                {
                    Debug.Log("shits in way, not doin it");
                    return;
                }
            }            

            GameObject tempObj;
            tempObj = (GameObject)Instantiate(toSpawn, new Vector3(hit.point.x, hit.point.y + (objSize.bounds.size.y * .5f), hit.point.z), Quaternion.identity);//instantiate objects on surface of raycast. The getcomponent is nasty, but I can't see a way around it.

            if (!nodeParent)
                nodeParent = new GameObject("clutterParent");

            tempObj.name = toSpawn.name;
            tempObj.transform.parent = nodeParent.transform;  
        }
    }
}