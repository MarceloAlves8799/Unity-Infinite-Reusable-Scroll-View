using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Metadata;

public class InfiniteReusableScrollView : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _elementPrefabRectTransform;
    [SerializeField] private DatabaseSO _dbElementName;
    [SerializeField] private Animator pullToRequestAnimator;

    private ScrollRect _scrollRect;
    private RectTransform _viewport;
    private RectTransform _content;
    private HorizontalOrVerticalLayoutGroup _layoutGroup;

    private ObjectPool _scrollViewElementPool;
    private HandleScrollViewElement _handleScrollViewElement;

    [Header("Settings")]
    public bool isLoading;
    public bool isSendingToPool;
    public bool isPullingElements;
    public float scrollThreshold = 0.95f;
    public int visibleElementsAmount;

    public int disableOffset;

    public float timeToGenerate;

    public Vector2 contentPositionToStop;
    public float contentInitialPosition;
    public float distanceFromTop;


    private void Awake()
    {
        _scrollViewElementPool = GetComponentInChildren<ObjectPool>();
        _scrollRect = GetComponentInChildren<ScrollRect>();
        _handleScrollViewElement = GetComponent<HandleScrollViewElement>();

        if (_dbElementName == null)
            Debug.LogError($"You need to add DatabaseSO as reference in object {gameObject.name} Inspector");

        if (_scrollRect == null)
        {
            Debug.LogError($"You need to add Scroll View as child object to {gameObject.name}");
            return;
        }

        _viewport = _scrollRect.viewport;
        _content = _scrollRect.content;

        if (_content != null)
            _layoutGroup = _content.GetComponent<HorizontalOrVerticalLayoutGroup>();
    }

    private void Start()
    {
        visibleElementsAmount = Mathf.CeilToInt(_viewport.rect.height / (_elementPrefabRectTransform.sizeDelta.y + _layoutGroup.spacing));

        contentInitialPosition = _content.anchoredPosition.y;
        contentPositionToStop = new Vector2(_scrollRect.content.anchoredPosition.x, contentInitialPosition - distanceFromTop);


        float timeToPopulate = 0.01f;
        StartCoroutine(PopulateScrollView(visibleElementsAmount, timeToPopulate));
    }

    private IEnumerator PopulateScrollView(int amount, float timeToWait, bool spawnOnBottom = true)
    {
        TMP_Text elementTMP = null;
        isLoading = true;

        for (int i = 0; i < amount; i++)
        {
            _handleScrollViewElement.HandleElementCount(1);

            DisableFirstElementAboveViewport();

            yield return new WaitForSeconds(timeToWait);

            GameObject elementInstance = _scrollViewElementPool.GetObjectFromPool();

            elementInstance.transform.SetParent(_content);
            
            if(spawnOnBottom)
                elementInstance.transform.SetAsLastSibling();
            else
                elementInstance.transform.SetAsFirstSibling();


            elementTMP = elementInstance.GetComponentInChildren<TMP_Text>();

            if (elementTMP != null)
                elementTMP.SetText(_dbElementName.GetNameFromDatabase());
        }

        isLoading = false;
    }

    public void GenerateElementsScroll(Vector2 scrollPosition)
    {
        if (isLoading) return;

        if (_scrollRect.verticalNormalizedPosition <= 1 - scrollThreshold)
        {
            StartCoroutine(PopulateScrollView(visibleElementsAmount, timeToGenerate));
        }

        if(_scrollRect.verticalNormalizedPosition >= 1.1f && !isPullingElements)
        {
            StartCoroutine(PullToRequest());
        }
    }

    public void DisableFirstElementAboveViewport()
    {
        int startToDisable = 20;
        if (_handleScrollViewElement.VisibleElementCount < startToDisable) return;

        Vector3 topViewportEdge = _viewport.TransformPoint(new Vector3(0, _viewport.rect.yMax, 0));

        foreach (Transform child in _content)
        {
            RectTransform childRect = child.GetComponent<RectTransform>();

            if (childRect.position.y > topViewportEdge.y + disableOffset)
            {
                _scrollViewElementPool.ReturnObjectToPool(child.gameObject);
                break;
            }
        }
    }

    IEnumerator PullToRequest()
    {
        isPullingElements = true;
        pullToRequestAnimator.gameObject.SetActive(true);
        pullToRequestAnimator.enabled = true;
        AnimatorStateInfo stateInfo = pullToRequestAnimator.GetCurrentAnimatorStateInfo(0);
        _content.anchoredPosition = contentPositionToStop;
        _scrollRect.enabled = false;

        yield return new WaitForSeconds(stateInfo.length);

        _scrollRect.enabled = true;
        pullToRequestAnimator.enabled = false;
        pullToRequestAnimator.gameObject.SetActive(false);
        isPullingElements = false;

        StartCoroutine(PopulateScrollView(visibleElementsAmount, timeToGenerate, false));

    }

}