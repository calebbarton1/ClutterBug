using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class Clutter : MonoBehaviour {

    //Made by Caleb

    //enums for collider selection in inspector
    public enum colliderMenu
    {
        Box,
        Sphere,
    }

    //buttons for generating objects
    //credit to "zaikman" for the script

    [Space(10)]

    [InspectorButton("SpawnObjectsInArea")]//Calls this function
    public bool SpawnObjects;//makes a button with this bool

    [Space(10)]

    [InspectorButton("DeleteObject")]
    public bool DeleteObjects;

    [Space(10)]


    //initialise enum and colliders
    [Tooltip("The shape of the area where objects are placed")]
    public colliderMenu shape = colliderMenu.Box;

    [Tooltip("Adds clutter per click instead of rerolling")]
    public bool additive = false;

    public bool allowOverlap = false;

    [Tooltip("Number of clutter created per click")]
    public int numberToSpawn;

    [Tooltip("If the collider's angle is less than or equal to this value, the clutter wont spawn.")]
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
        if (!additive)
            DeleteObject(); //Delete previously placed objects

        if (goList.Count != 0)
        {
            switch (shape)
            {
                case colliderMenu.Box:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));//random x,y,z in a box
                        InstantiateObject(spawnPos);
                    }

                    break;

                case colliderMenu.Sphere:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                        spawnPos.y = 1;
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

        Renderer toSpawnRender = toSpawn.GetComponent<Renderer>();//caching render of prefab we want to spawn

        _loc = transform.TransformPoint(_loc * .45f); //takes transform in world space and modifies it using random value


        //system will use the location, then raycast down to place the object
        while (!Physics.Linecast(_loc, Vector3.down * 50))//will check if cast goes through floor, and keep moving the position up until a solid ground is found
        {
            _loc.y += 10;
            ++breakLimit;

            Debug.Log("moving object up");

            if (breakLimit > 5)//will break function if there is no ground
            {
                Debug.Log("No collider found. Object not instantiated.");
                return;
            }
        }


        RaycastHit hit;
        bool cast;       

        if (allowOverlap)        
            cast = Physics.SphereCast(_loc, toSpawnRender.bounds.size.x * .5f, Vector3.down, out hit, 50, 8);//ignore clutter in the casting when enabled. Allows clutter to overlap each other.

        else
            cast = Physics.SphereCast(_loc, toSpawnRender.bounds.size.x * .5f, Vector3.down, out hit, 50);


        if (cast)
        {
            if (Vector3.Angle(Vector3.down, hit.normal) <= (180 - degreeLimit))//determines if an object will spawn depending on the angle of the collider below it. Set by user.
            {
                Debug.Log("Angle too sharp. Object " + hit.collider.name + " not instantiated");
                return;
            }

            
            if (hit.collider.gameObject.layer == 8 && !allowOverlap)//if the user chooses, objects will not overlap
            {
                Debug.Log("Clutter in the way. Object " + hit.collider.name + " not instantiated");
                return;
            }

            GameObject tempObj;
            tempObj = (GameObject)Instantiate(toSpawn, new Vector3(hit.point.x, hit.point.y + (toSpawnRender.bounds.size.y * .5f), hit.point.z), Quaternion.identity);//instantiate objects on surface of raycast. The getcomponent is nasty, but I can't see a way around it.

            if (!nodeParent)
                nodeParent = new GameObject("clutterParent");

            tempObj.name = toSpawn.name;//get rid of (clone)
            tempObj.transform.parent = nodeParent.transform;
        }
    }
}