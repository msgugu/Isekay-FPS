using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using InGame.UI.Tweens;
using System.Collections.Generic;

namespace InGame.UI
{
    // UI 요소의 상태 전환(강조, 선택, 누름 등)을 관리하고, 전환 방식(색상 변환, 스프라이트 교체, 애니메이션)을 설정할 수 있는 컴포넌트
	public class UIHighlightTransition : MonoBehaviour
    {
        // UI 요소의 시각적 상태를 정의 
        public enum VisualState
		{
            Normal,         // 기본 상태
            Highlighted,    // 마우스 오버 또는 포커스 상태
            Selected,       // 선택된 상태
            Pressed,        // 클릭 또는 눌린 상태
            Active          // 활성화된 상태
        }

        // 전환 효과의 종류를 정의
        public enum Transition
		{
            None,           // 전환 효과 없음
            ColorTint,      // 색상 틴트
            SpriteSwap,     // 스프라이트 교체
            Animation,      // 애니메이션
            TextColor,      // 텍스트 색상 변경
            CanvasGroup     // 캔버스 그룹 투명도 조절
        }

        // 사용할 시각적 전환 효과의 종류를 지정합니다. 기본적으로는 전환 효과를 적용하지 않음(None)으로 설정
        [SerializeField] private Transition m_Transition = Transition.None;

        // 기본 상태에서의 색상을 설정 (ColorBlock의 기본 색상사용)
        [SerializeField] private Color m_NormalColor = ColorBlock.defaultColorBlock.normalColor;

        // 요소가 선택될 때의 색상을 설정(ColorBlock의 강조 색상을 재사용)
        [SerializeField] private Color m_HighlightedColor = ColorBlock.defaultColorBlock.highlightedColor;

        // 요소가 선택될 때의 색상을 설정(ColorBlock의 강조 색상을 재사용)
        [SerializeField] private Color m_SelectedColor = ColorBlock.defaultColorBlock.highlightedColor;

        // 요소가 눌렸을 때의 색상을 설정(ColorBlock의 눌린 상태의 색상을 사용)
        [SerializeField] private Color m_PressedColor = ColorBlock.defaultColorBlock.pressedColor;

        // 요소가 활성화될 때의 색상을 설정합니다. 여기서도 ColorBlock의 강조 색상을 재사용
        [SerializeField] private Color m_ActiveColor = ColorBlock.defaultColorBlock.highlightedColor;

        // 전환 효과의 지속 시간을 설정
        [SerializeField] private float m_Duration = 0.1f;

        // 색상 효과를 강화하기 위한 배수를 설정(1배에서 6배까지 설정 가능)
        [SerializeField, Range(1f, 6f)] private float m_ColorMultiplier = 1f;

        // 강조될 때 사용할 스프라이트를 설정
        [SerializeField] private Sprite m_HighlightedSprite;

        // 선택될 때 사용할 스프라이트를 설정
        [SerializeField] private Sprite m_SelectedSprite;

        // 눌렸을 때 사용할 스프라이트를 설정
        [SerializeField] private Sprite m_PressedSprite;

        // 활성화될 때 사용할 스프라이트를 설정
        [SerializeField] private Sprite m_ActiveSprite;

        // 각 상태에 따른 애니메이션 트리거 이름을 설정
        [SerializeField] private string m_NormalTrigger = "Normal";
		[SerializeField] private string m_HighlightedTrigger = "Highlighted";
		[SerializeField] private string m_SelectedTrigger = "Selected";
		[SerializeField] private string m_PressedTrigger = "Pressed";
        [SerializeField] private string m_ActiveBool = "Active";

        // 상태에 따른 캔버스 그룹의 투명도(alpha) 값을 설정
        [SerializeField][Range(0f, 1f)] private float m_NormalAlpha = 0f;
        [SerializeField][Range(0f, 1f)] private float m_HighlightedAlpha = 1f;
        [SerializeField][Range(0f, 1f)] private float m_SelectedAlpha = 1f;
        [SerializeField][Range(0f, 1f)] private float m_PressedAlpha = 1f;
        [SerializeField][Range(0f, 1f)] private float m_ActiveAlpha = 1f;

        // 전환 효과를 적용할 대상 그래픽을 지정
		private Graphic m_TargetGraphic;

        // 전환 효과를 적용할 대상 게임 오브젝트를 지정
		private GameObject m_TargetGameObject;

