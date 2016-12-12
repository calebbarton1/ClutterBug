// Copyright (C) 2016 Caleb Barton (caleb.barton@hotmail.com)
//github.com/calebbarton1
//Released under MIT License
//https://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[ExecuteInEditMode]
public class Clutter : MonoBehaviour
{
    public bool debug = false;

    public bool allowOverlap = false;

    public bool faceNormal = false;
    
    public float angleLimit = 45;
  
    public Vector2 rotX, rotY, rotZ;

    public Vector3 rotationOverride;

    public Vector2 randomScale;

    public Vector3 scaleOverride = Vector3.zero;

    public LayerMask clutterMask;

    public int numberToSpawn = 1;

    public List<GameObject> prefabList;
    public List<float> prefabWeights;

    public int RandomObject()
    {
        float currCount = 0;
        float totalWieght = 0;

        foreach (float weight in prefabWeights)
        {
            totalWieght += weight;//get total weight
        }
        
        float rand = Random.Range(0, totalWieght);

        for (int index = 0; index < prefabWeights.Count; ++index)
        {
            currCount += prefabWeights[index];

            if (currCount > rand)
            {
                return index;
            }
        }

        //Debug.Log("Using " + prefabList[0].name);
        return Random.Range(0,prefabList.Count-1);//otherwise return a pure random
    }

    public void InstantiateObject(Vector3 _loc, float _mult, float _dist, Transform toParent)//instantiates object with given location
    {
        //gets random object
        int toSpawn;
        toSpawn = RandomObject();

        _loc = transform.TransformPoint(_loc * _mult * _dist); //takes local transform into world space and modifies it using random value     

        GameObject tempObj;
        tempObj = Instantiate(prefabList[toSpawn], new Vector3(1000, 1000, 1000), Quaternion.identity) as GameObject;//get object into world

        //modify the object as needed
        tempObj = SetTransform(tempObj);
        tempObj.transform.parent = toParent;
        tempObj.name = prefabList[toSpawn].name;

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
                //ignore user layermasks if chosen
                cast = Physics.SphereCast(_loc, sphereSize, -transform.up, out hit, Mathf.Infinity, ~clutterMask);
            }


            else
            {
                cast = Physics.SphereCast(_loc, sphereSize, -transform.up, out hit, Mathf.Infinity);
            }
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

        //now check for other user variables
        else
        {
            float angle = Vector3.Angle(-transform.up, hit.normal);
            if (angle < (180 - angleLimit))//determines if an object will spawn depending on the angle of the collider below it.
            {
                if (debug)
                    Debug.Log("Angle is sharper than " + angleLimit + ". Object " + tempObj.name + " not instantiated");
                DestroyImmediate(tempObj);
                return;
            }



            if ((clutterMask.value & (1 << hit.collider.gameObject.layer)) != 0 && !allowOverlap)//checking if its allowed to collide with hit point
            {
                if (debug)
                    Debug.Log("Object with selected layermask in way. Object " + tempObj.name + " not instantiated");
                DestroyImmediate(tempObj);
                return;
            }

            //Move object where it's supposed to be and offset y position so it's not in collider           
            tempObj.transform.position = hit.point + (hit.normal * (tempObj.transform.lossyScale.y * .5f));

            if (faceNormal)
            {
                tempObj.transform.rotation = Quaternion.FromToRotation(tempObj.transform.up, hit.normal);
            }
        }


        //if the child has a clutter child script on it, then instantiate more from prefab.
        NodeChild child = tempObj.GetComponent<NodeChild>();
        if (child != null)
            child.SpawnObjectsInArea();

    }


    public GameObject SetTransform(GameObject go)
    {
        //set the rotation of the object if there is an override
        Vector3 tempRot = SetRotation();
        go.transform.rotation = Quaternion.Euler(tempRot);

        //set the scale of the object
        go.transform.localScale = SetScale(go.transform);

        return go;
    }

    public Vector3 SetRotation()
    {
        Vector3 toReturn = new Vector3(0, 0, 0);


        if (rotationOverride != Vector3.zero)//checks if there is an override, but will only override user input             
        {
            //x
            if (rotationOverride.x == 0 && rotX != Vector2.zero)//if there is no override, then use random value
                toReturn.x = Random.Range(rotX.x, rotX.y);

            else if (rotationOverride.x != 0)
                toReturn.x = rotationOverride.x;

            //y
            if (rotationOverride.y == 0 && rotY != Vector2.zero)
                toReturn.y = Random.Range(rotY.x, rotY.y);

            else if (rotationOverride.y != 0)
                toReturn.y = rotationOverride.y;

            //z
            if (rotationOverride.y == 0 && rotZ != Vector2.zero)
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

    public Vector3 SetScale(Transform go2)
    {
        Vector3 toReturn = new Vector3(0, 0, 0);

        if (scaleOverride == Vector3.zero && randomScale != Vector2.zero)
        {
            float rand = Random.Range(randomScale.x, randomScale.y);
            toReturn = go2.localScale * rand;

            return toReturn;
        }

        else
        {
            if (scaleOverride.x != 0)
                toReturn.x = scaleOverride.x;

            else
                toReturn.x = go2.localScale.x;

            if (scaleOverride.y != 0)
                toReturn.y = scaleOverride.y;

            else
                toReturn.y = go2.localScale.y;

            if (scaleOverride.z != 0)
                toReturn.z = scaleOverride.z;

            else
                toReturn.z = go2.localScale.z;

            return toReturn;
        }
    }
}
#endif