namespace InGame.UI
{
    // IUIProgressBar 인터페이스
    // UI 프로그레스 바 컴포넌트가 구현해야 하는 기본적인 기능을 정의
    public interface IUIProgressBar
    {
        // fillAmount 프로퍼티는 프로그레스 바의 현재 채워진 양을 나타냄
        // (0.0에서 1.0 사이의 값) 0.0은 완전히 비어있음, 1.0은 완전히 채워짐을 의미
        float fillAmount { get; set; }
    }
}
