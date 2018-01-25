// Copyright (C) 2018 Caleb Barton (caleb.barton@hotmail.com)
//github.com/calebbarton1
//Released under MIT License
//https://opensource.org/licenses/MIT

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
/// <summary>
/// The class that is on the node. It inherits from clutter to share clutter spawning with other classes
/// </summary>
[ExecuteInEditMode]
public class Node : Clutter
{
    #region Clutter Options
    //if creating new clutter deletes the instantiated clutter
    public bool additive = false;
    //stop clutter moving along an axis
    public bool lockX, lockZ;    
    //initialise enum and colliders
    public colliderMenu shape = colliderMenu.Box;
    #endregion

    #region Member Variables
    //enums for collider selection in inspector
    public enum colliderMenu
    {
        Box,
        Sphere,
    }
    //parent clutter will be spawned under
    public GameObject m_clutterParent;
    public bool m_IsChild = false;
    #endregion

    /// <summary>
    /// Draw sphere and cube in inspector
    /// </summary>
    private void OnDrawGizmos()
    {
        //TODO: Colour Options
        if (!m_IsChild)
        {
            Gizmos.color = new Color(0.50f, 1.0f, 1.0f, 0.5f);
            switch (shape)
            {
                case colliderMenu.Box:
                    Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, transform.localScale);//making a matrix based on the transform, draw shape based on it
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
            }
        }
    }

    //Deletes clutterparent, which quickly deletes all clutter
    public void DeleteClutter()
    {
        //We destroy the parent because deleting individual objects would be very slow
        DestroyImmediate(m_clutterParent);
    }

    /// <summary>
    /// Spawns clutter within the area of the node
    /// </summary>
    public void StartClutterSpawn(bool _child)
    {
        //disable gizmo for children
        m_IsChild = _child;

        if (!additive  && !m_IsChild)
            DeleteClutter(); //Delete previously placed objects

        if (!m_clutterParent && !m_IsChild)
            m_clutterParent = new GameObject("clutterParent - " + gameObject.name);

        CheckNegativeScale();

        if (prefabList.Count != 0)
        {
            try
            {
                SpawnClutter(_child);
            }
            catch(System.Exception ex)
            {
                Debug.LogErrorFormat("Exception in starting clutter creation. Message: {0}", ex.Message);
            }            
        }

        else
        {
            Debug.LogWarning("Node has no prefabs in List!");
            return;
        }
    }

    /// <summary>
    /// Checks if node is negative scale, and reverts it if it was
    /// </summary>
    private void CheckNegativeScale()
    {
        //check if scale on node is negative (it shouldn't be)
        Vector3 temp = transform.localScale;

        if (transform.localScale.x < 0)
            temp.x = System.Math.Abs(temp.x);

        if (transform.localScale.y < 0)
            temp.y = System.Math.Abs(temp.y);

        if (transform.localScale.z < 0)
            temp.z = System.Math.Abs(temp.z);

        transform.localScale = temp;
    }

    /// <summary>
    /// Call the function in base class to instantiate clutter
    /// </summary>
    private void SpawnClutter(bool _child)
    {
        if (numberToSpawn < 1)
            numberToSpawn = 1;

        Vector3 spawnPos = new Vector3();
        float posMult = 1;

        for (int index = 0; index < numberToSpawn; ++index)
        {
            GetSpawnPosition(ref spawnPos, ref posMult);

            if (lockX)
                spawnPos.x = 0;

            if (lockZ)
                spawnPos.z = 0;

            GameObject parent = _child ? gameObject : m_clutterParent;
            float dist = _child ? distance : 1;
            if (m_IsChild)
            {
                Vector3 temp = spawnPos;
                temp.y += 2;
                spawnPos = temp;
            }
            //base.InstantiateClutter(spawnPos, posMult, 1, m_clutterParent.transform);
            base.InstantiateClutter(spawnPos, posMult, dist, parent.transform);
        }
    }

    private void GetSpawnPosition(ref Vector3 _spawnPos, ref float posMult)
    {
        switch (shape)
        {
            case colliderMenu.Box:
                _spawnPos = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));
                posMult = 0.45f;
                break;

            case colliderMenu.Sphere:
                _spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                //top of sphere
                _spawnPos.y = 1;
                posMult = 0.95f;
                break;
        }
    }
}
#endif