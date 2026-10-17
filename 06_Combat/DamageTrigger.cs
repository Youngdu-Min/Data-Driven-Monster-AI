using Sirenix.OdinInspector;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    [SerializeField] private float _damage;
    [Required] private CircleCollider2D _collider;

    public void Init(float radius, int? damage = null)
    {
        _collider = GetComponent<CircleCollider2D>();
        _collider.radius = radius;

        if(damage != null)
        {
            _damage = (int)damage;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent(out DamageablePart part))
        {
            return;
        }

        part.SendDamage(_damage);
    }
}
