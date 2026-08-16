using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using XRPlayer;

public class StoreItemTests
{
    [Test]
    public void StoreItem_Awake_AddsInteractable_AndOnItemClicked_TogglesState()
    {
        var root = new GameObject("StoreItem");
        var counter = new GameObject("Counter");
        counter.transform.position = new Vector3(10f, 0f, 0f);

        var item = root.AddComponent<StoreItem>();
        var counterField = typeof(StoreItem).GetField("counterPosition", BindingFlags.Instance | BindingFlags.NonPublic);
        counterField.SetValue(item, counter.transform);


        item.OnItemClicked();

        var targetField = typeof(StoreItem).GetField("targetPosition", BindingFlags.Instance | BindingFlags.NonPublic);
        var movingField = typeof(StoreItem).GetField("isMoving", BindingFlags.Instance | BindingFlags.NonPublic);

        Assert.That(item.isOnCounter, Is.True);
        Assert.That((bool)movingField.GetValue(item), Is.True);
        Assert.That((Vector3)targetField.GetValue(item), Is.EqualTo(counter.transform.position));

        Object.DestroyImmediate(counter);
        Object.DestroyImmediate(root);
    }

}
