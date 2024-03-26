using UnityEngine;
using Photon.Pun;

public class PlayerUI : MonoBehaviour
{
    PhotonView PV;

    private void Awake()
    {
        PV = GetComponentInParent<PhotonView>();
    }

    private void Start()
    {
        if(!PV.IsMine)
        {
            gameObject.SetActive(false);
        }
    }
}
