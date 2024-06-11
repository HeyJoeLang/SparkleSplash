using UnityEngine;

public class BlockController : MonoBehaviour {
    public Renderer[] animals;
    public void ResetAnimals()
    {
        foreach (Renderer rend in animals)
        {
            rend.gameObject.tag = "Animal";
            rend.material.SetFloat("_Threshold", 1.1f);
        }
    }
}
