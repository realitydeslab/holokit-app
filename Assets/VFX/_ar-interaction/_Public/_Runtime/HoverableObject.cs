using UnityEngine;

public class HoverableObject : MonoBehaviour
{

    [SerializeField] float _triggerDistance = .2f;

    [SerializeField] float _loadTime = 1f;

    float _loadSpeed;

    HandObject _ho;

    float _process = 0;

    public float Process { get { return _process; } }

    private void Start()
    {
        _ho = HandObject.Instance;
        _loadSpeed = 1 / _loadTime;
    }
    private void OnEnable()
    {

    }
    private void OnDisable()
    {

    }

    void Update()
    {
        if (Vector3.Distance(_ho.transform.position, transform.position) < _triggerDistance)
        {
            _process += Time.deltaTime * _loadSpeed;
            if (_process > 1) _process = 1;
        }
        else
        {
            _process -= Time.deltaTime * _loadSpeed;
            if (_process < 0) _process = 0;
        }
    }
}
