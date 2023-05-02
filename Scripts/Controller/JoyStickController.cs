using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Serialization;

public class JoyStickController : MonoBehaviour
{
    [Header("조이스틱 방식")]
    [Tooltip("Fixed : 누른 위치에 고정\nHardFixed : 이미 고정 되어있음\nFollow : 원 밖으로 나가면 원이 따라옴\nSlowFollow : 원 밖으로 나가면 원이 천천히 따라옴\nRunningGame : 런 방식처럼 좌우 오프셋만 인식")]
    [FormerlySerializedAs("eJoyStickMethod")]
    public Define.JoyStickMethod Method;
    [Header("조이스틱이 돌아다닐 수 있는 반경")]
    public float JoyStickBound;
    [Header("움직일 물체 (Rigidbody필요)")]
    public Rigidbody MoveObjectRig;

    public float Threshold = 10;


    public float Speed = 5;
    private float _currentSpeed;
    public float XBound;
    public bool AutoRun;
    [FormerlySerializedAs("X_Sensitivity")]
    public float XSensitivity;
    [FormerlySerializedAs("X_Acceletor")]
    public float XAccelerator;

    public bool UseAccelerate;
    public float Accelerate = 100;


    private RectTransform _canvasRect;
    private RectTransform _joystick;
    private RectTransform _joystickHandle;
    [SerializeField]
    private Image _joystickImage;
    private Image _joystickHandleImage;

    private Vector3 _originPosition;
    private Vector3 _moveDirection = Vector3.zero;
    private float _originXPosition;

    [HideInInspector]
    public bool CanMove = false;
    private bool _isButtonClicked = false;
    private bool _isMouseDown = false;

    public System.Action DownAction;
    public System.Action<Vector2> JoystickMoveAction;
    public System.Action MoveAction;
    public System.Action UpAction;

    public void Init()
    {
        _currentSpeed = 0;
        _canvasRect = GetComponent<RectTransform>();
        _joystickImage = transform.GetChild(0).GetComponent<Image>();
        _joystick = _joystickImage.GetComponent<RectTransform>();
        _joystickHandle = _joystick.GetChild(0).GetComponent<RectTransform>();
        _joystickHandleImage = _joystickHandle.GetComponent<Image>();
        _joystickImage.enabled = false;
        _joystickHandleImage.enabled = false;

        AddDownEvent(() =>
        {
            _isMouseDown = true;
            Managers.Game.Player.SetAnimState(Define.AnimState.Run);
        });

        AddUpEvent(() =>
        {
            _isMouseDown = false;
            Managers.Game.Player.CheckTimeback();
        });

        switch (Method)
        {
            case Define.JoyStickMethod.DoNotUse:
                _joystick.gameObject.SetActive(false);

                break;
            case Define.JoyStickMethod.Fixed:
                break;

            case Define.JoyStickMethod.Follow:
                break;
        }

        CanMove = true;
        _isButtonClicked = false;

        Managers.Game.ActiveJoyStick = this;

        MoveObjectRig = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody>();
    }

