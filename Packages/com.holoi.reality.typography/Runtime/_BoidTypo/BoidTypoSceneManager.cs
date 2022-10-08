using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using BoidsSimulationOnGPU;


namespace Holoi.Reality.Typography
{
    public class BoidTypoSceneManager : MonoBehaviour
    {
        [SerializeField] GPUBoids _boids;
        [SerializeField] VisualEffect _vfx;

        void Update()
        {
            _vfx.SetGraphicsBuffer("DataSet", _boids.GetBoidDataBuffer());
        }
    }
}