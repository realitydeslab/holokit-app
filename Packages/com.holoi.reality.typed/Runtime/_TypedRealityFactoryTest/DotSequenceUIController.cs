using UnityEngine;
using Holoi.Library.HoloKitAppLib.UI;
using UnityEngine.VFX;
using UnityEngine.UI;
using TMPro;

namespace Holoi.Reality.Typography
{
    public class DotSequenceUIController : HoloKitAppUIPanel
    {
        public override string UIPanelName => "TheFactoryTestUIPanel";

        public override bool OverlayPreviousPanel => false;

        [SerializeField] TMP_InputField Spacing;
        [SerializeField] TMP_InputField Distance;
        [SerializeField] TMP_InputField ParticleSize;

        VisualEffect _vfx;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _vfx = FindObjectOfType<DotSequenceController>()._vfx;
            Spacing.onValueChanged.AddListener(delegate { SpacingValueChanged(); });
            Distance.onValueChanged.AddListener(delegate { DistanceValueChanged(); });
            ParticleSize.onValueChanged.AddListener(delegate { ParticleSizeValueChanged(); });
        }

        protected virtual void Update()
        {

        }

        public void SpacingValueChanged()
        {
            var result = 0f;
            if (float.TryParse(Spacing.text, out result))
            {
                _vfx.SetFloat("Spacing", result);
            }
            else
            {
                // Not a number, do something else with it.
            }
            //    _vfx.SetFloat("Spacing", string.IsNullOrEmpty(Distance.text) ? 0.1f:float.Parse(Spacing.text));
            //Debug.Log("SpacingValueChanged");
        }

        public void ParticleSizeValueChanged()
        {
            var result = 0f;
            if (float.TryParse(ParticleSize.text, out result))
            {
                _vfx.SetFloat("Size", result);
            }
            //    _vfx.SetFloat("Size", string.IsNullOrEmpty(Distance.text) ? 0.01f:float.Parse(ParticleSize.text));
            //Debug.Log("ParticleSizeValueChanged");
        }

        public void DistanceValueChanged()
        {
            var result = 0f;
            if (float.TryParse(Distance.text, out result))
            {
                _vfx.transform.position = new Vector3(0, 0, result);
            }
            //    _vfx.transform.position = string.IsNullOrEmpty(Distance.text) ? new Vector3(0,0,1) : new Vector3(0, 0, float.Parse(Distance.text));
            //Debug.Log("DistanceValueChanged");
        }
    }
}
