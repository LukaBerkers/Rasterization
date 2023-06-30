This is the rasterizer of the following students:
Luuk Berkers #6793592
Qu Xing Zhu #1677977

We have implemented the following minimum requirements:

1.  A working camera that you can move left, right, forward, backward with the WASD keys and also up and down with spacebar and shift respectively.
    Furthermore you can also use the mouse for up and down movement and look while moving.

2.  A scene graph that contains a hierarchy of meshes and renders perfectly with any number of meshes (our scene has 4 meshes).

    The functionality of the scene graph is shown by the small teapots attached to the big teapot.
    Their position is defined with respect to their parents, the big teapot, but they don’t need to know about that and only take care of their own rotation.

    Our scene graph also supports ‘virtual’ nodes, i.e. nodes that do not have a mesh but can be used to group nodes under a common parent.
    In our demo only the top-level world node is such a node.

3.  A working shader that implemented the Phong illumination.


For the bonus assignment we implemented the following:

1.  Vignetting and chromatic aberration.

2.  We have a partial implementation of frustum culling.
    We didn’t fully get it to work in time but it works for the teapot with respect to the near-plane.
    That is, the teapots are not rendered if they are behind the near-plane.
    And for the floor with all planes of the frustum except the far plane.

    The ‘virtual node’ functionality of our scene graph can be used to group objects in a common bounding box.


Sources (links) that helped us with the implementation of our engine:

1. https://www.youtube.com/watch?v=3CsNRBme6nU (For Vignetting and chromatic aberration)
2. https://learnopengl.com/Guest-Articles/2021/Scene/Frustum-Culling (For frustum culling)
