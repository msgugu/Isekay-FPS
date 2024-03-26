using UnityEngine;
using UnityEngine.Events;

namespace InGame.UI.Tweens
{
	public struct FloatTween : ITweenValue
	{
        // 트윈 중 변경 사항을 위한 콜백 이벤트
        public class FloatTweenCallback : UnityEvent<float> {}

        // 트윈이 완료됐을 때 호출될 콜백 이벤트
        public class FloatFinishCallback : UnityEvent {}
		
		private float m_StartFloat;				// 시작 값 
		private float m_TargetFloat;			// 목표 값 
		private float m_Duration;				// 지속 시간
		private bool m_IgnoreTimeScale;			// 시간척도 무시여부
		private TweenEasing m_Easing;			// 이징 타입

		private FloatTweenCallback m_Target;    // 변경시 호출될 콜백 
		private FloatFinishCallback m_Finish;   // 완료시 호출될 콜백

        /// <summary>
        /// 시작 값 설정 및 조회
        /// </summary>
        /// <value>The start float.</value>
        public float startFloat
		{
			get { return m_StartFloat; }
			set { m_StartFloat = value; }
		}
		
		/// <summary>
		/// 목표 값 설정 및 조회 
		/// </summary>
		/// <value>The target float.</value>
		public float targetFloat
		{
			get { return m_TargetFloat; }
			set { m_TargetFloat = value; }
		}
		
		/// <summary>
		/// 트윈의 지속 시간 설정 및 조시간 척도 무시 여부 설정 및 조회
		/// </summary>
		/// <value>The duration.</value>
		public float duration
		{
			get { return m_Duration; }
			set { m_Duration = value; }
		}

        /// <summary>
        ///	시간 척도 무시 여부 설정 및 조회
        /// </summary>
        /// <value> if ignore time scale; otherwise </value>
        public bool ignoreTimeScale
		{
			get { return m_IgnoreTimeScale; }
			set { m_IgnoreTimeScale = value; }
		}
		
		/// <summary>
		/// easing 메서드 설정 및 조회
		/// </summary>
		/// <value>The easing.</value>
		public TweenEasing easing
		{
			get { return m_Easing; }
			set { m_Easing = value; }
		}

        /// <summary>
        //  주어진 비율에 따라 트윈을 실행, 변경 콜백을 호출
        /// </summary>
        /// <param name="floatPercentage">Float percentage.</param>
        public void TweenValue(float floatPercentage)
		{
			if (!ValidTarget())
				return;
			
			m_Target.Invoke( Mathf.Lerp (m_StartFloat, m_TargetFloat, floatPercentage) );
		}

        /// <summary>
        /// 변경 시 호출될 콜백을 추가
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void AddOnChangedCallback(UnityAction<float> callback)
		{
			if (m_Target == null)
				m_Target = new FloatTweenCallback();
			
			m_Target.AddListener(callback);
		}

        /// <summary>
        /// 트윈 완료 시 호출될 콜백을 추가
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void AddOnFinishCallback(UnityAction callback)
		{
			if (m_Finish == null)
				m_Finish = new FloatFinishCallback();
			
			m_Finish.AddListener(callback);
		}

        // 시간 척도 무시 여부를 반환
        public bool GetIgnoreTimescale()
		{
			return m_IgnoreTimeScale;
		}

        // 트윈의 지속 시간을 반환
        public float GetDuration()
		{
			return m_Duration;
		}

        // 유효한 변경 대상이 있는지 확인
        public bool ValidTarget()
		{
			return m_Target != null;
		}

        /// <summary>
        /// 트윈 완료 콜백을 호출
        /// </summary>
        public void Finished()
		{
			if (m_Finish != null)
				m_Finish.Invoke();
		}
	}
}
