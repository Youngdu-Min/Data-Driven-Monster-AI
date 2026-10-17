using UnityEngine;

public class StatusEffectReceiver : MonoBehaviour
{
    [SerializeField] private StatusEffect _statusEffect;
    private EffectableEntity _effectableEntity;

    private void Start()
    {
        _effectableEntity = GetComponent<EffectableEntity>();
    }

    [ContextMenu("TriggerSlow")]
    public void TriggerSlowStatus()
    {
        _effectableEntity.ApplyEffect(_statusEffect);
    }
}
