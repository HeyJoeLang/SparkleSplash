using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOptionCharacter : MonoBehaviour
{
    static MenuOptionCharacter picked = null;

    Color startColor;
    public bool initalChoice;
    private void Start()
    {
        startColor = GetComponent<Image>().color;
        if (initalChoice)
            PickThis();
    }

    public void PickThis()
    {
        if (picked != null)
            picked.GetComponent<Button>().targetGraphic.color = startColor;

    //    Debug.Log("Clicked this!", this.gameObject);

        picked = this;
        picked.GetComponent<Button>().targetGraphic.color = Color.white;
    }
    
}
