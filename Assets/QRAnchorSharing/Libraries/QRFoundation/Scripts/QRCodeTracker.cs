using System;
using System.Collections;
using System.Collections.Generic;
using CSQR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

namespace QRFoundation
{
    [Serializable]
    public enum SizeDeterminationMode
    {
        Fixed = 0,
        Map = 1,
        Function = 2,
        FunctionWithOffset = 3
    }

    [Serializable]
    public enum InversionAttempt
    {
        Invert = 0,
        DontInvert = 1
    }

    [Serializable]
    public class WidthMapping
    {
        public string contains;
        public float width;
    }

    [Serializable]
    public class OnCodeDetectedEvent : UnityEvent<string> { };

    [Serializable]
    public class OnCodeLostEvent : UnityEvent { };

    [Serializable]
    public class OnStabilizeFailure : UnityEvent<string> { };

    [Serializable]
    public class OnCodeRegisteredEvent : UnityEvent<string, GameObject> { };

    [Serializable]
    public class OnPoseRegisteredEvent : UnityEvent<string, Pose> { };

    public delegate float GetWidthDelegate(string codeContent);
    public delegate void GetWidthAndOffsetDelegate(string serialized, out Pose pose, out float width, out int anchorId, out string rest);

    [Serializable]
    public enum TrackingState
    {
        Searching,
        Stabilizing,
        Registered
    }

    public class QRCodeTracker : MonoBehaviour
    {
        public struct PoseResult
        {
            public Pose pose;
            public Vector3[] corners;
            public float weight;

            public static float Difference(PoseResult a, PoseResult b)
            {
                return Vector3.Angle(a.pose.forward, b.pose.forward);
                /*
                return Vector3.Distance(a.pose.forward, b.pose.forward);
                return
                    Vector3.Distance(a.corners[0], b.corners[0]) +
                    Vector3.Distance(a.corners[1], b.corners[1]) +
                    Vector3.Distance(a.corners[2], b.corners[2]) +
                    Vector3.Distance(a.corners[3], b.corners[3]);
                    */
            }
        }

        // **************************
        // ** INSPECTOR PROPERTIES **
        // **************************
        [HideInInspector]
        public SizeDeterminationMode sizeDetermination = SizeDeterminationMode.Fixed;

        [HideInInspector]
        public float codeWidth = 0;

        [HideInInspector]
        public List<WidthMapping> codeWidths = new List<WidthMapping>(new WidthMapping[] { new WidthMapping { contains = "", width = 0 } });

        [HideInInspector]
        public GetWidthDelegate getWidthDelegate;

        [HideInInspector]
        public GameObject prefab;

        [HideInInspector]
        public InversionAttempt inversionAttempt = InversionAttempt.Invert;

        [HideInInspector]
        public OnCodeDetectedEvent onCodeDetected;
        [HideInInspector]
        public OnCodeRegisteredEvent onCodeRegistered;
        [HideInInspector]
        public OnPoseRegisteredEvent onPoseRegistered;
        [HideInInspector]
        public OnCodeLostEvent onCodeLost;
        [HideInInspector]
        public OnStabilizeFailure onStabilizeFailure;

        [HideInInspector]
        public float maxScanResolution = 500 * 500;

        [HideInInspector]
        public float searchScanInterval = 0.5f;

        [HideInInspector]
        public float stabilizeScanInterval = 0f;

        [HideInInspector]
        public float refineScanInterval = 0.1f;

        [HideInInspector]
        public float codeLossTimeout = 5f;

        // TODO
        // [SerializeField]
        // private bool targetMoving = false;

        [HideInInspector]
        public float truncateTimeout = float.PositiveInfinity;

        [HideInInspector]
        public bool debugMode = false;

        [HideInInspector]
        public float requiredPrecision = 1.5f;

        [HideInInspector]
        public bool showLifecycleEvents = true;
        [HideInInspector]
        public bool showAdvanedSettings = false;

        private Camera cam;

        private AABB lastBounds = null;
        private AABB lastSuccessfulBounds = null;
        private AABB lastScan = null;


