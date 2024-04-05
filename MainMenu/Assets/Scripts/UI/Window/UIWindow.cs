using UnityEngine;
using InGame.UI.Tweens;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;

namespace InGame.UI
{
	
	public class UIWindow : MonoBehaviour
	{

        // 전환 효과의 유형을 정의 즉시 변경(Instant)과 페이드(Fade)를 지원합니다.
        public enum Transition
		{
			Instant,  // 즉시 변경
            Fade      // 효과를 주며 천천히 변경
        }

        // 윈도우의 시각적 상태
        public enum VisualState
		{
			Shown,  // 표시상태 
			Hidden  // 숨김상태
		}

		//  Esc키를 눌렀을 때의 동작을 정의 (None), 숨김(Hide), 포커스된 경우에만 숨김(HideIfFocused), 표시/숨김 토글(Toggle) 등을 지원합니다.
		public enum EscapeKeyAction
		{
			None,			// 동작을 하지 않음
            Hide,			// 숨김
			HideIfFocused,  // 활성화 상태에만 숨김
			Toggle			// Toggle의 활성화 / 숨김상태
							// => Esc키를 누르면 숨겨지고, 반대로 숨겨져 있는 상태일때 Esc키를 누르면 다시 화면에 나타남
		}


        // TransitionBeginEvent는 윈도우의 시각적 전환(표시 또는 숨김)이 시작될 때 호출될 이벤트
        // 이 이벤트는 전환 대상이 되는 UIWindow 인스턴스, 목표 VisualState(표시 또는 숨김)  전환을 즉시 수행할지 여부를 나타내는 bool 값을 매개변수로 받음
        [Serializable] public class TransitionBeginEvent : UnityEvent<UIWindow, VisualState, bool> {}

        // TransitionCompleteEvent는 윈도우의 시각적 전환(표시 또는 숨김)이 완료될 때 호출될 이벤트
        // 이 이벤트는 전환 대상이 되는 UIWindow 인스턴스와 전환 후의 최종 VisualState(표시 또는 숨김)를 매개변수로 받음
        // 전환의 시작과 끝에 필요한 추가 로직을 구현
        [Serializable] public class TransitionCompleteEvent : UnityEvent<UIWindow, VisualState> {}

        // 현재 활성화된 윈도우를 관리
        // 정적 변수로 모든 UIWindow 인스턴스 공유
        protected static UIWindow m_FucusedWindow;
        public static UIWindow FocusedWindow { get { return m_FucusedWindow; } private set { m_FucusedWindow = value; } }


		[SerializeField] private int m_CustomWindowId = 0;								    // CustomWindow ID (Label)
		[SerializeField] private VisualState m_StartingState = VisualState.Hidden;			// 시작상태
		[SerializeField] private EscapeKeyAction m_EscapeKeyAction = EscapeKeyAction.Hide;  // Esc키 동작
        [SerializeField] private bool m_FocusAllowReparent = true;                          // 활성화시 부모 변경 혀용여부(UI 계층에서 해당 객체를 다른 위치로 이동시키는 등의 동작)
        [SerializeField] private Transition m_Transition = Transition.Instant;				// 전환 방식
		[SerializeField] private TweenEasing m_TransitionEasing = TweenEasing.InOutQuint;   // 전환 효과
		[SerializeField] private float m_TransitionDuration = 0.1f;						    // 전환 지속 시간

		protected bool m_IsFocused = false;								// 사용자의 입력에 응답하는 활성 상태에 있는지(포커스 상태) 확인
        private VisualState m_CurrentVisualState = VisualState.Hidden;  // 윈도우가 화면에 보이는 상태인지 아니면 숨겨진 상태인지(시각적 상태) 확인 

        private CanvasGroup m_CanvasGroup;                              // Unity의 CanvasGroup 컴포넌트에 대한 참조

        // 각종 속성의 getter/setter 정의 및 윈도우의 동작을 제어하는 메소드들을 정의

