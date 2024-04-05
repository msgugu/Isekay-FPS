using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHP
{
    public float CurrentHp { get; set; }

    public float MaxHp { get; }

    event Action<float> OnChangeHp;
    event Action<float> OnRecoverHp;
    event Action<float> OnDepleteHp;
    event Action onHpMin;
    event Action onHpMax;

    void RecoverHp(float amount);

    void DepleteHp(float amount);
}
