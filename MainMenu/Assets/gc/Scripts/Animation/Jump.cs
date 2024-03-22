using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Isekai.GC.Ani
{

    public class Jump : StateMachineBehaviourBase
    {
        private float _force = 10.0f;
        private Rigidbody _rigidbody;

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
            base.OnStateEnter(animator, stateInfo, layerIndex);
            animator.transform.position += Vector3.up * 0.3f;
            _rigidbody = animator.GetComponent<Rigidbody>();
            _rigidbody.AddForce(Vector3.up * _force, ForceMode.Impulse);
            
        }



    }
}