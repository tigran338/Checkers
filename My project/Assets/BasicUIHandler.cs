using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicUIHandler : MonoBehaviour
{
    // Start is called before the first frame update
    public delegate void PrimitiveChange(int index);
    public static event PrimitiveChange OnPrimitiveChange;

    public delegate void MaterialChange(int index);
    public static event MaterialChange OnMaterialChange;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonClick(int index)
    {
        Debug.Log($"You click Debug having index" + index);
        OnPrimitiveChange?.Invoke(index);
    }

    public void ButtonMaterialClick(int index)
    {
        Debug.Log($"You click Debug having index" + index);
        OnMaterialChange?.Invoke(index);
    }
}