        /// <summary>
        /// CustomID: 윈도우의 고유 식별자 (윈도우를 구별할 때 사용)
        /// </summary>
        /// <value>The custom window identifier.</value>
        public int CustomID
		{
			get { return this.m_CustomWindowId; }   
            set { this.m_CustomWindowId = value; } 
		}

        /// <summary>
        /// Esc키를 눌렀을 때 윈도우가 어떻게 반응할지 결정 (윈도우를 숨기거나 표시 상태를 토글)
        /// </summary>
        public EscapeKeyAction escapeKeyAction
		{
			get { return this.m_EscapeKeyAction; }
			set { this.m_EscapeKeyAction = value; }
		}

        /// <summary>
        /// 윈도우가 포커스를 받았을 때, 부모 오브젝트를 변경할 수 있는지 여부를 결정
		/// UI 계층구조에서 윈도우의 위치를 동적으로 조정할 수 있게 해줌
        /// </summary>
        public bool focusAllowReparent
        {
            get { return this.m_FocusAllowReparent; }
            set { this.m_FocusAllowReparent = value; }
        }

        /// <summary>
        /// 윈도우의 표시 또는 숨김 상태를 전환할 때 사용되는 애니메이션 유형
		/// (Instant 변화 또는 Fade)
        /// </summary>
        /// <value> The transition </value>
        public Transition transition
		{
			get { return this.m_Transition; }
			set { this.m_Transition = value; }
		}

        /// <summary>
        /// 전환(표시 또는 숨김) 애니메이션의 속도 조절 함수
		/// (애니메이션의 시작과 끝에서의 속도를 조절하여 보다 자연스러운 전환 효과를 생성)
        /// </summary>
        /// <value> The transition easing </value>
        public TweenEasing transitionEasing
		{
			get { return this.m_TransitionEasing; }
			set { this.m_TransitionEasing = value; }
		}

        /// <summary>
        /// 전환 애니메이션이 완료되기까지 걸리는 시간
        /// </summary>
        /// <value> The duration of the transition </value>
        public float transitionDuration
		{
			get { return this.m_TransitionDuration; }
			set { this.m_TransitionDuration = value; }
		}

        /// <summary>
		/// 윈도우의 시각적 상태 전환(표시 또는 숨김)이 시작할때 호출
        /// </summary>
        public TransitionBeginEvent onTransitionBegin = new TransitionBeginEvent();
		
		/// <summary>
		/// 윈도우의 시각적 생태 전환(표시 또는 숨김)이 완료될때 호출
		/// </summary>
		public TransitionCompleteEvent onTransitionComplete = new TransitionCompleteEvent();

        /// <summary>
        /// 윈도우가 현재 보이는 상태인지 확인
        /// </summary>
        public bool IsVisible
		{
			get { return (this.m_CanvasGroup != null && this.m_CanvasGroup.alpha > 0f) ? true : false; }
		}

        /// <summary>
        /// 윈도우가 현재 열려 있는 상태인지 확인
        /// </summary>
        public bool IsOpen
		{
			get { return (this.m_CurrentVisualState == VisualState.Shown); }
		}
		
		/// <summary>
		/// 포커스(활성화)된 상태인지 확인 
		/// </summary>
		public bool IsFocused
		{
			get { return this.m_IsFocused; }
		}

        // Tween controls
        // TweenRunner<FloatTween>: 이 클래스는 FloatTween 객체를 사용하여 속성 값의 부드러운 전환(예: 페이드 인/아웃)을 관리
		// UIWindow 클래스의 애니메이션 처리에 사용
        [NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

		// 생성자에서 TweenRunner<FloatTween> 인스턴스를 초기화합니다.
        protected UIWindow()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}
		
		protected virtual void Awake()
		{
            // 새로운 커스텀 ID를 할당
            this.m_CanvasGroup = this.gameObject.GetComponent<CanvasGroup>();

            // 게임이 실행될 때 설정된 시작 상태를 적용
            if (Application.isPlaying)
                this.ApplyVisualState(this.m_StartingState);
        }

