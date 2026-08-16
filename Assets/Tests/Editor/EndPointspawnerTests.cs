using NUnit.Framework;
using UnityEngine;

public class EndPointspawnerTests
{
    [Test]
    public void SpawnEndpoints_CreatesExpectedNumberOfPositions()
    {
        var root = new GameObject("EndpointSpawner");
        var spawner = root.AddComponent<EndPointspawner>();

        spawner.SpawnEndpoints(3, 2f);

        Assert.That(root.transform.childCount, Is.EqualTo(3));
        Assert.That(root.transform.GetChild(0).name, Is.EqualTo("Position 1"));
        Assert.That(root.transform.GetChild(1).name, Is.EqualTo("Position 2"));
        Assert.That(root.transform.GetChild(2).name, Is.EqualTo("Position 3"));

        Assert.That(root.transform.GetChild(0).localPosition.x, Is.EqualTo(0f).Within(0.0001f));
        Assert.That(root.transform.GetChild(1).localPosition.x, Is.EqualTo(2f).Within(0.0001f));
        Assert.That(root.transform.GetChild(2).localPosition.x, Is.EqualTo(4f).Within(0.0001f));

        Object.DestroyImmediate(root);
    }

    [Test]
    public void SpawnEndpoints_WithNegativeCount_DoesNotCreateAnyPositions()
    {
        var root = new GameObject("EndpointSpawner");
        var spawner = root.AddComponent<EndPointspawner>();

        spawner.SpawnEndpoints(-5, 1.5f);

        Assert.That(root.transform.childCount, Is.EqualTo(0));

        Object.DestroyImmediate(root);
    }
}
