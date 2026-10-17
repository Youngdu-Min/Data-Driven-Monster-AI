using Cysharp.Threading.Tasks;
using Sirenix.Utilities;
using UnityEngine;

public class DebuggingMonsterScene : AbstractScene
{
    // ==================== Inspector 노출 ====================
    [Header("플레이어 설정")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private ProjectilePoolManager _projectilePoolManager;
    [SerializeField] private SleepAreaManager _sleepAreaManager;

    // ==================== AbstractScene ====================

    protected override int SceneIdx => -1;

    /// <summary>
    /// 서비스 등록 단계.
    /// AbstractScene.Start()에서 BeginGame 이후 _soundService.Clear()가 호출되므로
    /// NullReferenceException을 방지하기 위해 NullSoundService stub을 주입한다.
    /// </summary>
    protected override void BindObjects()
    {
        // 오디오 서비스가 없는 디버그 환경에서도 AbstractScene이 정상 동작하도록 stub 주입
        _soundService = new NullSoundService();

        if (_playerController == null)
            Debug.LogError("[DebuggingPlayerScene] PlayerController가 Inspector에 할당되지 않았습니다.");
    }

    /// <summary>Addressables 프리로드 없음 — 즉시 완료.</summary>
    protected override async UniTask InitializeObjects()
    {
        await UniTask.CompletedTask;
    }

    protected override async UniTask CreateObjects()
    {
        if (_playerController == null)
        {
            Debug.LogError("[DebuggingPlayerScene] PlayerController가 없어 CreateObjects를 건너뜁니다.");
            await UniTask.CompletedTask;
            return;
        }

        _sleepAreaManager?.Initialize();
        _projectilePoolManager?.Initialize(_sleepAreaManager);
        _playerController.Initialize(Camera.main, _projectilePoolManager);

        BaseFsm[] fsms = FindObjectsByType<BaseFsm>(FindObjectsSortMode.None);
        fsms.ForEach(fsm => fsm.Init());

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// UpdatePublisher 구독 시작 및 디버그용 이벤트 리스너를 등록한다.
    /// </summary>
    protected override void PrepareGame()
    {
        if (_playerController == null)
            return;

        _playerController.StartUpdate();

        // 디버그 로그로 이벤트 수신 확인
        _playerController.OnHpChanged += OnHpChanged;
        _playerController.OnDied += OnDied;
        _playerController.OnWeaponSwapped += OnWeaponSwapped;
    }

    /// <summary>게임 루프 시작 — 디버그 씬에서는 별도 처리 없음.</summary>
    protected override async UniTask BeginGame()
    {
        await UniTask.CompletedTask;
    }

    // ==================== Unity 콜백 ====================

    private void OnDestroy()
    {
        // 씬 전환 시 PlayerController의 UpdatePublisher 구독이 누수되지 않도록 정리
        _playerController?.Cleanup();
        _projectilePoolManager?.Cleanup();
        _sleepAreaManager?.Cleanup();
    }

    // ==================== 디버그 이벤트 핸들러 ====================

    private void OnHpChanged(float currentHp, float maxHp)
    {
        Debug.Log($"[DebuggingPlayerScene] HP 변경: {currentHp} / {maxHp}");
    }

    private void OnDied()
    {
        Debug.Log("[DebuggingPlayerScene] 플레이어 사망.");
    }

    private void OnWeaponSwapped(WeaponType equipment)
    {
        Debug.Log($"[DebuggingPlayerScene] 장비 교체: {equipment}");
    }
}
