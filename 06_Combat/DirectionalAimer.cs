using UnityEngine;

public class DirectionalAimer : MonoBehaviour, MonsterInterface.ISetMultiplier
{
    [SerializeField] private Transform _pivot;
    private Quaternion _targetRotation;
    private float _rotateSpeedMultiplier = 1f;

    public void SetTarget(Vector3 targetPosition)
    {
        Vector3 dir = targetPosition - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _targetRotation = Quaternion.Euler(0, 0, angle);
    }

    public void SetDirection(Vector2 dir)
    {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _targetRotation = Quaternion.Euler(0, 0, angle);
    }

    public void Tick(float rotationSpeed)
    {
        if (_pivot == null) { return; }

        _pivot.rotation = Quaternion.RotateTowards(
            _pivot.rotation,
            _targetRotation,
            rotationSpeed * _rotateSpeedMultiplier * Time.deltaTime
        );
    }

    public void SetMultiplier(float multiplier) => _rotateSpeedMultiplier = multiplier;
    public void ResetMultiplier() => _rotateSpeedMultiplier = 1f;
}