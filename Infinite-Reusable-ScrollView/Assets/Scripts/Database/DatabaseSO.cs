using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptable Objects/Database", order = 1)]
public class DatabaseSO : ScriptableObject, ISerializationCallbackReceiver
{
    [Header("Messages")]
    [SerializeField] private string databaseEmptyMessage = "Database Element Empty";
    [SerializeField] private string runOutElementsMessage = "You run out the elements name!";

    [Header("Stored names")]
    [SerializeField] private List<string> _elementNames = new List<string>();
    private int _index;

    
    public string GetNameFromDatabase()
    {
        if (HasElementNamesEmpty())
            return databaseEmptyMessage;

        if (_index >= _elementNames.Count)
            return runOutElementsMessage;

        return _elementNames[_index++];
    }

    public bool HasElementNamesEmpty()
    {
        return _elementNames.Count == 0;
    }

    [ContextMenu("Generate Database")]
    private void BasicElementNamesGenerator()
    {
        int amountNamesToGenerate = 10000;
        string elementName = "Element ";

        _elementNames = new List<string>(amountNamesToGenerate);

        for (int i = 0; i < amountNamesToGenerate; i++)
        {
            _elementNames.Add(elementName + (i + 1).ToString());
        }
    }

    #region ISerializationCallbackReceiver Methods

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        _index = 0;
    }

    #endregion
}
