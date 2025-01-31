using NUnit.Framework;
using UnityEngine;

public class PlayerBehaviourTests
{
    private GameObject playerObject;
    private PlayerController playerController;

    [SetUp]
    public void SetUp()
    {
     
        playerObject = new GameObject();
        playerController = playerObject.AddComponent<PlayerController>();
        playerController.soldierPrefab = new GameObject();
        playerController.bigSoldierPrefab = new GameObject();
    }

    [Test]
    public void TestSplitBigEnemy()
    {
        playerController.SplitBigEnemy();

       
        int soldierCount = playerController.soldiers.Count;
        Assert.AreEqual(0, soldierCount, "SplitBigEnemy nu a spawnat soldatii corect");
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(playerObject);
    }
}

