using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using XRPlayer;

public class CashRegisterControllerTests
{
    [Test]
    public void CashRegisterController_CalculateAndDisplayTotal_UpdatesDisplayAndSpeech()
    {
        var registerRoot = new GameObject("CashRegister");
        var register = registerRoot.AddComponent<CashRegisterController>();

        var registerTextObject = new GameObject("RegisterText");
        var registerText = registerTextObject.AddComponent<TextMeshPro>();
        register.registerScreenText = registerText;

        var speechParent = new GameObject("SpeechBalloon");
        var speechTextObject = new GameObject("SpeechText");
        speechTextObject.transform.SetParent(speechParent.transform);
        var speechText = speechTextObject.AddComponent<TextMeshPro>();
        register.cashierSpeechText = speechText;
        register.speechBalloonObject = speechParent;

        var apple = new GameObject("Apple");
        var appleItem = apple.AddComponent<StoreItem>();
        appleItem.itemName = "Apple";
        appleItem.price = 3f;
        appleItem.isOnCounter = true;

        var bread = new GameObject("Bread");
        var breadItem = bread.AddComponent<StoreItem>();
        breadItem.itemName = "Bread";
        breadItem.price = 12.5f;
        breadItem.isOnCounter = true;

        var listField = typeof(CashRegisterController).GetField("storeItems", BindingFlags.Instance | BindingFlags.NonPublic);
        listField.SetValue(register, new List<StoreItem> { appleItem, breadItem });

        register.CalculateAndDisplayTotal();

        Assert.That(registerText.text, Is.EqualTo("$15.50"));
        Assert.That(speechText.text, Does.Contain("Apple"));
        Assert.That(speechText.text, Does.Contain("Bread"));
        Assert.That(speechText.text, Does.Contain("$15.50"));

        register.ShowSpeechBalloon();
        Assert.That(speechParent.activeSelf, Is.True);

        register.HideSpeechBalloon();
        Assert.That(speechParent.activeSelf, Is.False);

        Object.DestroyImmediate(registerTextObject);
        Object.DestroyImmediate(speechParent);
        Object.DestroyImmediate(registerRoot);
        Object.DestroyImmediate(apple);
        Object.DestroyImmediate(bread);
    }
}
