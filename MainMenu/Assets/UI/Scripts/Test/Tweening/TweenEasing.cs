namespace InGame.UI.Tweens
{

    // 트윈(tween) 애니메이션에서 사용될 수 있는 다양한 이징(easing) 함수의 타입
    // 이징 함수는 애니메이션의 속도 변화를 제어하여, 움직임이 더 자연스럽거나 의도된 효과를 내도록 함.
    // 트윈 애니메이션의 움직임 패턴을 결정하는데 사용
    public enum TweenEasing
	{
        Linear,      // 선형 이징: 시간에 따라 균일하게 변화합니다.
        Swing,       // 스윙: 시작과 끝에서 속도가 부드럽게 변화합니다.
        InQuad,      // 가속: 처음에 천천히 시작하여 점점 빨라집니다.
        OutQuad,     // 감속: 처음에 빠르게 시작하여 점점 느려집니다.
        InOutQuad,   // 가속 후 감속: 처음과 끝에서는 천천히, 중간에는 빠르게 변화합니다.
        InCubic,     // 큐빅 가속: InQuad보다 변화가 더욱 강합니다.
        OutCubic,    // 큐빅 감속: OutQuad보다 변화가 더욱 강합니다.
        InOutCubic,  // 큐빅 가속 후 감속: InOutQuad보다 변화가 더욱 강합니다.
        InQuart,     // 4차 가속
        OutQuart,    // 4차 감속
        InOutQuart,  // 4차 가속 후 감속
        InQuint,     // 5차 가속
        OutQuint,    // 5차 감속
        InOutQuint,  // 5차 가속 후 감속
        InSine,      // 사인 가속
        OutSine,     // 사인 감속
        InOutSine,   // 사인 가속 후 감속
        InExpo,      // 지수 가속
        OutExpo,     // 지수 감속
        InOutExpo,   // 지수 가속 후 감속
        InCirc,      // 원형 가속
        OutCirc,     // 원형 감속
        InOutCirc,   // 원형 가속 후 감속
        InElastic,   // 탄성 가속: 시작 시 탄성 있는 '끌기' 효과가 있습니다.
        OutElastic,  // 탄성 감속: 끝날 때 탄성 있는 '끌기' 효과가 있습니다.
        InOutElastic,// 탄성 가속 후 감속
        InBack,      // 백 가속: 초과하여 뒤로 당겨지는 효과가 있습니다.
        OutBack,     // 백 감속: 초과하여 앞으로 밀려나는 효과가 있습니다.
        InOutBack,   // 백 가속 후 감속
        InBounce,    // 바운스 가속: 튕기는 효과가 있습니다.
        OutBounce,   // 바운스 감속: 튕기는 효과가 있습니다.
        InOutBounce  // 바운스 가속 후 감속: 튕기는 효과가 있습니다.
    }
}

