using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Isekai.GC.Ani;
using Photon.Pun;

namespace Isekai.GC
{
    public class PlayerMove : MonoBehaviour// IPunObservable
    {

        PhotonView PV;
        PhotonAnimatorView PAni;

        //카메라 위치 잡는거
        public GameObject cameraHolder;

        private float _speedGain; // 달리기
        // 카메라 회전
        [SerializeField] private float _mouseSensitivity;
        [SerializeField] private float _smoothTime;
        private float _verticalLookRotation;

        public Rigidbody rb;
        

        // 캐릭터 이동
        public float speed = 5f;
        private float _horizontal;
        private float _vertical;
        Vector3 moveAmount;
        Vector3 smoothMoveVelocity;
        private float _mouseX;

        private Animator _animator;
        public Camera playerCamera;

        // 줌 땡기는거
        private float normalFOV;
        private float zoomFOV = 40f;
        private bool isZoom;


        private void Awake()
        {
            _animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody>();
            PV = GetComponent<PhotonView>();
            PAni = GetComponent<PhotonAnimatorView>();
            if (!PV.IsMine)
            {
                Destroy(GetComponentInChildren<Camera>().gameObject);
            }
            
        }

        private void Start()
        {
            InitAnimatorBehaviour();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            normalFOV = Camera.main.fieldOfView;
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
            _speedGain = Input.GetKey(KeyCode.LeftShift) ? 2.5f : 1;

            _horizontal = Input.GetAxis("Horizontal") * _speedGain * speed;
            _vertical = Input.GetAxis("Vertical") * _speedGain * speed;

            Vector3 moveVec = new Vector3(_horizontal, 0, _vertical).normalized;
            moveAmount = Vector3.SmoothDamp(moveAmount, moveVec * _speedGain, ref smoothMoveVelocity, _smoothTime);
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);

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
            _verticalLookRotation = Mathf.Clamp(_verticalLookRotation, -45f, 55f);

            cameraHolder.transform.localEulerAngles = Vector3.left * _verticalLookRotation;
        }

        /// <summary>
        /// 마우스 오른쪽키 누르면 줌땡겨짐
        /// </summary>
        private void Zoom()
        {
            if (PV.IsMine)
            {
                if (Input.GetMouseButton(1))
                {
                    isZoom = true;
                    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, zoomFOV, Time.deltaTime * 5f);
                }
                else
                {
                    isZoom = false;
                    Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, normalFOV, Time.deltaTime * 5f);
                }
            }
        }    
    }
}

