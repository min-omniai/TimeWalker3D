using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField]
    [FormerlySerializedAs("_movePosList")]
    private List<Vector3> _movePositions = new List<Vector3>();
    public List<Vector3> MovePositions
    {
        get { return _movePositions; }
    }
    private bool _canTimeback = false;
    public bool CanTimeback
    {
        get { return _canTimeback; }
    }

    private Animator _animator = null;
    private SkinnedMeshRenderer _skinnedMeshRenderer = null;
    private TrailRenderer[] _trails = null;
    private Rigidbody _rigidbody = null;

    public void Init()
    {
        _animator = GetComponent<Animator>();
        _skinnedMeshRenderer = Util.FindChild<SkinnedMeshRenderer>(gameObject, "Sphere", true);
        _trails = transform.GetComponentsInChildren<TrailRenderer>();
        _rigidbody = GetComponent<Rigidbody>();

        foreach (var trail in _trails)
        {
            trail.time = 0f;
        }
    }


    private Coroutine _recordRoutine = null;
    private IEnumerator RecordPositionRoutine()
    {
        yield return Util.GetWaitForSeconds(.75f);

        _alterEgoRoutine = StartCoroutine(AlterEgoRoutine());

        while (true)
        {
            _movePositions.Add(transform.position);

            yield return Util.GetWaitForSeconds(.1f);
        }
    }

    private Coroutine _alterEgoRoutine = null;
    private GameObject _alterEgo = null;
    private IEnumerator AlterEgoRoutine()
    {
        yield return Util.GetWaitForSeconds(1f);

        _canTimeback = true;
        SetMaterial(true);

        foreach (var trail in _trails)
        {
            trail.time = 1.1f;
        }

        _alterEgo = Managers.Resource.Instantiate("AlterEgo");
        _alterEgo.transform.position = _movePositions[0];
    }
    public bool RemovePosition()
    {
        if (_movePositions.Count > 0)
        {
            _movePositions.Remove(_movePositions[0]);
            return true;
        }

        return false;
    }

    private Coroutine _timebackRoutine = null;
    private IEnumerator TimebackRoutine()
    {
        _canTimeback = false;

        Managers.Sound.Play(Define.SoundType.TimeBack.ToString(), Define.SoundType.TimeBack);
        Managers.Sound.Play(Define.SoundType.TickTock.ToString(), Define.SoundType.TickTock);

        _movePositions.Reverse();

        yield return Util.GetWaitForSeconds(.05f);

        Managers.Resource.Instantiate("Time", transform);

        for (int n = 0; n < _movePositions.Count; n++)
        {
            transform.position = _movePositions[n];

            yield return Util.GetWaitForSeconds(.011f);
        }

        SetMaterial(false);

        foreach (var trail in _trails)
        {
            trail.time = 0f;
        }

        DestroyAlterEgo();
        _movePositions.Clear();
        SetAnimState(Define.AnimState.Idle);
    }


    public void CheckTimeback()
    {
        if (_canTimeback)
            SetAnimState(Define.AnimState.Timeback);
        else
            SetAnimState(Define.AnimState.Idle);
    }

    public void SetAnimState(Define.AnimState state)
    {
        switch (state)
        {
            case Define.AnimState.Idle:
                SetIdle();
                break;

            case Define.AnimState.Run:
                SetRun();
                break;

            case Define.AnimState.Timeback:
                SetTimeback();
                break;

            case Define.AnimState.Death:
                _animator.SetTrigger("Death");
                break;
        }
    }

    private void SetIdle()
    {
        _animator.ResetTrigger("Run");
        _animator.SetTrigger("Idle");

        if (_recordRoutine != null)
        {
            StopCoroutine(_recordRoutine);
            _recordRoutine = null;
        }

        if (_alterEgoRoutine != null)
        {
            DestroyAlterEgo();
            StopCoroutine(_alterEgoRoutine);
            _alterEgoRoutine = null;
        }

        if (_timebackRoutine != null)
        {
            StopCoroutine(_timebackRoutine);
            _timebackRoutine = null;
        }

        _movePositions.Clear();
    }

    private void SetRun()
    {
        _animator.ResetTrigger("Idle");
        _animator.SetTrigger("Run");

        if (_recordRoutine == null)
            _recordRoutine = StartCoroutine(RecordPositionRoutine());
    }

    private void SetTimeback()
    {
        _animator.ResetTrigger("Run");
        _animator.SetTrigger("Idle");

        if (_recordRoutine != null)
        {
            StopCoroutine(_recordRoutine);
            _recordRoutine = null;
        }

        if (_alterEgoRoutine != null)
        {
            DestroyAlterEgo();
            StopCoroutine(_alterEgoRoutine);
            _alterEgoRoutine = null;
        }

        if (_timebackRoutine == null)
            _timebackRoutine = StartCoroutine(TimebackRoutine());
    }

    public void SetMaterial(bool timeback)
    {
        if (timeback)
            _skinnedMeshRenderer.material = Managers.Resource.Load<Material>("Timeback");
        else
            _skinnedMeshRenderer.material = Managers.Resource.Load<Material>("Origin");
    }

    public void DestroyAlterEgo()
    {
        if (_alterEgo != null)
        {
            Destroy(_alterEgo);
            _alterEgo = null;
        }
    }
}
