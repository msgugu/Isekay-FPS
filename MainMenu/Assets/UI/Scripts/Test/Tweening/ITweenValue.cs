namespace InGame.UI.Tweens
{
    // 트윈 애니메이션 값을 처리하는 인터페이스
    // UI 컴포넌트의 속성 변화(ex) 색상, 위치, 크기 등)를 부드럽게 전환하기 위한 메서드와 속성을 정의
    internal interface ITweenValue
	{

        // 트윈 애니메이션의 진행 정도를 기반으로 값을 업데이트하는 함수
        void TweenValue(float floatPercentage);

        // 애니메이션 진행 시 Unity의 Time.timeScale의 영향을 받을지 여부를 결정하는 속성 
        // true일 경우, 애니메이션은 실제 시간을 기준으로 진행
        bool ignoreTimeScale { get; }

        // 트윈 애니메이션이 완료되기까지 걸리는 시간(초 단위)
        float duration { get; }

        // 애니메이션의 이징(Easing) 함수 타입을 결정
        TweenEasing easing { get; }

        // 현재 트윈 애니메이션의 대상이 유효한지 확인하는 함수
        bool ValidTarget();

        // 트윈 애니메이션이 완료될 때 호출되는 함수
        void Finished();
	}
}
