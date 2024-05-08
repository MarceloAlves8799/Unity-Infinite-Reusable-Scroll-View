using DG.Tweening;
using UnityEngine;

public class ScrollviewElement : MonoBehaviour, IPoolable
{
    [Header("Animation time")]
    [SerializeField] private float spawnAnimationDuration;
    [SerializeField] private float despawnAnimationDuration;


    public void OnSpawn()
    {
        transform.localScale = Vector3.zero;
        transform.DOScale(Vector3.one, spawnAnimationDuration);
    }

    public void OnDespawn()
    {
        transform.localScale = Vector3.one;
        transform.DOScale(Vector3.zero, despawnAnimationDuration).
                  OnComplete(() => gameObject.SetActive(false));
    }
}