        // ************
        // ** TIMING **
        // ************
        protected int MAX_FAILS_UNTIL_UNFOCUS = 5;
        private int focusFails = 0;

        protected int MAX_FAILS_UNTIL_SEARCH = 10;
        private int stabilizeFails = 0;

        // ***************************
        // *** QR Anchor additions ***
        // ***************************
        [HideInInspector]
        public GetWidthAndOffsetDelegate getWidthAndOffsetDelegate;
        [HideInInspector]
        public bool restrictY = false;
        [HideInInspector]
        public string lastMeta = "";
        private readonly float offsetDamping = 2.5f;

        // **********************
        // ** RAW SCAN RESULTS **
        // **********************
        private float lastSearch = 0;
        private float lastStabilize = 0;
        private float lastRefine = 0;
        [HideInInspector]
        public float lastPrecision = 0;
        [HideInInspector]
        public float stabilizingTime = 0;
        //private float lastStdDev = 0;
        private float lastFoundTime = 0;
        private float registeredStdDev = float.MaxValue;
        private string lastContent = null;
        private QRScanResult lastScanResult = null;


        // *****************
        // ** DEBUG STUFF **
        // *****************
        private float lastScanStart = 1;
        private float lastScanDuration = 1;
        private float lastCalcStart = 1;
        private float lastCalcDuration = 1;

        private int maxDebugBubbles = 10;
        private int debugBubblesPointer = 0;
        private GameObject[] debugBubbles;

        protected int maxSamples = 500;
        private readonly float confidence = 1f;
        private PoseResult[] samples;
        private int pointer = 0;

        public TrackingState trackingState { get; private set; } = TrackingState.Searching;

        // **************************
        // ** REFINED SCAN RESULTS **
        // **************************
        [HideInInspector]
        public Pose intermediate { get; private set; }
        [HideInInspector]
        public Pose intermediateSmoothed { get; private set; }
        [HideInInspector]
        public Pose registeredPose { get; private set; }
        [HideInInspector]
        public string registeredString { get; private set; } = null;
        [HideInInspector]
        public float lastWidth { get; private set; }
        [HideInInspector]
        public GameObject registeredGameObject;

        // ********************
        // ** MULTITHREADING **
        // ********************
        // Problem: I want to perform the scanning in a background thread to not freeze the UI, but eventually
        // the results have to be returned to the main thread. Unity provides no implementation for this.
        // My quick solution: When the scanning is done, the callback function is assigned to a variable.
        // This variable is checked each frame in the "Update" function (main thread) and executed if set.
        private Action qrScanCallback = null;
        private bool scanning = false;

        private GameObject cameraGameObject;
        private Camera camCopy;

        public void Start()
        {
            cam = GetComponent<Camera>();
            cameraGameObject = new GameObject();
            camCopy = cameraGameObject.AddComponent<Camera>();

            ResetSamples();

            debugBubbles = new GameObject[maxDebugBubbles];

            PNP.debugMode = this.debugMode;
        }

        public void Reset()
        {
            stabilizeFails = 0;
            focusFails = 0;
            ResetSamples();
            trackingState = TrackingState.Searching;
            lastContent = null;
        }

        /// <summary>
        /// Clear the data of all samples and reset the pointer to position 0.
        /// </summary>
        private void ResetSamples()
        {
            samples = new PoseResult[maxSamples];

            for (int i = 0; i < maxSamples; i++)
            {
                samples[i] = new PoseResult
                {
                    pose = new Pose(),
                    corners = new Vector3[] {
                        new Vector3(),
                        new Vector3(),
                        new Vector3(),
                        new Vector3(),
                    },
                    weight = 0,
                };
            }
            pointer = 0;
        }

