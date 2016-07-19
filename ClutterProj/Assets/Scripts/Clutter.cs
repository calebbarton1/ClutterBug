using UnityEngine;
using System.Collections;

public class Clutter : MonoBehaviour {

    //enums for collider selection in inspector
    public enum colliderMenu
    {
        Box,
        Sphere,
        Cylinder
    }

    //initialise enum and colliders
    public colliderMenu shape = colliderMenu.Box;
    Collider col;


    public void Awake()
    {
        col = gameObject.GetComponent<Collider>();
    }

	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
      
    }


    public void ShapeColliders()
    {
        switch (shape)
        {
            case colliderMenu.Box:
                if (col.GetType() != typeof(BoxCollider))//checks what collider is currently on node
                {
                    Destroy(col);//destroys previous collider
                    col = gameObject.AddComponent<BoxCollider>();//makes new one
                }
                break;

            case colliderMenu.Sphere:
                if (col.GetType() != typeof(SphereCollider))
                {
                    Destroy(col);
                    col = gameObject.AddComponent<SphereCollider>();
                }
                break;

            case colliderMenu.Cylinder:
                if (col.GetType() != typeof(CapsuleCollider))
                {
                    Destroy(col);
                    col = gameObject.AddComponent<CapsuleCollider>();
                }
                break;

            default:
                break;
        }
    }
}
