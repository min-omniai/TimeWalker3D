using System.Collections;
using UnityEngine;

public class Poolable : MonoBehaviour
{
    Coroutine _timerRoutine;
    private bool _isUsing;
    public bool IsUsing
    {
        get { return _isUsing; }
        set
        {
            _isUsing = value;
            if (!_isUsing && _timerRoutine != null)
            {
                StopCoroutine(_timerRoutine);
            }
        }
    }
    ///<summary>잠시 후 오브젝트를 끄고 Pool로 Push하는 역할</summary>
    public void Timer(float timer)
    {
        _timerRoutine = StartCoroutine(TimerRoutine(timer));
    }
    IEnumerator TimerRoutine(float timer)
    {
        yield return Util.GetWaitForSeconds(timer);
        Managers.Resource.Destroy(gameObject);
    }
}