        public void Update()
        {
            // Execute the "scan complete callback" if set.
            // Unset afterwards.
            if (qrScanCallback != null)
            {
                try { 
                    qrScanCallback();
                } catch (Exception e) {
                    Debug.LogError(e);
                }
                qrScanCallback = null;
            }
            // Trigger the tracking only if not currently ongiong in the background thread.
            if (!scanning)
            {
                switch (trackingState)
                {
                    case TrackingState.Searching:
                        if (lastSearch < Time.timeSinceLevelLoad - searchScanInterval)
                        {
                            lastSearch = Time.timeSinceLevelLoad;
                            SearchStep();
                        }
                        break;
                    case TrackingState.Stabilizing:
                        if (lastStabilize < Time.timeSinceLevelLoad - stabilizeScanInterval)
                        {
                            lastStabilize = Time.timeSinceLevelLoad;
                            StabilizeStep();
                        }
                        break;
                    case TrackingState.Registered:
                        if (lastRefine < Time.timeSinceLevelLoad - refineScanInterval)
                        {
                            lastRefine = Time.timeSinceLevelLoad;
                            RefineStep();
                        }
                        break;
                }
            }

            // While a code is being tracked, smoothly interpolate the pose of the output game object to the updated scan results.
            if (registeredString != null)
            {
                registeredGameObject.transform.position = Vector3.Lerp(registeredGameObject.transform.position, registeredPose.position, 0.2f);
                registeredGameObject.transform.rotation = Quaternion.Slerp(registeredGameObject.transform.rotation, registeredPose.rotation, 0.2f);
            }
        }

        /// <summary>
        /// Perform a step in "Searching" state.
        /// </summary>
        private void SearchStep()
        {
            RecordSample((bool success) => {
                if (success)
                {
                    trackingState = TrackingState.Stabilizing;
                    onCodeDetected.Invoke(lastContent);
                }
            });
        }

        /// <summary>
        /// Perform a step in "Stabilizing" state.
        /// </summary>
        private void StabilizeStep()
        {
            RecordSample((bool success) =>
            {
                if (success)
                {
                    stabilizeFails = 0;
                    this.stabilizingTime = Time.timeSinceLevelLoad - lastSearch;
                    if ((lastPrecision < requiredPrecision || (Time.timeSinceLevelLoad - lastSearch) > truncateTimeout)
                    && (registeredString == null || !registeredString.Equals(lastContent)))
                    {
                        Register(intermediateSmoothed, lastContent);
                        registeredGameObject.transform.localScale = Vector3.one * lastWidth;
                        trackingState = TrackingState.Registered;
                    }
                }
                else
                {
                    if (++stabilizeFails > MAX_FAILS_UNTIL_SEARCH)
                    {
                        stabilizeFails = 0;
                        focusFails = 0;
                        ResetSamples();
                        trackingState = TrackingState.Searching;
                        onStabilizeFailure.Invoke(lastContent);
                    }
                }
            });
        }

        /// <summary>
        /// Perform a step in "Refining" state.
        /// </summary>
        private void RefineStep()
        {
            string lastRegisteredString = this.registeredString;
            RecordSample((bool success) =>
            {
                if (success)
                {
                    if (!lastRegisteredString.Equals(this.lastContent))
                    {
                        Unregister();
                        trackingState = TrackingState.Stabilizing;
                    }
                }
                else
                {
                    if (lastFoundTime < Time.realtimeSinceStartup - codeLossTimeout)
                    {
                        Unregister();
                        ResetSamples();
                        trackingState = TrackingState.Searching;
                    }
                }
            });
        }

