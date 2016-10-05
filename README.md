# ClutterBug

If you have any questions contact me at caleb.barton@hotmail.com

If you find any bugs please notify me using the Github report system.

# System Rquirements

Clutterbug requires Unity 5.4.0f3.

# Tutorial

## Getting Started

### Layers

Before using ClutterBug, you must create a layer in your project named "Clutter".

Then, change the layer of any prefabs you want to use with the newly created "Clutter". This is required so that clutter doesn't stack on top of each other.

![alt tag](https://calebbartonblog.files.wordpress.com/2016/08/clutterbug-tute5.png?w=594)

### Colliders

If you don't want your clutter to spawn inside each other, then the clutter you are using must have a collider of some form.

### Technical details

When the "Spawn Clutter" button is pressed, the node makes a RigidbodySweep test from the top of the node. If your clutter doesn't have a Rigidbody, it will make a spherecast using the MeshFilter to base the radius of the sphere. These obviously require a collider to place objects.

Clutterbug runs in editor mode by default.

## Placing a Node

Clutter bug works with nodes that have area of effects. Place a node by clicking "Create Node" in the ClutterBug menu, or drag the prefab in the folder.

![alt tag](https://calebbartonblog.files.wordpress.com/2016/08/clutter1.png?w=594)

Once you've done  this, you can modify the transform as you would a normal game object.

## Spawning Clutter

Place the Clutternode above an object with a collider.

Then simply drag your desired prefabs into the PrefabList, input the number of clutter you wish to spawn, then hit "Spawn Clutter" while in editor mode.

## Node Options

![alt tag](https://calebbartonblog.files.wordpress.com/2016/08/clutter2.png?w=594)

There are a variety of settings to modify the clutter node:

**Spawn Clutter**- Spawns the clutter in the node

**Delete Clutter**- Deletes clutter associated with that node.

**Enable Debugging** - Tick this option to enable debug logs in Unity's console. It's recommended to have this off, as it lowers performance. The log contains information as to whether clutter has failed to instantiate, and why.

**Shape** - The shape of the clutter node. 3D nodes can be a box or sphere. 2D nodes can be square or circle.

**Clutter Mask** - Clutter won't spawn on any object belonging to this list of layermasks.

**Number of Clutter** - The number of prefabs to be spawned as clutter.

**Prefab List** - The prefabs that will be spawned as clutter. Objects are randomly selected from this list to be spawned.

**Enable Clutter Overlap** - This option allows clutter to overlap with one another.

**Enable Additive Clutter**- If enabled the Spawn Clutter button will not delete the previously spawned clutter.

**Rotate to Surface Normal**- If enabled, clutter will rotate to face the surface where the clutter is being spawned.

**Surface Angle Limit**- This slider value, ranging from 1 to 89, is the maximum angle (in degrees) clutter can be spawned on on. If the surface angle is higher than the given value, the clutter will not spawn.

**Randomise Rotation** - The rotation of the spawned clutter can be randomly assigned within a range.

**Rotation Override** - These values will override the above Randomise Rotation, and the prefab value. Leave at 0 to keep prefab value.

**Random Scale** - The clutter will be uniformly scaled between the random min and max value.

**Scale Override** - Will override the above Random Scale, and any prefab value. Leave at 0 to keep prefab value.

## Child Nodes

In addition to the above, clutter spawned from a node can spawn their own clutter around them using the "Node Child" script.

**NOTE: This is currently unavailable on 2D nodes.**

![alt tag](https://calebbartonblog.files.wordpress.com/2016/08/clutterbug-tute4.png?w=666&h=336)

If a prefab has this script, and is spawned from a 3DNode, it will create clutter in an area around that clutter.

This functions the same as a ClutterNode, with the exception of the Distance variable.

**Distance** - is the distance in world space the object can spawn from it's parent clutter.

This distance variable adds the width of its meshrenderer, so that clutter will not spawn inside of itself.

This Child Node script can also be placed on a prefab that has been spawned from a ChildNode script.

![alt tag](https://calebbartonblog.files.wordpress.com/2016/08/clutterbug-tute4.png?w=666&h=336)

As seen from this screenshot, the Blue Capsule is being spawned by the Node. The blue capsule then spawns its own Red Cubes as clutter. These red cubes then spawn their own yellow spheres. As the yellow spheres don't have the NodeChild script on them, the function ends there.

# FAQ

Q:Why does my clutter spawn inside each other?

A: If your clutter is bigger than the node, then the nodes raycast starts inside your clutter. Either scale the clutter down, or make the node bigger.

Q: Why does my clutter spawns on top of each other?

A: Make sure you have a layer named "Clutter" and that your prefabs are a part of that layer.
