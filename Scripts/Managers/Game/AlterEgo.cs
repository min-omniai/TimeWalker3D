using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlterEgo : MonoBehaviour
{
    private Vector3 _targetPosition = Vector3.zero;
    private Rigidbody _rigidbody = null;

    private void OnEnable()
    {
        if (_rigidbody == null) _rigidbody = GetComponent<Rigidbody>();

        transform.position = Managers.Game.Player.MovePositions[0];
        if (Managers.Game.Player.RemovePosition())
            _targetPosition = Managers.Game.Player.MovePositions[0];
        else
            Managers.Game.Player.DestroyAlterEgo();
    }

    private void FixedUpdate()
    {
        if (Managers.Game.GameStatePlay)
        {
            if (Managers.Game.Player.CanTimeback)
            {
                float distance = (transform.position - _targetPosition).sqrMagnitude;
                if (distance <= 5f)
                {
                    if (Managers.Game.Player.RemovePosition())
                        _targetPosition = Managers.Game.Player.MovePositions[0];
                }
                else
                {
                    Vector3 dir = _targetPosition - transform.position;
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 10);
                }

                _rigidbody.position += transform.forward * 35 * Time.deltaTime;
            }
        }
    }
}
