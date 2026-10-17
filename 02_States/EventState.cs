using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// FSM의 각 주기(Enter, Update, Exit)에 UnityEvent를 호출할 수 있는 상태 클래스
/// </summary>
public class EventState : BaseBahaviorState
{
    [Header("Event Settings")]
    [SerializeField] private UnityEvent[] _onEnterEvents;
    [SerializeField] private UnityEvent[] _onUpdateEvents;
    [SerializeField] private UnityEvent[] _onExitEvents;

    protected override void Fsm_Step_Enter()
    {
        base.Fsm_Step_Enter();
        ExecuteEvents(_onEnterEvents);
    }

    protected override void Fsm_Step_Update()
    {
        base.Fsm_Step_Update();
        ExecuteEvents(_onUpdateEvents);
    }

    protected override void Fsm_Step_Exit()
    {
        base.Fsm_Step_Exit();
        ExecuteEvents(_onExitEvents);
    }

    /// <summary>
    /// 등록된 UnityEvent 배열을 순회하며 실행
    /// </summary>
    private void ExecuteEvents(UnityEvent[] events)
    {
        if (events == null || events.Length == 0) return;

        foreach (var evt in events)
        {
            evt?.Invoke();
        }
    }
}