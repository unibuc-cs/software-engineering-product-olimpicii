using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class PlayerControllerTests
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
    public void TestSpawnSoldiers()
    {
        int initialSoldierCount = playerController.soldiers.Count;

        playerController.SpawnSoldiers(5);

        Assert.AreEqual(initialSoldierCount + 5, playerController.soldiers.Count, "Soldatii nu au fost spawnati corect");
    }

    [Test]
    public void TestRemoveSoldiers()
    {
     
        playerController.SpawnSoldiers(10); 
        int initialSoldierCount = playerController.soldiers.Count;

        playerController.RemoveSoldiers(5); 

        Assert.AreEqual(initialSoldierCount - 5, playerController.soldiers.Count, "Soldatii nu au fost eliminati corect");
    }

    [TearDown]
    public void TearDown()
    {
    
        GameObject.Destroy(playerObject);
    }
}