        // 전환 효과를 적용할 대상 캔버스 그룹을 지정
        private CanvasGroup m_TargetCanvasGroup;

        // 토글(toggle)을 사용하여 활성화 상태를 제어할지 여부를 지정
        [SerializeField] private bool m_UseToggle = false;

        // 전환 효과와 연동될 토글 컴포넌트를 지정
        [SerializeField] private Toggle m_TargetToggle;


        private bool m_Highlighted = false; // 현재 강조 상태 여부
        private bool m_Selected = false;    // 현재 선택 상태 여부
        private bool m_Pressed = false;     // 현재 눌린 상태 여부
        private bool m_Active = false;      // 현재 활성화 상태 여부

        // Selectable 컴포넌트를 참조합니다. 이를 통해 버튼 등의 UI 요소가 현재 선택 가능한지 여부를 판단
        private Selectable m_Selectable;

        // 부모 CanvasGroup 컴포넌트들이 상호작용을 허용하는지 여부를 나타냄
        // 상위에 있는 CanvasGroup이 상호작용을 비활성화하면, 이 컴포넌트도 영향을 받아 상호작용이 비활성됨
        private bool m_GroupsAllowInteraction = true;

        /// <summary>
        /// Gets or sets the transition type.
        /// </summary>
        /// <value>The transition.</value>
        public Transition transition
		{
            // transition 속성 값을 가져오는 접근자 => 현재 설정된 전환 효과의 종류를 반환
			get
			{
				return this.m_Transition;
			}

            // transition 속성 값을 설정하는 접근자 => UI 컴포넌트에 적용할 새로운 전환 효과의 종류를 설정
            set
            {
				this.m_Transition = value;
			}
		}

        /// <summary>
        /// 시각적 전환 효과에 사용될 대상 그래픽을 가져오거나 설정
        /// 이 프로퍼티는 주로 색상 변화나 스프라이트 교체 등의 효과를 적용할 UI 요소를 지정할 때 사용
        /// </summary>
        /// <value> The target graphic </value>
        public Graphic targetGraphic
		{
			get
			{
				return this.m_TargetGraphic;  // 현재 설정된 대상 그래픽을 반환
            }
			set
			{
				this.m_TargetGraphic = value;  // 새로운 대상 그래픽을 설정
			}
		}

        /// <summary>
        /// 시각적 전환 효과에 사용될 대상 게임 오브젝트를 가져오거나 설정
        /// 특정 게임 오브젝트에 직접적인 변화를 적용할 때 사용
        /// </summary>
        /// <value>The target game object.</value>
        public GameObject targetGameObject
		{
			get
			{
				return this.m_TargetGameObject;  // 현재 설정된 대상 게임 오브젝트를 반환
            }
			set
			{
                this.m_TargetGameObject = value;  // 새로운 대상 게임 오브젝트를 설정
            }
		}

        /// <summary>
        /// 시각적 전환 효과에 사용될 대상 캔버스 그룹을 가져오거나 설정
        /// 이 프로퍼티는 투명도 조절 등의 효과를 적용할 캔버스 그룹을 지정할 때 사용
        /// </summary>
        /// <value>The target canvas group.</value>
        public CanvasGroup targetCanvasGroup
        {
            get
            {
                return this.m_TargetCanvasGroup;  // 현재 설정된 대상 캔버스 그룹을 반환
            }
            set
            {
                this.m_TargetCanvasGroup = value;  // 새로운 대상 캔버스 그룹을 설정
            }
        }

        /// <summary>
        /// 대상 게임 오브젝트에 연결된 애니메이터 컴포넌트를 가져옴
        /// 이 속성은 애니메이션 전환 효과를 구현할 때 사용되며, 대상 게임 오브젝트에 애니메이터 컴포넌트가 있을 경우에만 유효
        /// </summary>
        /// <value> The animator </value>
        public Animator animator
		{
			get
			{
                // 대상 게임 오브젝트가 null이 아니라면 해당 오브젝트의 Animator 컴포넌트를 반환
                if (this.m_TargetGameObject != null)
					return this.m_TargetGameObject.GetComponent<Animator>();

                // 대상 게임 오브젝트가 null이거나 Animator 컴포넌트가 없을 경우 null을 반환(Default)
                return null;
			}
		}

