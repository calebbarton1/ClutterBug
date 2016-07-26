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
    public colliderMenu shape = colliderMenu.Box;
    Collider col;

    public GameObject go;//temp

    public int numberToSpawn;

    [HideInInspector]
    public List<Object> spawnedObjects;




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


    public void ShapeColliders()//currently just tells me what the shape of the transform is
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
        DeleteObject(); //Delete previously placed objects

        switch (shape)
        {
            case colliderMenu.Box:

                for (int index = 0; index < numberToSpawn; ++index)
                {
                    Object tempObj;
                    Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));//random x,y,z
                    spawnPos = transform.TransformPoint(spawnPos * .5f); //takes transform in world space and modifies it


                    RaycastHit hit;
                    int breakLimit = 0;

                    while (!Physics.Linecast(spawnPos, Vector3.down * 100))//will check if cast goes through floor, and keep moving the position up until a solid ground is found
                    {
                        ++spawnPos.y;
                        ++breakLimit;

                        if (breakLimit > 25)//will stop function if there is no ground at all
                        {
                            Debug.LogWarning("No collider found. Object not instantiated.");
                            return;
                        }
                    }                       

                    if (Physics.Raycast(spawnPos, Vector3.down, out hit))//raycast down from random location to the floor
                    {
                        tempObj = Instantiate(go, new Vector3(hit.point.x, hit.point.y + (go.transform.localScale.y * .5f), hit.point.z), Quaternion.identity);//instantiate objects on surface of raycast
                        spawnedObjects.Add(tempObj);//add to an invisible list so we can delete them if need be
                    }
                }

                break;

            case colliderMenu.Sphere:

                for (int index = 0; index < numberToSpawn; ++index)
                {
                    Object tempObj;
                    Vector3 spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                    spawnPos = transform.TransformPoint(spawnPos * .5f); //takes transform in world space and modifies it using random value


                    RaycastHit hit;
                    int breakLimit = 0;

                    while (!Physics.Linecast(spawnPos, Vector3.down * 100))//will check if cast goes through floor, and keep moving the position up until a solid ground is found
                    {
                        ++spawnPos.y;
                        ++breakLimit;

                        if (breakLimit > 100)
                        {
                            Debug.LogWarning("No collider found. Object not instantiated.");
                            return;
                        }
                    }


                    if (Physics.Raycast(spawnPos, Vector3.down, out hit))
                    {
                        tempObj = Instantiate(go, new Vector3(hit.point.x, hit.point.y + (go.transform.localScale.y * .5f), hit.point.z), Quaternion.identity);//instantiate objects on surface of raycast
                        spawnedObjects.Add(tempObj);
                    }
                }
                break;

            case colliderMenu.Cylinder:

                break;

            default:
                break;
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
    
}