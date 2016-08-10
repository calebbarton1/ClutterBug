﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class ClutterChild : MonoBehaviour {

#if UNITY_EDITOR

    [Tooltip("If enabled, clutter can overlap each other.")]
    public bool allowOverlap = false;
    
    [Tooltip("Object will face surface normal. Overrides all rotation (currently)")]
    public bool faceNormal = false;

    [Tooltip("Distance from the clutter to spawn")]
    public float dist;

    [Tooltip("If the collider's angle is less than or equal to this value, the clutter wont spawn.")]
    [Range(0, 89)]
    public int angleLimit = 45;

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
    [Header("Random Scale")]
    public Vector2 scaleX;
    public Vector2 scaleY, scaleZ;

    [Space(10)]

    [Tooltip("Overrides prefab scale. Leave at zero to use prefab setting.")]
    public Vector3 scaleOverride = Vector3.zero;

    [Space(10)]

    [Tooltip("Number of clutter created per click")]
    public int numberToSpawn;

    [Space(5)]

    [Tooltip("Objects to spawn around clutter")]
    public List<GameObject> childClutterList;

    [HideInInspector]
    public Transform parent;

    private int recursiveCounter;

    public void Recursive(int count)
    {
        recursiveCounter += count; 
    }

    public void SpawnObjectsInArea(Transform toParent)
    {
        if (recursiveCounter != 50)
        {
            parent = toParent;

            if (childClutterList.Count != 0)
            {
                for (int index = 0; index < numberToSpawn; ++index)
                {
                    Vector3 spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                    spawnPos.y = 1;
                    InstantiateObject(spawnPos, 1);
                }
            }
        }

        else
            Debug.LogWarning("Too many recursive loops. Breaking Function.");
    }

    public GameObject RandomObject()//temp
    {
        GameObject go;
        int objIndex;

        objIndex = Random.Range(0, childClutterList.Count);
        go = childClutterList[objIndex];

        return go;
    }

    public void InstantiateObject(Vector3 _loc, float mult)//instantiates object with given location
    {
        //gets random object
        GameObject toSpawn;
        toSpawn = RandomObject();

        _loc = transform.TransformPoint(_loc * mult * dist); //takes transform in world space and modifies it using random value

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

            cast = Physics.SphereCast(_loc, (tempObj.transform.localScale.x * .5f), Vector3.down, out hit, 20, mask);//the new vector is so the spherecast doesn't start inside of a clutter in the case of very large objects
        }


        else
            cast = Physics.SphereCast(_loc, (tempObj.transform.localScale.x * .5f), Vector3.down, out hit, 20);


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

            if (faceNormal)
            {
                tempObj.transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;//placeholder
            }
        }

        //if the child has a clutter child script on it, then instantiate more clutter.
        if (tempObj.GetComponent<ClutterChild>() != null)
        {
            tempObj.SendMessage("Recursive", 1);
            tempObj.SendMessage("SpawnObjectsInArea", this.transform);
        }
    }


    public GameObject SetTransform(GameObject go)
    {
        //add new object to an empty parent
        go.transform.parent = parent;

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
        Vector3 toReturn = new Vector3(0, 0, 0);


        if (scaleOverride != Vector3.zero) //if there is a value in rot override                
        {
            //x
            if (scaleOverride.x == 0 && scaleX != Vector2.zero)//if x is 0 and the scale has value, return a random value
                toReturn.x = Random.Range(scaleX.x, scaleX.y);

            else if (scaleOverride.x != 0) //otherwise use given value
                toReturn.x = scaleOverride.x;

            //y
            if (scaleOverride.y == 0 && scaleY != Vector2.zero)
                toReturn.y = Random.Range(scaleY.x, scaleY.y);

            else if (scaleOverride.y != 0)
                toReturn.y = scaleOverride.y;

            //z
            if (scaleOverride.z == 0 && scaleZ != Vector2.zero)
                toReturn.z = Random.Range(scaleZ.x, scaleZ.y);

            else if (scaleOverride.z != 0)
                toReturn.z = scaleOverride.z;

            return toReturn;
        }

        else //if the override is zero, check if random has any input. Otherwise use prefab data
        {
            if (scaleX != Vector2.zero)
                toReturn.x = Random.Range(scaleX.x, scaleX.y);

            else toReturn.x = go2.transform.localScale.x;

            if (scaleY != Vector2.zero)
                toReturn.y = Random.Range(scaleY.x, scaleY.y);

            else toReturn.y = go2.transform.localScale.y;

            if (scaleZ != Vector2.zero)
                toReturn.z = Random.Range(scaleZ.x, scaleZ.y);

            else toReturn.z = go2.transform.localScale.z;

            return toReturn;
        }
    }
#endif
}
