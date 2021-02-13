using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class Save
{
    public StoryManager.Stage storyStage;
    public string areaName;

    public List<KeyValuePair<string, int>> playerInventoryContents;
    public List<KeyValuePair<string, int>> miningStationInventoryContents;
    
    public int o2GenContents;
    public int hullRepairerContents;
    public int jumpDriveContents;
    public KeyValuePair<string, int> craftingSystemContents;

    public string playerHull;
    public string[] playerWeapons;

    public float playerHealth;
    public float playerMaxHealth;
    public float playerMaxShields;
    public float playerO2Levels;
    public float jumpDriveFillLevel;

    public List<Blueprint> unlockedBlueprints;

    private const string extension = ".save";
    private static readonly string path = Application.persistentDataPath + "/Saves/";

    private Save() { }

    /// <summary>
    /// Saves the current game state to a time and date stamped .save file in persistent data path
    /// </summary>
    public static void SaveGame() {
        GameObject playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
        GameObject miningStation = GameObject.FindGameObjectWithTag("MiningStation");
        Save save = new Save
        {
            storyStage = StoryManager.StoryStage,
            areaName = GameManager.CurrentArea.systemName,
            playerInventoryContents = new List<KeyValuePair<string, int>>(),
            miningStationInventoryContents = new List<KeyValuePair<string, int>>(),
            playerHull = playerShip.GetComponent<HullSpawner>().hull,
            playerHealth = playerShip.GetComponent<HealthAndShieldsResource>().HealthValue,
            playerMaxShields = playerShip.GetComponent<HealthAndShieldsResource>().MaxShieldValue,
            playerMaxHealth = playerShip.GetComponent<HealthAndShieldsResource>().MaxHealthValue,
            playerO2Levels = playerShip.GetComponent<OxygenResource>().Value,
            jumpDriveFillLevel = miningStation.GetComponent<JumpResource>().Value,
            unlockedBlueprints = CraftingSystem.Instance.UnlockedBlueprints
        };

        ItemFrame o2Frame = MiningStationController.Instance.m_O2Gen.slot.StoredItemFrame;
        save.o2GenContents = o2Frame == null ? 0 : o2Frame.m_InventoryItem.quantity;

        ItemFrame ironFrame = MiningStationController.Instance.m_HullRepairer.slot.StoredItemFrame;
        save.hullRepairerContents = ironFrame == null ? 0 : ironFrame.m_InventoryItem.quantity;

        ItemFrame jumpFrame = MiningStationController.Instance.m_JumpDriveFueler.slot.StoredItemFrame;
        save.jumpDriveContents = jumpFrame == null ? 0 : jumpFrame.m_InventoryItem.quantity;

        InventoryItem craftingSystemItem = CraftingSystem.Instance.outputSlot.StoredItemFrame?.m_InventoryItem;
        save.craftingSystemContents = craftingSystemItem == null ?
            new KeyValuePair<string, int>() :
            new KeyValuePair<string, int>(craftingSystemItem.systemName, craftingSystemItem.quantity);

        List<string> playerWeaponNames = new List<string>();
        foreach (Transform hullChild in playerShip.transform.GetChild(0)) {
            if (hullChild.CompareTag("WeaponAttachment") && hullChild.childCount > 0) {
                playerWeaponNames.Add(
                    hullChild.GetChild(0).gameObject.GetComponent<Weapon>().inventoryItem);
            }
        }
        if (playerWeaponNames.Count > 0)
            save.playerWeapons = playerWeaponNames.ToArray();

        foreach (InventoryItem item in GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>().Contents)
        {
            if (item != null)
            {
                save.playerInventoryContents.Add(new KeyValuePair<string, int>(item.systemName, item.quantity));
            }
            else
            {
                save.playerInventoryContents.Add(new KeyValuePair<string, int>());
            }
        }

        foreach (InventoryItem item in GameObject.FindGameObjectWithTag("MiningStation").GetComponent<Inventory>().Contents)
        {
            if (item != null)
            {
                save.miningStationInventoryContents.Add(new KeyValuePair<string, int>(item.systemName, item.quantity));
            }
            else
            {
                save.miningStationInventoryContents.Add(new KeyValuePair<string, int>());
            }
        }

        BinaryFormatter bf = new BinaryFormatter();

        string dateAndTime = System.DateTime.Now.ToString();
        Regex pattern = new Regex("[://]");
        dateAndTime = pattern.Replace(dateAndTime, "_");

        FileStream file = File.Create(path + "save " + dateAndTime + extension);
        bf.Serialize(file, save);
        file.Close();

    }

    /// <summary>
    /// Returns the game state from a .save file wrapped in a Save object
    /// </summary>
    /// <param name="saveName">The extensionless string name of the save file to search for</param>
    /// <returns>Save object, wrapping game state</returns>
    public static Save LoadGame(string saveName) {
        if (File.Exists(path + saveName + extension))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path + saveName + extension, FileMode.Open);
            Save save = (Save)bf.Deserialize(file);
            file.Close();
            return save;
        }
        return null;
    }

    public static string[] GetAllSaveNames() {
        Debug.Log("Saves are stored in: " + path);
        Debug.Log("Please check this directory to remove any unwanted data after playing for now!");
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            return new string[0];
        }

        DirectoryInfo info = new DirectoryInfo(path);
        FileInfo[] files = info.GetFiles().OrderByDescending(p => p.CreationTime).ToArray();
        List<string> result = new List<string>();

        foreach (FileInfo file in files) {
            if (file.Extension == extension)
            {
                result.Add(Path.GetFileNameWithoutExtension(file.Name));
            }
        }

        return result.ToArray();
    }
}
