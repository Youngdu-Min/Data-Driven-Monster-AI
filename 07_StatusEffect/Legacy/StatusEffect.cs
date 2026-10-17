using UnityEngine;

public abstract class StatusEffect : ScriptableObject
{
    [field: SerializeField] public string EffectName { get; private set; }

    [field: SerializeField] public float Duration { get; private set; } // Duration of the effect
    [field: SerializeField] public bool IsStackable { get; private set; } // Can this effect stack?

    public abstract void ApplyEffect(GameObject target);
    public abstract void RemoveEffect(GameObject target);

    // Optional: Logic for effects that tick over time
    public virtual void Tick(GameObject target, float deltaTime) { }
}