        // 새로운 커스텀 ID를 할당
        protected virtual void Start()
		{
            // ID가 0인 경우 다음 사용 가능한 ID를 설정
            if (this.CustomID == 0)
				this.CustomID = UIWindow.NextUnusedCustomID;

            // EscapeKeyAction이 None이 아닌 경우 
            if (this.m_EscapeKeyAction != EscapeKeyAction.None)
            {
                UIWindowManager manager = Component.FindObjectOfType<UIWindowManager>();

                // 씬에 UIWindowManager가 있는지 확인하고 없으면 생성
                if (manager == null)
                {
                    GameObject newObj = new GameObject("Window Manager");
                    newObj.AddComponent<UIWindowManager>();
                    newObj.transform.SetAsFirstSibling();
                }
            }
        }

        // 윈도우가 활성화 상태인지 확인(유효성 검증)
        protected virtual void OnValidate()
		{
			// 전환 지속 시간이 음수가 되지 않도록 함
			this.m_TransitionDuration = Mathf.Max(this.m_TransitionDuration, 0f);
		}


        /// <summary>
        /// 윈도우가 현재 활성화(Enabled) 상태이고 게임 오브젝트가 활성화된 상태인지 확인
        /// </summary>
        protected virtual bool IsActive()
		{
			return (this.enabled && this.gameObject.activeInHierarchy);
		}

        /// <summary>
        /// // 윈도우가 선택되었을 때 호출되는 이벤트 핸들러(윈도우에 포커스)
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnSelect(BaseEventData eventData)
		{
			// 윈도우 활성화
			this.Focus();
		}

        /// <summary>
        /// // 윈도우 위에서 마우스 버튼이 눌렸을 때 호출되는 이벤트 핸들러(윈도우에 포커스)
        /// </summary>
        /// <param name="eventData">Event data.</param>
        public virtual void OnPointerDown(PointerEventData eventData)
		{
			// Focus the window
			this.Focus();
		}

        /// <summary>
        /// 윈도우에 포커스
        /// </summary>
        public virtual void Focus()
		{
            // 이미 포커스가 있는 경우에는 아무런 동작을 하지 않음
            if (this.m_IsFocused)
				return;
			
			// 활성화상태에 맞게 츠
			this.m_IsFocused = true;
			
			// Call the static on focused window
			UIWindow.OnBeforeFocusWindow(this);

           
		}



        /// <summary>
        /// 윈도우의 표시 상태를 토글(Show/Hide)
        /// </summary>
        public virtual void Toggle()
		{
			if (this.m_CurrentVisualState == VisualState.Shown)
				this.Hide();
			else
				this.Show();
		}
		
		/// <summary>
		/// 윈도우를 보여줌
		/// </summary>
		public virtual void Show()
		{
			this.Show(false);
		}

        /// <summary>
        /// 윈도우를 표시(인자로 instant가 주어지면 즉시표시, Fade가 주어지면 설정된 전환 효과를 사용)
        /// </summary>
        /// <param name="instant">If set to <c>true</c> instant.</param>
        public virtual void Show(bool instant)
		{
			if (!this.IsActive())
				return;
            
			// Focus the window
			this.Focus();

            // 창이 이미 표시되어 있는지 확인
            if (this.m_CurrentVisualState == VisualState.Shown)
				return;
            
            // Transition
            this.EvaluateAndTransitionToVisualState(VisualState.Shown, instant);
		}

		/// <summary>
		/// 윈도우를 숨김
		/// </summary>
		public virtual void Hide()
		{
			this.Hide(false);
		}

