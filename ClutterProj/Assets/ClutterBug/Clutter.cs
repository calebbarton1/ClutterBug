using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;


[ExecuteInEditMode]
public class Clutter : MonoBehaviour
{
    //Made by Caleb

    [Tooltip("Will display console alerts to prefabs not instantiated. May cause performance drops.")]
    public bool debug = false;

    [Space(10)]

    [Tooltip("If enabled, clutter can overlap each other.")]
    public bool allowOverlap = false;


    [Tooltip("Object will face surface normal")]
    public bool faceNormal = false;

    [Tooltip("If the collider's angle is less than this value, the clutter wont spawn.")]
    [Range(0, 89)]
    public int angleLimit = 45;

    [Tooltip("Randomised rotation value between a Min and Max. Is overriden by rotation override.")]
    [Header("Randomise Rotation")]
    public Vector2 rotX;
    [Tooltip("Randomised rotation value between a Min and Max. Is overriden by rotation override.")]
    public Vector2 rotY, rotZ;

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
    public int numberToSpawn = 1;

    [Space(5)]

    [Tooltip("Objects to be created as clutter")]
    public List<GameObject> prefabList;


    public GameObject RandomObject()
    {
        GameObject go;
        int objIndex;

        objIndex = Random.Range(0, prefabList.Count);
        go = prefabList[objIndex];

        return go;
    }

    public void InstantiateObject(Vector3 _loc, float _mult, float _dist, Transform toParent)//instantiates object with given location
    {
        //gets random object
        GameObject toSpawn;
        toSpawn = RandomObject();

        _loc = transform.TransformPoint(_loc * _mult * _dist); //takes local transform into world space and modifies it using random value     

        GameObject tempObj;
        tempObj = Instantiate(toSpawn, new Vector3(1000, 1000, 1000), Quaternion.identity) as GameObject;//get object into world

        //modify the object as needed
        tempObj = SetTransform(tempObj);
        tempObj.transform.parent = toParent;
        tempObj.name = toSpawn.name;

        //get collider info
        Mesh mesh = tempObj.GetComponent<MeshFilter>().sharedMesh;
        Rigidbody rb = tempObj.GetComponent<Rigidbody>();

        //RB Sweep can't use concave mesh colliders, so this checks for those
        MeshCollider col = tempObj.GetComponent<MeshCollider>();
        bool conv = true;

        if (col != null)
            conv = col.convex;


        //setting up info
        RaycastHit hit;
        bool cast;

        //if there us a rigidbody, use RBSweep
        //RBSweep can't mask, so we spherecast if overlap is on
        if (rb != null && !allowOverlap && conv)
        {
            if (debug)
                Debug.Log("Using RBSweep");

            //move the object so it can cast
            tempObj.transform.position = _loc;

            cast = rb.SweepTest(-transform.up, out hit, Mathf.Infinity);
        }

        //otherwise spherecast it
        else if (mesh != null)
        {
            if (debug)
                Debug.Log("Using casting");
            //make spherecast size the largest of the mesh
            float sphereSize;

            if (mesh.bounds.extents.x > mesh.bounds.extents.z)
                sphereSize = mesh.bounds.extents.x;

            else
                sphereSize = mesh.bounds.extents.z;

            if (allowOverlap)
            {
                int mask = LayerMask.NameToLayer("Clutter");//grab layer of clutter
                mask = 1 << mask;//bitshift it
                mask = ~mask;//we want to cast against everything else but the clutter

                cast = Physics.SphereCast(_loc, sphereSize, -transform.up, out hit, Mathf.Infinity, mask);
            }


            else
                cast = Physics.SphereCast(_loc, sphereSize, -transform.up, out hit, Mathf.Infinity);
        }


        else
        {
            Debug.LogWarning("Could not find a Rigidbody or a Meshfilter on " + tempObj.name + ". ClutterBug requires either of these components to function.");
            return;
        }

        //if cast doesn't hit anything break out of function.
        if (!cast)
        {
            if (debug)
                Debug.Log("Collider not found in bounds of node. Object " + tempObj.name + " not instantiated");
            DestroyImmediate(tempObj);
            return;
        }


        else
        {
            if (Vector3.Angle(-transform.up, hit.normal) < (180 - angleLimit))//determines if an object will spawn depending on the angle of the collider below it.
            {
                if (debug)
                    Debug.Log("Angle too sharp. Object " + tempObj.name + " not instantiated");
                DestroyImmediate(tempObj);
                return;
            }


            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Clutter") && !allowOverlap)//checking if another clutter is there
            {
                if (debug)
                    Debug.Log("Clutter in the way. Object " + tempObj.name + " not instantiated");
                DestroyImmediate(tempObj);
                return;
            }

            //Move object where it's supposed to            
            tempObj.transform.position = hit.point;
            //offset so it isn't in the ground
            tempObj.transform.position = Vector3.MoveTowards(tempObj.transform.position, _loc, tempObj.transform.lossyScale.y * .5f);

            if (faceNormal)
            {
                tempObj.transform.rotation = Quaternion.FromToRotation(tempObj.transform.up, hit.normal);
            }
        }


        NodeChild child = tempObj.GetComponent<NodeChild>();

        //if the child has a clutter child script on it, then instantiate more clutter.
        if (child != null)
            child.SpawnObjectsInArea();

    }


    public GameObject SetTransform(GameObject go)
    {
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
            if (rotationOverride.x == 0 && rotX != Vector2.zero)//if x is 0 and the bool is checked, return a random value
                toReturn.x = Random.Range(rotX.x, rotX.y);

            else if (rotationOverride.x != 0) //otherwise use given value
                toReturn.x = rotationOverride.x;

            //y
            if (rotationOverride.y == 0 && rotY != Vector2.zero)//if x is 0 and the bool is checked, return a random value
                toReturn.y = Random.Range(rotY.x, rotY.y);

            else if (rotationOverride.y != 0)
                toReturn.y = rotationOverride.y;

            //z
            if (rotationOverride.y == 0 && rotZ != Vector2.zero)//if x is 0 and the bool is checked, return a random value
                toReturn.y = Random.Range(rotZ.x, rotZ.y);

            else if (rotationOverride.z != 0)
                toReturn.z = rotationOverride.z;


            return toReturn;
        }

        else//if the override is zero, just use the random bools
        {
            if (rotX != Vector2.zero)
                toReturn.x = Random.Range(rotX.x, rotX.y);

            if (rotY != Vector2.zero)
                toReturn.y = Random.Range(rotY.x, rotY.y);

            if (rotZ != Vector2.zero)
                toReturn.z = Random.Range(rotZ.x, rotZ.y);

            return toReturn;
        }

    }

    public Vector3 SetScale(GameObject go2)
    {
        Vector3 toReturn = new Vector3(0, 0, 0);

        if (scaleOverride == Vector3.zero && randomScale != Vector2.zero)
        {
            float rand = Random.Range(randomScale.x, randomScale.y);
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