using UnityEngine;

public class MonsterMediator : MonoBehaviour
{
    private BaseFsm _fsm;
    private StabilizeHandler _stabilize;
    //private StatusEffectHandler _statusEffect;

    public bool IsStabilized => _stabilize != null && _stabilize.IsStabilized;

    private void Awake()
    {
        _fsm = GetComponent<BaseFsm>();
        _stabilize = GetComponent<StabilizeHandler>();
        //_statusEffect = GetComponent<StatusEffectHandler>();

        // 초기화 시 중재자 자신을 각 컴포넌트에 등록 (순환 참조 해소)
        _fsm.SetMediator(this);
        _stabilize.SetMediator(this);
    }

    public void OnNotify(Component sender, string eventName)
    {
        switch (eventName)
        {
            case "OnStabilized":
            case "OnSleepEntered":
                _fsm.SetSystemPause(true);
                break;

            case "OnStabilizeExpired":
            case "OnSleepExited":
                _fsm.SetSystemPause(false);
                _fsm.TransitionToStateNonDelay(BaseFsm.BaseFsmStateType.Idle);
                break;
            default:
                break;
        }
    }
}