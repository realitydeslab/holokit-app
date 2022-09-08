using UnityEngine;
using UnityEditor;

namespace StarUI
{
    [ExecuteInEditMode]
    public class ResolutionSetting : MonoBehaviour
    {
        static ViewportHandler _VPH;
        static CameraAnchor _CA;
        static StarUIManager _SUM;

        [SerializeField] static Vector2 _resolutionX = new Vector2(2436, 1125);
        [SerializeField] static Vector2 _resolutionXS = new Vector2(2436, 1125);
        [SerializeField] static Vector2 _resolutionXSMax = new Vector2(2688, 1242);

        [SerializeField] static Vector2 _resolutionXR = new Vector2(1792, 828);

        [SerializeField] static Vector2 _resolution11 = new Vector2(1792, 828);
        [SerializeField] static Vector2 _resolution11Pro = new Vector2(2436, 1125);
        [SerializeField] static Vector2 _resolution11ProMax = new Vector2(2688, 1242);

        [SerializeField] static Vector2 _resolution12Mini = new Vector2(2340, 1080);
        [SerializeField] static Vector2 _resolution12 = new Vector2(2532, 1170);
        [SerializeField] static Vector2 _resolution12ProMax = new Vector2(2778, 1284);

        [SerializeField] static Vector2 _resolution13Mini = new Vector2(2340, 1080);
        [SerializeField] static Vector2 _resolution13 = new Vector2(2532, 1170);
        [SerializeField] static Vector2 _resolution13ProMax = new Vector2(2778, 1284);

        static float iPhoneX = 5.85f; // XS, 11Pro, 
        static float iPhoneXSMAx = 6.46f; // 11ProMax, 
        static float iPhoneXR = 6.06f; // 12Pro, 12, 13Pro, 13
        static float iPhone11 = 6.1f; //
        static float iPhone12ProMax = 6.68f; // 13ProMax
                                             //static float iPhone12Mini = 5.42f; // 13Mini,


        private void Start()
        {
            _VPH = FindObjectOfType<ViewportHandler>();
            _CA = FindObjectOfType<CameraAnchor>();
            _SUM = FindObjectOfType<StarUIManager>();
        }

        [MenuItem("Tools/Update Resolution/X&XS&11Pro")]
        private static void UpdateResolutionX()
        {
            _VPH.UnitsSize = iPhoneX;
            var offset = (_resolutionX.y - _resolutionX.y);
            _SUM.GetComponent<RectTransform>().localPosition = new Vector3(0, 529.5f - offset, 0);
        }
        [MenuItem("Tools/Update Resolution/XSMax&11ProMax")]
        private static void UpdateResolutionXSMax()
        {
            _VPH.UnitsSize = iPhoneXSMAx;
            var offset = (_resolutionXSMax.y - _resolutionX.y) / 2;
            _SUM.GetComponent<RectTransform>().localPosition = new Vector3(0, 529.5f - offset, 0);
            Debug.Log(offset);
            Debug.Log(529.5f - offset);
        }

        [MenuItem("Tools/Update Resolution/12&12Pro&13&13Pro")]
        private static void UpdateResolution12()
        {
            _VPH.UnitsSize = iPhoneXR;
            var offset = (_resolution12.y - _resolutionX.y) / 2;
            _SUM.GetComponent<RectTransform>().localPosition = new Vector3(0, 529.5f - offset, 0);
        }
        [MenuItem("Tools/Update Resolution/11&XR")]
        private static void UpdateResolution11()
        {
            _VPH.UnitsSize = iPhone11;
            var offset = 0;
            _SUM.GetComponent<RectTransform>().localPosition = new Vector3(0, 529.5f - offset, 0);
        }

        [MenuItem("Tools/Update Resolution/12ProMax&13ProMax")]
        private static void UpdateResolution12ProMax()
        {
            _VPH.UnitsSize = iPhone12ProMax;
            var offset = (_resolution12ProMax.y - _resolutionX.y) / 2;
            _SUM.GetComponent<RectTransform>().localPosition = new Vector3(0, 529.5f - offset, 0);
        }
    }
}
