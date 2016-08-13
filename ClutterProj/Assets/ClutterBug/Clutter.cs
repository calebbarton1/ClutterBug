using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;


[ExecuteInEditMode]
public class Clutter : MonoBehaviour {
    

    //Made by Caleb
       
    [Tooltip("Adds clutter per click instead of rerolling")]
    public bool additive = false;

    [Tooltip("If enabled, clutter can overlap each other.")]
    public bool allowOverlap = false;
    

    [Tooltip("Object will face surface normal. Overrides all rotation (currently)")]
    public bool faceNormal = false;

    [Tooltip("If the collider's angle is less than or equal to this value, the clutter wont spawn.")]
    [Range(0,89)]
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
    public Vector2 randomScale;

    [Space(10)]

    [Tooltip("Overrides prefab scale. Leave at zero to use prefab setting.")]
    public Vector3 scaleOverride = Vector3.zero;

    [Space(10)]

    [Tooltip("Number of clutter created per click")]
    public int numberToSpawn = 1;

    [Space(5)]

    [Tooltip("Objects to be created as clutter")]
    public List<GameObject> prefabList;//temp    
    

    public GameObject RandomObject()//temp
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
        tempObj = (GameObject)Instantiate(toSpawn, new Vector3(1000, 1000, 1000), Quaternion.identity);//get object into world

        tempObj = SetTransform(tempObj);

        tempObj.transform.parent = toParent;
        tempObj.name = toSpawn.name;

        Collider col = tempObj.GetComponent<Collider>();

        RaycastHit hit;
        bool cast;

        float sphereSize;

        if (col.bounds.size.x > col.bounds.size.z)
            sphereSize = col.bounds.size.x;

        else
            sphereSize = col.bounds.size.z;

        if (allowOverlap)
        {
            int mask = LayerMask.NameToLayer("Clutter");//grab layer of clutter
            mask = 1 << mask;//bitshift it
            mask = ~mask;//we want to cast against everything else but the clutter

            cast = Physics.SphereCast(_loc, (sphereSize * .5f), Vector3.down, out hit, Mathf.Infinity, mask);
        }


        else
            cast = Physics.SphereCast(_loc, (sphereSize * .5f), Vector3.down, out hit, Mathf.Infinity);


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