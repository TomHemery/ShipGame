using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using UnityEngine;

[System.Serializable]
public class Save
{
    public StoryManager.Stage storyStage;
    public string areaName;

    public Dictionary<string, int> playerInventoryContents;
    public Dictionary<string, int> miningStationInventoryContents;
    
    public int o2GenContents;
    public int hullRepairerContents;
    public int jumpDriveContents;

    public string playerHull;
    public string[] playerWeapons;

    public float playerHealth;
    public float playerMaxHealth;
    public float playerMaxShields;
    public float playerO2Levels;
    public float jumpDriveFillLevel;

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
            playerInventoryContents = new Dictionary<string, int>(),
            miningStationInventoryContents = new Dictionary<string, int>(),
            playerHull = playerShip.GetComponent<HullSpawner>().hull,
            playerHealth = playerShip.GetComponent<HealthAndShieldsResource>().HealthValue,
            playerMaxShields = playerShip.GetComponent<HealthAndShieldsResource>().MaxShieldValue,
            playerMaxHealth = playerShip.GetComponent<HealthAndShieldsResource>().MaxHealthValue,
            playerO2Levels = playerShip.GetComponent<OxygenResource>().Value,
            jumpDriveFillLevel = miningStation.GetComponent<JumpResource>().Value
        };

        ItemFrame o2Frame = MiningStationController.Instance.m_O2Gen.slot.StoredItemFrame;
        save.o2GenContents = o2Frame == null ? 0 : o2Frame.m_InventoryItem.quantity;

        ItemFrame ironFrame = MiningStationController.Instance.m_HullRepairer.slot.StoredItemFrame;
        save.hullRepairerContents = ironFrame == null ? 0 : ironFrame.m_InventoryItem.quantity;

        ItemFrame jumpFrame = MiningStationController.Instance.m_JumpDriveFueler.slot.StoredItemFrame;
        save.jumpDriveContents = jumpFrame == null ? 0 : jumpFrame.m_InventoryItem.quantity;

        List<string> playerWeaponNames = new List<string>();
        foreach (Transform hullChild in playerShip.transform.GetChild(0)) {
            if (hullChild.CompareTag("WeaponAttachment") && hullChild.childCount > 0) {
                playerWeaponNames.Add(
                    hullChild.GetChild(0).gameObject.GetComponent<Weapon>().m_inventoryItem.systemName);
            }
        }
        if (playerWeaponNames.Count > 0)
            save.playerWeapons = playerWeaponNames.ToArray();

        foreach (KeyValuePair<string, InventoryItem> pair in 
            GameObject.FindGameObjectWithTag("PlayerShip").GetComponent<Inventory>().Contents)
        {
            save.playerInventoryContents.Add(pair.Key, pair.Value.quantity);
        }

        foreach (KeyValuePair<string, InventoryItem> pair in
            GameObject.FindGameObjectWithTag("MiningStation").GetComponent<Inventory>().Contents)
        {
            save.miningStationInventoryContents.Add(pair.Key, pair.Value.quantity);
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
        if (!Directory.Exists(path)) {
            Directory.CreateDirectory(path);
        }

        string[] files = Directory.GetFiles(path);
        List<string> result = new List<string>();

        foreach (string file in files) {
            if (Path.GetExtension(file) == extension)
            {
                result.Add(Path.GetFileNameWithoutExtension(file));
            }
        }

        string[] saveNames = result.ToArray();
        System.Array.Sort(saveNames);
        System.Array.Reverse(saveNames);

        return saveNames;
    }
}
