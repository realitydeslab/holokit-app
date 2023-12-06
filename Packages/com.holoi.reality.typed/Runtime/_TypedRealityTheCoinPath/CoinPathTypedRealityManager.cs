using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;


namespace Holoi.Reality.Typography
{
    public class CoinPathTypedRealityManager : RealityManager
    {
        public int Score = 0;

        [SerializeField] CoinPathController _coinPathController;
        [SerializeField] LineRenderer _pathRenderer;

        bool _directionForward = true;

        void Start()
        {
        
        }

        private void Update()
        {
            if(Score == 17)
            {
                _coinPathController.Reset();
                Score = 0;
                if (_directionForward)
                {
                    _directionForward = false;
                    _pathRenderer.material.SetVector("_Speed", new Vector4(2, 0, 0, 0));
                }
                else
                {
                    _directionForward = true;
                    _pathRenderer.material.SetVector("_Speed", new Vector4(-2, 0, 0, 0));
                }
            }
        }
    }
}
