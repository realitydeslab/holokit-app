using UnityEngine;
using Unity.Netcode;
using Holoi.Library.HoloKitApp;


namespace Holoi.Reality.BlankPiecesOfPaper
{
    public class TypedRealityBlankPiecesofPaperManager : RealityManager
    {
        [SerializeField] NetworkObject SinglePagePrefab;
        NetworkObject _singlePageInstance;
        Transform _centereye;

        void Start()
        {
            _centereye = HoloKit.HoloKitCamera.Instance.CenterEyePose;
        }

        public void CreateSinglePage()
        {
            Debug.Log("CreateSinglePage");
            _singlePageInstance = Instantiate(SinglePagePrefab);
            _singlePageInstance.transform.position = _centereye.position + _centereye.forward * 2f;
            _singlePageInstance.Spawn();
        }
    }
}
