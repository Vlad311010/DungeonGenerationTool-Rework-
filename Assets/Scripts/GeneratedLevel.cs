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

        /*
        if (generatedLevel == null)
            return;

        foreach (Transform child in generatedLevel.transform)
        {
            Destroy(child.gameObject);
        }
        Destroy(generatedLevel);*/
    }

    public void CleanUp()
    {
        if (Application.isPlaying)
        {
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("ToDestroy");
            for (int i = 0; i < objectsToDestroy.Length; i++)
                Destroy(objectsToDestroy[i]);
        }
        else
        {
            GameObject[] objectsToDestroy = GameObject.FindGameObjectsWithTag("ToDestroy");
            for (int i = 0; i < objectsToDestroy.Length; i++)
                DestroyImmediate(objectsToDestroy[i]);
        }
    }
}
