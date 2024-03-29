using System.Collections;
using UnityEngine;
using TMPro;

public class CountDown : MonoBehaviour
{
    public GameObject Content;            // 카운트다운이 활성화될 때 표시되는 UI의 부모 객체
    public TextMeshProUGUI CountDownText; // 카운트다운 타이머를 표시할 텍스트 객체
    public AudioClip CountAudio;          // 카운트다운 시 재생할 오디오 클립
    private Animator CountAnim;           // 카운트다운 애니메이션을 제어할 Animator

    [SerializeField]
    private AudioSource audioSource;      // 오디오를 재생하는데 사용될 AudioSource 컴포넌트
    [SerializeField]
    private int countDown = 5;            // 카운트다운 시작 시간(초)

    void Start()
    {
        // 시작할 때 AudioSource 컴포넌트를 가져옴
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // AudioSource 컴포넌트가 없다면 추가
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // 시작할 때 Animator 컴포넌트를 가져옴
        if (Content != null)
        {
            CountAnim  = Content.GetComponent<Animator>();
        }
        // 카운트다운을 시작
        StartCountDown();
    }

    public void StartCountDown()
    {
        StartCoroutine(CountDownRoutine()); // 코루틴을 사용하여 카운트다운을 수행
    }

    IEnumerator CountDownRoutine()
    {
        while (countDown > 0)
        {
            OnCountChanged(countDown);                 // 카운트다운 값이 변경될 때마다 OnCountChanged 호출
            CountDownText.text = countDown.ToString(); // UI에 현재 카운트다운 값을 표시
            PlayCountDownSound();                      // 카운트다운 소리를 재생

            yield return new WaitForSeconds(1);        // 1초 대기
            countDown--;                               // 카운트다운 값을 감소
        }
        CountDownText.text = "Start!";                 // 카운트다운이 끝나면 "Start!"를 표시
        Content.SetActive(false);                      // 카운트다운이 완료되면 카운트다운 UI를 비활성화
    }

    private void PlayCountDownSound()
    {
        if (CountAudio != null)
        {
            audioSource.clip = CountAudio; // 지정된 오디오 클립을 AudioSource에 설정
            audioSource.Play();            // 오디오를 재생
        }
    }

    private void OnCountChanged(int count)
    {
        if (count > 0)
        {
            Content.SetActive(true);

            CountAnim = Content.GetComponent<Animator>();
            CountAnim.Play("count", 0, 0);
        }
        else
        {
            Content.SetActive(false);
        }
    }
}
