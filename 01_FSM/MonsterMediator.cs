using UnityEngine;

public class MonsterMediator : MonoBehaviour
{
    private BaseFsm _fsm;
    private StabilizationHandler _stabilization;
    //private StatusEffectHandler _statusEffect;

    public bool IsStabilized => _stabilization != null && _stabilization.IsStabilized;

    private void Awake()
    {
        _fsm = GetComponent<BaseFsm>();
        _stabilization = GetComponent<StabilizationHandler>();
        //_statusEffect = GetComponent<StatusEffectHandler>();

        // 초기화 시 중재자 자신을 각 컴포넌트에 등록 (순환 참조 해소)
        _fsm.SetMediator(this);
        _stabilization.SetMediator(this);
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