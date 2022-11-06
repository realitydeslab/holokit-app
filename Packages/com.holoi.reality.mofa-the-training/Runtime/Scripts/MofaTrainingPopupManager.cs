using UnityEngine;
using Holoi.Library.MOFABase;

namespace Holoi.Reality.MOFATheTraining
{
    public class MofaTrainingPopupManager : MofaPopupManager
    {
        [Header("MOFA The Training")]
        [SerializeField] private GameObject _findPlanePrefab;

        [SerializeField] private GameObject _placeAvatarPrefab;

        protected override void Start()
        {
            base.Start();
            SpawnPopup(_findPlanePrefab);
            MofaTrainingRealityManager.OnFoundPlane += OnFoundPlane;
            MofaTrainingRealityManager.OnLostPlane += OnLostPlane;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            MofaTrainingRealityManager.OnFoundPlane -= OnFoundPlane;
            MofaTrainingRealityManager.OnLostPlane -= OnLostPlane;
        }

        private void OnFoundPlane()
        {
            SpawnPopup(_placeAvatarPrefab);
        }

        private void OnLostPlane()
        {
            SpawnPopup(_findPlanePrefab);
        }
    }
}
