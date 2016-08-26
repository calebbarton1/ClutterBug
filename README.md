# ClutterBug
Tutorial for program can be found here:
https://calebbartonblog.wordpress.com/2016/08/22/clutterbug-tutorial/

If you have any questions contact me at caleb.barton@hotmail.com

If you find any bugs please notify me using the Github report system.

# FAQ

Q:Why does my clutter spawn inside each other?

A: If your clutter is bigger than the node, then the nodes raycast starts inside your clutter. Either scale the clutter down, or make the node bigger.

Q: Why does my clutter spawns on top of each other?

A: Make sure you have a layer named "Clutter" and that your prefabs are a part of that layer.
