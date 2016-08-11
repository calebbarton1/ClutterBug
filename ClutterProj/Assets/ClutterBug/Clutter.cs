using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;


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

    [InspectorButton("DeleteClutter")]
    public bool DeleteObjects;

    [Space(10)]


    //initialise enum and colliders
    [Tooltip("The shape of the area where objects are placed")]
    public colliderMenu shape = colliderMenu.Box;

    [Tooltip("Adds clutter per click instead of rerolling")]
    public bool additive = false;

    [Tooltip("If enabled, clutter can overlap each other.")]
    public bool allowOverlap = false;

    [Space(10)]

    [Tooltip("Object will face surface normal. Overrides all rotation (currently)")]
    public bool faceNormal = false;

    [Tooltip("If the collider's angle is less than or equal to this value, the clutter wont spawn.")]
    [Range(0,89)]
    public int angleLimit = 45;

    public float distance;

    [Tooltip("Randomised rotation value. Is overriden by rotation override.")]
    [Header("Randomise Rotation")]
    public bool objX;
    public bool objY, objZ;

    [Space(10)]
    
    [Tooltip("Overrides prefab rotation and random rotation. Leave at zero to use prefab setting.")]
    public Vector3 rotationOverride;

    [Space(10)]

    //temporary. will build custom inspector later for two varaible inputs
    [Tooltip("Will scale objects between two variables.")]
    public Vector2 randomScale;

    [Space(10)]

    [Tooltip("Overrides prefab scale. Leave at zero to use prefab setting.")]
    public Vector3 scaleOverride = Vector3.zero;

    [Space(10)]

    [Tooltip("Number of clutter created per click")]
    public int numberToSpawn;

    [Space(5)]

    [Tooltip("Objects to be created as clutter")]
    public List<GameObject> goList;//temp
    

    private GameObject nodeParent;


    public void Awake ()
    {
        //so that the parent is remembered if the game is run.
        //nodeParent = GameObject.Find("clutterParent");
    }

    public void OnDrawGizmos()//currently just tells me what the shape of the transform is in the editor
    {
        Gizmos.color = new Color(0.50f, 1.0f, 1.0f, 0.5f);
        switch (shape)
        {
            case colliderMenu.Box:
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, transform.localScale);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Gizmos.color = new Color(0, 0, 0, .75f);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                Gizmos.matrix = Matrix4x4.identity;
                break;

            case colliderMenu.Sphere:
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, transform.localScale);
                Gizmos.DrawSphere(Vector3.zero, 1);
                Gizmos.color = new Color(0, 0, 0, .75f);
                Gizmos.DrawWireSphere(Vector3.zero, 1);
                Gizmos.matrix = Matrix4x4.identity;
                break;

            default:
                break;
        }
    }

   

    private void SpawnObjectsInArea()
    {
        if (!additive)
            DeleteClutter(); //Delete previously placed objects

        if (goList.Count != 0)
        {
            switch (shape)
            {
                case colliderMenu.Box:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));//random x and z on top of box
                        InstantiateObject(spawnPos,.45f);
                    }

                    break;

                case colliderMenu.Sphere:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                        spawnPos.y = 1;
                        InstantiateObject(spawnPos,1);
                    }

                    break;

                default:
                    break;
            }            
        }

        else
        {
            Debug.LogWarning("No Objects in List!");
            return;
        }
    }
    

    private void DeleteClutter()
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

    public void InstantiateObject(Vector3 _loc, float mult)//instantiates object with given location
    {        
        //gets random object
        GameObject toSpawn;
        toSpawn = RandomObject(); 

        _loc = transform.TransformPoint(_loc * mult); //takes transform in world space and modifies it using random value

        GameObject tempObj;
        tempObj = (GameObject)Instantiate(toSpawn, new Vector3(1000, 1000, 1000), Quaternion.identity);//get object into world

        tempObj = SetTransform(tempObj);

        tempObj.name = toSpawn.name;

        RaycastHit hit;
        bool cast;

        if (allowOverlap)
        {
            int mask = LayerMask.NameToLayer("Clutter");//grab layer of clutter
            mask = 1 << mask;//bitshift it
            mask = ~mask;//we want to cast against everything else but the clutter

            cast = Physics.SphereCast(_loc, (tempObj.transform.localScale.x * .5f), Vector3.down, out hit, transform.localScale.y, mask);//the new vector is so the spherecast doesn't start inside of a clutter in the case of very large objects
        }


        else
            cast = Physics.SphereCast(_loc, (tempObj.transform.localScale.x * .5f), Vector3.down, out hit, transform.localScale.y);


        //if raycast doesn't hit anything break out of function.
        if (!cast)
        {
            Debug.Log("Collider not found in bounds of node. Object " + tempObj.name + " not instantiated");
            DestroyImmediate(tempObj);
            return;
        }

        //do the thing
        else
        {
           
            if (Vector3.Angle(Vector3.down, hit.normal) <= (180 - angleLimit))//determines if an object will spawn depending on the angle of the collider below it. Set by user.
            {
                Debug.Log("Angle too sharp. Object " + tempObj.name + " not instantiated");
                DestroyImmediate(tempObj);
                return;
            }

            
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Clutter") && !allowOverlap)//if the user chooses, objects will not overlap
            {
                Debug.Log("Clutter in the way. Object " + tempObj.name + " not instantiated");
                DestroyImmediate(tempObj);
                return;
            }

            //move object to point
            tempObj.transform.position = hit.point;

            //offset the hit point so the object is on the surface
            Vector3 tempPos = tempObj.transform.position;
            tempPos.y += tempObj.transform.localScale.y * .5f;
            tempObj.transform.position = tempPos;
            
            if (faceNormal)//temp. currently overrides all other rotation values
            {
                tempObj.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;//placeholder
            }
        }

        ClutterChild child = tempObj.GetComponent<ClutterChild>();

        //if the child has a clutter child script on it, then instantiate more clutter.
        if (child != null) { }
            child.SpawnObjectsInArea(tempObj.transform, 1);
    }


    public GameObject SetTransform(GameObject go)
    {
        if (!nodeParent)
            nodeParent = new GameObject("clutterParent");

        //add new object to an empty parent
        go.transform.parent = nodeParent.transform;

        //set the rotation of the object if there is an override
        Vector3 tempRot = SetRotation();
        go.transform.rotation = Quaternion.Euler(tempRot);

        //set the scale of the object
        go.transform.localScale = SetScale(go);
        
        return go;
    }

    public Vector3 SetRotation()
    {
        Vector3 toReturn = new Vector3(0, 0, 0);


        if (rotationOverride != Vector3.zero) //if there is a value in rot override                
        {
            //x
            if (rotationOverride.x == 0 && objX)//if x is 0 and the bool is checked, return a random value
                toReturn.x = Random.Range(0, 360);

            else if (rotationOverride.x != 0) //otherwise use given value
                toReturn.x = rotationOverride.x;

            //y
            if (rotationOverride.y == 0 && objY)
                toReturn.y = Random.Range(0, 360);

            else if (rotationOverride.y != 0)
                toReturn.y = rotationOverride.y;

            //z
            if (rotationOverride.z == 0 && objZ)
                toReturn.z = Random.Range(0, 360);

            else if (rotationOverride.z != 0)
                toReturn.z = rotationOverride.z;


            return toReturn;
        }

        else//if the override is zero, just use the random bools
        {
            if (objX)
                toReturn.x = Random.Range(0, 360);

            if (objY)
                toReturn.y = Random.Range(0, 360);

            if (objZ)
                toReturn.z = Random.Range(0, 360);

            return toReturn;
        }

    }

    public Vector3 SetScale(GameObject go2)
    {
        Vector3 toReturn = new Vector3(0,0,0);

        if (scaleOverride == Vector3.zero && randomScale != Vector2.zero)
        {
            float rand = Random.Range(randomScale.x, randomScale.y + 1);
            toReturn = go2.transform.localScale * rand;

            return toReturn;
        }

        else
        {
            if (scaleOverride.x != 0)
                toReturn.x = scaleOverride.x;

            else
                toReturn.x = go2.transform.localScale.x;

            if (scaleOverride.y != 0)
                toReturn.y = scaleOverride.y;

            else
                toReturn.y = go2.transform.localScale.y;

            if (scaleOverride.z != 0)
                toReturn.z = scaleOverride.z;

            else
                toReturn.z = go2.transform.localScale.z;

            return toReturn;
        }
    }
}
#endif