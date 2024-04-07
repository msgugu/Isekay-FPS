using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

/// <summary>
/// 킬뎃 UI
/// </summary>
public class Scoreboard : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreboardItemPrefab;
    [SerializeField] CanvasGroup canvasGroup;

    Dictionary<Player, ScoreboardItem> scoreboardItems = new Dictionary<Player, ScoreboardItem>();
    /// <summary>
    /// 플레이어들을 돌면서 UI생성
    /// </summary>
    private void Start()
    {
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            AddScoreboardItem(player);
        }
    }
    /// <summary>
    /// 방나가면 없어짐
    /// </summary>
    /// <param name="otherPlayer"></param>
     public override void OnPlayerLeftRoom(Player otherPlayer)
     {
        RemoveScoreboardItem(otherPlayer);
     }

    /// <summary>
    /// 플레이어가 방에 들어오면 
    /// </summary>
    /// <param name="newPlayer"></param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreboardItem(newPlayer);
    }

    /// <summary>
    /// UI만들기
    /// </summary>
    /// <param name="player"></param>
    void AddScoreboardItem(Player player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab,container).GetComponent<ScoreboardItem>();
        item.Initalize(player);
        scoreboardItems[player] = item;
    }                   

    /// <summary>
    /// UI 없애기 
    /// </summary>
    /// <param name="player"></param>
    void RemoveScoreboardItem(Player player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            canvasGroup.alpha = 1;
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            canvasGroup.alpha = 0;
        }
    }
}
