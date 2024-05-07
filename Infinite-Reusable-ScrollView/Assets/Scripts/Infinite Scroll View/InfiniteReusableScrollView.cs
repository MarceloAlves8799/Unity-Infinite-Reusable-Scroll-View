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

    private ScrollRect _scrollRect;
    private RectTransform _viewport;
    private RectTransform _content;
    private HorizontalOrVerticalLayoutGroup _layoutGroup;

    private ObjectPool _scrollViewElementPool;
    private HandleScrollViewElement _handleScrollViewElement;

    [Header("Settings")]
    public bool isLoading;
    public bool isSendingToPool;
    public float scrollThreshold = 0.95f;
    public int visibleElementsAmount;

    public int disableOffset;

    public float timeToGenerate;


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

        float timeToPopulate = 0.01f;
        StartCoroutine(PopulateScrollView(visibleElementsAmount, timeToPopulate));
    }

    private IEnumerator PopulateScrollView(int amount, float timeToWait)
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
            elementInstance.transform.SetAsLastSibling();

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
    }

    public void DisableFirstElementAboveViewport()
    {
        if (_handleScrollViewElement.VisibleElementCount < 20) return;

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


  
}
