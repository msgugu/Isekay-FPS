using System.Collections;
using UnityEngine;

namespace InGame.UI.Tweens
{
    // TweenRunner 클래스 정의: ITweenValue 인터페이스를 구현하는 구조체를 이용해 트윈(점진적 변화)을 실행
    internal class TweenRunner<T> where T : struct, ITweenValue
	{
		protected MonoBehaviour m_CoroutineContainer;  // 코루틴을 실행할 컨테이너 (UI 컴포넌트 또는 gameobject)
        protected IEnumerator m_Tween;                 // 현재 실행 중인 트윈 코루틴

        // Tween을 실행하기 위한 정적 코루틴
        private static IEnumerator Start(T tweenInfo)
		{
            // 트윈 대상이 유효하지 않으면 코루틴을 종료
            if (!tweenInfo.ValidTarget())
				yield break;

            // 경과 시간변수
            float elapsedTime = 0.0f;

            // 지정된 지속 시간 동안 루프를 실행
            while (elapsedTime < tweenInfo.duration)
			{
				elapsedTime += (tweenInfo.ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);

                // 트윈 이징 메서드를 적용하여 트윈 진행 비율을 계산
                float percentage = TweenEasingHandler.Apply(tweenInfo.easing, elapsedTime, 0.0f, 1.0f, tweenInfo.duration);

                // 계산된 비율을 이용해 트윈 값을 적용
                tweenInfo.TweenValue(percentage);
                
                yield return null;
			}

            // 트윈 완료 후 최종 값을 적용하고 완료 콜백 호출
            tweenInfo.TweenValue(1.0f);
			tweenInfo.Finished();
		}

        // 코루틴 컨테이너를 초기화
        public void Init(MonoBehaviour coroutineContainer)
		{
            this.m_CoroutineContainer = coroutineContainer;
		}

        // 새 트윈을 시작
        public void StartTween(T info)
		{
            // 코루틴 컨테이너가 설정되지 않았으면 경고를 출력후 종료
            if (this.m_CoroutineContainer == null)
			{
				Debug.LogWarning ("코루틴 컨테이너가 설정되지 않았습니다. Init에 호출 해주세요");
				return;
			}

            // 이미 실행 중인 트윈이 있다면 중지
            this.StopTween();

            // 코루틴 컨테이너의 게임 오브젝트가 활성 상태가 아니면
            if (!this.m_CoroutineContainer.gameObject.activeInHierarchy)
			{
                // 트윈을 즉시 완료 처리
                info.TweenValue(1.0f);
				return;
			}

            // 새 트윈 코루틴을 시작
            this.m_Tween = Start (info);
            this.m_CoroutineContainer.StartCoroutine (this.m_Tween);
		}

        // 현재 실행 중인 트윈을 중지합니다.
        public void StopTween()
        {
            if (this.m_Tween != null)
            {
                this.m_CoroutineContainer.StopCoroutine(this.m_Tween);
                this.m_Tween = null;
            }
        }
    }
}
