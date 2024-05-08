using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteReusableScrollView : MonoBehaviour
{
    // Scroll View References
    private ScrollRect _scrollRect;
    private RectTransform _viewport;
    private RectTransform _content;
    private HorizontalOrVerticalLayoutGroup _layoutGroup;

    // Composition references
    private ObjectPool _scrollViewElementPool;
    private HandleScrollViewElement _handleScrollViewElement;

    [Header("Database")]
    [SerializeField] private DatabaseSO _dbElementName;

    [Header("Generate Elements")]
    [SerializeField] private RectTransform _elementPrefabRectTransform;
    [SerializeField] private float _scrollThreshold;
    [SerializeField] private float _disableOffset;
    [SerializeField] private float _generationTime;

    private bool _isLoading;
    private int _visibleElementsCount;

    [Header("Pull to refresh")]
    [SerializeField] private Animator _pullToRefreshAnimator;
    [SerializeField] private float _distanceFromTop;

    private Vector2 _stopContentPosition;
    private float _initialContentPosition;
    private bool _isPullingElements;


    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _scrollViewElementPool = GetComponentInChildren<ObjectPool>();
        _handleScrollViewElement = GetComponent<HandleScrollViewElement>();

        // Check if database reference is missing
        if (_dbElementName == null)
            Debug.LogError($"You need to add DatabaseSO as a reference in object {gameObject.name} Inspector");

        // Check if ScrollRect component is missing
        if (_scrollRect != null)
        {
            _viewport = _scrollRect.viewport;
            _content = _scrollRect.content;

            if (_content != null)
                _layoutGroup = _content.GetComponent<HorizontalOrVerticalLayoutGroup>();
            else
                Debug.LogError($"You need to add a Layout Group in your {_content.name}");
        }

        else
        {
            Debug.LogError($"You need to add Scroll View as a child object to {gameObject.name}");
        }

        // Check if ObjectPool component is missing
        if (_scrollViewElementPool == null)
            Debug.LogError($"You need a Object Pool reference as a child gameObject in {gameObject.name}");

        // Check if HandleScrollViewElement component is missing
        if (_handleScrollViewElement == null)
            Debug.LogError($"You need a HandleScrollViewElement reference in {gameObject.name} Inspector");

    }

    private void OnEnable()
    {
        _scrollRect.onValueChanged.AddListener(GenerateElementsScroll);
    }

    private void Start()
    {
        // Calculate the number of visible elements based on viewport size
        _visibleElementsCount = Mathf.CeilToInt(_viewport.rect.height / (_elementPrefabRectTransform.sizeDelta.y + _layoutGroup.spacing));
        int startVisibleElements = _visibleElementsCount * 3;

        // Store initial content position and position to stop content when pulling to refresh
        _initialContentPosition = _content.anchoredPosition.y;
        _stopContentPosition = new Vector2(_scrollRect.content.anchoredPosition.x, _initialContentPosition - _distanceFromTop);

        // Populating the scroll view with the starter elements
        float timeToPopulate = 0.01f;
        StartCoroutine(PopulateScrollView(startVisibleElements, timeToPopulate));
    }

    // Method to handle generation of scroll view elements based on scrolling
    public void GenerateElementsScroll(Vector2 scrollPosition)
    {
        if (_isLoading) return;

        // Check if scroll position is close to bottom threshold for generating more elements
        float offsetBottom = 1 - _scrollThreshold;
        if (_scrollRect.verticalNormalizedPosition <= offsetBottom)
        {
            StartCoroutine(PopulateScrollView(_visibleElementsCount, _generationTime));
        }

        // Check if scroll position is close to top threshold for pulling to refresh
        float offsetTop = 1.05f;
        if(_scrollRect.verticalNormalizedPosition > offsetTop && !_isPullingElements)
        {
            StartCoroutine(PullToRefresh());
        }
    }

    // Coroutine to handle pull-to-refresh functionality
    IEnumerator PullToRefresh()
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

        StartCoroutine(PopulateScrollView(_visibleElementsCount, _generationTime, false));
    }

    // Coroutine to handle populating the scroll view with elements 
    private IEnumerator PopulateScrollView(int amount, float timeToWait, bool spawnOnBottom = true)
    {
        TMP_Text elementTMP = null;
        _isLoading = true;

        for (int i = 0; i < amount; i++)
        {
            _handleScrollViewElement?.HandleElementCount(1);
            DisableFirstElementAboveViewport(); // Disable elements out the viewport each time that spawn a new element

            yield return new WaitForSeconds(timeToWait);

            // Get object from pool, set text and parent element
            GameObject elementInstance = _scrollViewElementPool?.GetObjectFromPool();
            elementTMP = elementInstance.GetComponentInChildren<TMP_Text>();

            elementTMP?.SetText(_dbElementName.GetNameFromDatabase());

            elementInstance.transform.SetParent(_content);

            if (spawnOnBottom)
                elementInstance.transform.SetAsLastSibling();
            else
                elementInstance.transform.SetAsFirstSibling();
        }

        _isLoading = false;
    }

    // Method to disable elements above the viewport
    public void DisableFirstElementAboveViewport()
    {
        // Handle to starting to send elements to the pool when is more than disableStart
        int disableStart = 20;
        if (_handleScrollViewElement.VisibleElementCount < disableStart) return;

        Vector3 topViewportEdge = _viewport.TransformPoint(new Vector3(0, _viewport.rect.yMax, 0));

        // Send to pool the first element and the far from viewport
        foreach (Transform child in _content)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();

            if (childRect.position.y > topViewportEdge.y + _disableOffset)
            {
                _scrollViewElementPool.ReturnObjectToPool(child.gameObject);
                break;
            }
        }
    }

    private void OnDisable()
    {
        _scrollRect.onValueChanged.RemoveListener(GenerateElementsScroll);
    }

}