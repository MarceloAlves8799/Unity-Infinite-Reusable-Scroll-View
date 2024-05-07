using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
        int initialElementsAmount = Mathf.CeilToInt(_viewport.rect.height / (_elementPrefabRectTransform.sizeDelta.y + _layoutGroup.spacing));

        PopulateScrollView(initialElementsAmount);
    }

    private void PopulateScrollView(int amount)
    {
        TMP_Text elementTMP = null;

        for (int i = 0; i < amount; i++)
        {
            GameObject elementInstance = _scrollViewElementPool.GetObjectFromPool();

            elementInstance.transform.SetParent(_content);
            elementInstance.transform.SetAsFirstSibling();

            elementTMP = elementInstance.GetComponentInChildren<TMP_Text>();

            if (elementTMP != null)
                elementTMP.SetText(_dbElementName.GetNameFromDatabase());
        }
    }
}
