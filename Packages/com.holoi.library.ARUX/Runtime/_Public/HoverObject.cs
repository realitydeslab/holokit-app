using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Library.ARUX
{
    public class HoverObject : MonoBehaviour
    {
        private readonly List<HoverableObject> _hoverableObjectList = new();

        protected virtual void Awake()
        {
            HoverableObject.OnHoverableObjectEnabled += OnHoverableObjectEnabled;
            HoverableObject.OnHoverableObjectDisabled += OnHoverableObjectDisabled;
        }

        protected virtual void OnDestroy()
        {
            HoverableObject.OnHoverableObjectEnabled -= OnHoverableObjectEnabled;
            HoverableObject.OnHoverableObjectDisabled -= OnHoverableObjectDisabled;
        }

        private void OnHoverableObjectEnabled(HoverableObject hoverableObject)
        {
            _hoverableObjectList.Add(hoverableObject);
        }

        private void OnHoverableObjectDisabled(HoverableObject hoverableObject)
        {
            _hoverableObjectList.Remove(hoverableObject);
        }

        protected virtual void FixedUpdate()
        {
            var list = new List<HoverableObject>(_hoverableObjectList);
            foreach (var hoverableObject in list)
            {
                if (Vector3.Distance(transform.position, hoverableObject.CenterPosition) < hoverableObject.Radius)
                {
                    hoverableObject.OnLoaded(Time.fixedDeltaTime);
                }
                else
                {
                    hoverableObject.OnUnloaded(Time.fixedDeltaTime);
                }
            }
        }
    }
}