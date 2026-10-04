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

## Roles and interactions

| Role | Can do | Cannot do |
| --- | --- | --- |
| Host | Walk around; the room is meshed and ribbons of letters spread over it from the scanner centre | — |
| Spectator | Watch the host's reality from their own phone ("Join As Spectator"), including when joining late | Change anything |

## Automated tests

| Test | What it does | Run |
| --- | --- | --- |
| `TypedTheScannerJourneyTests` | Three Editors: the room is meshed as the host walks, every mesh centres its ribbons on the scanner centre, the floor shows ribbons, the waiting hint fades; spectators mesh their own view around the host's scanner centre. Baselines per role | `Tools/run_multiplayer_test.sh Typed TypedTheScannerJourneyTests` |
| `TypedThe*SceneTests` | Scene, prefab and reference checks | EditMode suite |

## Not covered by the Editor tests

- Meshing in the Editor comes from XR Simulation's room, not LiDAR.
- Multipeer Connectivity and QR alignment (the Editor uses UnityTransport on 127.0.0.1 in one simulated room).
