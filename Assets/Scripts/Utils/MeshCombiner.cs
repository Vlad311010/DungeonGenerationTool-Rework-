using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshCombiner
{
    public static void CombineMeshes(GameObject obj)
    {
        MeshFilter objMeshFilter;
        MeshRenderer objMeshRenderer;
        if (!obj.TryGetComponent(out objMeshFilter))
            obj.AddComponent<MeshFilter>();
        if (!obj.TryGetComponent(out objMeshRenderer))
            obj.AddComponent<MeshRenderer>();

        Vector3 originalPos = obj.transform.position;
        obj.transform.position = Vector3.zero;

        MeshFilter[] meshFilters = obj.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length - 1];
        int i = 1;
        while (i < meshFilters.Length)
        {

            combine[i - 1].mesh = meshFilters[i].sharedMesh;
            combine[i - 1].transform = meshFilters[i].transform.localToWorldMatrix;
            //meshFilters[i].gameObject.SetActive(false); // change to Destroy
            meshFilters[i].gameObject.tag = "ToDestroy";
            //Destroy(meshFilters[i].gameObject);
            i++;
        }
        obj.transform.GetComponent<MeshFilter>().mesh = new Mesh();
        obj.transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true, true);
        obj.transform.gameObject.SetActive(true);
        obj.transform.GetComponent<MeshRenderer>().material = meshFilters[1].gameObject.GetComponent<MeshRenderer>().material;

        obj.transform.position = originalPos;
    }
}
