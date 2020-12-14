# Minecraft Grip Digital Task

Entry scene is in **_Project/Scenes/Mcgd_**

I used Standard Assets Package for FPS Controller

Optimizations:
- remove mesh colliders on block meshes, in case of more spawned 3d objects it is pretty expensive (related to below)
- I chose the wrong path and created building blocks separately and also keep track of them separately, they should be part
of the original block world mesh.This causes performance problems when crawling the world. Could be optimised by using CombineInstance and Mesh.CombineMeshes with different materials
- Could decouple some dependencies like dependency of BuildController.cs with World.cs would be neat to use EventBus and  communicate 
with objects using events rather than directly address the BlockController.cs using GetComponent()





