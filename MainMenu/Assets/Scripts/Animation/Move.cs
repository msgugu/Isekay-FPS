using Isekai.GC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Isekai.GC.Ani
{
    public class Move : StateMachineBehaviourBase
    {
        public override void Init(PlayerMove controller)
        {
            base.Init(controller);

            transitions = new Dictionary<State, System.Func<Animator, bool>>()
            {
                    { State.Jump, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.Space) &&
                    animator.IsGround();
                }},

                { State.Crouch, (animator) =>
                {
                    return Input.GetKeyDown(KeyCode.C) &&
                    animator.IsGround();

                }},
                
            };
        }


    }


}


