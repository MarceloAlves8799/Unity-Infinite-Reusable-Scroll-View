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

    [Header("Settings")]
    public bool isLoading;
    public float scrollThreshold = 0.95f;
    public int visibleElementsAmount;


    private void Awake()
    {
        _scrollViewElementPool = GetComponentInChildren<ObjectPool>();
        _scrollRect = GetComponentInChildren<ScrollRect>();

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

        PopulateScrollView(visibleElementsAmount);
    }

    private void PopulateScrollView(int amount)
    {
        TMP_Text elementTMP = null;
        isLoading = true;

        for (int i = 0; i < amount; i++)
        {
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
            PopulateScrollView(visibleElementsAmount);
        }
    }
}
