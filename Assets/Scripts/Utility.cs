using UnityEngine;

public static class Utility
{
    public static readonly int VerticalSpeed_ParameterId = Animator.StringToHash("VerticalSpeed");
    public static readonly int AbsoluteHorizontalSpeed_ParameterId = Animator.StringToHash("AbsoluteHorizontalSpeed");
    public static readonly int IsGrounded_ParameterId = Animator.StringToHash("IsGrounded");
    public static readonly int JetPack_ParameterId = Animator.StringToHash("Jetpack");
     
    public static readonly int IsActive_ParameterId = Animator.StringToHash("IsActive");
    public static readonly int IsOpen_ParameterId = Animator.StringToHash("IsOpen");


    public static bool Includes(this LayerMask layerMask, int layer)
    {
        return layerMask == (layerMask | (1 << layer));
    }
    public static bool IsAstronautNotStarted(this GameState gameState)
    {
        return gameState is GameState.BlueAstronautNotStarted or GameState.OrangeAstronautNotStarted;
    }
    public static bool IsAstronautStarted(this GameState gameState)
    {
        return gameState is GameState.BlueAstronautStarted or GameState.OrangeAstronautStarted;
    }
    public static bool IsPlayable(this GameState gameState)
    {
        return gameState.IsAstronautNotStarted() || gameState.IsAstronautStarted();
    }

}