        // UI 컴포넌튼의 생명주기 이벤트와 상태전환 조직을을 처리한다.
        protected void Awake()
        {
            // m_UseToggle이 true인 경우, Toggle 컴포넌트를 찾아 설정하고 현재 상태를 기록
            if (this.m_UseToggle)
            {
                if (this.m_TargetToggle == null)
                    this.m_TargetToggle = this.gameObject.GetComponent<Toggle>();

                if (this.m_TargetToggle != null)
                    this.m_Active = this.m_TargetToggle.isOn;
            }

            // 해당 게임 오브젝트에서 Selectable 컴포넌트를 찾아 설정
            this.m_Selectable = this.gameObject.GetComponent<Selectable>();
        }

        // 컴포넌트가 활성화될 때 호출
        protected void OnEnable()
		{
            // Toggle 컴포넌트가 있으면, 그 값이 변경될 때 호출될 리스너를 추가
            if (this.m_TargetToggle != null)
                this.m_TargetToggle.onValueChanged.AddListener(OnToggleValueChange);

            // 현재 상태를 기반으로 상태 전환을 수행 (즉시 전환)
            this.InternalEvaluateAndTransitionToNormalState(true);
		}

        // 컴포넌트가 비활성화될 때 호출
        protected void OnDisable()
		{
            // Toggle 컴포넌트의 값 변경 리스너를 제거
            if (this.m_TargetToggle != null)
                this.m_TargetToggle.onValueChanged.RemoveListener(OnToggleValueChange);

            // 시각적 상태를 즉시 초기 상태로 재설정
            this.InstantClearState();
		}

        /// <summary>
		/// 현재 상태에 따라 적절한 시각적 전환을 수행
		/// </summary>
		/// <param name="instant">만약 set값이 true라면 <c>true</c> instant.</param>
		private void InternalEvaluateAndTransitionToNormalState(bool instant)
        {
            // m_Active가 true라면 Active 상태로, 그렇지 않으면 Normal 상태로 전환
            this.DoStateTransition(this.m_Active ? VisualState.Active : VisualState.Normal, instant);
        }


        protected void OnValidate()
		{
			this.m_Duration = Mathf.Max(this.m_Duration, 0f);
			
			if (this.isActiveAndEnabled)
			{
				this.DoSpriteSwap(null);

                if (this.m_Transition != Transition.CanvasGroup)
				    this.InternalEvaluateAndTransitionToNormalState(true);
			}
		}

      
        private readonly List<CanvasGroup> m_CanvasGroupCache = new List<CanvasGroup>();

        //캔버스 그룹의 변경 사항을 감지하고 상호작용 가능 여부에 따라 상태를 조정
        protected void OnCanvasGroupChanged()
        {
            // 상위 캔버스 그룹의 상호작용 가능 여부를 검사
            var groupAllowInteraction = true;
            Transform t = transform;
            while (t != null)
            {
                t.GetComponents(m_CanvasGroupCache);
                bool shouldBreak = false;
                for (var i = 0; i < m_CanvasGroupCache.Count; i++)
                {
                    // if the parent group does not allow interaction
                    // we need to break
                    if (!m_CanvasGroupCache[i].interactable)
                    {
                        groupAllowInteraction = false;
                        shouldBreak = true;
                    }
                    // if this is a 'fresh' group, then break
                    // as we should not consider parents
                    if (m_CanvasGroupCache[i].ignoreParentGroups)
                        shouldBreak = true;
                }
                if (shouldBreak)
                    break;

                t = t.parent;
            }

            // 상호작용 가능 상태에 변화가 있다면, 적절한 시각적 상태로 전환
            if (groupAllowInteraction != this.m_GroupsAllowInteraction)
            {
                this.m_GroupsAllowInteraction = groupAllowInteraction;
                this.InternalEvaluateAndTransitionToNormalState(true);
            }
        }

        // 현재 컴포넌트의 상호작용 가능 여부를 반환
        public virtual bool IsInteractable()
        {
            // Selectable 컴포넌트와 그룹 상호작용 가능 여부를 기반으로 판단
            if (this.m_Selectable != null)
                return this.m_Selectable.IsInteractable() && this.m_GroupsAllowInteraction;

            return this.m_GroupsAllowInteraction;
        }

