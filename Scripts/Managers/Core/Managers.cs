using UnityEngine;

public class Managers : MonoBehaviour
{
    ///<summary>내부적으로 사용되는 Managers 변수</summary>
    static Managers s_instance;
    ///<summary>내부적으로 사용되는 Managers Property</summary>
    static Managers Instance { get { Init(); return s_instance; } }

    [SerializeField]
    DataManager _data = new DataManager();
    PoolManager _pool = new PoolManager();
    ResourceManager _resource = new ResourceManager();
    SoundManager _sound = new SoundManager();

    [HideInInspector]
    GameManager _game;

    public static DataManager Data => Instance._data;
    public static PoolManager Pool => Instance._pool;
    public static ResourceManager Resource => Instance._resource;
    public static SoundManager Sound => Instance._sound;

    public static GameManager Game => Instance._game;



    ///<summary>가장 처음 매니저 만들때 한번 Init</summary>
    static void Init()
    {
        if (s_instance == null && Application.isPlaying)
        {
            Application.targetFrameRate = 60;
            QualitySettings.vSyncCount = 0;
            UnityEngine.Input.multiTouchEnabled = false;

            GameObject go = new GameObject { name = "@Managers" };
            go.AddComponent<Managers>();
            DontDestroyOnLoad(go);
            s_instance = go.GetComponent<Managers>();

            s_instance._sound.Init();
            s_instance._data.Init();
            s_instance._pool.Init();
            s_instance._resource.Init();

            s_instance._game = go.AddComponent<GameManager>();

            GameInit();
        }
    }

    public static void GameInit()
    {
        Instance._game.Init();
    }

    ///<summary>새로운 씬으로 갈때마다 클리어</summary>
    public static void Clear()
    {
        Sound.Clear();
        Pool.Clear();
        Resource.Clear();
        Game.Clear();
    }

}