        /// <summary>
        /// Triggers a scan.
        /// </summary>
        /// <param name="callback">Will be called with true/false if a code was detected/not detected.</param>
        private void RecordSample(Action<bool> callback)
        {
            lastScanStart = Time.realtimeSinceStartup;
            StopWatch watch = new StopWatch("Record");
            watch.Start();

            camCopy.CopyFrom(cam);

            // By default, search entire sceen
            AABB scanArea = new AABB(0, 0, Screen.width, Screen.height);

            // If there is a region in which a code was found previously, look there
            //if (lastBounds != null && sizeDetermination != SizeDeterminationMode.FunctionWithOffset)
            //{
            //    int smaller = Math.Min(Screen.width, Screen.height);
            //    int margin = smaller / 10;
            //    scanArea = new AABB
            //    {
            //        x = Math.Max(0, lastBounds.x - margin),
            //        y = Math.Max(0, lastBounds.y - margin),
            //        x2 = Math.Min(Screen.width, lastBounds.x2 + margin),
            //        y2 = Math.Min(Screen.height, lastBounds.y2 + margin)
            //    };
            //}

            // Downsize the search area to keep the amount of pixels scanned somewhat consistent
            float downsize = Math.Max(Mathf.Sqrt((scanArea.w * scanArea.h) / maxScanResolution), 1);
            AABB scaledArea = scanArea / downsize;
            scanArea = scaledArea * downsize;

            lastScan = scanArea;
            // Copy the camera texture of the search region into a texture
            //GetComponent<ARCameraManager>().TryAcquireLatestCpuImage(out var cpuImage);
            Texture2D texture = RTImage(scanArea, scaledArea);
            downsize = ((float)scanArea.y2) / scaledArea.y2;
            watch.Round("Snapshot");
            // Convert image to the QR code scanner's format
            byte[] byteData = new byte[scaledArea.w * scaledArea.h * 4];
            Color32[] colors = texture.GetPixels32();
            /*
            for (int x = 0; x < scaledArea.w; x++)
            {
                for (int y = 0; y < scaledArea.h; y++)
                {
                    int yt = scaledArea.h - y - 1;
                    Color32 color = colors[yt * scaledArea.w + x];
                    byteData[(y * scaledArea.w + x) * 4 + 0] = color.r;
                    byteData[(y * scaledArea.w + x) * 4 + 1] = color.g;
                    byteData[(y * scaledArea.w + x) * 4 + 2] = color.b;
                }
            }
            */
            Destroy(texture);
            watch.Round("Format");

            /*
            bool scanOriginal = false;
            bool scanInverted = false;
            if (trackingState == TrackingState.Searching)
            {
                scanOriginal = inversionAttempt != InversionAttempt.OnlyInvert;
                scanInverted = inversionAttempt != InversionAttempt.DontInvert;
            }
            else if (lastScanResult != null)
            {
                scanOriginal = !lastScanResult.inverted;
                scanInverted = lastScanResult.inverted;
            }*/

            // Do the scan
            scanning = true;
            CsQr.ScanAsyncZXing(colors, scaledArea.w, scaledArea.h, inversionAttempt == InversionAttempt.Invert, (QRScanResult scanResult) =>
            {
                // OUTSIDE MAIN THREAD!
                scanning = false;
                // Scan is done. Assign the callback.
                qrScanCallback = () =>
                {
                    // INSIDE MAIN THREAD
                    if (scanResult != null)
                    {
                        watch.Round("Scan");
                        lastScanDuration = Time.realtimeSinceStartup - lastScanStart;
                        lastCalcStart = Time.realtimeSinceStartup;

                        if (lastBounds != null)
                        {
                            // Code has been detected inside the search area,
                            // therefore reset the counter for consecutive detection failures
                            focusFails = 0;
                        }

                        lastScanResult = scanResult;
                        lastContent = scanResult.content;
                        lastFoundTime = Time.realtimeSinceStartup;

                        // The locations of the feature points of the code (finder- and alignment patterns)
                        QRLocation location = scanResult.position;
                        // The locations of the corners of the code
                        QRLocation bounds = scanResult.bounds;

                        // Map the locations of the feature points back onto the screen coordinates
                        Vector2 ul = new Vector2(scanArea.x + (float)location.topLeft.x * downsize, scanArea.y + scanArea.h - (float)location.topLeft.y * downsize);
                        Vector2 ur = new Vector2(scanArea.x + (float)location.topRight.x * downsize, scanArea.y + scanArea.h - (float)location.topRight.y * downsize);
                        Vector2 lr = new Vector2(scanArea.x + (float)location.alignmentPattern.x * downsize, scanArea.y + scanArea.h - (float)location.alignmentPattern.y * downsize);
                        Vector2 ll = new Vector2(scanArea.x + (float)location.bottomLeft.x * downsize, scanArea.y + scanArea.h - (float)location.bottomLeft.y * downsize);

                        // Same for the corners
                        Vector2 ulb = new Vector2(scanArea.x + (float)bounds.topLeft.x * downsize, scanArea.y + scanArea.h - (float)bounds.topLeft.y * downsize);
                        Vector2 urb = new Vector2(scanArea.x + (float)bounds.topRight.x * downsize, scanArea.y + scanArea.h - (float)bounds.topRight.y * downsize);
                        Vector2 lrb = new Vector2(scanArea.x + (float)bounds.alignmentPattern.x * downsize, scanArea.y + scanArea.h - (float)bounds.alignmentPattern.y * downsize);
                        Vector2 llb = new Vector2(scanArea.x + (float)bounds.bottomLeft.x * downsize, scanArea.y + scanArea.h - (float)bounds.bottomLeft.y * downsize);

                        // Calculate the bounding box of the code on screen
                        lastSuccessfulBounds = lastBounds;
                        lastBounds = new AABB((int)ulb.x, (int)ulb.y);
                        lastBounds.Encapsulate((int)urb.x, (int)urb.y);
                        lastBounds.Encapsulate((int)lrb.x, (int)lrb.y);
                        lastBounds.Encapsulate((int)llb.x, (int)llb.y);

                        // Determine the size of the code based on the selected method
                        float targetWidth = 0;
                        Pose offset = Pose.identity;
                        switch (sizeDetermination)
                        {
                            case SizeDeterminationMode.Fixed:
                                targetWidth = codeWidth;
                                break;
                            case SizeDeterminationMode.Map:
                                foreach (WidthMapping mapping in codeWidths)
                                {
                                    if (lastContent.Contains(mapping.contains))
                                    {
                                        targetWidth = mapping.width;
                                        break;
                                    }
                                }
                                break;
                            case SizeDeterminationMode.Function:
                                targetWidth = getWidthDelegate(lastContent);
                                break;
                            case SizeDeterminationMode.FunctionWithOffset:
                                getWidthAndOffsetDelegate(lastContent, out offset, out targetWidth, out int anchorId, out string restContent);
                                lastContent = anchorId + "";
                                lastMeta = restContent;
                                break;
                        }
                        if (targetWidth == 0)
                            return;

                        lastWidth = targetWidth;

                        float moduleSize = targetWidth / location.dimension;

                        // Convert samples
                        List<Sample> s = new List<Sample>(scanResult.samples.Length);
                        foreach (Sample sample in scanResult.samples)
                        {
                            Point p = sample.image;
                            // Convert image location from the search region to the screen.
                            Vector2 pp = new Vector2(scanArea.x + (float)p.x * downsize, scanArea.y + scanArea.h - (float)p.y * downsize);
                            Ray ppRay = camCopy.ScreenPointToRay(pp);
                            // Convert the location within the QR code to real-world size.
                            Point param = new Point(sample.param.x * moduleSize - targetWidth / 2, sample.param.y * moduleSize - targetWidth / 2);
                            s.Add(new Sample(new Point(pp.x, pp.y), param));
                        }

                        List<PoseResult> poses = PNP.PNPoses(s.ToArray(), targetWidth, camCopy, offset);
                        float maxWeight = poses.Min((PoseResult x, PoseResult y) => x.weight - y.weight < 0 ? 1 : -1).weight;
                        float targetStdDev = lastWidth;
                        if (sizeDetermination == SizeDeterminationMode.FunctionWithOffset)
                        {
                            // Take the offset into account. The values are expected
                            // to fluctuate more as the offset gets larger
                            targetStdDev *= 1 + offset.position.magnitude * offsetDamping;
                        }
                        else
                        {
                            targetStdDev *= 4;
                        }
                        foreach (PoseResult poseRes in poses)
                        {
                            if (debugMode)
                            {
                                float color = poseRes.weight / maxWeight;
                                Debug.DrawRay(poseRes.pose.position, poseRes.pose.forward, new Color(color, color, color));
                            }
                            PoseResult poseResCpy = poseRes;
                            if (restrictY)
                            {
                                poseResCpy.pose.rotation = Quaternion.FromToRotation(poseResCpy.pose.up, Vector3.up) * poseResCpy.pose.rotation;
                            }
                            samples[pointer] = poseRes;
                            pointer = (pointer + 1) % maxSamples;
                        }

                        Pose smoothed = Smoothed(out float stdDev);

                        float precision = stdDev / targetStdDev;
                        // Register again if the stdDev is lower than before or the pose has
                        // drastically changed from the last registered one.
                        bool insecure = stdDev > targetStdDev * 2;
                        bool shiftedPosition = Vector3.Distance(smoothed.position, registeredPose.position) > targetStdDev / 6;
                        bool shiftedOrientation = Quaternion.Angle(smoothed.rotation, registeredPose.rotation) > 10;

                        if (insecure || ((shiftedPosition || shiftedOrientation) && precision < requiredPrecision))
                        {
                            // Reset current position of the standard deviation is very high
                            // or the pose has changed significantly with certainty.
                            registeredStdDev = float.MaxValue;
                        }

                        if (stdDev < registeredStdDev && precision < requiredPrecision)
                        {
                            if (registeredString != null)
                            {
                                registeredStdDev = stdDev;
                                registeredPose = smoothed;
                            }
                        }

                        lastPrecision = precision;
                        intermediateSmoothed = smoothed;

                        // Debug bubbles
                        if (debugMode)
                        {
                            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                            sphere.transform.localScale = Vector3.one * 0.005f;
                            sphere.GetComponent<MeshRenderer>().material.color = Color.red;
                            sphere.transform.position = intermediateSmoothed.position;

                            debugBubbles[debugBubblesPointer] = sphere;
                            debugBubblesPointer = (debugBubblesPointer + 1) % maxDebugBubbles;
                            if (debugBubbles[debugBubblesPointer] != null)
                            {
                                Destroy(debugBubbles[debugBubblesPointer]);
                            }
                            debugBubbles[debugBubblesPointer] = null;
                        }

                        lastCalcDuration = Time.realtimeSinceStartup - lastCalcStart;
                        watch.Round("Calculate");

                        callback(true);
                    }
                    else
                    {
                        if (lastBounds != null && ++focusFails > MAX_FAILS_UNTIL_UNFOCUS)
                        {
                            lastBounds = null;
                            focusFails = 0;
                        }
                        callback(false);
                    }
                    //watch.PrintResults();
                };
            });
        }