        // Toggle 값이 변경될 때 호출되며, 변경된 상태에 따라 시각적 전환을 수행
        protected void OnToggleValueChange(bool value)
        {
            if (!this.m_UseToggle || this.m_TargetToggle == null)
                return;

            // m_TargetToggle의 상태를 m_Active에 할당
            this.m_Active = this.m_TargetToggle.isOn;

            // 현재 전환 타입이 애니메이션이면, 애니메이터에 boolean 값을 설정하여 상태를 전환
            if (this.m_Transition == Transition.Animation)
            {
                // 타겟 게임 오브젝트나 애니메이터가 없거나, 애니메이터가 비활성화 상태이거나, 애니메이션 트리거 이름이 지정되지 않았다면 반환
                if (this.targetGameObject == null || this.animator == null || !this.animator.isActiveAndEnabled || this.animator.runtimeAnimatorController == null || string.IsNullOrEmpty(this.m_ActiveBool))
                    return;

                // 애니메이터에 설정된 boolean 값을 변경하여 상태를 전환
                this.animator.SetBool(this.m_ActiveBool, this.m_Active);
            }
            // 새로운 상태로 전환을 수행합니다. m_Active 상태에 따라 Active, Selected, Highlighted, 또는 Normal 상태로 전환
            this.DoStateTransition(this.m_Active ? VisualState.Active : 
                (this.m_Selected ? VisualState.Selected : 
                    (this.m_Highlighted ? VisualState.Highlighted : VisualState.Normal)), false);
        }

        /// <summary>
        /// 시각적 상태를 즉시 초기화
        /// 색상 트윈, 스프라이트 변경, 텍스트 색상 변경, CanvasGroup 알파값 설정 등에 대한 처리를 포함
        /// </summary>
        protected void InstantClearState()
		{
			switch (this.m_Transition)
			{
				case Transition.ColorTint:
                    // 타겟 그래픽의 색상을 즉시 흰색으로 변경합니다.
                    this.StartColorTween(Color.white, true);
					break;
				case Transition.SpriteSwap:
                    // 스프라이트를 즉시 기본값(null)으로 변경합니다.
                    this.DoSpriteSwap(null);
					break;
                case Transition.TextColor:
                    this.SetTextColor(this.m_NormalColor);
                    break;
                case Transition.CanvasGroup:
                    // CanvasGroup의 알파값을 1로 설정하여 완전 불투명하게 만듭니다.
                    this.SetCanvasGroupAlpha(1f);
                    break;
            }
		}

        /// <summary>
        /// 사용자가 컴포넌트를 선택할 때 호출됩니다.
        /// </summary>
        /// <param name="eventData"> 이벤트 데이터 </param>
        public void OnSelect(BaseEventData eventData)
		{
			this.m_Selected = true;  // 컴포넌트가 선택된 상태로 설정

            if (this.m_Active)  // 이미 활성 상태라면 더 이상의 처리를 하지 않음
                return;

            // 선택된 상태로 시각적 전환을 수행
            this.DoStateTransition(VisualState.Selected, false);
		}

        /// <summary>
        /// 사용자가 컴포넌트 선택을 해제할 때 호출
        /// </summary>
        /// <param name="eventData"> 이벤트 데이터 </param>
        public void OnDeselect(BaseEventData eventData)
		{
            this.m_Selected = false;  // 선택 해제 상태로 설정

            if (this.m_Active)  // 이미 활성 상태라면 더 이상의 처리를 하지 않음
                return;

            // 선택 해제 상태에 따른 시각적 전환을 수행
            this.DoStateTransition((this.m_Highlighted ? VisualState.Highlighted : VisualState.Normal), false);
		}

        /// <summary>
        /// 사용자가 컴포넌트 위에 마우스 포인터를 올렸을 때 호출
        /// </summary>
        /// <param name="eventData"> 이벤트 데이터 </param>
        public void OnPointerEnter(PointerEventData eventData)
		{
			this.m_Highlighted = true;  // 하이라이트 상태로 설정

            // 이미 다른 상태(selet, press, active)에 있지 않다면 하이라이트 상태로 전환
            if (!this.m_Selected && !this.m_Pressed && !this.m_Active)
				this.DoStateTransition(VisualState.Highlighted, false);
		}

        /// <summary>
        /// 사용자가 컴포넌트 위에서 마우스 포인터를 떼었을 때 호출
        /// </summary>
        /// <param name="eventData"> 이벤트 데이터 </param>
        public void OnPointerExit(PointerEventData eventData)
		{
            this.m_Highlighted = false;  // 하이라이트 상태 해제

            // 이미 다른 상태에 있지 않다면 기본 상태로 전환
            if (!this.m_Selected && !this.m_Pressed && !this.m_Active)
				this.DoStateTransition(VisualState.Normal, false);
		}

