using System;
using UnityEngine;
using UnityEngine.UI;
using InGame.UI.Tweens;  // 게임 내 UI 애니메이션(트윈) 기능을 사용하기 위한 커스텀 네임스페이스

namespace InGame.UI
{
	public class Test_UIProgressBar : MonoBehaviour
	{

        // UI에 표시할 텍스트의 형식을 정의하는 열거형
        public enum TextVariant
		{
			Percent,  // 백분율 형태로 표시
            Value,    // 실제 값으로 표시
            ValueMax  // 실제 값 / 최대값 형태로 표시
        }

        // 스크립트에서 조작할 UI 요소 및 관련 설정
        public UIProgressBar bar;								 // 프로그래스 바 UI 컴포넌트
        public float Duration = 5f;								 // 애니메이션 지속 시간
        public TweenEasing Easing = TweenEasing.InOutQuint;      // 애니메이션의 가속도 조절을 위한 이징 함수
        public Text m_Text;									     // 프로그래스 바와 함께 표시할 텍스트 UI 컴포넌트
        public TextVariant m_TextVariant = TextVariant.Percent;  // 텍스트 표시 형식
        public int m_TextValue = 100;							 // 텍스트로 표시될 값 (주로 최대값을 의미)
        public string m_TextValueFormat = "0";                   // 텍스트 표시 형식 지정 (문자열 형식 지정)

        // 트윈 애니메이션을 관리하기 위한 변수, 직렬화되지 않아 인스펙터에서 보이지 않음
        [NonSerialized] private readonly TweenRunner<FloatTween> m_FloatTweenRunner;

        // (클래스 생성자) 초기화 작업을 수행
        protected Test_UIProgressBar()
		{
			if (this.m_FloatTweenRunner == null)
				this.m_FloatTweenRunner = new TweenRunner<FloatTween>();
			
			this.m_FloatTweenRunner.Init(this);
		}

        // (객체가 활성화될 때 호출) 초기 애니메이션 시작
        protected void OnEnable()
		{
			if (this.bar == null)
				return;
			
			this.StartTween(0f, (this.bar.fillAmount * this.Duration));
		}

        // 프로그래스 바의 채움 비율을 설정하고 관련 텍스트를 업데이트
        public void SetFillAmount(float amount)
		{
			if (this.bar == null)
				return;
			
			this.bar.fillAmount = amount; // progress bar 채움 비율 설정


            // 텍스트 UI 업데이트
            if (this.m_Text != null)
			{
				if (this.m_TextVariant == TextVariant.Percent)
				{
					this.m_Text.text = Mathf.RoundToInt(amount * 100f).ToString() + "%";
				}
				else if (this.m_TextVariant == TextVariant.Value)
				{
					this.m_Text.text = ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat);
				}
				else if (this.m_TextVariant == TextVariant.ValueMax)
				{
					this.m_Text.text =  ((float)this.m_TextValue * amount).ToString(this.m_TextValueFormat) + "/" + this.m_TextValue;
				}
			}
		}

        // 트윈 애니메이션이 종료될 때 호출
        protected void OnTweenFinished()
		{
            // 프로그래스 바 컴포넌트가 설정되지 않았으면 메소드 종료
            if (this.bar == null)
				return;

            // 프로그래스 바의 현재 채움 상태에 따라 다음 시작 상태를 결정 (0이면 1로, 그렇지 않으면 0으로)
            // 즉, 프로그래스 바가 비어있다면 가득 차게 하고, 가득 차 있다면 비우는 트윈을 시작
            this.StartTween((this.bar.fillAmount == 0f ? 1f : 0f), this.Duration);
		}

        // 지정된 목표값까지 프로그래스 바의 채움 비율을 변경하는 트윈을 시작하는 메소드
        protected void StartTween(float targetFloat, float duration)
		{
            // 프로그래스 바 컴포넌트가 설정되지 않았으면 메소드 종료
            if (this.bar == null)
				return;

            // FloatTween 구조체 인스턴스를 생성하고 트윈 애니메이션의 파라미터를 설정
            // 1.트윈 애니메이션의 지속 시간, 2.시작 값으로 현재 프로그래스 바의 채움 비율을 설정, 3.목표 값
            var floatTween = new FloatTween { duration = duration, startFloat = this.bar.fillAmount, targetFloat = targetFloat };

            // FloatTween에 콜백 메소드 추가
            // 트윈이 변경될 때마다 호출되어 UI를 업데이트하는 메소드
            floatTween.AddOnChangedCallback(SetFillAmount);

            // 트윈 애니메이션이 종료될 때 호출되는 메소드
            floatTween.AddOnFinishCallback(OnTweenFinished);

			floatTween.ignoreTimeScale = true; // TimeScale의 영향을 받지 않도록 설정 (즉, 게임이 일시정지 상태여도 트윈은 진행)
            floatTween.easing = this.Easing;   // 이징 함수 설정

            // m_FloatTweenRunner를 통해 트윈 애니메이션 시작
            this.m_FloatTweenRunner.StartTween(floatTween);
		}
	}
}
