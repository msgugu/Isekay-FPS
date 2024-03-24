using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Isekai.GC.Ani
{
    public class Crouch : StateMachineBehaviourBase
    {
        private bool isCrouching = false;
        private Vector3 originalScale;
        private float _crouchSpeed = 0.3f;
        PlayerMove playerMove;
        private float _originalSpeed;
        private GameObject _camera;
        PhotonView PV;
         

        public override void Init(PlayerMove controller)
        {
            base.Init(controller);
            playerMove = controller;
            _originalSpeed = controller.speed;
            PV = controller.GetComponent<PhotonView>();
            if(PV.IsMine)
            {
                _camera = controller.cameraHolder;
            }

            transitions = new Dictionary<State, System.Func<Animator, bool>>()
            {
                { State.Jump, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.Space) &&
                    animator.IsGround();
               }},
                { State.Idle, (animator) =>
                {

                    return Input.GetKeyDown(KeyCode.V) &&
                    animator.IsGround();
                }},
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            {
                controller.GetComponent<CapsuleCollider>().height = 0.8f;
                controller.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.4f, 0);
                isCrouching = true;
                controller.speed *= _crouchSpeed;
                if (PV.IsMine)
                {
                    _camera.transform.localPosition = new Vector3(0f, 0.55f, 0.3f);
                }
            }
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateExit(animator, stateInfo, layerIndex);
            controller.GetComponent<CapsuleCollider>().height = 1.5f;
            controller.GetComponent<CapsuleCollider>().center = new Vector3(0, 0.75f, 0);
            isCrouching = false;
            if (PV.IsMine)
            {
                _camera.transform.localPosition = new Vector3(0f, 1.3f, 0.3f);
            }

            if (controller != null)
            {
                controller.speed = _originalSpeed;
            }
        }


        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateUpdate(animator, stateInfo, layerIndex);
        }
    }
}


