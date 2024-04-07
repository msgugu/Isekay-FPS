using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

/// <summary>
/// 방에서 만들어질 플레이어 리스트
/// </summary>
public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;
    Player player;
    /// <summary>
    /// UI Text 변경
    /// </summary>
    /// <param name="_player"></param>
    public void SetUp(Player _player)
    {
        player = _player;
        text.text = _player.NickName;
    }

    /// <summary>
    /// 방나가면 삭제~
    /// </summary>
    /// <param name="otherPlayer"></param>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if(player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