        /// <summary>
        /// For debugging purposes
        /// </summary>
        private void OnGUI()
        {
            int i = 0;
            int dist = 30;
            GUI.skin.textField.fontSize = 20;
            if (debugMode && lastContent != null)
            {
                GUI.TextField(new Rect(30, 10 + (i++) * dist, 700, 30), "QR Content: "+lastContent);
                GUI.TextField(new Rect(30, 10 + (i++) * dist, 700, 30), "Tracking state: "+trackingState.ToString());
                GUI.TextField(new Rect(30, 10 + (i++) * dist, 700, 30), "Precision: " + lastPrecision);
                GUI.TextField(new Rect(30, 10 + (i++) * dist, 700, 30), "Samples: " + (lastScanResult != null ? lastScanResult.samples.Length + "" : "/"));
                GUI.TextField(new Rect(30, 10 + (i++) * dist, 700, 30), "Scan time: " + lastScanDuration);
                GUI.TextField(new Rect(30, 10 + (i++) * dist, 700, 30), "Calc time: " + lastCalcDuration);
            }
        }

        private Pose Smoothed(out float stdDev)
        {
            // Calculate average pose
            Vector3[] avgPositions = new Vector3[4];
            int numSamples = 0;
            for (int i = 0; i < maxSamples; i++)
            {
                if (samples[i].weight <= 0)
                {
                    continue;
                }
                numSamples++;
                avgPositions[0] += samples[i].corners[0];
                avgPositions[1] += samples[i].corners[1];
                avgPositions[2] += samples[i].corners[2];
                avgPositions[3] += samples[i].corners[3];
            }
            avgPositions[0] /= numSamples;
            avgPositions[1] /= numSamples;
            avgPositions[2] /= numSamples;
            avgPositions[3] /= numSamples;

            // Calculate standard deviations
            stdDev = 0;
            for (int i = 0; i < maxSamples; i++)
            {
                if (samples[i].weight <= 0)
                {
                    continue;
                }
                float deviation =
                    Vector3.Distance(avgPositions[0], samples[i].corners[0]) +
                    Vector3.Distance(avgPositions[1], samples[i].corners[1]) +
                    Vector3.Distance(avgPositions[2], samples[i].corners[2]) +
                    Vector3.Distance(avgPositions[3], samples[i].corners[3]);
                stdDev += Mathf.Pow(deviation, 2);
            }
            stdDev = Mathf.Sqrt(stdDev / numSamples);

            // Calculate average again, but without outliers
            Vector3 avgLocation = samples[0].pose.position;
            Quaternion avgRotation = samples[0].pose.rotation;
            float weightSum = 0;
            int outliers = 0;
            for (int i = 0; i < maxSamples; i++)
            {
                if (samples[i].weight <= 0)
                {
                    continue;
                }
                float deviation =
                    Vector3.Distance(avgPositions[0], samples[i].corners[0]) +
                    Vector3.Distance(avgPositions[1], samples[i].corners[1]) +
                    Vector3.Distance(avgPositions[2], samples[i].corners[2]) +
                    Vector3.Distance(avgPositions[3], samples[i].corners[3]);
                if (deviation <= confidence * stdDev)
                {
                    weightSum += samples[i].weight;
                    float l = samples[i].weight / weightSum;
                    avgLocation = avgLocation * (1 - l) + samples[i].pose.position * l;
                    avgRotation = Quaternion.Slerp(avgRotation, samples[i].pose.rotation, l);
                }
                else
                {
                    outliers++;
                }
            }

            //            Debug.Log("outliers: " + (maxSamples - counted));
            Pose res = new Pose(avgLocation, avgRotation);
            return res;
        }

