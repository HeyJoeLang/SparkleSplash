using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TI_ActivateGazeCaster : MonoBehaviour {

    public GazeCaster caster;
    private void OnEnable()
    {
        caster.enabled = true;
    }
    private void OnDisable()
    {
        caster.enabled = false;
    }
}
