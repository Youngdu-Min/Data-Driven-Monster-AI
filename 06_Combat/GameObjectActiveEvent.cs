using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;

public class GameObjectActiveEvent : MonoBehaviour
{
    [SerializeField] private GameObject[] _gameObjects;
    [SerializeField] private float _showTimer;
    private CancellationTokenSource _cts;

    public async UniTask TriggerEventAsync(bool isTrue)
    {
        _cts?.Cancel();
        _cts = new CancellationTokenSource();

        try
        {
            foreach (var obj in _gameObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(isTrue);
                }
            }

            if (isTrue)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(_showTimer), cancellationToken: _cts.Token);
                await TriggerEventAsync(false);
            }
        }
        catch (System.OperationCanceledException)
        {

        }
    }

    public void SetTriggerEvent(bool isTrue)
        => TriggerEventAsync(isTrue).Forget();

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}
