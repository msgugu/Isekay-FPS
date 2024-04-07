using UnityEngine;

/// <summary>
/// 플레이어가 사용할수 있는 아이템 구분(최상위)
/// </summary>
public abstract class Item : MonoBehaviour
{
    public ItemInfo itemInfo;
    public GameObject itemGameObject;

    public abstract void Use();
}
