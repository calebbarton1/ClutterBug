using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class NodeChild : Clutter
{
    [Space(10)]

    [Tooltip("Distance between parent clutter and child (currently scales down with object scale. TOFIX)")]
    public float distance = 1;

    

    public void SpawnObjectsInArea()
    {
        Mesh col = GetComponent<MeshFilter>().sharedMesh;        

        //so objects aren't being spawned inside their parent. Their parent is essentially another node.
        {
            //use the largest value of the scale to ensure objects aren't inside parent
            float toMove;
            if (col.bounds.extents.x > col.bounds.extents.z)
                toMove = col.bounds.extents.x;

            else
                toMove = col.bounds.extents.z;

            distance = distance + toMove;
        }

       

        if (prefabList.Count != 0 && numberToSpawn != 0)
        {
            for (int index = 0; index < numberToSpawn; ++index)
            {
                Vector3 spawnPos = Random.insideUnitSphere;//gets value within a sphere that has radius of 1
                spawnPos.y = 10;
                InstantiateObject(spawnPos, 1.1f, distance, transform);
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
