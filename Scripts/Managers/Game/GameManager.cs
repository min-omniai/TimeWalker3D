using UnityEngine;

public class GameManager : MonoBehaviour
{
    private Define.GameState _currentGameState = Define.GameState.Ready;
    public bool GameStateReady => _currentGameState == Define.GameState.Ready;
    public bool GameStatePlay => _currentGameState == Define.GameState.Play;
    public bool GameStateEnd => _currentGameState == Define.GameState.End;


    private Player _player = null;
    private CameraManager _cameraManager = null;
    private JoyStickController _joyStick = null;

    public Player Player { get { CheckNull(); return _player; } }
    public CameraManager CameraManager { get { CheckNull(); return _cameraManager; } }
    public JoyStickController JoyStick { get { CheckNull(); return _joyStick; } }


    ///<summary>JoyStickController.Init에서 자신을 등록. 입력 이벤트 연결에 사용</summary>
    public JoyStickController ActiveJoyStick { get; set; }
    public void SetDownAction(System.Action action)
    {
        ActiveJoyStick?.AddDownEvent(action);
    }

    public void SetUpAction(System.Action action)
    {
        ActiveJoyStick?.AddUpEvent(action);
    }

    public void SetMoveAction(System.Action<Vector2> action)
    {
        ActiveJoyStick?.AddMoveEvent(action);
    }

    public void Init()
    {
        CheckNull();

        Managers.Resource.Instantiate("Player");
        Managers.Resource.Instantiate("Ground");
        Managers.Resource.Instantiate("Directional Light");

        Player.Init();
        CameraManager.Init();
        JoyStick.Init();

        _currentGameState = Define.GameState.Play;
    }

    private void CheckNull()
    {
        if (_player == null)
            _player = FindObjectOfType<Player>();


        if (_cameraManager == null)
            _cameraManager = FindObjectOfType<CameraManager>();

        if (_joyStick == null)
            _joyStick = FindObjectOfType<JoyStickController>();


        if (_joyStick == null)
        {
            GameObject joystick = Managers.Resource.Instantiate("Canvas_Joystick");
            _joyStick = joystick.GetComponent<JoyStickController>();

            Managers.Resource.Instantiate("EventSystem", joystick.transform);
        }

    }

    public void Clear()
    {
        if (ActiveJoyStick != null)
        {
            ActiveJoyStick.DownAction = null;
            ActiveJoyStick.UpAction = null;
            ActiveJoyStick.JoystickMoveAction = null;
        }
    }
}
