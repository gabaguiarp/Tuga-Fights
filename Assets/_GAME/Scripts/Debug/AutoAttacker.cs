using System.Collections;
using UnityEngine;
using MemeFight.Animation;

public class AutoAttacker : MonoBehaviour
{
    [SerializeField] bool _active = true;
    [SerializeField] float _attackSpeed = 7.0f;
    [SerializeField] float _loopInterval = 1.0f;
    [Tooltip("The portion of the journey to enable/disable the damager.")]
    [Range(0.0f, 1.0f)]
    [SerializeField] float _damageThreshold = 0.8f;

    [Header("References")]
    [SerializeField] Transform _armPivot;
    [SerializeField] Transform _armRig;
    [SerializeField] Transform _armTargetPosition;

    [Header("Components")]
    [SerializeField] FighterAnimationEvents _animationEvents;

    bool _runningLoop;
    bool _damagerEnabled;

    void Start()
    {
        if (_active)
            StartCoroutine(AttackLoop());
    }

    void Update()
    {
        if (_active && !_runningLoop)
        {
            StartCoroutine(AttackLoop());
        }
    }

    void SetArmPositionInTime(float t)
    {
        _armRig.position = Vector3.Lerp(_armPivot.position, _armPivot.position - (transform.right * _armTargetPosition.position.x), t);
    }

    IEnumerator AttackLoop()
    {
        float t = 0.0f;
        _runningLoop = true;

        while (_active)
        {
            // Punch
            while (t < 1)
            {
                t += _attackSpeed * Time.deltaTime;

                if (t >= _damageThreshold && !_damagerEnabled)
                {
                    _animationEvents.PunchStart();
                    _damagerEnabled = true;
                }

                SetArmPositionInTime(t);

                yield return null;
            }

            // Retract arm
            while (t > 0)
            {
                t -= _attackSpeed * Time.deltaTime;

                if (t <= _damageThreshold && _damagerEnabled)
                {
                    _animationEvents.PunchEnd();
                    _damagerEnabled = false;
                }

                SetArmPositionInTime(t);

                yield return null;
            }

            yield return CoroutineUtils.GetWaitTime(_loopInterval);
        }

        OnLoopOver();

        _runningLoop = false;
    }

    void OnLoopOver()
    {
        SetArmPositionInTime(0);
        _animationEvents.PunchEnd();
        _damagerEnabled = false;
    }
}
