using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Database", menuName = "Scriptable Objects/Database", order = 1)]
public class DatabaseSO : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField] private List<string> elementNames;
    [SerializeField] private int index;

    public string GetNameFromDatabase()
    {
        if (HasElementNamesEmpty())
            return "Database Element Empty";

        if (index >= elementNames.Count)
            return "Over pass 10.000 element name!";

        return elementNames[index++];
    }

    public bool HasElementNamesEmpty()
    {
        return elementNames.Count == 0;
    }

    [ContextMenu("Generate Database")]
    private void GenerateElementNames()
    {
        int amountNamesToGenerate = 10000;
        string elementName = "Element ";

        elementNames = new List<string>(amountNamesToGenerate);

        for (int i = 0; i < amountNamesToGenerate; i++)
        {
            elementNames[i] = elementName + (i + 1).ToString();
        }
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        index = 0;
    }
}