    //JoyStickEditor 에서 활용함
    public void SetRigidBody()
    {
        MoveObjectRig.isKinematic = false;
        MoveObjectRig.mass = 100;
        MoveObjectRig.drag = 100;
        MoveObjectRig.angularDrag = 100;
        MoveObjectRig.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;

        if (UseAccelerate)
        {
            if (!_isMouseDown)
            {
                _currentSpeed = Mathf.Max(0, _currentSpeed - Accelerate * Time.deltaTime);
            }

            MoveObjectRig.position += _moveDirection * Time.deltaTime * _currentSpeed;
        }


        switch (Method)
        {
            case Define.JoyStickMethod.DoNotUse:
                return;

            case Define.JoyStickMethod.Fixed:
                if (Input.GetMouseButtonDown(0))
                {
                    if (CheckButtonClick()) return;

                    _joystickImage.enabled = true;
                    _joystickHandleImage.enabled = true;

                    _joystick.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
                    _joystickHandle.anchoredPosition = Vector2.zero;
                    _originPosition = _joystick.anchoredPosition;

                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !_isButtonClicked)
                {
                    _joystickHandle.anchoredPosition = Input.mousePosition * 2688f / Screen.height - _originPosition;
                    if (_joystickHandle.anchoredPosition.magnitude > JoyStickBound)
                    {
                        _joystickHandle.anchoredPosition = _joystickHandle.anchoredPosition.normalized * JoyStickBound;
                    }

                    if (_joystickHandle.anchoredPosition.magnitude < Threshold) return;

                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);

                    Vector3 dir = new Vector3(_joystickHandle.anchoredPosition.x, 0, _joystickHandle.anchoredPosition.y);

                    if (MoveObjectRig != null)
                    {
                        Move(dir);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !_isButtonClicked)
                {
                    _joystickImage.enabled = false;
                    _joystickHandleImage.enabled = false;
                    UpAction?.Invoke();
                }
                break;

            case Define.JoyStickMethod.Follow:
                if (Input.GetMouseButtonDown(0))
                {
                    if (CheckButtonClick()) return;

                    _joystickImage.enabled = true;
                    _joystickHandleImage.enabled = true;

                    _joystick.anchoredPosition = Input.mousePosition * 2688f / Screen.height;
                    _joystickHandle.anchoredPosition = Vector2.zero;
                    _originPosition = _joystick.anchoredPosition;

                    DownAction?.Invoke();
                }
                else if (Input.GetMouseButton(0) && !_isButtonClicked)
                {
                    _joystickHandle.anchoredPosition = Input.mousePosition * 2688f / Screen.height - _originPosition;
                    if (_joystickHandle.anchoredPosition.magnitude > JoyStickBound)
                    {
                        _joystick.anchoredPosition = (Vector2)_originPosition + _joystickHandle.anchoredPosition - JoyStickBound * _joystickHandle.anchoredPosition.normalized;
                        _joystickHandle.anchoredPosition = _joystickHandle.anchoredPosition.normalized * JoyStickBound;
                        _originPosition = _joystick.anchoredPosition;
                    }
                    if (_joystickHandle.anchoredPosition.magnitude < Threshold) return;

                    JoystickMoveAction?.Invoke(_joystickHandle.anchoredPosition);

                    Vector3 dir = new Vector3(_joystickHandle.anchoredPosition.x, 0, _joystickHandle.anchoredPosition.y);
                    if (MoveObjectRig != null)
                    {
                        Move(dir);
                    }
                }
                else if (Input.GetMouseButtonUp(0) && !_isButtonClicked)
                {
                    _joystickImage.enabled = false;
                    _joystickHandleImage.enabled = false;
                    UpAction?.Invoke();
                }
                break;
        }
    }


    public void AddDownEvent(System.Action action)
    {
        DownAction -= action;
        DownAction += action;
    }
    public void AddMoveEvent(System.Action<Vector2> action)
    {
        JoystickMoveAction -= action;
        JoystickMoveAction += action;
    }
    public void AddPlayerMoveEvent(System.Action action)
    {
        MoveAction -= action;
        MoveAction += action;
    }
    public void AddUpEvent(System.Action action)
    {
        UpAction -= action;
        UpAction += action;
    }

    private bool CheckButtonClick()
    {
        if (EventSystem.current?.currentSelectedGameObject?.GetComponent<Button>())
        {
            _isButtonClicked = true;
            return true;
        }
        else
        {
            _isButtonClicked = false;
            return false;
        }
    }

    private void Move(Vector3 dir)
    {
        _moveDirection = dir.normalized;

        MoveObjectRig.rotation = Quaternion.LookRotation(_moveDirection);

        if (UseAccelerate)
        {
            _currentSpeed = Mathf.Min(Speed, _currentSpeed + Accelerate * Time.deltaTime);
        }
        else
        {
            MoveObjectRig.position += _moveDirection * Speed * Time.deltaTime;
        }
    }
}
