using UnityEngine;

public class EndPointspawner : MonoBehaviour
{
    [SerializeField] private int pointCount = 5;
    [SerializeField] private float spacing = 1f;

    [ContextMenu("Spawn Endpoints")]
    public void SpawnEndpoints()
    {
        SpawnEndpoints(pointCount, spacing);
    }

    public void SpawnEndpoints(int count, float spaceBetweenPoints)
    {
        if (count < 0)
        {
            count = 0;
        }

        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
            {
                Destroy(child.gameObject);
            }
            else
            {
                DestroyImmediate(child.gameObject);
            }
        }

        for (int i = 0; i < count; i++)
        {
            GameObject point = new GameObject($"Position {i + 1}");
            point.transform.SetParent(transform, false);
            point.transform.localPosition = new Vector3(i * spaceBetweenPoints, 0f, 0f);
        }
    }
}
