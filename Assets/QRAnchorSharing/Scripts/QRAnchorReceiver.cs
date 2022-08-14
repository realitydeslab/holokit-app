using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace QRFoundation
{
    [Serializable]
    public enum RotationMode
    {
        AllAxes = 0,
        AlignY = 1
    }

    [Serializable]
    public enum OutputMode
    {
        ManagedAnchor = 0,
        Pose = 1
    }

    [Serializable]
    public class OnAnchorReceivedEvent : UnityEvent<int, string, ARAnchor> { }

    [Serializable]
    public class OnPoseReceivedEvent : UnityEvent<int, string, Pose> { }

    public class QRAnchorReceiver : QRCodeTracker
    {
        [HideInInspector]
        public RotationMode rotationMode = RotationMode.AllAxes;

        [HideInInspector]
        public OutputMode outputMode = OutputMode.ManagedAnchor;

        [HideInInspector]
        public OnAnchorReceivedEvent onAnchorReceived;
        [HideInInspector]
        public OnPoseReceivedEvent onPoseReceived;

        [HideInInspector]
        public GameObject sessionOrigin;

        [HideInInspector]
        public float _searchScanInterval = 1;
        [HideInInspector]
        public float _truncateTimeout = 4;

        private Dictionary<int, ARAnchor> anchorsForward;
        private Dictionary<ARAnchor, int> anchorsBackward;

        private ARAnchorManager anchorManager;

        new void Start()
        {
            this.searchScanInterval = this._searchScanInterval;
            this.truncateTimeout = this._truncateTimeout;

            this.maxSamples = 200;
            this.MAX_FAILS_UNTIL_SEARCH = 20;
            this.sizeDetermination = SizeDeterminationMode.FunctionWithOffset;
            this.inversionAttempt = InversionAttempt.DontInvert;
            this.getWidthAndOffsetDelegate = QRAnchorReceiver.DecodePose;
            this.refineScanInterval = float.PositiveInfinity;
            this.onPoseRegistered.AddListener(OnPoseRegistered);
            this.anchorsForward = new Dictionary<int, ARAnchor>();
            this.anchorsBackward = new Dictionary<ARAnchor, int>();
            if (this.outputMode == OutputMode.ManagedAnchor)
            {
                try
                {
                    this.anchorManager = sessionOrigin.GetComponent<ARAnchorManager>();
                    this.anchorManager.anchorsChanged += (ARAnchorsChangedEventArgs obj) => 
                    {
                        obj.added.ForEach((anchor) =>
                        {
                            if (this.anchorsBackward.ContainsKey(anchor))
                            {
                                int anchorId = this.anchorsBackward[anchor];
                                this.onAnchorReceived.Invoke(anchorId, this.lastMeta, anchor);
                            }
                        });

                        obj.removed.ForEach((anchor) =>
                        {
                            if (this.anchorsBackward.ContainsKey(anchor))
                            {
                                int anchorId = this.anchorsBackward[anchor];
                                this.anchorsForward.Remove(anchorId);
                                this.anchorsBackward.Remove(anchor);
                            }
                        });
                    };
                }
                catch
                {
                    this.anchorManager = null;
                    Debug.LogError("No anchor manager found");
                }
            }

            if (rotationMode == RotationMode.AlignY)
            {
                this.restrictY = true;
            }

            base.Start();
        }

        new void Update()
        {
            base.Update();
        }

        private void OnPoseRegistered(string anchorIdString, Pose pose)
        {
            if (Application.isEditor)
            {
                return;
            }
            int anchorId = Int32.Parse(anchorIdString);

            if (this.outputMode == OutputMode.Pose)
            {
                this.onPoseReceived.Invoke(anchorId, this.lastMeta, pose);
            } else if (this.outputMode == OutputMode.ManagedAnchor)
            {
                if (!this.anchorsForward.ContainsKey(anchorId))
                {
                    ARAnchor anchor = this.anchorManager.AddAnchor(pose);
                    this.anchorsForward.Add(anchorId, anchor);
                    this.anchorsBackward.Add(anchor, anchorId);
                }
            }
        }

        public static void DecodePose(string serialized, out Pose offset, out float width, out int anchorId, out string rest)
        {
            //if (serialized.Equals("normal_zugbeewqztkjhgpauhkkztg"))
            //{
            //    Vector3 position = new Vector3(0, 0.01f, 0);
            //    Quaternion rotation = Quaternion.identity;
            //    offset = new Pose(position, rotation);
            //    offset = Pose.identity;
            //    width = 0.04616129032258f;
            //    anchorId = 42;
            //    rest = serialized;
            //}
            //else
            //{
                try
                {
                    EncodeDecode.DecodePose(serialized, out offset, out width, out anchorId, out rest);
                }
                catch
                {
                    offset = new Pose();
                    width = 0;
                    anchorId = 0;
                    rest = "";
                }
            //}
        }
    }
}