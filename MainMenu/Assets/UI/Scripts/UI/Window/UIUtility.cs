using UnityEngine;
using System;

namespace InGame.UI
{
    // 정적 클래스 UIUtility 정의: 이 클래스는 인스턴스화되지 않으며, UI 관련 유틸리티 함수를 포함
    public static class UIUtility
    {

        /// <summary>
        /// 지정된 게임 오브젝트의 부모 오브젝트에서 지정된 타입의 컴포넌트를 찾아 반환
        /// 이 메서드는 게임 오브젝트의 계층 구조를 상위로 탐색하며 지정된 타입의 컴포넌트가 있을 경우 이를 반환
        /// </summary>
        /// <returns> 찾아낸 컴포넌트(없으면 null) </returns>
        /// <param name="go"> 탐색을 시작할 게임 오브젝트 </param>
        /// <typeparam name="T"> 찾고자 하는 컴포넌트의 타입 </typeparam>
        public static T FindInParents<T>(GameObject go) where T : Component
        {
            if (go == null) return null;

            var comp = go.GetComponent<T>(); // 현재 게임 오브젝트에서 컴포넌트를 불러와 찾음

            if (comp != null)
                return comp; // 컴포넌트를 찾았다면 즉시 반환

            Transform t = go.transform.parent; // 부모 오브젝트로 이동

            while (t != null && comp == null) // 최상위 부모까지 탐색을 계속진행 
            {
                comp = t.gameObject.GetComponent<T>(); // 현재 수준에서 컴포넌트를 찾음
                t = t.parent; // 다음 상위 부모로 이동
            }

            return comp; // 찾아낸 컴포넌트를 반환(만약 찾지 못했다면 null을 반환함)
        }
    }
}