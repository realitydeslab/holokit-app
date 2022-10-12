using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.Rendering.Universal;

/*
Usage:
1. Attach this script to your chosen camera's game object.
2. Set that camera's Clear Flags field to Solid Color.
3. Use the inspector to set frameRate and framesToCapture
4. Choose your desired resolution in Unity's Game window (must be less than or equal to your screen resolution)
5. Turn on "Maximise on Play"
6. Play your scene. Screenshots will be saved to YourUnityProject/Screenshots by default.
*/

public class TransparentBackgroundScreenshotRecorder : MonoBehaviour
{

    #region public fields
    [Tooltip("A folder will be created with this base name in your project root")]
    public string folderBaseName = "Screenshots";
    [Tooltip("How many frames should be captured per second of game time")]
    public int frameRate = 24;
    [Tooltip("How many frames should be captured before quitting")]
    public int framesToCapture = 24;
    #endregion
    #region private fields
    private string folderName = "";
    private GameObject whiteCamGameObject;
    private Camera whiteCam;
    private UniversalAdditionalCameraData whiteCamAddData;
    private GameObject blackCamGameObject;
    private Camera blackCam;
    private UniversalAdditionalCameraData blackCamAddData;
    private Camera mainCam;
    private int videoFrame = 0; // how many frames we've rendered
    private float originalTimescaleTime;
    private bool done = false;
    private int screenWidth;
    private int screenHeight;
    [SerializeField] private Texture2D textureBlack;
    [SerializeField] private Texture2D textureWhite;
    [SerializeField] private Texture2D textureTransparentBackground;

    //private Texture2D textureTransparentBackground = new Texture2D(256, 256, TextureFormat.ARGB32, false);
    #endregion

    void Awake()
    {
        mainCam = gameObject.GetComponent<Camera>();
        CreateBlackAndWhiteCameras();
        CreateNewFolderForScreenshots();
        CacheAndInitialiseFields();
        Time.captureFramerate = frameRate;
    }

    void LateUpdate()
    {
        if (!done)
        {
            StartCoroutine(CaptureFrame());
        }
        else
        {
            Debug.Log("Complete! " + videoFrame + " videoframes rendered. File names are 0 indexed)");
            Debug.Break();
        }
    }

    IEnumerator CaptureFrame()
    {
        yield return new WaitForEndOfFrame();
        if (videoFrame < framesToCapture)
        {
            RenderCamToTexture(whiteCam, textureWhite);
            RenderCamToTexture(blackCam, textureBlack);

            CalculateOutputTexture();
            SavePng();
            videoFrame++;
            Debug.Log("Rendered frame " + videoFrame);
        }
        else
        {
            done = true;
            StopCoroutine("CaptureFrame");
        }
    }

    void RenderWiteRT2Texture()
    {

    }
    void RenderCamToTexture(Camera cam, Texture2D tex)
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        RenderTexture renderTexture = new RenderTexture(screenWidth, screenHeight, 24);

        cam.targetTexture = renderTexture;
        cam.Render();

        RenderTexture.active = renderTexture;
        WriteScreenImageToTexture(tex);

        cam.targetTexture = null;
        RenderTexture.active = null;

        Destroy(renderTexture);
        renderTexture = null;

    }

    void WriteScreenImageToTexture(Texture2D tex)
    {
        tex.ReadPixels(new Rect(0, 0, screenWidth, screenHeight), 0, 0);
        tex.Apply();
    }

    void CreateBlackAndWhiteCameras()
    {
        blackCamGameObject = (GameObject)new GameObject();
        blackCamGameObject.name = "Black Background Camera";
        blackCam = blackCamGameObject.AddComponent<Camera>();
        blackCamAddData = blackCamGameObject.AddComponent<UniversalAdditionalCameraData>();
        blackCamAddData.renderPostProcessing = true;
        blackCam.CopyFrom(mainCam);
        blackCam.backgroundColor = Color.black;
        blackCamGameObject.transform.SetParent(gameObject.transform, true);

        whiteCamGameObject = (GameObject)new GameObject();
        whiteCamGameObject.name = "White Background Camera";
        whiteCam = whiteCamGameObject.AddComponent<Camera>();
        whiteCamAddData = whiteCamGameObject.AddComponent<UniversalAdditionalCameraData>();
        whiteCamAddData.renderPostProcessing = true;
        whiteCam.CopyFrom(mainCam);
        whiteCam.backgroundColor = Color.white;
        whiteCamGameObject.transform.SetParent(gameObject.transform, true);
    }

    void CreateNewFolderForScreenshots()
    {
        // Find a folder name that doesn't exist yet. Append number if necessary.
        folderName = folderBaseName;
        int count = 1;
        while (System.IO.Directory.Exists(folderName))
        {
            folderName = folderBaseName + count;
            count++;
        }
        System.IO.Directory.CreateDirectory(folderName); // Create the folder
    }

    void CalculateOutputTexture()
    {
        Color color;
        for (int y = 0; y < textureTransparentBackground.height; ++y)
        {
            // each row
            for (int x = 0; x < textureTransparentBackground.width; ++x)
            {
                // each column
                float alpha = textureWhite.GetPixel(x, y).r - textureBlack.GetPixel(x, y).r;
                alpha = 1.0f - alpha;
                if (alpha == 0)
                {
                    color = Color.clear;
                }
                else
                {
                    color = textureBlack.GetPixel(x, y) / 1;
                }
                color.a = alpha;
                textureTransparentBackground.SetPixel(x, y, color);
            }
        }
    }

    void SavePng()
    {
        string name = string.Format("{0}/{1:D04} shot.png", folderName, videoFrame);
        var pngShot = textureTransparentBackground.EncodeToPNG();
        File.WriteAllBytes(name, pngShot);
    }

    void CacheAndInitialiseFields()
    {
        originalTimescaleTime = Time.timeScale;
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        textureBlack = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        textureWhite = new Texture2D(screenWidth, screenHeight, TextureFormat.RGB24, false);
        textureTransparentBackground = new Texture2D(screenWidth, screenHeight, TextureFormat.ARGB32, false);
    }
}