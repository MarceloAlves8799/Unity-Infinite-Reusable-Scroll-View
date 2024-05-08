using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InfiniteReusableScrollView))]
public class HandleScrollViewElement : MonoBehaviour
{
    [field: SerializeField] public int VisibleElementCount { get; private set; }
    

    public void HandleElementCount(int value)
    {
        VisibleElementCount += value;

        VisibleElementCount = Mathf.Clamp(0, VisibleElementCount, VisibleElementCount);
    }
}
