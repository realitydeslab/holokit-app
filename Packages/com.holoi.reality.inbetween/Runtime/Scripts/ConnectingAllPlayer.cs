using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class ConnectingAllPlayer : MonoBehaviour
{
    LineRenderer _lineRenderer;
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }
    
    // Update is called once per frame
    void Update()
    {
        var p = FindObjectsOfType<InbetweenPlayer>()?.Select(p => p.gameObject.transform.position).ToArray();
        _lineRenderer.positionCount = p.Length;
        _lineRenderer.SetPositions(p);
    }
}
