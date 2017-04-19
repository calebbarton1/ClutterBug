# ClutterBug

ClutterBug is an open source Unity based Tool for placing large amounts of prefabs within a space.

Unlike other "brush" based prefab tools, ClutterBug uses a hierarchal node system to have some organisation in your random placements. Each node can place a given set of objects, and each object can place their own given objects, and THOSE objects can place their own given objects (and so on).

ClutterBug is distributed under the MIT license, which means ClutterBug is completely open source and free to use and modify. Please give credit when using or modifying. 

If you have any questions  please contact me at caleb.barton AT hotmail DOT com

Please report any bugs using the Github report system or by contacting me.

# Tutorial

## Getting Started

### Layers

Before using ClutterBug, you must create a layer in your project named "Clutter".

Then, change the layer of any prefabs you want to use with the newly created "Clutter". This is required so that clutter doesn't stack on top of each other.

![alt tag](https://calebbartonblog.files.wordpress.com/2016/08/clutterbug-tute5.png?w=594)

### Colliders

If you don't want your clutter to spawn inside each other, then the clutter you are using must have a collider of some form.

### Technical details

When the "Spawn Clutter" button is pressed, the node makes a RigidbodySweep test from the top of the node. If your clutter doesn't have a Rigidbody, it will make a spherecast using the MeshFilter to base the radius of the sphere. These obviously require a collider of some form to place objects.

2D Clutter uses the Sprite Renderer to check for overlaps on clutter placement. Like the 3D Clutter, this required a 2D collider of some form.

Clutterbug runs in editor mode by default. If you wish to use ClutterBug at runtime, remove "#if UNITY_EDITOR" from the start of the Clutter.cs, Node.cs and NodeChild.cs scripts.

## Placing a Node

Clutter bug works with nodes that have area of effects. Place a node by clicking "Create Node" in the ClutterBug menu, or drag the prefab in the folder.

![alt tag](https://calebbartonblog.files.wordpress.com/2016/08/clutter1.png?w=594)

Once you've done  this, you can modify the transform as you would a normal game object.

## Spawning Clutter

Place the Clutternode above an object with a collider.

Then simply drag your desired prefabs into the PrefabList, input the number of clutter you wish to spawn, then hit "Spawn Clutter" while in editor mode.

## Node Options

![alt tag](http://i.imgur.com/NYwsTK4.jpg)

There are a variety of settings to modify the clutter node:

**Spawn Clutter**- Spawns the clutter in the node

**Delete Clutter**- Deletes clutter associated with that node.

**Enable Debugging** - Tick this option to enable debug logs in Unity's console. It's recommended to have this off, as it lowers performance when creating clutter. The log contains information as to whether clutter has failed to instantiate, and the type of casting it used.

**Shape** - The shape of the clutter node. 3D nodes can be a box or sphere. 2D nodes can be square or circle.

**Clutter Mask** - Clutter won't spawn on any object belonging to this list of layermasks.

**Number of Clutter** - The number of prefabs the node will attempt place as clutter

**Prefab List** - The prefabs that will be spawned as clutter. Each object has a weighting affecting its chance of being chosen.

**Enable Clutter Overlap** - This option allows clutter to overlap with one another.

**Enable Additive Clutter**- If enabled the previously created clutter will not be deleted.

**Rotate to Surface Normal**- If enabled, clutter will rotate to face the surface where the clutter is being spawned.

**Surface Angle Limit**- This slider value, ranging from 1 to 89, is the maximum angle (in degrees) clutter can be spawned on on. If the surface angle is higher than the given value, the clutter will not spawn.

**Lock X/Z Position**- These bools will force the spawned clutter into a straight line in the selected axis instead of filling the node area.

**Use Mesh Scaling**- This option will make the Clutter Node use the Meshes bounds.extents Y value to align the gameobject with the ground. This may be used if the mesh was larger or smaller than Unity's default scale. If this is the case, the models may not line up with the ground correctly.

**Randomise Rotation** - The rotation of the spawned clutter can be randomly assigned within a range.

**Rotation Override** - These values will override the above Randomise Rotation, and the prefab value. Leave at 0 to keep prefab value.

**Random Scale** - The clutter will be uniformly scaled between the random min and max value.

**Scale Override** - Will override the above Random Scale, and any prefab value. Leave at 0 to keep prefab value.

## Child Nodes

In addition to the above, clutter spawned from a node can spawn their own clutter around them using the "Node Child" script.

**NOTE: This is currently unavailable on 2D nodes.**

![alt tag](http://i.imgur.com/QvqArLB.jpg)

If a prefab has this script, and is spawned from a 3DNode, it will create clutter in an area around that clutter.

This functions the same as a ClutterNode, with the exception of the Distance variable.

**Distance** - is the distance in world space the object can spawn from it's parent clutter.

This distance variable adds the width of its MeshRenderer or scale (depending if useMesh is enabled).

This Child Node script can also be placed on a prefab that has been spawned from a ChildNode script.

![alt tag](http://i.imgur.com/8NfHvLB.jpg)

As seen from this screenshot, the Blue Capsule is being spawned by the Node. The blue capsule then spawns its own Red Cubes as clutter. These red cubes then spawn their own yellow spheres. As the yellow spheres don't have the NodeChild script on them, the function ends there.

## 2D Node Options

![alt tag](http://i.imgur.com/HSbrHdu.jpg)

**Random Order in Layer**- Randomise the layer order of the clutter between the given range.

**Order Layer**- Manually set the Order Layer of instantiated clutter.

# FAQ

Q:Why does my clutter spawn inside each other?

A: If your clutter is bigger than the node, then the nodes raycast starts inside your clutter. Either scale the clutter down, or make the node bigger.

Q: Why does my clutter spawn on top of each other?

A: Make sure you have a layer named "Clutter" and that your prefabs are a part of that layer.

Q: Can I use ClutterBug outside of the Unity Editor?

A: Yes! Whilee ClutterBug is designed for editor use, it can work for dynamically generating clutter in game. To do this, remove all instances of "#if UNITY_EDITOR" from the scripts in the ClutterBug folder. Then when you want to tell a node to create clutter, call the "SpawnObjectsInArea()" function on the node.

Q: Why does my clutter not appear?

A: There can be a few reasons for this. Here are the most common situations:

1.)The selected Clutter Mask is the same as the place the Clutter Node is trying to spawn your object (like the ground). Check the Clutter Mask, or the layer that the ground belongs to.

2.)The gameobjects default mesh size is too big (at transform scale 1), and ClutterBug tries to raycast through the ground. This is fixed by changing the meshes scale factor.

# Known Issues
1.) When dragging prefabs onto the Prefab List, you cannot replace an individual element with the dragged object. You must drag the object onto the list to create the new object. This is because I'm purposefully not allowing drag operations to change the list while the editor is updating. This prevents out of index errors. I'm working on a workaround.

2.) Prefabs that are empty game objects and contain multiple children with mutiple meshes are currently not able to be used by ClutterBug. This is because it uses MeshFilters that are on the prefab itself, and not its children. If you want to use ClutterBug, the models primary mesh needs to be on the prefab as parent.
