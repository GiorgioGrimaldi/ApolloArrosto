using System;

[Serializable]
public struct InputState
{
    public float Horizontal { get; private set; }
    public bool Jump { get; private set; }
    public bool Interact { get; private set; }

    public InputState(float horizontal, bool jump, bool interact)
    {
        this.Horizontal = horizontal;
        this.Jump = jump;
        this.Interact = interact;
    }
}
