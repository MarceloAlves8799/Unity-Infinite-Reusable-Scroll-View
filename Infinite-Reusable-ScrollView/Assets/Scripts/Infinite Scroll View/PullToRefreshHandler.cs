using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(InfiniteReusableScrollView))]
public class PullToRefreshHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Animator _pullToRefreshAnimator;

    private InfiniteReusableScrollView _infiniteScrollView;
    private RectTransform _content;

    [Header("Settings")]
    [SerializeField] private float _distanceFromTop;

    private Vector2 _stopContentPosition;
    private float _initialContentPosition;
    private bool _isPullingElements;


    private void Awake()
    {
        if(_scrollRect != null)
            _content = _scrollRect.content;

        _infiniteScrollView = GetComponent<InfiniteReusableScrollView>();
    }

    private void OnEnable()
    {
        _infiniteScrollView.OnPullToRefresh += OnPullToRefresh;
    }

    private void Start()
    {
        // Store initial content position and position to stop content when pulling to refresh
        _initialContentPosition = _content.anchoredPosition.y;
        _stopContentPosition = new Vector2(_scrollRect.content.anchoredPosition.x, _initialContentPosition - _distanceFromTop);
    }

    private void OnPullToRefresh()
    {
        if (_isPullingElements) return;

        StartCoroutine(PullToRefresh());
    }

    // Coroutine to handle pull-to-refresh functionality
    private IEnumerator PullToRefresh()
    {
        _isPullingElements = true;

        _pullToRefreshAnimator.gameObject.SetActive(true); // Enable feedback
        AnimatorStateInfo stateInfo = _pullToRefreshAnimator.GetCurrentAnimatorStateInfo(0); // Get current animation info
        float animationDuration = stateInfo.length; // Store the animation duration time
        _content.anchoredPosition = _stopContentPosition; // Lock the content to position
        _scrollRect.enabled = false; // Disable interaction with scroll

        yield return new WaitForSeconds(animationDuration);

        _scrollRect.enabled = true;
        _pullToRefreshAnimator.gameObject.SetActive(false);

        _isPullingElements = false;

        _infiniteScrollView.OnGenerateElementsOnTop(_infiniteScrollView.VisibleElementsCount);
    }

    private void OnDisable()
    {
        _infiniteScrollView.OnPullToRefresh -= OnPullToRefresh;
    }

}
