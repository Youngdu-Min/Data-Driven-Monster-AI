using SO.Monster;
using UnityEngine;

public class ReturnDistanceCondition : BaseDistanceCondition
{
    private Vector3 _spawnPos; // 초기화 시점의 몬스터 위치

    public override void Init(FsmData fsmData)
    {
        SetupConfig<DetectionConfigSO>(fsmData, config => config.ReturnArrivalRadius);
        _spawnPos = fsmData.BaseFsm.transform.position;
    }

    // 플레이어가 아닌 스폰 위치까지의 거리로 도착 여부를 판정
    public override bool IsConditionMet(Transform owner, FsmData fsmData, float stateEnterTime)
    {
        float dist = Vector3.Distance(fsmData.BaseFsm.transform.position, _spawnPos);
        return ExitIfNear ? (dist <= _distanceRadius) : (dist >= _distanceRadius);
    }
}
