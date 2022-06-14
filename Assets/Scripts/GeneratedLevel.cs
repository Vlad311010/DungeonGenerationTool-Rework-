using UnityEngine;

public class GeneratedLevel : MonoBehaviour
{
    public void DeleteLevel()
    {
        if (Application.isPlaying)
        {
            foreach (Transform child in this.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(this.gameObject);
        }
        else
        {
            foreach (Transform child in this.transform)
            {
                DestroyImmediate(child.gameObject);
            }
            DestroyImmediate(this.gameObject);
        } 
    }
}
