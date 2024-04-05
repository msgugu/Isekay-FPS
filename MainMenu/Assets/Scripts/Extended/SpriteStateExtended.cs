using System;
using UnityEngine;

namespace InGame.UI
{
    [Serializable]

    // 이 구조체의 인스턴스가 Unity 인스펙터에 노출
    public struct SpriteStateExtended
    {
        // Sprite 상태변화를 인스펙터 창에서 설정하기위한 변수
        [SerializeField] private Sprite m_HighlightedSprite;        // 강조 표시될 때      
        [SerializeField] private Sprite m_PressedSprite;            // 눌렸을 때 
        [SerializeField] private Sprite m_ActiveSprite;             // 활성화 상태일때
        [SerializeField] private Sprite m_ActiveHighlightedSprite;  // 강조 표시될 때
        [SerializeField] private Sprite m_ActivePressedSprite;      // 활성화 상태에서 눌렸을 때
        [SerializeField] private Sprite m_DisabledSprite;           // 비활성화 상태일때


        // 강조(하이라이트) 상태일 때 표시될 Sprite를 설정
        public Sprite highlightedSprite
        {
            get
            {
                return this.m_HighlightedSprite;
            }
            set
            {
                this.m_HighlightedSprite = value;
            }
        }

        // 버튼이 눌렸을 때 표시될 Sprite를 설정
        public Sprite pressedSprite
        {
            get
            {
                return this.m_PressedSprite;
            }
            set
            {
                this.m_PressedSprite = value;
            }
        }

        // 활성화 상태일 때 기본으로 표시될 Sprite를 설정
        public Sprite activeSprite
        {
            get
            {
                return this.m_ActiveSprite;
            }
            set
            {
                this.m_ActiveSprite = value;
            }
        }

        // 활성화 상태이며 강조(하이라이트) 상태일 때 표시될 Sprite를 설정
        public Sprite activeHighlightedSprite
        {
            get
            {
                return this.m_ActiveHighlightedSprite;
            }
            set
            {
                this.m_ActiveHighlightedSprite = value;
            }
        }

        // 활성화 상태이며 버튼이 눌렸을 때 표시될 Sprite를 설정
        public Sprite activePressedSprite
        {
            get
            {
                return this.m_ActivePressedSprite;
            }
            set
            {
                this.m_ActivePressedSprite = value;
            }
        }

        // 버튼이 비활성화 상태일 때 표시될 Sprite를 설정 
        public Sprite disabledSprite
        {
            get
            {
                return this.m_DisabledSprite;
            }
            set
            {
                this.m_DisabledSprite = value;
            }
        }
    }
}
