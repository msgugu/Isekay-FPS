using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Isekai.GC
{
    public static class FSMIsGround
    {
        public static bool IsGround(this Component component)
        {
            return Physics.OverlapSphere(component.transform.position, 0.15f, 1 << LayerMask.NameToLayer("Ground")).Length > 0;
        }
    }
}
