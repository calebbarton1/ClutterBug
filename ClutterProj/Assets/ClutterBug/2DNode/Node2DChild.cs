// Copyright (C) 2016 Caleb Barton (caleb.barton@hotmail.com)
//github.com/calebbarton1
//Released under MIT License
//https://opensource.org/licenses/MIT

using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
public class Node2DChild : Clutter2D
{
    [Space(10)]

    [Tooltip("Distance between parent clutter and child")]
    public float distance = 1;

    public void SpawnObjectsInArea()
    {
        //check if prefab has negative scale (it shouldn't)
        Vector3 temp = transform.localScale;

        if (transform.localScale.x < 0)
            temp.x -= transform.localScale.x * 2;

        if (transform.localScale.y < 0)
            temp.y -= transform.localScale.y * 2;

        transform.localScale = temp;

        SpriteRenderer col = GetComponent<SpriteRenderer>();

        //so objects aren't being spawned inside their parent. Their parent is essentially another node.
        {
            //use the largest value of the scale to ensure objects aren't inside parent
            float toMove;
            if (col.bounds.size.x > col.bounds.size.y)
                toMove = col.bounds.size.x;

            else
                toMove = col.bounds.size.y;

            distance = distance + toMove;

            if (distance == 0)
                distance = 1;
        }



        if (prefabList.Count != 0 && numberToSpawn != 0)
        {
            for (int index = 0; index < numberToSpawn; ++index)
            {
                Vector2 spawnPos = Random.insideUnitCircle;//gets value within a sphere that has radius of 1
                InstantiateObject(spawnPos, 1f, distance, transform);
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
