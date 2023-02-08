using System.Collections.Generic;
using UnityEngine;

public class ActivatableController : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    [SerializeField] private Collider2D Collider;

    private void OnEnable()
    {
        Animator.SetBool(Utility.IsActive_ParameterId, true);
        Collider.enabled = true;
    }

    private void OnDisable()
    {
        Animator.SetBool(Utility.IsActive_ParameterId, false);
        Collider.enabled = false;
    }
}