        /// <summary>
        // 윈도우를 숨김(인자로 instant가 주어지면 즉시 숨기고 그렇지 않으면 설정된 전환 효과를 사용)
        /// </summary>
        public virtual void Hide(bool instant)
		{
			// 윈도우가 활성화 상태인지 확인(활성화 상태가 아니면 메서드 실행 중단)
			if (!this.IsActive())
				return;

            // 윈도우의 현재 시각적 상태가 이미 숨김 상태인지 확인
            // 이미 숨겨져 있는 상태라면, 추가적인 숨김 처리를 수행하지 않음
            if (this.m_CurrentVisualState == VisualState.Hidden)
				return;

            // 윈도우의 시각적 상태를 '숨김'으로 전환합니다.
            // 이 때, instant 매개변수에 따라 즉시 전환할지, 아니면 애니메이션 효과를 사용할지 결정
            this.EvaluateAndTransitionToVisualState(VisualState.Hidden, instant);
		}

        /// <summary>
        /// 지정된 시각적 상태로 전환을 평가하고 실행
        /// </summary>
        /// <param name="state"> 전환할 상태 (표시 또는 숨김) </param>
        /// <param name="instant">true일 경우 즉시 전환, false일 경우 설정된 전환 효과를 사용</param>
        protected virtual void EvaluateAndTransitionToVisualState(VisualState state, bool instant)
		{
            // 목표 알파값 설정 (표시 상태일 때 1, 숨김 상태일 때 0)
            float targetAlpha = (state == VisualState.Shown) ? 1f : 0f;

            // 전환 시작 이벤트 호출
            if (this.onTransitionBegin != null)
				this.onTransitionBegin.Invoke(this, state, (instant || this.m_Transition == Transition.Instant));

            // 설정된 전환 방식에 따라 알파값 조정
            if (this.m_Transition == Transition.Fade)
			{
                // 전환 지속 시간 설정
                float duration = (instant) ? 0f : this.m_TransitionDuration;

                // 알파 트윈 실행
                this.StartAlphaTween(targetAlpha, duration, true);
			}
			else
			{
                // 알파값 직접 설정
                this.SetCanvasAlpha(targetAlpha);

                // 전환 완료 이벤트 호출
                if (this.onTransitionComplete != null)
					this.onTransitionComplete.Invoke(this, state);
			}

            // 현재 시각적 상태 저장
            this.m_CurrentVisualState = state;

            // 표시 상태로 전환할 경우, 캔버스 그룹의 레이캐스트 차단 활성화
            if (state == VisualState.Shown)
			{
				this.m_CanvasGroup.blocksRaycasts = true;
				//this.m_CanvasGroup.interactable = true;
			}
		}

        /// <summary>
        /// 지정된 시각적 상태를 즉시 적용
        /// </summary>
        /// <param name="state"> 적용할 상태(표시 또는 숨김) </param>
        public virtual void ApplyVisualState(VisualState state)
        {
            float targetAlpha = (state == VisualState.Shown) ? 1f : 0f;

            // 알파값 직접 설정
            this.SetCanvasAlpha(targetAlpha);

            // 현재 시각적 상태 저장
            this.m_CurrentVisualState = state;

            // 표시 상태로 전환할 경우, 캔버스 그룹의 레이캐스트 차단 활성화
            if (state == VisualState.Shown)
            {
                this.m_CanvasGroup.blocksRaycasts = true;
            }
            else
            {
                this.m_CanvasGroup.blocksRaycasts = false;
            }
        }

        /// <summary>
        /// 알파 트윈을 시작
        /// </summary>
        /// <param name="targetAlpha"> 목표 알파값 </param>
        /// <param name="duration"> 트윈 지속 시간 </param>
        /// <param name="ignoreTimeScale"> 타임스케일을 무시할지 여부 </param>
        public void StartAlphaTween(float targetAlpha, float duration, bool ignoreTimeScale)
		{
			if (this.m_CanvasGroup == null)
				return;

			// FloatTween 객체 생성 및 설정
            var floatTween = new FloatTween { duration = duration, startFloat = this.m_CanvasGroup.alpha, targetFloat = targetAlpha };
			floatTween.AddOnChangedCallback(SetCanvasAlpha);
			floatTween.AddOnFinishCallback(OnTweenFinished);
			floatTween.ignoreTimeScale = ignoreTimeScale;
			floatTween.easing = this.m_TransitionEasing;

            // 트윈 실행
            this.m_FloatTweenRunner.StartTween(floatTween);
		}

