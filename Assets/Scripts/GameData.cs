using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; 
using System.IO; 
using System.Runtime.Serialization.Formatters.Binary; 

[Serializable]
public class SaveData {
    [SerializeField] private bool[] actives; 
    [SerializeField] private int[] highScores; 
    [SerializeField] private int[] stars; 
    [SerializeField] private bool summon; 

    public void setActive(int i, bool status) {
        actives[i] = status; 
    }

    public bool isActive(int i) {
        return actives[i]; 
    }

    public void setStar(int i, int star) {
        stars[i] = star; 
    }

    public int getStar(int i) {
        return stars[i];
    }

    public void setHighScore(int i, int score) {
        highScores[i] = score; 
    }

    public int getHighScore(int i) {
        return highScores[i];
    }

    public void Summoned() {
        summon = true; 
    }

    public bool hasSummoned() {
        return summon; 
    }

    public void NewSave(int lvls) {
        actives = new bool[lvls]; 
        stars = new int[lvls];
        highScores = new int[lvls]; 
        actives[0] = true; 
    }

    public int Count() {
        if (stars.Length == highScores.Length && stars.Length == actives.Length) {
            return stars.Length; 
        }
        else {
            return -1; 
        }
    }

}

public class GameData : MonoBehaviour {

    public static GameData gameData; 
    [SerializeField] internal World world; 
    public SaveData saveData; 
    // Start is called before the first frame update
    void Awake()
    {
        if (gameData == null) {
            DontDestroyOnLoad(this.gameObject); 
            gameData = this; 
        }
        else {
            Destroy(this.gameObject); 
        }

        Load(); 
    }

    private void Start() {
    }

    public void Save() {
        // create a binary formatter that can read binary files 
        BinaryFormatter formatter = new BinaryFormatter(); 

        // open file stream 
        FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Create); 

        // create a copy of save data
        SaveData data = new SaveData();
        data = saveData;  


        // write the save data to the file
        formatter.Serialize(file, data); 

        // close data stream *important*
        file.Close(); 

        Debug.Log("Saved"); 
    }

    public void Load() {
        // Check if the save game file exists 
        if (File.Exists(Application.persistentDataPath + "/player.dat")) {
            // create a binary formatter 
            BinaryFormatter formatter = new BinaryFormatter(); 
            FileStream file = File.Open(Application.persistentDataPath + "/player.dat", FileMode.Open); 

            saveData = formatter.Deserialize(file) as SaveData; 

            file.Close(); 

            // if saveData is corrupted or from an old version of the game, shoot an error
            if (saveData.Count() != world.levels.Length) {
                // For now, write old file under a new name 

                FileStream fileBkp = File.Open(Application.persistentDataPath + "/playerbkp.dat", FileMode.Create);
                SaveData bkpData = new SaveData(); 
                bkpData = saveData; 
                formatter.Serialize(fileBkp, bkpData); 
                fileBkp.Close(); 

                // Load fresh save
                Debug.Log("Save corrupted or from wrong version: fresh save created");
                ClearSave(); 
                Load(); 
            }

            Debug.Log("Save loaded from file"); 
        }
        else { 
            Debug.Log("No saves found: fresh save created"); 
            ClearSave(); 
            Load(); 
        }
    }

    private void OnDisable() {
        Save(); 
    }

    private void OnApplicationPause() {
        Save(); 
    }

    private void OnApplicationQuit() {
        Save(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearSave() {
        saveData.NewSave(world.levels.Length); 
        Save(); 
        Debug.Log("Save cleared"); 
    }
}
