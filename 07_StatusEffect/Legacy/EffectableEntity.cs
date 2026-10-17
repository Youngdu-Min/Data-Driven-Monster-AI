using System;
using System.Collections.Generic;
using UnityEngine;

public class EffectableEntity : MonoBehaviour, IUpdateObserver
{
    private List<ActiveEffect> _activeEffects = new List<ActiveEffect>();

    private void Awake()
    {
        UpdatePublisher.SubscribeObserver(this);
    }

    public void ObserverUpdate(float dt)
    {
        float deltaTime = Time.deltaTime;

        // Update each active effect
        for (int i = _activeEffects.Count - 1; i >= 0; i--)
        {
            ActiveEffect effect = _activeEffects[i];
            effect.Tick(deltaTime);

            if (effect.IsExpired)
            {
                RemoveEffect(effect);
            }
        }
    }

    public void ApplyEffect(StatusEffect effect)
    {
        if (!effect.IsStackable && _activeEffects.Exists(e => e.Effect == effect))
        {
            Debug.Log($"{effect.EffectName} is already applied and is not stackable.");
            return;
        }

        ActiveEffect newEffect = new ActiveEffect(effect, gameObject);
        _activeEffects.Add(newEffect);
        newEffect.Apply();
    }

    private void RemoveEffect(ActiveEffect effect)
    {
        effect.Remove();
        _activeEffects.Remove(effect);
    }

    public void Dispose()
    {
        UpdatePublisher.DiscribeObserver(this);
    }
}

[Serializable]
public class ActiveEffect
{
    public StatusEffect Effect { get; private set; }
    private GameObject _target;
    private float _remainingDuration;

    public bool IsExpired => _remainingDuration <= 0;

    public ActiveEffect(StatusEffect effect, GameObject target)
    {
        Effect = effect;
        _target = target;
        _remainingDuration = effect.Duration;
    }

    public void Apply()
    {
        Effect.ApplyEffect(_target);
    }

    public void Tick(float deltaTime)
    {
        _remainingDuration -= deltaTime;
        Effect.Tick(_target, deltaTime);
    }

    public void Remove()
    {
        Effect.RemoveEffect(_target);
    }
}
