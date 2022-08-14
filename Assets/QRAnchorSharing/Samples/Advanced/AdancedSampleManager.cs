using System.Collections.Generic;
using QRFoundation;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class AdancedSampleManager : MonoBehaviour
{
    enum State
    {
        Placing,
        Placed,
        Sharing,
        Scanning
    }

    private static readonly float codeScale = 0.8f;

    public GameObject anchorPrefab;

    public Texture2D guiBack;
    public Texture2D guiBar;
    public Texture2D guiRect;

    private ARRaycastManager rayCastManager;
    private ARAnchorManager anchorManager;
    private ARPlaneManager planeManager;
    private QRAnchorSender anchorSender;
    private QRAnchorReceiver anchorReceiver;

    private State state = State.Placing;
    private GameObject anchorGameObject;
    private ARAnchor sceneAnchor;
    private Texture2D currentCodeTexture;

    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;

        this.rayCastManager = this.transform.parent.GetComponent<ARRaycastManager>();
        this.planeManager = this.transform.parent.GetComponent<ARPlaneManager>();
        this.anchorManager = this.transform.parent.GetComponent<ARAnchorManager>();
        this.anchorSender = this.GetComponent<QRAnchorSender>();
        this.anchorReceiver = this.GetComponent<QRAnchorReceiver>();

        this.anchorGameObject = Instantiate(anchorPrefab);

        this.anchorReceiver.onCodeDetected.AddListener((string _) => 
        {
            this.state = State.Scanning;
        });
        this.anchorReceiver.onAnchorReceived.AddListener((int anchorId, string meta, ARAnchor anchor) => 
        {
            Debug.Log("Metadata is: " + meta);
            this.ReceiveSharedAnchor(anchor);
        });
        this.anchorReceiver.onStabilizeFailure.AddListener((string _) => {
            this.state = State.Placing;
        });
    }

    void Update()
    {
        switch (state)
        {
            case State.Placing:
                this.PlacingStep();
                break;
            case State.Placed:
                this.PlacedStep();
                break;
            case State.Sharing:
                this.SharingStep();
                break;
            case State.Scanning:
                this.ScanningStep();
                break;
        }
    }

    private void PlacingStep()
    {
        // Find the closest plane
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

        bool hitPlane = false;
        ARRaycastHit closestHit = new ARRaycastHit();

        if (this.rayCastManager.Raycast(new Vector2(Screen.width / 2, Screen.height / 2), hits))
        {
            float closestDistance = float.PositiveInfinity;
            foreach (ARRaycastHit hit in hits)
            {
                if ((hit.hitType & UnityEngine.XR.ARSubsystems.TrackableType.Planes) != 0)
                {
                    hitPlane = true;
                    float distance = Vector3.Distance(hit.pose.position, this.transform.position);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestHit = hit;
                    }
                }
            }
        }

        if (hitPlane)
        {
            // If a plane is in front, align target with it.
            ARPlane plane = planeManager.GetPlane(closestHit.trackableId);

            this.anchorGameObject.transform.position = closestHit.pose.position;
            this.anchorGameObject.transform.rotation = Quaternion.LookRotation(-(new Vector3(this.transform.forward.x, 0, this.transform.forward.z)).normalized, Vector3.up);
            this.anchorGameObject.SetActive(true);
        }
        else
        {
            // If no plane is in front, keep target in front of the camera.
            this.anchorGameObject.transform.position = this.transform.position + this.transform.forward * 1;
            this.anchorGameObject.transform.rotation = Quaternion.LookRotation(-(new Vector3(this.transform.forward.x, 0, this.transform.forward.z)).normalized, Vector3.up);
            this.anchorGameObject.SetActive(!this.anchorGameObject.activeSelf); // Blink
        }
    }

    private void PlacedStep()
    {
    }

    private void SharingStep()
    {
    }

    private void ScanningStep()
    {
    }

    private void OnGUI()
    {
        switch (state)
        {
            case State.Placing:
                this.PlacingStepGUI();
                break;
            case State.Placed:
                this.PlacedStepGUI();
                break;
            case State.Sharing:
                this.SharingStepGUI();
                break;
            case State.Scanning:
                this.ScanningStepGUI();
                break;
        }

        GUI.TextArea(new Rect(20, 20, 500, 70),
            "1. Move your device around until a horizontal plane is detected (object sticks to it).\n" +
            "2. Click 'Place'.\n" +
            "3. Click 'Share'.\n" +
            "4. With you second device, scan the code."
        );
    }

    private void PlacingStepGUI()
    {
        int width = 200;
        int height = 50;
        int padding = 50;
        if (GUI.Button(new Rect(Screen.width - padding - width, Screen.height - padding - height, width, height), "Place"))
        {
            Place();
        }
    }

    private void PlacedStepGUI()
    {
        int width = 200;
        int height = 50;
        int padding = 50;
        if (GUI.Button(new Rect(Screen.width - padding - width, Screen.height - padding - height, width, height), "Share"))
        {
            StartSharing();
        }
    }

    private void SharingStepGUI()
    {
        int width = 200;
        int height = 50;
        int padding = 50;
        if (GUI.Button(new Rect(Screen.width - padding - width, Screen.height - padding - height, width, height), "Stop Sharing"))
        {
            StopSharing();
        }

        // Draw the QR code on the screen via the GUI API.
        // Note, that it is scaled down by the codeScale variable. This has to be identical to this.anchorSender.codeScale.
        if (this.currentCodeTexture)
        {
            float codeSize = Mathf.Min(Screen.width, Screen.height) * codeScale;
            GUI.DrawTexture(
                new Rect(
                    (Screen.width - codeSize) / 2,
                    (Screen.height - codeSize) / 2,
                    codeSize,
                    codeSize
                ),
                this.currentCodeTexture
            );
        }
    }

    private void ScanningStepGUI()
    {
        // Drawing the progress indicator
        if (this.anchorReceiver.trackingState == TrackingState.Stabilizing)
        {
            float width = Screen.width / 1.1f;
            float height = (this.guiBack.height / (float)this.guiBack.width) * width;

            GUI.DrawTexture(
                new Rect(
                    Screen.width / 2 - width / 2,
                    Screen.height / 2 - height / 2,
                    width, height
                    ),
                this.guiBack
            );

            float barWidth = (352f / 500f) * width * (this.anchorReceiver.stabilizingTime / this.anchorReceiver.truncateTimeout);
            float barX = (width / this.guiBack.width) * 74;

            GUI.DrawTexture(
                new Rect(
                    Screen.width / 2 - width / 2 + barX,
                    Screen.height / 2 - height / 2,
                    barWidth, height
                    ),
                this.guiBar
            );

            float osc = Mathf.Sin(Time.timeSinceLevelLoad * 5) / 10 + 1;
            width *= osc;
            height *= osc;

            GUI.DrawTexture(
                new Rect(
                    Screen.width / 2 - width / 2,
                    Screen.height / 2 - height / 2,
                    width, height
                    ),
                this.guiRect
            );
        }
    }

    /// <summary>
    /// Place the anchor and switch to "Placed" state.
    /// </summary>
    private void Place()
    {
        this.sceneAnchor = this.anchorManager.AddAnchor(new Pose(this.anchorGameObject.transform.position, this.anchorGameObject.transform.rotation));
        this.anchorGameObject.SetActive(true);
        this.anchorGameObject.transform.parent = sceneAnchor.transform;
        this.anchorGameObject.transform.localPosition = Vector3.zero;
        this.anchorGameObject.transform.localRotation = Quaternion.identity;

        this.state = State.Placed;
    }

    /// <summary>
    /// Starts the sharing.
    /// </summary>
    private void StartSharing()
    {
        this.anchorSender.target = this.sceneAnchor.gameObject;
        this.anchorSender.metaData = "Hello World!";
        // Apply the code scale.
        // This has to be identical to the actual scale by which you end up drawing the code.
        this.anchorSender.codeScale = codeScale;
        this.anchorSender.onCodeUpdate.AddListener((Texture2D texture) => 
            {
                this.currentCodeTexture = texture;
            });
        this.anchorSender.StartSharing();

        this.state = State.Sharing;
    }

    /// <summary>
    /// Stops the sharing.
    /// </summary>
    private void StopSharing()
    {
        this.anchorSender.StopSharing();

        this.state = State.Placed;
    }

    private void ReceiveSharedAnchor(ARAnchor anchor)
    {
        this.sceneAnchor = anchor;
        this.anchorGameObject.SetActive(true);
        this.anchorGameObject.transform.parent = this.sceneAnchor.transform;
        this.anchorGameObject.transform.localPosition = Vector3.zero;
        this.anchorGameObject.transform.localRotation = Quaternion.identity;

        AttachToPlane atp = anchor.gameObject.AddComponent<AttachToPlane>();
        atp.adjustOrientation = false;
        atp.rayCastManager = this.rayCastManager;
        atp.planeManager = this.planeManager;
        atp.direction = Vector3.up;

        this.state = State.Placed;
    }
}
