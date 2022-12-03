using UnityEngine;
using HoloKit;
using Holoi.Library.ARUX;


namespace Holoi.Reality.Typography
{
    public class TypedRealityTheForceRealityManager : MonoBehaviour
    {
        Transform _centereye;
        [SerializeField] Transform _handJoint;
        [SerializeField] Player _player;
        [SerializeField]GameObject _prefabMC;
        [SerializeField]GameObject _prefabMO;

        int n = 0;
        int m = 0;
        float _lastTriggerTime = 4;

        private void Start()
        {
            _centereye = HoloKitCamera.Instance.CenterEyePose;
        }

        void SceneSetup()
        {
            LayerMask lm = 1 << 6;
            Vector3 dir = new Vector3(Random.Range(-1f,1f), -1 , Random.Range(0f,1f));
            Ray ray = new Ray(_centereye.position, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 3.0f, lm))
            {
                Debug.Log("hit ar sroundings");
                CreateMagicObjct(hit.point);
                n++;
            }
        }

        void CreateMagicCube()
        {
            var go = Instantiate(_prefabMC);
            go.transform.position = _centereye.position + _centereye.forward;
            go.transform.GetComponent<FollowMovementManager>().FollowTarget = _handJoint;
        }
        void CreateMagicObjct(Vector3 position)
        {
            var go = Instantiate(_prefabMO);
            go.transform.position = position + Vector3.up*0.5f;
        }

        private void Update()
        {
            if (m == 0)
            {
                CreateMagicCube();
                m++;
            }

            if (n == 0)
            {
                if (Time.time - _lastTriggerTime > 4f)
                {
                    SceneSetup();
                    _lastTriggerTime = Time.time;
                }
            }
        }
    }
}