        /// <summary>
        /// 사용자가 컴포넌트를 클릭했을 때 호출
        /// </summary>
        /// <param name="eventData"> 이벤트 데이터 </param>
        public virtual void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            if (!this.m_Highlighted)
                return;

            if (this.m_Active)
                return;

            this.m_Pressed = true;
            this.DoStateTransition(VisualState.Pressed, false);
        }
        /// <summary>
        /// 사용자가 컴포넌트에서 포인터를 떼었을 때의 로직을 처리
        /// </summary>
        /// <param name="eventData">이벤트 데이터를 포함하며, 마우스 버튼 정보를 제공</param>
        public virtual void OnPointerUp(PointerEventData eventData)
        {
            // 왼쪽 마우스 버튼 클릭이 아니면 반환
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            // 마우스 버튼을 떼면 'Pressed' 상태를 해제
            this.m_Pressed = false;

            // 새로운 상태를 결정
            // 활성화, 선택된 상태, 하이라이트된 상태 또는 기본 상태 중 하나
            VisualState newState = VisualState.Normal;

            if (this.m_Active)
            {
                newState = VisualState.Active;
            }
            else if (this.m_Selected)
            {
                newState = VisualState.Selected;
            }
            else if (this.m_Highlighted)
            {
                newState = VisualState.Highlighted;
            }

            // 결정된 새로운 상태로 시각적 전환을 수행
            this.DoStateTransition(newState, false);
        }

        /// <summary>
        /// 지정된 상태로 시각적 전환을 수행합니다.
        /// </summary>
        /// <param name="state">전환하려는 상태입니다.</param>
        /// <param name="instant">즉시 전환할지 여부를 결정합니다.</param>
        protected virtual void DoStateTransition(VisualState state, bool instant)
		{
            // 활성화된 씬 내에 없다면 반환
            if (!this.gameObject.activeInHierarchy)
				return;

            // 상호작용 가능하지 않다면 'Normal' 상태로 강제 설정
            if (!this.IsInteractable())
                state = VisualState.Normal;

            // 상태에 따라 색상, 스프라이트, 애니메이션 트리거, 투명도를 준비
            Color color = this.m_NormalColor;
			Sprite newSprite = null;
			string triggername = this.m_NormalTrigger;
            float alpha = this.m_NormalAlpha;

            // 전환 값을 준비
            switch (state)
			{
				case VisualState.Normal:
					color = this.m_NormalColor;
					newSprite = null;
					triggername = this.m_NormalTrigger;
                    alpha = this.m_NormalAlpha;
                    break;
				case VisualState.Highlighted:
					color = this.m_HighlightedColor;
					newSprite = this.m_HighlightedSprite;
					triggername = this.m_HighlightedTrigger;
                    alpha = this.m_HighlightedAlpha;
                    break;
				case VisualState.Selected:
					color = this.m_SelectedColor;
					newSprite = this.m_SelectedSprite;
					triggername = this.m_SelectedTrigger;
                    alpha = this.m_SelectedAlpha;
                    break;
                case VisualState.Pressed:
                    color = this.m_PressedColor;
                    newSprite = this.m_PressedSprite;
                    triggername = this.m_PressedTrigger;
                    alpha = this.m_PressedAlpha;
                    break;
                case VisualState.Active:
                    color = this.m_ActiveColor;
                    newSprite = this.m_ActiveSprite;
                    alpha = this.m_ActiveAlpha;
                    break;
			}

            // 전환을 수행
            switch (this.m_Transition)
			{
				case Transition.ColorTint:
					this.StartColorTween(color * this.m_ColorMultiplier, instant);
					break;
				case Transition.SpriteSwap:
					this.DoSpriteSwap(newSprite);
					break;
				case Transition.Animation:
					this.TriggerAnimation(triggername);
					break;
                case Transition.TextColor:
                    this.StartTextColorTween(color, false);
                    break;
                case Transition.CanvasGroup:
                    this.StartCanvasGroupTween(alpha, instant);
                    break;
            }
		}

        /// <summary>
        /// 지정된 색상으로 색상 전환을 시작
        /// </summary>
        /// <param name="targetColor">목표 색상입니다.</param>
        /// <param name="instant">즉시 전환할지 여부입니다.</param>
        private void StartColorTween(Color targetColor, bool instant)
		{
			if (this.m_TargetGraphic == null)
				return;
			
			if (instant || this.m_Duration == 0f || !Application.isPlaying)
			{
				this.m_TargetGraphic.canvasRenderer.SetColor(targetColor);
			}
			else
			{
				this.m_TargetGraphic.CrossFadeColor(targetColor, this.m_Duration, true, true);
			}
		}