        /// <summary>
        /// 캔버스 그룹의 알파값을 설정합니다.
        /// </summary>
        /// <param name="alpha"> 설정할 알파값 </param>
        public void SetCanvasAlpha(float alpha)
		{
			if (this.m_CanvasGroup == null)
				return;
			
			// 알파값 설정
			this.m_CanvasGroup.alpha = alpha;

            // 알파값이 0이면 레이캐스트 차단 비활성화
            if (alpha == 0f)
			{
				this.m_CanvasGroup.blocksRaycasts = false;
			}
		}

        /// <summary>
        /// 트윈이 완료되었을 때 호출되는 메서드
        /// </summary>
        protected virtual void OnTweenFinished()
		{
            // 전환 완료 이벤트 호출
            if (this.onTransitionComplete != null)
				this.onTransitionComplete.Invoke(this, this.m_CurrentVisualState);
		}


        /// <summary>
        /// 씬 내의 모든 윈도우 객체를 반환 (비활성화된 객체 포함)
        /// </summary>
        /// <returns> 윈도우 객체 리스트 </returns>
        public static List<UIWindow> GetWindows()
		{
			List<UIWindow> windows = new List<UIWindow>();
			
			UIWindow[] ws = Resources.FindObjectsOfTypeAll<UIWindow>();
			
			foreach (UIWindow w in ws)
			{
                // 계층에서 창이 활성화되어 있는지 확인
                if (w.gameObject.activeInHierarchy)
					windows.Add(w);
			}
			
			return windows;
		}

        /// <summary>
        /// 커스텀 윈도우 ID를 기준으로 윈도우 객체를 정렬합니다.
        /// </summary>
        /// <param name="w1">비교할 첫 번째 윈도우</param>
        /// <param name="w2">비교할 두 번째 윈도우</param>
        /// <returns> 정렬 순서 </returns>
        public static int SortByCustomWindowID(UIWindow w1, UIWindow w2)
		{
			return w1.CustomID.CompareTo(w2.CustomID);
		}

        /// <summary>
        /// 다음에 사용할 수 있는 커스텀 윈도우 ID를 반환
        /// </summary>
        /// <value> 사용 가능한 다음 커스텀 ID </value>
        public static int NextUnusedCustomID
		{
			get
			{
				// 윈도우를 받아옴
				List<UIWindow> windows = UIWindow.GetWindows();
				
				if (GetWindows().Count > 0)
				{
					// ID별로 창 정렬
					windows.Sort(UIWindow.SortByCustomWindowID);

                    // 마지막 창 ID에 하나를 더한 값을 반환
                    return windows[windows.Count - 1].CustomID + 1;
				}
				
				// No windows, return 0
				return 0;
			}
		}


        /// <summary>
        /// 주어진 커스텀 ID를 가진 윈도우를 찾음
        /// </summary>
        /// <param name="customId">찾고자 하는 윈도우의 커스텀 ID</param>
        /// <returns>찾아진 윈도우 객체, 없으면 null</returns>
        public static UIWindow GetWindowByCustomID(int customId)
		{
			// Get the windows and try finding the window with the given id
			foreach (UIWindow window in UIWindow.GetWindows())
				if (window.CustomID == customId)
					return window;
			
			return null;
		}

        /// <summary>
        /// 윈도우에 포커스를 설정하기 전에 호출되는 메서드
        /// </summary>
        /// <param name="window"> 포커스를 받을 윈도우 </param>
        protected static void OnBeforeFocusWindow(UIWindow window)
		{
			if (m_FucusedWindow != null)
				m_FucusedWindow.m_IsFocused = false;
			
			m_FucusedWindow = window;
		}
	}
}