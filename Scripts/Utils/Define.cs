public class Define
{
    public enum GameState : byte
    {
        Ready,
        Play,
        End,
    }

    public enum JoyStickMethod : byte
    {
        DoNotUse,
        Fixed,
        Follow,
    }

    public enum AnimState : byte
    {
        Idle,
        Run,
        Timeback,
        Death
    }

    public enum SoundType : byte
    {
        Bgm,
        TimeBack,
        TickTock,
        MaxCount
    }
}
