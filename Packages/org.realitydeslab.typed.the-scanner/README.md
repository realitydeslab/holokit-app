# TypedRealityTheScanner

TypedRealityTheScanner creates and Stereo-AR experience where  white rectangular ribbons in a style of typography on the surrounding floors, walls, and other environmental surfaces. These ribbons nest layer by layer, expanding over time.

<img src="Documentation~/images/scanner.avif" width="320" />


In this reality, we use Meshing feature to get the mesh information of our surroundings in real-time, to change the material of mesh to render rectangular ribbons effect.

***Customization:***

The material of meshes control all the visual elements of scene Scanning. 

<img src="Documentation~/images/scanner01.png" width="320" />

Select "Meshing" object and click on “Mesh Prefab” field to check prefabs we provided.

<img src="Documentation~/images/scanner02.png" width="320" />

In the initial design, we created different prefabs for various types of meshing, with the sole distinction being the different materials. Each material is specifically overlaid with text of that type.

However, we couldn't address the UV mapping issue with meshing, which resulted in a material being unable to consistently represent both vertical and horizontal planes. Consequently, we primarily scan the ground, using only one prefab, “DetectedMeshesHorizontal_Floor”

If you wish to customize the appearance of the scanned meshing, please duplicate the prefab, modify its material, and replace the original “Mesh Prefab” with your modified prefab.
