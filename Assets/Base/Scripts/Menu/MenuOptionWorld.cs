using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptionWorld : MonoBehaviour
{
    static MenuOptionWorld picked = null;

    Color startColor;
    public bool initalChoice;
    private void Start()
    {
        startColor = GetComponent<Button>().targetGraphic.color;
        if (initalChoice)
            PickThis();
    }
    public void PickThis()
    {

        if (picked != null)
            picked.GetComponent<Button>().targetGraphic.color = startColor;

        picked = this;
        picked.GetComponent<Button>().targetGraphic.color = Color.white;
    }
    
}
