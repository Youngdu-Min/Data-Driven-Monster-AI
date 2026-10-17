using UnityEngine;

public class SpriteFlipper : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public void SetFlip(bool flip) => _spriteRenderer.flipX = flip;

    void Start()
    {
        Debug.Assert(_spriteRenderer != null, $"{gameObject.name}에 SpriteRenderer가 없습니다!");
    }
}
