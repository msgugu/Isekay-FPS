using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Isekai.GC.Ani
{
    
    public class Jump : StateMachineBehaviourBase
    {
        private float _force = 5.0f;
        private Rigidbody _rigidbody;
        PhotonView PV;

        public override void Init(PlayerMove controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>()
            {
                { State.Idle, (animator) =>
                {
                    if(_rigidbody == null)
                        return false;

                    return animator.IsGround();
                }}
            };
        }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _rigidbody = animator.GetComponent<Rigidbody>();
            base.OnStateEnter(animator, stateInfo, layerIndex);
            if(PV == null)
            {
                PV = animator.GetComponent<PhotonView>();
                if (PV == null)
                {
                    PV = animator.GetComponentInParent<PhotonView>();
                }
            }

            if (PV != null && PV.IsMine)
            {
                animator.transform.position += Vector3.up * 0.3f;
                if (_rigidbody == null)
                { 
                    _rigidbody = animator.GetComponent<Rigidbody>();
                }
                _rigidbody.velocity = Vector3.up * _force;
            }

        }

    }
}