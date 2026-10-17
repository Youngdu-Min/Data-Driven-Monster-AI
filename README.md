# ProjectFS-MonsterAI — 몬스터 AI 시스템

> **ProjectFS 개인 기여 코드 시리즈** — 팀 프로젝트 ProjectFS(비공개 저장소)에서 본인이 작성한 코드만 추출·재분류한 저장소입니다.
> 1. [ProjectFS-MonsterAI](https://github.com/Youngdu-Min/ProjectFS-MonsterAI) — 몬스터 AI (~2026-04-19)
> 2. [ProjectFS-Multiplayer-2DMap](https://github.com/Youngdu-Min/ProjectFS-Multiplayer-2DMap) — 탈출·Worm·생존자 추적·2D 절차적 맵 생성 (2026-04-20 ~ 09-09)
> 3. [ProjectFS-3DMap-Rendering](https://github.com/Youngdu-Min/ProjectFS-3DMap-Rendering) — 3D 맵 생성·LevelModule·Convolution 포스트 프로세싱 (2026-09-10 ~)

- 기준 시점: 2026-04-19 (본인 마지막 커밋 `129fa988`, 2026-04-11)
- 추출 기준: `git blame`상 본인(youngdoomin) 작성 라인 기준. 팀원 작성 코드(Bind / SleepArea 등)는 제외
- 파일 내용은 원본 그대로이며, 폴더 구조만 기능별로 재분류함

## 폴더 구조

| 폴더 | 내용 |
|---|---|
| `01_FSM` | FSM 코어 (`Fsm`, `BaseFsm`, `MonsterFsm`) 및 FSM–안정화 중재자 `MonsterMediator` |
| `02_States` | 행동 상태 (Idle / Alert / Attack / Event), `Move/` 이동 계열 (Chase / Escape / Return / Wander) |
| `03_Conditions` | 상태 전이 조건 — `Distance/` 거리, `Sight/` 시야(FOV), `Time/` 시간, AND 복합 조건 |
| `04_Stabilization` | 안정화(비살상 제압) 스택·배율·면역 처리 |
| `05_PathFinding` | `PathfindingManager`(월드↔그리드 변환). `ThirdParty_Ported/`는 오픈소스 A*(Sebastian Lague / Ronen Ness) 포팅본 |
| `06_Combat` | 공용 전투 모듈 — Health, DamageablePart, DamageTrigger, DirectionalAimer, SpriteFlipper |
| `07_StatusEffect` | 상태이상 데이터 기반 (Bind / Sleep), `Legacy/` 구버전 구조 |
| `08_Data` | ScriptableObject 설정 (Movement / Detection / Attack / Stabilization), 몬스터 인터페이스 |
| `09_Debug` | 몬스터 단독 테스트용 디버깅 씬 스크립트 |

## 참고

- `09_Debug/DebuggingMonsterScene.cs`는 104줄 중 95줄 본인 작성 (9줄 팀원 수정 포함)
- `05_PathFinding/ThirdParty_Ported/`는 외부 코드이므로 포트폴리오에서 "직접 구현"으로 표기하지 않도록 주의
- 프로젝트 의존성(UniTask, Odin Inspector, UpdatePublisher 등)이 없어 이 폴더 단독으로는 컴파일되지 않음

## 원본 경로 매핑

| 분류 경로 | 원본 경로 (`Assets/GameAssets/02_Scripts/` 기준) |
|---|---|
| `01_FSM/Fsm.cs` | `03_Gameflow/Monster/01_Fsm/Fsm.cs` |
| `01_FSM/BaseFsm.cs` | `03_Gameflow/Monster/01_Fsm/BaseFsm.cs` |
| `01_FSM/MonsterFsm.cs` | `03_Gameflow/Monster/01_Fsm/MonsterFsm.cs` |
| `01_FSM/FsmExample.cs` | `03_Gameflow/Monster/01_Fsm/FsmExample.cs` |
| `01_FSM/MonsterMediator.cs` | `03_Gameflow/Monster/MonsterMediator.cs` |
| `02_States/BaseBahaviorState.cs` | `03_Gameflow/Monster/02_CommonState/BaseBahaviorState.cs` |
| `02_States/IdleState.cs` | `03_Gameflow/Monster/02_CommonState/IdleState.cs` |
| `02_States/AlertState.cs` | `03_Gameflow/Monster/02_CommonState/AlertState.cs` |
| `02_States/AttackState.cs` | `03_Gameflow/Monster/02_CommonState/AttackState.cs` |
| `02_States/EventState.cs` | `03_Gameflow/Monster/02_CommonState/EventState.cs` |
| `02_States/Move/MovableState.cs` | `03_Gameflow/Monster/02_CommonState/Move/MovableState.cs` |
| `02_States/Move/ChaseState.cs` | `03_Gameflow/Monster/02_CommonState/Move/ChaseState.cs` |
| `02_States/Move/EscapeState.cs` | `03_Gameflow/Monster/02_CommonState/Move/EscapeState.cs` |
| `02_States/Move/ReturnState.cs` | `03_Gameflow/Monster/02_CommonState/Move/ReturnState.cs` |
| `02_States/Move/WanderState.cs` | `03_Gameflow/Monster/02_CommonState/Move/WanderState.cs` |
| `03_Conditions/BehaviorStateCondition.cs` | `03_Gameflow/Monster/03_Condition/BehaviorStateCondition.cs` |
| `03_Conditions/AndCompositeCondition.cs` | `03_Gameflow/Monster/03_Condition/AndCompositeCondition.cs` |
| `03_Conditions/Distance/BaseDistanceCondition.cs` | `03_Gameflow/Monster/03_Condition/Distance/BaseDistanceCondition.cs` |
| `03_Conditions/Distance/AttackDistanceCondition.cs` | `03_Gameflow/Monster/03_Condition/Distance/AttackDistanceCondition.cs` |
| `03_Conditions/Distance/DetectionDistanceCondition.cs` | `03_Gameflow/Monster/03_Condition/Distance/DetectionDistanceCondition.cs` |
| `03_Conditions/Distance/EscapeDistanceCondition.cs` | `03_Gameflow/Monster/03_Condition/Distance/EscapeDistanceCondition.cs` |
| `03_Conditions/Distance/ReturnDistanceCondition.cs` | `03_Gameflow/Monster/03_Condition/Distance/ReturnDistanceCondition.cs` |
| `03_Conditions/Distance/LostDistanceDelayCondition.cs` | `03_Gameflow/Monster/03_Condition/Distance/LostDistanceDelayCondition.cs` |
| `03_Conditions/Distance/LostDistanceInAlertCondition.cs` | `03_Gameflow/Monster/03_Condition/Distance/LostDistanceInAlertCondition.cs` |
| `03_Conditions/Distance/LostDistanceTimeCondition.cs` | `03_Gameflow/Monster/03_Condition/Distance/LostDistanceTimeCondition.cs` |
| `03_Conditions/Sight/BaseSightCondition.cs` | `03_Gameflow/Monster/03_Condition/Sight/BaseSightCondition.cs` |
| `03_Conditions/Sight/DetectionSightCondition.cs` | `03_Gameflow/Monster/03_Condition/Sight/DetectionSightCondition.cs` |
| `03_Conditions/Sight/LostSightTimeCondition.cs` | `03_Gameflow/Monster/03_Condition/Sight/LostSightTimeCondition.cs` |
| `03_Conditions/Time/BaseTimeCondition.cs` | `03_Gameflow/Monster/03_Condition/Time/BaseTimeCondition.cs` |
| `03_Conditions/Time/AttackTimeCondition.cs` | `03_Gameflow/Monster/03_Condition/Time/AttackTimeCondition.cs` |
| `04_Stabilization/StabilizationHandler.cs` | `03_Gameflow/Monster/05_Stabilization/StabilizationHandler.cs` |
| `05_PathFinding/PathfindingManager.cs` | `03_Gameflow/Monster/99_PathFinding/PathfindingManager.cs` |
| `05_PathFinding/ThirdParty_Ported/PathFinding.cs` | `03_Gameflow/Monster/99_PathFinding/PathFinding.cs` |
| `05_PathFinding/ThirdParty_Ported/Grid.cs` | `03_Gameflow/Monster/99_PathFinding/Grid.cs` |
| `05_PathFinding/ThirdParty_Ported/Node.cs` | `03_Gameflow/Monster/99_PathFinding/Node.cs` |
| `05_PathFinding/ThirdParty_Ported/Point.cs` | `03_Gameflow/Monster/99_PathFinding/Point.cs` |
| `06_Combat/Health.cs` | `09_Etc/Health.cs` |
| `06_Combat/DamageablePart.cs` | `09_Etc/DamageablePart.cs` |
| `06_Combat/DamageTrigger.cs` | `09_Etc/DamageTrigger.cs` |
| `06_Combat/DirectionalAimer.cs` | `09_Etc/DirectionalAimer.cs` |
| `06_Combat/SpriteFlipper.cs` | `09_Etc/SpriteFlipper.cs` |
| `06_Combat/GameObjectActiveEvent.cs` | `09_Etc/GameObjectActiveEvent.cs` |
| `07_StatusEffect/StatusEffectStat.cs` | `03_Gameflow/Monster/06_StatusEffect/StatusEffectStat.cs` |
| `07_StatusEffect/BindEffectStat.cs` | `03_Gameflow/Monster/06_StatusEffect/BindEffectStat.cs` |
| `07_StatusEffect/SleepEffectStat.cs` | `03_Gameflow/Monster/06_StatusEffect/SleepEffectStat.cs` |
| `07_StatusEffect/Legacy/EffectableEntity.cs` | `03_Gameflow/Monster/06_StatusEffect/Lagacy/EffectableEntity.cs` |
| `07_StatusEffect/Legacy/StatusEffect.cs` | `03_Gameflow/Monster/06_StatusEffect/Lagacy/StatusEffect.cs` |
| `07_StatusEffect/Legacy/StatusEffectReceiver.cs` | `03_Gameflow/Monster/06_StatusEffect/Lagacy/StatusEffectReceiver.cs` |
| `08_Data/MovementConfigSO.cs` | `07_Data/Monster/MovementConfigSO.cs` |
| `08_Data/DetectionConfigSO.cs` | `07_Data/Monster/DetectionConfigSO.cs` |
| `08_Data/EscapeDetectionConfigSO.cs` | `07_Data/Monster/EscapeDetectionConfigSO.cs` |
| `08_Data/AttackConfigSO.cs` | `07_Data/Monster/AttackConfigSO.cs` |
| `08_Data/Stabilization/BaseStabilizationConfigSO.cs` | `07_Data/Monster/Stabilization/BaseStabilizationConfigSO.cs` |
| `08_Data/Stabilization/CombatStabilizationConfigSO.cs` | `07_Data/Monster/Stabilization/CombatStabilizationConfigSO.cs` |
| `08_Data/MonsterInterface.cs` | `07_Data/Monster/Etc/MonsterInterface.cs` |
| `09_Debug/DebuggingMonsterScene.cs` | `02_Scenes/DebuggingMonsterScene.cs` |
