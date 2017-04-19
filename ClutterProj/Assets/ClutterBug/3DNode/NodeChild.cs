// Copyright (C) 2016 Caleb Barton (caleb.barton@hotmail.com)
//github.com/calebbarton1
//Released under MIT License
//https://opensource.org/licenses/MIT

using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class NodeChild : Clutter
{
    [Space(10)]

    public float distance = 1;

    public bool lockX = false;
    public bool lockZ = false;

    public void SpawnObjectsInArea()
    {
        //check if prefab has negative scale (it shouldn't)
        Vector3 temp = transform.localScale;

        if (transform.localScale.x < 0)
            temp.x -= transform.localScale.x * 2;

        if (transform.localScale.y < 0)
            temp.y -= transform.localScale.y * 2;

        if (transform.localScale.x < 0)
            temp.z -= transform.localScale.z * 2;

        transform.localScale = temp;

        Mesh mCol;
        float toMove;

        //to make sure that the children aren't spawning inside of thier parent, we get the scale or the mesh, depening on user input.
        if (useMesh)
        {
            mCol = GetComponent<MeshFilter>().sharedMesh;

            if (mCol.bounds.size.x > mCol.bounds.size.z)
                toMove = mCol.bounds.size.x;

            else
                toMove = mCol.bounds.size.z;
        }

        else
        {
            if (transform.lossyScale.x > transform.lossyScale.z)
                toMove = transform.lossyScale.x;

            else
                toMove = transform.lossyScale.z;
        }

        distance += toMove;

        if (distance == 0)
            distance = 1;

        if (prefabList.Count != 0 && numberToSpawn != 0)
        {
            for (int index = 0; index < numberToSpawn; ++index)
            {
                Vector3 spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                spawnPos.y = 1;

                if (lockX)
                    spawnPos.x = 0;

                if (lockZ)
                    spawnPos.z = 0;

                InstantiateObject(spawnPos, 1.5f, distance, transform);
            }
        }

        else if (numberToSpawn == 0)
        {
            Debug.LogWarning(gameObject.name + " has number of spawned prefabs set to 0.");
            return;
        }

        else
        {
            Debug.LogWarning(gameObject.name + " has no prefabs in List!");
            return;
        }
    }
}
#endif
