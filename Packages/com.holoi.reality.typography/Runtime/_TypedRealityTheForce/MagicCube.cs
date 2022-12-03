using UnityEngine;
using Holoi.Library.ARUX;

namespace Holoi.Reality.Typography
{
    public class MagicCube : MonoBehaviour
    {
        [SerializeField] Player _player;
        [SerializeField] FollowMovementManager _FMM;
        public Transform MagicObject;
        [SerializeField] MeshRenderer _outlineMeshRenderer;
        [SerializeField] float _attractDist = 1f;

        [SerializeField] GameObject _burstVfx;
        [SerializeField] GameObject _releaseVfx;
        Material _mat;
        Animator _animator;
        bool _isTriggered = false;

        [HideInInspector] public enum IntakeState
        {
            idle,
            needFilled,
            filled,
            needRelease,
            coolingDown
        }

        [HideInInspector] public enum MovementState
        {
            Free,
            Attracted
        }

        [HideInInspector] public IntakeState _intakeState = IntakeState.idle;
        [HideInInspector] public MovementState _movemenrtState = MovementState.Free;

        private void OnEnable()
        {
            _player = FindObjectOfType<Player>();
            _player.OnCastSomthingAction += BeCasted;
            _player.OnCastNothingAction += NotBeCasted;
        }

        private void OnDisable()
        {
            _player.OnCastSomthingAction -= BeCasted;
            _player.OnCastNothingAction -= NotBeCasted;
        }

        private void Start()
        {
            _animator = GetComponent<Animator>();
            _mat = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            if(transform.position.y < -2f)
            {
                transform.position = HoloKit.HoloKitCamera.Instance.CenterEyePose.position +
                    HoloKit.HoloKitCamera.Instance.CenterEyePose.forward;
            }

            if (MagicObject)
            {
                switch (_movemenrtState)
                {
                    case MovementState.Free:
                        switch (_intakeState)
                        {
                            case IntakeState.idle:
                                break;
                            case IntakeState.needFilled:
                                var dist = Vector3.Distance(MagicObject.position, transform.position);
                                if (dist < _attractDist)
                                {
                                    if (!_isTriggered)
                                    {
                                        _isTriggered = true;
                                        IntakeObject();
                                    }
                                }
                                else
                                {
                                    _isTriggered = false;
                                }
                                break;
                            case IntakeState.filled:
                                break;
                        }

                        break;
                    case MovementState.Attracted:
                        break;
                }
            }
            else
            {

            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            switch (_intakeState)
            {
                case IntakeState.needFilled:
                    break;
                case IntakeState.filled:
                    break;
                case IntakeState.needRelease:
                    ReleaseObject();
                    break;
            }
        }


        public void IntakeObject()
        {
            _animator.SetTrigger("Intake");
            _intakeState = IntakeState.filled;

            MagicObject.GetComponent<MagicObject>().BeIntaken();
        }

        public void ReleaseObject()
        {
            GetComponent<Rigidbody>().velocity = Vector3.up * 1;
            _intakeState = IntakeState.idle;
            _animator.SetTrigger("Release");

            MagicObject.GetChild(0).gameObject.SetActive(true);
            MagicObject.position = transform.position;
            MagicObject.GetComponent<Rigidbody>().velocity = Vector3.up * 1;
            MagicObject.GetComponent<Animator>().SetTrigger("Release");
            MagicObject.GetComponent<MagicObject>().BeReleased();
        }

        public void BeAttracted()
        {
            if(_intakeState == IntakeState.needFilled)
            {
                _intakeState = IntakeState.idle; // hand gesture is not 100% accurate so we set idle again to decrease the times trigger un-expect event
            }
            
            SetAttractedState();
        }


        public void BeShoot(Vector3 direcion)
        {
            if(_intakeState == IntakeState.idle)
            {
                Debug.Log("state to needFilled");

                _intakeState = IntakeState.needFilled;
            }
            
            if (_intakeState == IntakeState.filled)
            {
                Debug.Log("state to needRelease");
                _intakeState = IntakeState.needRelease;
            }

            SetFreeState();
            transform.GetComponent<Rigidbody>().velocity = direcion * 3f;
        }

        public void SetAttractedState()
        {
            NotBeCasted();
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponent<Rigidbody>().useGravity = false;
            _movemenrtState = MovementState.Attracted;
            _FMM.enabled = true;
        }
        public void SetFreeState()
        {
            transform.GetComponent<Rigidbody>().isKinematic = false;
            transform.GetComponent<Rigidbody>().useGravity = true;
            _movemenrtState = MovementState.Free;
            _FMM.enabled = false;
        }

        public void BeCasted()
        {
            _outlineMeshRenderer.material.SetFloat("_Alpha_Multipier", 1);
        }

        public void NotBeCasted()
        {
            _outlineMeshRenderer.material.SetFloat("_Alpha_Multipier", 0);
        }

        public void IntakeVfxBrust()
        {
            var go = Instantiate(_burstVfx);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        }

        public void ReleaseVfxBrust()
        {
            var go = Instantiate(_releaseVfx);
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
        }
    }
}
