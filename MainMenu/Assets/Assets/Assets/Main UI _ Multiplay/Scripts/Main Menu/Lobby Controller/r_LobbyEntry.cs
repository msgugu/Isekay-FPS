using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;

/// <summary>
/// 이 구성 요소는 로비의 플레이어 목록 UI에 있는 플레이어
/// 이름과 같은 플레이어 정보를 표시하는 데 사용
/// </summary>
public class r_LobbyEntry : MonoBehaviour
{
    // Photon의 Player 오브젝트(플레이어의 네트워크 정보를 담고있음)
    [Header("Lobby Player UI")]
    public Player m_PhotonPlayer;

    // UI에서 플레이어 이름을 보여주는 텍스트
    [Header("Lobby Player UI")]
    public Text m_PlayerNameText;


    // 로비 플레이어 항목을 설정
    // 플레이어 정보를 UI에 연결하는 데 사용
    public void SetupLobbyPlayer(Player _Player)
    {
        m_PhotonPlayer = _Player;                          // 전달받은 Photon Player 오브젝트를 저장
        m_PlayerNameText.text = m_PhotonPlayer.NickName;   // Photon Player의 닉네임을 텍스트 컴포넌트에 설정
    }

}
