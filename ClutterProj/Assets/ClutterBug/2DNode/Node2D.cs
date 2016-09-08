using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class Node2D : Clutter2D
{
    //enums for collider selection in inspector
    public enum colliderMenu
    {
        Square,
        Circle
    }

    //buttons for generating objects
    //credit to "zaikman" for the script

    [Space(10)]

    [Tooltip("Adds clutter per click instead of rerolling")]
    public bool additive = false;

    [Space(10)]

    //initialise enum and colliders
    [Tooltip("The shape of the area where objects are placed")]
    public colliderMenu shape = colliderMenu.Square;

    [HideInInspector]
    public GameObject clutterParent;


    //TODO: Have transparent sprites instead of gizmo 3d objects
    public void OnDrawGizmos()
    {
        Gizmos.color = new Color(1.0f, .50f, 1.0f, 0.5f);
        switch (shape)
        {
            case colliderMenu.Square:

                Vector3 tempScale = new Vector3(transform.localScale.x, transform.localScale.y, 0.00001f);
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, tempScale);//making a matrix based on the transform, draw shape based on it
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Gizmos.color = new Color(0, 0, 0, .75f);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
                Gizmos.matrix = Matrix4x4.identity;
                break;

            case colliderMenu.Circle:
                Vector3 tempScale1 = new Vector3(transform.localScale.x, transform.localScale.y, 0.00001f);
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.localRotation, tempScale1);
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

        transform.localScale = temp;

        if (prefabList.Count != 0 && numberToSpawn != 0)
        {
            switch (shape)
            {
                case colliderMenu.Square:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector2 spawnPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);//random x and z on top of box
                        InstantiateObject(spawnPos, 1, .45f, clutterParent.transform);
                    }

                    break;

                case colliderMenu.Circle:

                    for (int index = 0; index < numberToSpawn; ++index)
                    {
                        Vector3 spawnPos = Random.insideUnitCircle;//gets value within a sphere that has radius of 1
                        InstantiateObject(spawnPos, 0.9f, .95f, clutterParent.transform);
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