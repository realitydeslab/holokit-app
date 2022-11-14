using UnityEngine;
using System.Collections.Generic;

 
namespace MalbersAnimations.Utilities
{
    public class TriggerTarget : MonoBehaviour 
    {
        public List<TriggerProxy> Proxies;
        public Collider m_collider;

        public static List<TriggerTarget> set;

        private void Awake()
        {
            if (set == null) set = new List<TriggerTarget>();
            hideFlags = HideFlags.HideInInspector;
        }

        private void OnEnable()
        {
            set.Add(this);
        }

        private void OnDisable()
        {
            if (Proxies != null)
                foreach (var p in Proxies)
                {
                    if (p != null) p.TriggerExit(m_collider, false);
                }

            Proxies = new List<TriggerProxy>();     //Reset

            set.Remove(this);
        }

        public void AddProxy(TriggerProxy trigger,Collider col)
        {
            if (Proxies == null) Proxies = new List<TriggerProxy>();
            /*if (!Proxies.Contains(trigger))*/ 
            
            Proxies.Add(trigger);
            m_collider = col;
        }

        public void RemoveProxy(TriggerProxy trigger)
        {
           /* if (Proxies.Contains(trigger)) */
            Proxies.Remove(trigger);
        }
    }
}