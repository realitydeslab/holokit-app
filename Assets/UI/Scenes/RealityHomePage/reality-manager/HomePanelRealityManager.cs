using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomePanelRealityManager : MonoBehaviour
{
    [SerializeField] Transform _canvas;
    [SerializeField] Transform _enterArrow;
    [SerializeField] Transform _flowArrow;
    [SerializeField] List<Transform> _realityContainer;
    Animator _animator;
    [SerializeField]Animator _canvasAnimator;
    int _currentIndex = 0;

    void Start()
    {
        _realityContainer[_currentIndex].GetComponent<Animator>().SetTrigger("Select");
        _animator = GetComponent<Animator>();
        _canvasAnimator = _canvas.GetComponent<Animator>();
    }

    void Update()
    {
        if (_currentIndex < _realityContainer.Count - 1)
        {

            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                _realityContainer[_currentIndex].GetComponent<Animator>().SetTrigger("UnSelect");
                switch (_currentIndex)
                {
                    case 0:
                        _animator.SetTrigger("01");
                        _canvasAnimator.SetTrigger("01");
                        break;
                    case 1:
                        _animator.SetTrigger("12");
                        _canvasAnimator.SetTrigger("12");
                        break;
                }
                _currentIndex += 1;
                _realityContainer[_currentIndex].GetComponent<Animator>().SetTrigger("Select");
                _enterArrow.GetComponent<Animator>().SetTrigger("out");
                _enterArrow.GetComponent<Animator>().SetTrigger("in");
                _flowArrow.GetComponent<Animator>().SetTrigger("toRight");

            }
        }
        if( _currentIndex >0)
        {
            if (Input.GetKeyUp(KeyCode.DownArrow))
            {
                _realityContainer[_currentIndex].GetComponent<Animator>().SetTrigger("UnSelect");
                switch (_currentIndex)
                {
                    case 1:
                        _animator.SetTrigger("10");
                        _canvasAnimator.SetTrigger("10");
                        break;
                    case 2:
                        _animator.SetTrigger("21");
                        _canvasAnimator.SetTrigger("21");
                        break;
                }
                _currentIndex -= 1;
                _realityContainer[_currentIndex].GetComponent<Animator>().SetTrigger("Select");
                _enterArrow.GetComponent<Animator>().SetTrigger("out");
                _enterArrow.GetComponent<Animator>().SetTrigger("in");
                _flowArrow.GetComponent<Animator>().SetTrigger("toLeft");

            }
        }
    }
}
