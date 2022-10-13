using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace Holoi.Reality.Dragon
{
    public class NavigationBaker : MonoBehaviour
    {
        public List<NavMeshSurface> surfaces;

        private void Start()
        {
#if UNITY_EDITOR
            BuildNavMesh();
#endif
        }

        // Use this for initialization
        public void BuildNavMesh()
        {
            for (int i = 0; i < surfaces.Count; i++)
            {
                surfaces[i].BuildNavMesh();
            }
        }

    }
}