        /// <summary>
        /// 지정된 스프라이트로 스프라이트를 교체
        /// </summary>
        /// <param name="newSprite"> 교체할 새 스프라이트 </param>
        private void DoSpriteSwap(Sprite newSprite)
		{
			Image image = this.m_TargetGraphic as Image;
			
			if (image == null)
				return;
			
			image.overrideSprite = newSprite;
		}

        /// <summary>
        /// 지정된 트리거로 애니메이션을 시작합니다.
        /// </summary>
        /// <param name="triggername">애니메이션 트리거 이름입니다.</param>
        private void TriggerAnimation(string triggername)
		{
			if (this.targetGameObject == null || this.animator == null || !this.animator.isActiveAndEnabled || this.animator.runtimeAnimatorController == null || !this.animator.hasBoundPlayables || string.IsNullOrEmpty(triggername))
				return;
            
            this.animator.ResetTrigger(this.m_HighlightedTrigger);
			this.animator.ResetTrigger(this.m_SelectedTrigger);
            this.animator.ResetTrigger(this.m_PressedTrigger);
            this.animator.SetTrigger(triggername);
		}

        
		private void StartTextColorTween(Color targetColor, bool instant)
        {
            if (this.m_TargetGraphic == null)
                return;

            if ((this.m_TargetGraphic is Text) == false)
                return;

            if (instant || this.m_Duration == 0f || !Application.isPlaying)
            {
                (this.m_TargetGraphic as Text).color = targetColor;
            }
        
        }

        /// <summary>
        /// 타겟 텍스트의 색상을 설정
        /// 이 메서드는 UI 컴포넌트의 텍스트 색상을 변경할 때 사용
        //사용자가 컴포넌트에 마우스를 올렸을 때 텍스트 색상을 변경하여 시각적 피드백을 제공
        /// </summary>
        /// <param name="targetColor"> 변경할 색상</param> 
        private void SetTextColor(Color targetColor)
        {
            if (this.m_TargetGraphic == null)
                return;

            if (this.m_TargetGraphic is Text)
            {
                (this.m_TargetGraphic as Text).color = targetColor;
            }
        }

        /// <summary>
        /// CanvasGroup의 알파값을 점진적으로 변경(UI 컴포넌트의 투명도를 조절 / Fade in 및 out 효과적용)
        /// </summary>
        /// <param name="targetAlpha"> 목표 알파값 </param>
        /// <param name="instant"> (즉시 변경할지 판단) true인 경우 즉시 변경, false인 경우 점진적으로 변경 </param>
        private void StartCanvasGroupTween(float targetAlpha, bool instant)
        {
            // 타겟 CanvasGroup이 설정되지 않았다면 아무 작업도 수행하지 않음
            if (this.m_TargetCanvasGroup == null)
                return;

            // 즉시 변경하거나, 지속 시간이 0이거나, 게임이 실행 중이 아니라면 알파값을 즉시 설정
            if (instant || this.m_Duration == 0f || !Application.isPlaying)
            {
                this.SetCanvasGroupAlpha(targetAlpha);
            }

            else
            {
                // FloatTween을 사용하여 알파값을 점진적으로 변경합니다. 이는 시각적인 부드러움을 추가
                var floatTween = new FloatTween { duration = this.m_Duration, startFloat = this.m_TargetCanvasGroup.alpha, targetFloat = targetAlpha };
                floatTween.AddOnChangedCallback(SetCanvasGroupAlpha);  // 알파값 변경 시 호출될 콜백을 설정
                floatTween.ignoreTimeScale = true;                     // 타임 스케일의 영향을 받지 않도록함     

            }
        }

        /// <summary>
        /// CanvasGroup의 알파값을 설정합니다.
        /// 이 메서드는 UI 컴포넌트의 투명도를 직접 조절할 때 사용됩니다.
        /// </summary>
        /// <param name="alpha">설정할 알파값입니다.</param>
        private void SetCanvasGroupAlpha(float alpha)
        {
            // 타겟 CanvasGroup이 설정되지 않았다면 아무 작업도 수행하지 않음
            if (this.m_TargetCanvasGroup == null)
                return;

            // CanvasGroup의 알파값을 지정된 값으로 설정
            this.m_TargetCanvasGroup.alpha = alpha;
        }
    }
}
