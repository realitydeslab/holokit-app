using QRFoundation;
using UnityEngine;
using UnityEngine.UI;

public class ShowContent : MonoBehaviour
{
    public GameObject textPrefab;

    public void OnCodeRegistered(string content, GameObject gameObject)
    {
        /*
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
        sphere.transform.parent = gameObject.transform;
        sphere.transform.localPosition = new Vector3(0, 0.5f, 0);
        sphere.transform.localRotation = Quaternion.identity;
        */

        // Instantiate the text display prefab...
        GameObject text = Instantiate(textPrefab);
        // ... make it a child of the registration game object...
        text.transform.parent = gameObject.transform;
        // ... and align it with its position and orientation.
        text.transform.localPosition = new Vector3(0, 0, 0);
        text.transform.localRotation = Quaternion.identity;

        // Set the text to the content of the QR code
        text.transform.Find("Canvas").Find("Text").GetComponent<Text>().text = content;

        // Rotate to show text at the ideal side (on the "bottom")
        // First, create a vector that points from the game object to the camera
        // in (x, z), and points down 45 deg.
        Vector3 toCam = transform.position - gameObject.transform.position;
        toCam.y = 0;
        toCam.y = -toCam.magnitude;

        // Then find the direction on the game object that aligns best with this
        // vector and rotate the text display accordingly.
        float bestRotation = 0;
        float bestLength = 0;
        float projection = Vector3.Dot(gameObject.transform.forward, toCam);
        if (projection > bestLength)
        {
            bestRotation = 180;
            bestLength = projection;
        }
        projection = Vector3.Dot(gameObject.transform.right, toCam);
        if (projection > bestLength)
        {
            bestRotation = -90;
            bestLength = projection;
        }
        projection = Vector3.Dot(-gameObject.transform.forward, toCam);
        if (projection > bestLength)
        {
            bestRotation = 0;
            bestLength = projection;
        }
        projection = Vector3.Dot(-gameObject.transform.right, toCam);
        if (projection > bestLength)
        {
            bestRotation = 90;
            bestLength = projection;
        }

        text.transform.Rotate(Vector3.up, bestRotation, Space.Self);
    }
}
