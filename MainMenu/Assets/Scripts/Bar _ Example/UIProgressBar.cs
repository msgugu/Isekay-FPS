using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace InGame.UI
{
    // UIProgressBar 클래스는 프로그레스 바의 시각적 표현과 관련된 기능을 제공
    public class UIProgressBar : MonoBehaviour, IUIProgressBar
    {
        // ProgressBar의 상태 변경 시 호출될 UnityEvent를 정의(ProgressBar Class의 변화를 추적하는 이벤트)
        [Serializable] public class ChangeEvent : UnityEvent<float> { }

        // ProgressBar의 동작 유형을 정의(동작유형 선택)
        public enum Type
		{
			Filled,   // 채우기 (Image 컴포넌트의 fillAmount를 사용하여 채움)
            Resize,   // 크기조정 (RectTransform의 크기를 조정하여 채움)
            Sprites   // 스프라이트 (여러 스프라이트 중 하나를 선택하여 표시)
        }

        // ProgressBar의 채우기 크기를 어떻게 결정할지 정의
        public enum FillSizing
		{
			Parent,  // 부모 요소의 크기에 맞춤
            Fixed    // 고정된 크기 사용
        }


        // 인스펙터에서 설정할 수 있는 필드
        [SerializeField] private Type m_Type = Type.Filled;                     // 현재 ProgressBar 유형
        [SerializeField] private Image m_TargetImage;							// ProgressBar를 표시할 이미지 컴포넌트
        [SerializeField] private Sprite[] m_Sprites;                            // ProgressBar를 표시할 스프라이트 배열
        [SerializeField] private RectTransform m_TargetTransform;               // ProgressBar의 RectTransform
        [SerializeField] private FillSizing m_FillSizing = FillSizing.Parent;   // 채우기 크기 결정 방식
        [SerializeField] private float m_MinWidth = 0f;                         // 최소 너비 (Resize 타입일 때 사용)			
        [SerializeField] private float m_MaxWidth = 100f;                       // 최대 너비 (Resize 타입일 때 사용)
        [SerializeField][Range(0f, 1f)] private float m_FillAmount = 1f;        // 채우기 양 (0.0 ~ 1.0)
        [SerializeField] private int m_Steps = 0;                               // ProgressBar를 몇 단계로 나눌지 설정 (0은 단계 없음)
        public ChangeEvent onChange = new ChangeEvent();                        // 프로그레스 바의 변경 사항을 추적하는 이벤트

        /// <summary>
        /// ProgressBar Type(Get or Set) _ Filled, Resize, Sprites 중 하나를 사용
        /// </summary>
        /// <value>The type.</value>
        public Type type {
			get { return this.m_Type; }
			set { this.m_Type = value; }
		}

        /// <summary>
        /// 프로그레스 바를 표시할 이미지 컴포넌트 (Get or Set)
        /// </summary>
        /// <value>The target image.</value>
        public Image targetImage {
			get { return this.m_TargetImage; }
			set { this.m_TargetImage = value; }
		}

        /// <summary>
        /// Type이 Sprites로 설정된 경우에 사용
		/// 프로그레스 바를 표현할 때 사용될 스프라이트 배열 (Get or Set)
        /// </summary>
        public Sprite[] sprites
        {
            get { return this.m_Sprites; }
            set { this.m_Sprites = value; }
        }

        /// <summary>
        /// Type이 Resize로 설정된 경우에 사용
		/// 프로그레스 바의 RectTransform을 설정 (Get or Set)
        /// </summary>
        /// <value>The target transform.</value>
        public RectTransform targetTransform {
			get { return this.m_TargetTransform; }
			set { this.m_TargetTransform = value; }
		}

        /// <summary>
        /// Resize 타입에서 사용될 최소 너비를 설정하거나 가져옵니다.
        /// </summary>
        /// <value>The minimum width.</value>
        public float minWidth {
			get { return this.m_MinWidth; }
			set {
				this.m_MinWidth = value;
				this.UpdateBarFill();
			}
		}
		
		/// <summary>
		/// Gets or sets the maximum width (Used for the resize type bar).
		/// </summary>
		/// <value>The maximum width.</value>
		public float maxWidth {
			get { return this.m_MaxWidth; }
			set {
				this.m_MaxWidth = value;
				this.UpdateBarFill();
			}
		}
		
		/// <summary>
		/// Gets or sets the fill amount.
		/// </summary>
		/// <value>The fill amount.</value>
		public float fillAmount {
			get {
				return this.m_FillAmount;
			}
			set {
				if (this.m_FillAmount != Mathf.Clamp01(value))
				{
					this.m_FillAmount = Mathf.Clamp01(value);
					this.UpdateBarFill();
					this.onChange.Invoke(this.m_FillAmount);
				}
			}
		}
		
		/// <summary>
		/// Gets or sets the steps (Zero for no stepping).
		/// </summary>
		/// <value>The steps.</value>
		public int steps {
			get { return this.m_Steps; }
			set { this.m_Steps = value; }
		}
		
		/// <summary>
		/// Gets or sets the current step.
		/// </summary>
		/// <value>The current step.</value>
		public int currentStep {
			get {
				if (this.m_Steps == 0)
					return 0;
				
				float perStep = 1f / (this.m_Steps - 1);
				return Mathf.RoundToInt(this.fillAmount / perStep);
			}
			set {
				if (this.m_Steps > 0)
				{
					float perStep = 1f / (this.m_Steps - 1);
					this.fillAmount = Mathf.Clamp(value, 0, this.m_Steps) * perStep;
				}
			}
		}
		
        protected virtual void Start()
        {
            // Make sure the fill anchor reflects the pivot
            if (this.m_Type == Type.Resize && this.m_FillSizing == FillSizing.Parent && this.m_TargetTransform != null)
            {
                float height = this.m_TargetTransform.rect.height;
                this.m_TargetTransform.anchorMin = this.m_TargetTransform.pivot;
                this.m_TargetTransform.anchorMax = this.m_TargetTransform.pivot;
                this.m_TargetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            // Update the bar fill
            this.UpdateBarFill();
        }

        protected virtual void OnRectTransformDimensionsChange()
        {
            // Update the bar fill
            this.UpdateBarFill();
        }

#if UNITY_EDITOR
        protected void OnValidate()
		{
            // Make sure the fill anchor reflects the pivot
            if (this.m_Type == Type.Resize && this.m_FillSizing == FillSizing.Parent && this.m_TargetTransform != null)
            {
                float height = this.m_TargetTransform.rect.height;
                this.m_TargetTransform.anchorMin = this.m_TargetTransform.pivot;
                this.m_TargetTransform.anchorMax = this.m_TargetTransform.pivot;
                this.m_TargetTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            }

            // Update the bar fill
            this.UpdateBarFill();
		}
		
		protected void Reset()
		{
			this.onChange = new ChangeEvent();
		}
#endif

		/// <summary>
		/// Updates the bar fill.
		/// </summary>
		public void UpdateBarFill()
		{
			if (!this.isActiveAndEnabled)
				return;

            if (this.m_Type == Type.Filled && this.m_TargetImage == null)
                return;

            if (this.m_Type == Type.Resize && this.m_TargetTransform == null)
                return;

            if (this.m_Type == Type.Sprites && this.m_Sprites.Length == 0)
                return;

            // Get the fill amount
            float fill = this.m_FillAmount;
			
			// Check for steps
			if (this.m_Steps > 0)
				fill = Mathf.Round(this.m_FillAmount * (this.m_Steps - 1)) / (this.m_Steps - 1);
			
			if (this.m_Type == Type.Resize)
			{
				// Update the bar fill by changing it's width
				// we are doing it this way because we are using a mask on the bar and have it's fill inside with static width and position
				if (this.m_FillSizing == FillSizing.Fixed)
				{
					this.m_TargetTransform.SetSizeWithCurrentAnchors(
						RectTransform.Axis.Horizontal, 
						(this.m_MinWidth + ((this.m_MaxWidth - this.m_MinWidth) * fill))
					);
				}
				else
				{
                    this.m_TargetTransform.SetSizeWithCurrentAnchors(
						RectTransform.Axis.Horizontal, 
						((this.m_TargetTransform.parent as RectTransform).rect.width * fill)
					);
				}
			}
            else if (this.m_Type == Type.Sprites)
            {
                int spriteIndex = Mathf.RoundToInt(fill * (float)this.m_Sprites.Length) - 1;

                if (spriteIndex > -1)
                {
                    this.targetImage.overrideSprite = this.m_Sprites[spriteIndex];
                    this.targetImage.canvasRenderer.SetAlpha(1f);
                }
                else
                {
                    this.targetImage.overrideSprite = null;
                    this.targetImage.canvasRenderer.SetAlpha(0f);
                }
            }
			else
			{
				// Update the image fill amount
				this.m_TargetImage.fillAmount = fill;
			}
		}
		
		/// <summary>
		/// Adds to the fill (Used for buttons).
		/// </summary>
		public void AddFill()
		{
			if (this.m_Steps > 0)
			{
				this.currentStep = this.currentStep + 1;
			}
			else
			{
				this.fillAmount = this.fillAmount + 0.1f;
			}
		}
		
		/// <summary>
		/// Removes from the fill (Used for buttons).
		/// </summary>
		public void RemoveFill()
		{
			if (this.m_Steps > 0)
			{
				this.currentStep = this.currentStep - 1;
			}
			else
			{
				this.fillAmount = this.fillAmount - 0.1f;
			}
		}
	}
}
