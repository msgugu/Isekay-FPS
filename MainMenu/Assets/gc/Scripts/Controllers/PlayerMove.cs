using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Isekai.GC.Ani;
using Photon.Pun;
using Cinemachine;

namespace Isekai.GC
{
    public class PlayerMove : MonoBehaviour// IPunObservable
    {

        PhotonView PV;

        //카메라 위치 잡는거
        public GameObject cameraHolder;

        private float _speedGain; // 달리기
        // 카메라 회전
        [SerializeField] private float _mouseSensitivity;
        [SerializeField] private float _smoothTime;
        private float _verticalLookRotation;

        public Rigidbody rb;
        private Transform _neck;
        private float _neckRotate;

        // 캐릭터 이동
        public float walkSpeed = 5f;
        public float sprintSpeed = 10f;
        private float _horizontal;
        private float _vertical;
        Vector3 moveAmount;
        Vector3 smoothMoveVelocity;
        private float _mouseX;

        private Animator _animator;
        [SerializeField] private CinemachineVirtualCamera playerCamera;

        // 줌 땡기는거
        private Vector3 _zoomPos = new Vector3(0.0549999774f, -0.00999999978f, 0.100000001f);
        private Quaternion _zoomRot = Quaternion.Euler(3.11f, 0, 0);
        private Vector3 originPos = Vector3.zero;
        private Quaternion originRot = Quaternion.identity;
        [SerializeField] private Camera _weaponCamera;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            PV = GetComponent<PhotonView>();
            if (!PV.IsMine)
            {
                Destroy(playerCamera);
                Destroy(GetComponentInChildren<Camera>().gameObject);
                Destroy(_weaponCamera);
            }
            
        }

        private void Start()
        {
            InitAnimatorBehaviour();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            _neck = _animator.GetBoneTransform(HumanBodyBones.Neck);

        }


        private void Update()
        {
            if (!PV.IsMine) return;
            LookRotate();
            Zoom();
        }

        /// <summary>
        /// 플레이어 이동 관련
        /// </summary>
        private void FixedUpdate()    
        {
            if (!PV.IsMine) return;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                _horizontal = Input.GetAxis("Horizontal") * sprintSpeed;
                _vertical = Input.GetAxis("Vertical") * sprintSpeed;
            }
            else
            {
                _horizontal = Input.GetAxis("Horizontal") * walkSpeed;
                _vertical = Input.GetAxis("Vertical") * walkSpeed;
            }
            Vector3 moveVec = new Vector3(_horizontal, 0, _vertical);
            rb.MovePosition(rb.position + transform.TransformDirection(moveVec) * Time.fixedDeltaTime);

        }

        /// <summary>
        /// 무브 애니메이션 받아오는거
        /// </summary>
        private void LateUpdate()
        {
            if (!PV.IsMine) return;

            _animator.SetFloat("turn", _mouseX);
            _animator.SetFloat("PosX", _horizontal);
            _animator.SetFloat("PosZ", _vertical);

            _neck.localRotation = Quaternion.Euler(0, 0, _neckRotate * 0.8f);
        }

        /// <summary>
        /// 상태머신 초기화
        /// </summary>
        private void InitAnimatorBehaviour()
        {
            var behaviours = _animator.GetBehaviours<StateMachineBehaviourBase>();
            foreach (var behaviour in behaviours)
            {
                behaviour.Init(this);
            }
        }

        /// <summary>
        /// 카메라 회전
        /// </summary>
        private void LookRotate()
        {
            transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * _mouseSensitivity);
            _verticalLookRotation += Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -45f, 50f);

            cameraHolder.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
            _neckRotate = _verticalLookRotation;
        }

        /// <summary>
        /// 마우스 오른쪽키 누르면 줌땡겨짐
        /// </summary>
        private void Zoom()
        {
            if (Input.GetMouseButton(1))
            {
                _weaponCamera.transform.localPosition = Vector3.Lerp(_weaponCamera.transform.localPosition, _zoomPos, Time.deltaTime * 10);
                _weaponCamera.transform.localRotation = Quaternion.Lerp(_weaponCamera.transform.localRotation, _zoomRot, Time.deltaTime * 10);
            }
            else
            {
                _weaponCamera.transform.localPosition = Vector3.Lerp(_weaponCamera.transform.localPosition, originPos, Time.deltaTime * 10);
                _weaponCamera.transform.localRotation = Quaternion.Lerp(_weaponCamera.transform.localRotation, originRot, Time.deltaTime * 10);
            }
        }

        
    }
}

