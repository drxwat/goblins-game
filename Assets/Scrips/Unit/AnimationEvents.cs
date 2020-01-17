using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void UnitAnimationEvent();
public class AnimationEvents : MonoBehaviour
{
    
    public UnitAnimationEvent onAttackEndEvent;
    public void OnAttackEnd()
    {
        onAttackEndEvent?.Invoke();
    }
}
