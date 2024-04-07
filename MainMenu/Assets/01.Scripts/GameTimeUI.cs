using UnityEngine;
using System;
using TMPro;

/// <summary>
/// 게임 시간 UI를 갱신시켜주는 UI
/// </summary>
public class GameTimeUI : MonoBehaviour
{
    public GameRule gameRule; // GameRule 스크립트의 인스턴스를 참조합니다.
    public TMP_Text timeText; // 남은 시간을 표시할 Text UI 컴포넌트의 참조입니다.

    void Update()
    {
        if (gameRule != null && timeText != null)
        {
            TimeSpan remainingTime = gameRule.RemainingTime;
            // 남은 시간을 "분:초" 형식으로 포맷합니다.
            string timeString = string.Format("{0:D2}:{1:D2}", remainingTime.Minutes, remainingTime.Seconds);
            timeText.text = timeString; // Text UI에 남은 시간을 표시합니다.
        }
    }
}