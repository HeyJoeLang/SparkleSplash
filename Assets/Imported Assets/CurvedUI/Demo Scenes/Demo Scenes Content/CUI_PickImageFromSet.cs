using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CurvedUI
{
    public class CUI_PickImageFromSet : MonoBehaviour
    {

        static CUI_PickImageFromSet picked = null;



        public virtual void PickThis()
        {
            /*
            if (picked != null)
                picked.GetComponent<Button>().targetGraphic.color = new Color(.5f,.5f,.5f,.9f);

            Debug.Log("Clicked this!", this.gameObject);


            picked = this;
            picked.GetComponent<Button>().targetGraphic.color = new Color(1, 1, 1, .9f);
            */
        }
    }
}


