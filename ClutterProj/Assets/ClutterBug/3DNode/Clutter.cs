// Copyright (C) 2018 Caleb Barton (caleb.barton@hotmail.com)
//github.com/calebbarton1
//Released under MIT License
//https://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class Clutter : MonoBehaviour
{
    //Below variables have their own inspector descriptions
    #region Clutter Options
    //if debug logs print or not
    public bool debug = false;
    //clutter spawning inside each other
    public bool allowOverlap = false;
    //face normal of surface
    public bool faceNormal = false;
    //max limit of surface angle
    public float angleLimit = 45;
    //layermask of clutter
    public LayerMask clutterMask;
    //number of clutter to be spawned
    public int numberToSpawn = 1;
    public float distance = 1;

    //TESTING
    //uses mesh size to create spherecasts instead of the scale
    public bool useMesh = false;
    //offset the position by half the y scale
    public bool offsetPos;

    #region Transform Variables
    //the random range of each rotation value
    public Vector2 rotX, rotY, rotZ;
    //manually setting rotation
    public Vector3 rotationOverride;
    //random range of scale
    public Vector2 randomScale;
    //manually setting scale value
    public Vector3 scaleOverride = Vector3.zero;
    #endregion

    #region Prefab Lists
    //list of prefabs to be randomly selected
    public List<GameObject> prefabList;
    //wieght of each prefab spawn
    public List<float> prefabWeights;
    #endregion

    private GameObject m_clutterToSpawnInstance;
    #endregion

    /// <summary>
    /// Instantiates 
    /// </summary>
    /// <param name="newLocation">Random range within the clutter node</param>
    /// <param name="_mult">Multiplier for _loc to keep clutter within node. .5 or less for cubes, while 1 or less for spheres</param>
    /// <param name="_dist">Distance multiplier for children of prefabs to move away from parent</param>
    /// <param name="_toParent">Parent of instantiated clutter</param>
    protected void InstantiateClutter(Vector3 _loc, float _mult, float _dist, Transform _toParent)
    {
        #region Object Setup
        //gets random object
        int toSpawn = RandomObject();

        //takes local transform into world space and modifies it
        Vector3 newLocation = transform.TransformPoint(_loc * _mult * _dist);

        m_clutterToSpawnInstance = Instantiate(prefabList[toSpawn], new Vector3(), Quaternion.identity) as GameObject;//get object into world

        //modify the object as needed
        SetRotation();
        SetScale();
        m_clutterToSpawnInstance.transform.parent = _toParent;
        m_clutterToSpawnInstance.name = prefabList[toSpawn].name;
        #endregion

        #region Mesh and Rigidbody getting
        //get collider info
        Mesh mesh = m_clutterToSpawnInstance.GetComponent<MeshFilter>().sharedMesh;
        Rigidbody rb = m_clutterToSpawnInstance.GetComponent<Rigidbody>();
        #endregion

        #region Raycasting
        //setting up info
        RaycastHit hit;
        bool cast;

        //if there us a rigidbody, use RBSweep
        //RBSweep can't mask, so we spherecast if overlap is on
        if (rb != null && !allowOverlap)
        {
            if (debug)
                Debug.Log("Using RBSweep");

            //move the object so it can cast
            m_clutterToSpawnInstance.transform.position = newLocation;

            cast = rb.SweepTest(-transform.up, out hit, Mathf.Infinity);
        }

        //otherwise spherecast it
        else if (mesh != null)
        {
            #region Size of Cast
            if (debug)
                Debug.Log("Using casting");
            //make spherecast size the largest of the mesh.
            float sphereSize;

            if (useMesh)
            {
                if (mesh.bounds.extents.x > mesh.bounds.extents.z)
                    sphereSize = mesh.bounds.extents.x;

                else
                    sphereSize = mesh.bounds.extents.z;
            }

            else
            {
                if (m_clutterToSpawnInstance.transform.lossyScale.x > m_clutterToSpawnInstance.transform.lossyScale.z)
                    sphereSize = m_clutterToSpawnInstance.transform.lossyScale.x;

                else
                    sphereSize = m_clutterToSpawnInstance.transform.lossyScale.z;
            }
            #endregion

            if (allowOverlap)
            {
                //ignore user layermasks if chosen
                cast = Physics.SphereCast(newLocation, sphereSize, -transform.up, out hit, Mathf.Infinity, ~clutterMask);
            }
            else
            {
                cast = Physics.SphereCast(newLocation, sphereSize, -transform.up, out hit, Mathf.Infinity);
            }
        }

        else
        {
            Debug.LogWarning("Could not find a Rigidbody or a Meshfilter on " + m_clutterToSpawnInstance.name + ". ClutterBug requires either of these components to function.");
            return;
        }
        #endregion

        #region Moving Clutter to Raycast
        //if cast doesn't hit anything break out of function.
        if (cast)
        {
            float angle = Vector3.Angle(-transform.up, hit.normal);
            if (angle < (180 - angleLimit))//determines if an object will spawn depending on the angle of the collider below it.
            {
                if (debug)
                    Debug.Log("Angle is sharper than " + angleLimit + ". Object " + m_clutterToSpawnInstance.name + " not instantiated");
                DestroyImmediate(m_clutterToSpawnInstance);
                return;
            }

            if ((clutterMask.value & (1 << hit.collider.gameObject.layer)) != 0 && !allowOverlap)//checking if its allowed to collide with hit point
            {
                if (debug)
                    Debug.Log("Object with selected layermask in way. Object " + m_clutterToSpawnInstance.name + " not instantiated");
                DestroyImmediate(m_clutterToSpawnInstance);
                return;
            }

            if (offsetPos)
            {
                //TODO Figure out how to make this less shit
                if (useMesh)
                {
                    if (mesh == null)
                    {
                        if (debug)
                            Debug.LogWarning("Cannot use Mesh Scaling without a Meshfilter. Swapping to global transform scaling.");

                        useMesh = false;
                    }

                    else
                        m_clutterToSpawnInstance.transform.position = hit.point + (hit.normal * (mesh.bounds.extents.y));//else use the mesh size                
                }

                else
                    m_clutterToSpawnInstance.transform.position = hit.point + (hit.normal * (m_clutterToSpawnInstance.transform.lossyScale.y * .5f));//Move object where it's supposed to be and offset y position so it's not in collider using global  
            }

            else
                m_clutterToSpawnInstance.transform.position = hit.point;//otherwise just move straight to the point

            if (faceNormal)
            {
                m_clutterToSpawnInstance.transform.rotation = Quaternion.FromToRotation(m_clutterToSpawnInstance.transform.up, hit.normal);
            }
        }

        else
        {
            if (debug)
                Debug.Log("Collider not found in bounds of node. Object " + m_clutterToSpawnInstance.name + " not instantiated");
            DestroyImmediate(m_clutterToSpawnInstance);
            return;
        }
        #endregion

        #region Activate Child Node
        //if the child has a clutter child script on it, then instantiate more from prefab.
        Node child = m_clutterToSpawnInstance.GetComponent<Node>();
        if (child != null)
            child.StartClutterSpawn(true);
        #endregion
    }

    /// <summary>
    /// Sets Rotation of spawned clutter
    /// </summary>
    private void SetRotation()
    {
        Vector3 newRotation = new Vector3(0, 0, 0);

        if (rotationOverride != Vector3.zero)//checks if there is an override, but will only override user input             
        {
            //x
            if (rotationOverride.x == 0 && rotX != Vector2.zero)//if there is no override, then use random value
                newRotation.x = Random.Range(rotX.x, rotX.y);

            else if (rotationOverride.x != 0)
                newRotation.x = rotationOverride.x;

            //y
            if (rotationOverride.y == 0 && rotY != Vector2.zero)
                newRotation.y = Random.Range(rotY.x, rotY.y);

            else if (rotationOverride.y != 0)
                newRotation.y = rotationOverride.y;

            //z
            if (rotationOverride.y == 0 && rotZ != Vector2.zero)
                newRotation.y = Random.Range(rotZ.x, rotZ.y);

            else if (rotationOverride.z != 0)
                newRotation.z = rotationOverride.z;
        }

        else//if the override is zero, just use the random bools
        {
            if (rotX != Vector2.zero)
                newRotation.x = Random.Range(rotX.x, rotX.y);

            if (rotY != Vector2.zero)
                newRotation.y = Random.Range(rotY.x, rotY.y);

            if (rotZ != Vector2.zero)
                newRotation.z = Random.Range(rotZ.x, rotZ.y);
        }

        m_clutterToSpawnInstance.transform.rotation = Quaternion.Euler(newRotation);
    }

    /// <summary>
    /// Sets scale of spawned clutter
    /// </summary>
    private void SetScale()
    {
        Vector3 newScale = new Vector3(0, 0, 0);
        Vector3 currScale = m_clutterToSpawnInstance.transform.localScale;

        if (scaleOverride == Vector3.zero && randomScale != Vector2.zero)
        {
            float rand = Random.Range(randomScale.x, randomScale.y);
            newScale = currScale * rand;
        }

        else
        {
            if (scaleOverride.x != 0)
                newScale.x = scaleOverride.x;

            else
                newScale.x = currScale.x;

            if (scaleOverride.y != 0)
                newScale.y = scaleOverride.y;

            else
                newScale.y = currScale.y;

            if (scaleOverride.z != 0)
                newScale.z = scaleOverride.z;

            else
                newScale.z = currScale.z;
        }

        m_clutterToSpawnInstance.transform.localScale = newScale;
    }

    /// <summary>
    /// Generates a random number for choosing prefabs
    /// </summary>
    /// <returns>Index of gameobject to be returned</returns>
    private int RandomObject()
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

        Debug.LogWarning("Function Shouldn't have returned here.");
        return Random.Range(0, prefabList.Count - 1);//otherwise return a pure random
    }
}
#endif