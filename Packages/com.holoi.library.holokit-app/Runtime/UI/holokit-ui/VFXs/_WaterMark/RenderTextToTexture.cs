using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Holoi.Library.HoloKitApp.UI
{
    public class RenderTextToTexture : MonoBehaviour
    {
        public string realityName;
        public MeshRenderer mr;

        public RenderTexture tempTex;

        [Header("Update Text Texture")]
        public RenderTexture sampleTex;
        public Camera targetCam;
        public TMPro.TMP_Text inputText;
        public Image arrowImage;
        public Image linearImage;

        void Start()
        {
            // todo: ask yc add value
            // realityName = HoloKitApp.Instance.CurrentReality.realityManager.GetComponent<RealityManager>().waterMarkName;
            OnSetRenderTextureAsMainTex(realityName);
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
            // set text
            inputText.text = text;
            var textPreferredWidth = inputText.preferredWidth;

            // fixed text length
            var tempString = text;
            while (textPreferredWidth > 15)
            {
                var newString = tempString.Remove(tempString.Length - 1);
                inputText.text = newString;

                textPreferredWidth = inputText.preferredWidth;
            }

            var linearWidth = 20 - 3 - textPreferredWidth;

            Debug.Log("width" + textPreferredWidth);
            Debug.Log("linearWidth" + linearWidth);

            linearImage.rectTransform.sizeDelta = new Vector2(linearWidth - 2, 5);
            linearImage.rectTransform.anchoredPosition = new Vector2(textPreferredWidth + 1, 0);

            arrowImage.rectTransform.anchoredPosition = new Vector2(textPreferredWidth + linearWidth - 1, 0);

            StartCoroutine(UpdateRenderTexture());
        }

        IEnumerator UpdateRenderTexture()
        {
            yield return new WaitForEndOfFrame();

            RenderTextureDescriptor rtd = new RenderTextureDescriptor(1024, 1024);
            tempTex = new RenderTexture(rtd);

            Graphics.CopyTexture(sampleTex, tempTex);

            foreach (var mat in mr.materials)
            {
                Debug.Log("set mat");
                mat.mainTexture = tempTex;
            }

            targetCam.gameObject.SetActive(false);
            inputText.gameObject.SetActive(false);
        }
    }
}