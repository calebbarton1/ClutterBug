// Copyright (C) 2016 Caleb Barton (caleb.barton@hotmail.com)
//github.com/calebbarton1
//Released under MIT License
//https://opensource.org/licenses/MIT

using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class Node : Clutter
{
    //enums for collider selection in inspector
    public enum colliderMenu
    {
        Box,
        Sphere,
    }
    public bool additive = false;

    public bool lockX = false;
    public bool lockZ = false;
    //initialise enum and colliders
    public colliderMenu shape = colliderMenu.Box;

    [HideInInspector]
    public GameObject clutterParent;

    public void OnDrawGizmos()
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

            default:
                break;
        }
    }

    public void DeleteClutter()
    {
        DestroyImmediate(clutterParent);
    }

    public void SpawnObjectsInArea()
    {
        if (!additive)
            DeleteClutter(); //Delete previously placed objects

        if (!clutterParent)
            clutterParent = new GameObject("clutterParent");

        //check if scale on node is negative (it shouldn't be)
        Vector3 temp = transform.localScale;

        if (transform.localScale.x < 0)
            temp.x -= transform.localScale.x * 2;

        if (transform.localScale.y < 0)
            temp.y -= transform.localScale.y * 2;

        if (transform.localScale.z < 0)
            temp.z -= transform.localScale.z * 2;

        transform.localScale = temp;

        if (prefabList.Count != 0 && numberToSpawn != 0)
        {
            switch (shape)
            {
                case colliderMenu.Box:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = new Vector3(Random.Range(-1f, 1f), 1, Random.Range(-1f, 1f));//random x and z on top of box

                        if (lockX)
                            spawnPos.x = 0;

                        if (lockZ)
                            spawnPos.z = 0;

                        InstantiateObject(spawnPos, .45f, 1, clutterParent.transform);
                    }

                    break;

                case colliderMenu.Sphere:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                        spawnPos.y = 1;

                        if (lockX)
                            spawnPos.x = 0;

                        if (lockZ)
                            spawnPos.z = 0;

                        InstantiateObject(spawnPos, 0.9f, 1, clutterParent.transform);
                    }

                    break;

                default:
                    break;
            }
        }

        else if (numberToSpawn == 0)
        {
            Debug.LogWarning("Node has number to spawn set to 0.");
            return;
        }

        else
        {
            Debug.LogWarning("Node has no prefabs in List!");
            return;
        }
    }
}
#endif