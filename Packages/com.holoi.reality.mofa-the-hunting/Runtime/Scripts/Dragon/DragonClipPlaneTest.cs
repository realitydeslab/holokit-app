using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Holoi.Reality.MOFATheHunting
{
    public class DragonClipPlaneTest : MonoBehaviour
    {
        public Transform Portal;
        UnkaDragonController Controller;
        // Start is called before the first frame update
        void Start()
        {
            Controller = GetComponent<UnkaDragonController>();
        }

        // Update is called once per frame
        void Update()
        {
            Controller.ClipPlane = Portal.transform.forward * -1f;
            Controller.ClipPlaneHeihgt = Portal.transform.position.magnitude;
            //Controller.ClipPlaneHeihgt = Portal.transform.position.magnitude * Mathf.Abs(Mathf.Cos(Portal.transform.eulerAngles.y));
        }
    }
}