        /// <summary>
        /// Unregister the currently registered code.
        /// </summary>
        public void Unregister()
        {
            onCodeLost.Invoke();
            Destroy(registeredGameObject);
            registeredString = null;
        }

        /// <summary>
        /// Register the QR code's pose and content string.
        /// </summary>
        /// <param name="pose">QR code Pose</param>
        /// <param name="content">QR code content</param>
        private void Register(Pose pose, string content)
        {
            if (prefab != null)
            {
                registeredGameObject = Instantiate(prefab);
            }
            else
            {
                registeredGameObject = new GameObject();
            }

            registeredGameObject.transform.parent = transform.parent;
            registeredGameObject.transform.position = pose.position;
            registeredGameObject.transform.rotation = pose.rotation;

            registeredString = content;
            registeredPose = pose;

            onCodeRegistered.Invoke(content, registeredGameObject);
            onPoseRegistered.Invoke(content, pose);
        }

        /// <summary>
        /// Takes a "screenshot" of the camera's background render texture.
        /// Base on https://docs.unity3d.com/ScriptReference/Camera.Render.html.
        /// </summary>
        /// <returns>The mage.</returns>
        /// <param name="region">Bounds.</param>
        /// <param name="scaledRegion">Rect.</param>
        private Texture2D RTImage(AABB region, AABB scaledRegion)
        {
            // Hide everything; Only render the background (camera) texture!
            int oldCullingMask = cam.cullingMask;
            cam.cullingMask = debugMode ? 1 << 8 : 0;

            float scale = (float)region.y2 / scaledRegion.y2;
            int width = (int)(Screen.width / scale);
            int height = (int)(Screen.height / scale);

            RenderTexture rt = new RenderTexture(width, height, 24);
            cam.targetTexture = rt; // Create new renderTexture and assign to camera
            Texture2D screenShot = new Texture2D(scaledRegion.w, scaledRegion.h, TextureFormat.RGB24, false); //Create new texture

            cam.Render();

            RenderTexture.active = rt;
            screenShot.ReadPixels(scaledRegion.ToRect(), 0, 0); // Apply pixels from camera onto Texture2D

            cam.targetTexture = null;
            RenderTexture.active = null; // Clean
            Destroy(rt); // Free memory

            // Restore previous culling mask
            cam.cullingMask = oldCullingMask;
            return screenShot;
        }
    }
}