using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfiniteReusableScrollView : MonoBehaviour
{
    // Events
    public event Action OnPullToRefresh;

    // Scroll View References
    private ScrollRect _scrollRect;
    public ScrollRect ScrollRect { get { return _scrollRect; } }
    private RectTransform _viewport;
    private RectTransform _content;
    private HorizontalOrVerticalLayoutGroup _layoutGroup;

    // Composition reference
    private ObjectPool _scrollViewElementPool;

    [Header("Database")]
    [SerializeField] private DatabaseSO _dbElementName;

    [Header("Generate Elements")]
    [SerializeField] private RectTransform _elementPrefabRectTransform;
    [SerializeField] private float _scrollThreshold = 0.95f;
    [SerializeField] private float _generationTime = 0.1f;

    private bool _isLoading;
    private int _visibleElementsCount;
    public int VisibleElementsCount { get { return _visibleElementsCount; } }
    private int _totalElementsCount;

    [Header("Remove Elements")]
    [SerializeField, Tooltip("The minimum distance from the viewport boundary at which an element should be disabled")] 
    private float _disableOffset;
    
    [SerializeField, Tooltip("Minimum number of elements that must be instantiated before considering returning elements to the object pool")] 
    private int _minAmountToStartDisable;


    private void Awake()
    {
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _scrollViewElementPool = GetComponentInChildren<ObjectPool>();

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

        // Calculate the number of visible elements based on viewport size
        _visibleElementsCount = Mathf.CeilToInt(_viewport.rect.height / (_elementPrefabRectTransform.sizeDelta.y + _layoutGroup.spacing));
    }

    private void OnEnable()
    {
        _scrollRect.onValueChanged.AddListener(GenerateElementsScroll);
    }

    private void Start()
    {
        int startVisibleElements = _visibleElementsCount * 3;

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
            OnGenerateElementsOnBottom(_visibleElementsCount);
        }

        // Check if scroll position is close to top threshold for pulling to refresh
        float offsetTop = 1.05f;
        if(_scrollRect.verticalNormalizedPosition > offsetTop)
        {
            OnPullToRefresh?.Invoke();
        }
    }

    #region Spawn Elements methods

    public void OnGenerateElementsOnTop(int amount)
    {
        StartCoroutine(PopulateScrollView(amount, _generationTime, false));
    }

    public void OnGenerateElementsOnBottom(int amount)
    {
        StartCoroutine(PopulateScrollView(amount, _generationTime));
    }

    // Coroutine to handle populating the scroll view with elements 
    private IEnumerator PopulateScrollView(int amount, float timeToWait, bool spawnOnBottom = true)
    {
        TMP_Text elementTMP = null;
        _isLoading = true;

        for (int i = 0; i < amount; i++)
        {
            _totalElementsCount++;

            if (spawnOnBottom)
                DisableFirstElementAboveViewport(); 
            else
                DisableFirstElementsBelowViewport();

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

    #endregion


    // Method to disable elements above the viewport
    public void DisableFirstElementAboveViewport()
    {
        // Handle to starting to send elements to the pool when is more than disableStart
        _minAmountToStartDisable = 20;
        if (_totalElementsCount < _minAmountToStartDisable) return;

        Vector3 topViewportEdge = _viewport.TransformPoint(new Vector3(0, _viewport.rect.yMax, 0));

        // Send to pool the first element and the far from viewport
        foreach (Transform child in _content)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();

            if (childRect.position.y > topViewportEdge.y + _disableOffset)
            {
                _totalElementsCount--;
                _scrollViewElementPool.ReturnObjectToPool(child.gameObject);
                break;
            }
        }
    }

    // Method to disable elements below the viewport
    public void DisableFirstElementsBelowViewport()
    {
        // Handle to starting to send elements to the pool when is more than disableStart
        _minAmountToStartDisable = 20;
        if (_totalElementsCount < _minAmountToStartDisable) return;

        Vector3 bottomViewportEdge = _viewport.TransformPoint(new Vector3(0, _viewport.rect.yMin, 0));

        // Send to pool elements below the viewport
        foreach (Transform child in _content)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();

            if (childRect.position.y < bottomViewportEdge.y - _disableOffset)
            {
                _totalElementsCount--;
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