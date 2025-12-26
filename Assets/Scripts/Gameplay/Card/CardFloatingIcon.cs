using UnityEngine;
using DG.Tweening;

public class CardFloatingIcon : MonoBehaviour
{
    [Header("Settings")]
    public float floatHeight = 0.5f;
    public float duration = 1.5f;

    void Start()
    {
        transform.DOLocalMoveY(transform.localPosition.y + floatHeight, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .SetLink(gameObject);
    }
    
}
