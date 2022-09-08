using UnityEngine;
using UnityEngine.UI;

public class RecordButton : MonoBehaviour
{
    [SerializeField] Sprite _record;
    [SerializeField] Sprite _recording;

    public void SetActive()
    {
        transform.Find("Image").GetComponent<Image>().sprite = _recording;
    }

    public void SetInactive()
    {
        transform.Find("Image").GetComponent<Image>().sprite = _record;

    }
}
