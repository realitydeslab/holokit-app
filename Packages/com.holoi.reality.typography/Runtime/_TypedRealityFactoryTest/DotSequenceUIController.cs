using UnityEngine;
using Holoi.Library.HoloKitApp.UI;
using UnityEngine.VFX;
using UnityEngine.UI;
using TMPro;

namespace Holoi.Reality.Typography
{
    public class DotSequenceUIController : HoloKitAppUIPanel
    {
        public override string UIPanelName => "FactoryText";

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
            if(Spacing.text !=null)
            _vfx.SetFloat("Spacing", float.Parse(Spacing.text));
            Debug.Log("SpacingValueChanged");
        }

        public void ParticleSizeValueChanged()
        {
            if (Spacing.text != null)
                _vfx.SetFloat("Size", float.Parse(ParticleSize.text));
            Debug.Log("ParticleSizeValueChanged");
        }

        public void DistanceValueChanged()
        {
            if (Spacing.text != null)
                _vfx.transform.position = new Vector3(0,0, float.Parse(Distance.text));
            Debug.Log("DistanceValueChanged");
        }
    }
}
