using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    private float _normalFOV;
    private float _zoomFOV = 45;
    private bool _isZoom;
    private CinemachineVirtualCamera _virtualCamera;

    
    void Start()
    {
        _virtualCamera = GetComponent<CinemachineVirtualCamera>();
        _normalFOV = _virtualCamera.m_Lens.FieldOfView;
    }

    void Update()
    {
        Zooming();
    }

    /// <summary>
    /// 마우스 오른쪽키 누르면 줌땡겨짐
    /// </summary>
    private void Zooming()
    {
        if (Input.GetMouseButton(1))
        {
            _isZoom = true;
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_virtualCamera.m_Lens.FieldOfView, _zoomFOV, Time.deltaTime * 5f);
        }
        else
        {
            _isZoom = false;
            _virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(_virtualCamera.m_Lens.FieldOfView, _normalFOV, Time.deltaTime * 5f);
        }
    }
}
