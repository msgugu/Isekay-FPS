using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class CinemachineRecoil : CinemachineExtension
{
    [SerializeField] private float recoilIntensity = 1f;
    private Vector3 recoilOffset = Vector3.zero;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Aim)
        {
            if (recoilOffset != Vector3.zero)
            {
                // 카메라 반동을 적용합니다.
                var offset = Quaternion.Euler(recoilOffset) * Vector3.forward;
                state.PositionCorrection += offset;
                recoilOffset = Vector3.Lerp(recoilOffset, Vector3.zero, deltaTime * recoilIntensity);
            }
        }
    }

    public void ApplyRecoil(Vector3 direction)
    {
        recoilOffset += direction;
    }
}