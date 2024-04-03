using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.SceneManagement;
using System.IO;


public class r_CreateRoomController : MonoBehaviour
{
    public static r_CreateRoomController instance;  // SingleTone Instance구현

    // 플레이어수 제한옵션
    [Header("Player Limit")]
    public byte[] m_PlayerLimit; [HideInInspector] public int m_CurrentPlayerLimit;

    // 방생성 UI설정
    [Header("UI")] 
    public r_CreateRoomControllerUI m_RoomUI;

    // 게임종료 버튼
    [Header("ExitButton")]
    public Button m_ExitGameButton; // 게임 종료 버튼에 대한 참조를 추가합니다.

    private void Awake()
    {
        // SingleTone Pattern 구현
        if (instance)  
        {
            Destroy(instance);
            Destroy(instance.gameObject);
        }

        instance = this;

        HandleButtons();
        UpdateUI();
    }
 
    /// <summary>
    /// 버튼기능 처리 => 메인메뉴에 표시될 UI정보 업데이트
    /// </summary>

    private void HandleButtons()
    {
        // 플레이어 제한변경 버튼
        m_RoomUI.m_NextPlayerLimitButton.onClick.AddListener(delegate { NextPlayerLimit(true); r_AudioController.instance.PlayClickSound(); });
        m_RoomUI.m_PreviousPlayerLimitButton.onClick.AddListener(delegate { NextPlayerLimit(false); r_AudioController.instance.PlayClickSound(); });
        m_ExitGameButton.onClick.AddListener(delegate { ExitGame(); });
        // 방생성 버튼 
        m_RoomUI.m_CreateRoomButton.onClick.AddListener(delegate {
            r_PhotonHandler.instance.CreateRoom(m_RoomUI.m_RoomNameInput.text, SetRoomOptions(false));
            r_AudioController.instance.PlayClickSound(); m_RoomUI.m_CreateRoomButton.interactable = false;



        });
    }

    /// <summary>
    /// 게임을 종료하는 메서드입니다.
    /// </summary>
    private void ExitGame()
    {
    // 애플리케이션을 종료(특정 에디터에서 동작x)
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    /// <summary>
    /// UI 업데이트
    /// </summary>
    private void UpdateUI()
    {
        if (m_RoomUI == null) return;

        m_RoomUI.m_PlayerLimitText.text = m_PlayerLimit[m_CurrentPlayerLimit].ToString() + " Players";
    }

    /// <summary>
    /// 방 설정 및 속성을 적용
    /// </summary>
    public RoomOptions SetRoomOptions(bool _RandomRoomOptions)
    {
        RoomOptions _RoomOptions = new RoomOptions
        {
            IsVisible = true,
            IsOpen = true,

            // 랜덤 옵션 여부에 따라 최대 플레이어 수 설정
            MaxPlayers = _RandomRoomOptions ? (byte) 8 : m_PlayerLimit[m_CurrentPlayerLimit],  
        };

        // Room 상태설정
        _RoomOptions.CustomRoomProperties = new Hashtable();
        _RoomOptions.CustomRoomProperties.Add("RoomState", _RandomRoomOptions ? "InLobby" : "InGame");

        // 로비에서 볼 수 있는 커스텀 속성지정
        string[] _CustomLobbyProperties = new string[4];

        _CustomLobbyProperties[0] = "GameMap";
        _CustomLobbyProperties[1] = "GameMapImageID";
        _CustomLobbyProperties[2] = "GameMode";
        _CustomLobbyProperties[3] = "RoomState";

        _RoomOptions.CustomRoomPropertiesForLobby = _CustomLobbyProperties;

        return _RoomOptions;
    }

    /// <summary>
    /// 다음 플레이어 제한으로 변경
    /// </summary>
    private void NextPlayerLimit(bool _Next)
    {
        if (_Next)
        {
            m_CurrentPlayerLimit++;
            if (m_CurrentPlayerLimit >= m_PlayerLimit.Length) m_CurrentPlayerLimit = 0;
        }
        else
        {
            m_CurrentPlayerLimit--;
            if (m_CurrentPlayerLimit < 0) m_CurrentPlayerLimit = m_PlayerLimit.Length - 1;
        }

        UpdateUI();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        // gameScene
        if (scene.buildIndex == 1)
        {
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }
    }
}
