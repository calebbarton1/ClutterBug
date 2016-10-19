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
public class Clutter2D : MonoBehaviour
{
    //Made by Caleb Barton
    //github.com/calebbarton1

    public bool debug = false;
    public bool allowOverlap = false;

    public SortingLayer sortingLayer;

    public Vector2 randomOrderLayer;
    public int orderLayerOverride = 0;

    public Vector2 posX, posY;
    public Vector2 positionOverride;

    public Vector2 rotZ;
    public float rotationOverride;

    //temporary. will build custom inspector later for two varaible inputs
    public Vector2 randomScale;
    public Vector2 scaleOverride = Vector2.zero;

    public LayerMask clutterMask;

    public int numberToSpawn = 1;

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
        tempObj = Instantiate(toSpawn, new Vector2(1000, 1000), Quaternion.identity) as GameObject;//get object into world

        //modify the object as needed
        tempObj = SetTransform(tempObj);
        tempObj.transform.parent = toParent;
        tempObj.name = toSpawn.name;


        //get collider info
        SpriteRenderer mesh = tempObj.GetComponent<SpriteRenderer>();

        if (orderLayerOverride == 0)
        {
            mesh.sortingOrder = (int)Random.Range(randomOrderLayer.x, randomOrderLayer.y + 1);
        }

        else
            mesh.sortingOrder = orderLayerOverride;

        //check if another 2d collider is there       
        if (Physics2D.OverlapBox(_loc, mesh.bounds.size, 0, clutterMask) && !allowOverlap)
        {
            if (debug)
                Debug.Log("Object with selected layermask in way. Object " + tempObj.name + " not instantiated");

            DestroyImmediate(tempObj);
            return;
        }

        //Postion
        {
            _loc = toParent.position - _loc;//offset the location so its relative to parent

            if (positionOverride != Vector2.zero)
            {

                //x
                if (positionOverride.x == 0 && posX != Vector2.zero)
                {
                    _loc.x = Random.Range(posX.x, posX.y);
                }

                else if (positionOverride.x != 0)
                {
                    _loc.x = positionOverride.x;
                }

                //x
                if (positionOverride.y == 0 && posY != Vector2.zero)
                {
                    _loc.y = Random.Range(posY.x, posY.y);
                }

                else if (positionOverride.y != 0)
                {
                    _loc.y = positionOverride.y;
                }

                tempObj.transform.localPosition = _loc;
            }


            else
            {
                if (posX != Vector2.zero)
                    _loc.x = Random.Range(posX.x, posX.y);


                if (posY != Vector2.zero)
                    _loc.y = Random.Range(posY.x, posY.y);

                tempObj.transform.localPosition = _loc;
            }
        }



        //if the child has a clutter child script on it, then instantiate more from prefab.
        //TODO: Get 2D children working
        //Node2DChild child = tempObj.GetComponent<Node2DChild>();
        //if (child != null)
        //child.SpawnObjectsInArea();

    }


    public GameObject SetTransform(GameObject go)
    {
        //set the rotation of the object if there is an override
        float tempZ = SetRotation();
        Vector3 tempRot = new Vector3(0, 0, tempZ);

        go.transform.rotation = Quaternion.Euler(tempRot);

        //set the scale of the object
        go.transform.localScale = SetScale(go.transform);

        return go;
    }

    public float SetRotation()
    {
        float toReturn = 0;

        if (rotationOverride == 0)//if there is no override, then use random value
        {
            toReturn = Random.Range(rotZ.x, rotZ.y);
            return toReturn;
        }

        else
        {
            toReturn = rotationOverride;
            return toReturn;
        }
    }

    public Vector2 SetScale(Transform go2)
    {
        Vector2 toReturn = new Vector3(0, 0);

        if (scaleOverride == Vector2.zero && randomScale != Vector2.zero)
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

            return toReturn;
        }
    }
}
#endif