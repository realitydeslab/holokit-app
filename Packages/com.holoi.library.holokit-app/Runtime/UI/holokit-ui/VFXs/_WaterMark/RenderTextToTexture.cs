using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextToTexture : MonoBehaviour
{
    public MeshRenderer mr;

    public RenderTexture tempTex;

    [Header("Update Text Texture")]
    public RenderTexture sampleTex;
    public Camera targetCam;
    public TMPro.TMP_Text inputText;

    void Start()
    {

    }

    public void OnClickTest()
    {
        var size = new Vector2(100, 100);
        tempTex = RenderTexture.GetTemporary(1000, 1000, 24, RenderTextureFormat.ARGB32);

        GL.PushMatrix(); // copy current camera matrix settings
        GL.LoadIdentity();
        var proj = Matrix4x4.Ortho(-size.x / 2.0f, size.x / 2.0f, -size.x / 2.0f, size.y / 2.0f, -10, 100);
        GL.LoadProjectionMatrix(proj);
        RenderTexture currentActiveRT = RenderTexture.active;
        Graphics.SetRenderTarget(tempTex);
        GL.Clear(false, true, new UnityEngine.Color(0, 0, 0, 0));

        inputText.GetComponent<Renderer>().material.SetPass(0);
        Graphics.DrawMeshNow(inputText.mesh, Matrix4x4.identity);
        GL.PopMatrix(); // Restore camera
        RenderTexture.active = currentActiveRT;

        mr.material.mainTexture = tempTex;
    }

    public void OnSetRenderTextureAsMainTex(string text)
    {
        targetCam.gameObject.SetActive(true);
        inputText.gameObject.SetActive(true);
        inputText.text = text;

        StartCoroutine(UpdateRenderTexture());
    }

    IEnumerator UpdateRenderTexture()
    {
        yield return new WaitForEndOfFrame();

        RenderTextureDescriptor rtd = new RenderTextureDescriptor(1024, 1024);
        tempTex = new RenderTexture(rtd);

        Graphics.CopyTexture(sampleTex, tempTex);
        
        if(mr.material) mr.material.mainTexture = tempTex;
        foreach (var mat in mr.materials)
        {
            mat.SetTexture("_MainTex", tempTex);
        }

        targetCam.gameObject.SetActive(false);
        inputText.gameObject.SetActive(false);
    }
}