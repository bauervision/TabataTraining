using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class DataSaver : MonoBehaviour
{
    static string destination = Application.persistentDataPath + "/SaveData.dat";

    ///<summary>Check to see if we have a saved data file on the device </summary>
    public static bool CheckFirstTimeData() { return File.Exists(destination); }

    ///<summary>If we have already saved our data, update it, otherwise create a new file and write to it. </summary>
    public static void Save_Data()
    {
        FileStream saveFile;
        // if we already have a save file, update it, otherwise create a new one
        if (File.Exists(destination)) saveFile = File.OpenWrite(destination);
        else saveFile = File.Create(destination);

        // write the file
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        binaryFormatter.Serialize(saveFile, DataManager.instance.data);
        saveFile.Close();
    }

    public static MyWorkoutData Load_Data()
    {
        FileStream loadFile;
        // if we found our save file, read from it it, otherwise throw an error
        if (File.Exists(destination)) loadFile = File.OpenRead(destination);
        else
        {
            Debug.LogError("Error: No file found to load from!");
            return null;
        }

        // write the file
        BinaryFormatter binaryFormatter = new BinaryFormatter();
        MyWorkoutData loadedData = (MyWorkoutData)binaryFormatter.Deserialize(loadFile);
        print("Loaded File from previous workout");
        loadFile.Close();
        return loadedData;
    }
}
