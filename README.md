# Minecraft Grip Digital Task

Entry scene is in **_Project/Scenes/Mcgd_**

I used Standard Assets Package for FPS Controller

Optimizations:
- remove mesh colliders on block meshes, in case of more spawned 3d objects it is pretty expensive (related to above)
- I chose the wrong path and created building blocks separately and also keep track of them separately, they should be part
of the original block world mesh.Could be optimised by using CombineInstance and Mesh.CombineMeshes with different materials
That would give also option to dig down the world blocks (which was actually part of my original solution)



