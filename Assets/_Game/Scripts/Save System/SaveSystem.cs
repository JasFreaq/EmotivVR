using System.IO;
using UnityEngine;

namespace ChatGPT_Detective
{
    public static class SaveSystem
    {
        private static string m_savePath = Path.Combine(Application.persistentDataPath, "laserKnightsSaveGame.sav");
        
        public static void SavePlayerData(GameSaveData gameSaveData)
        {
            string json = JsonUtility.ToJson(gameSaveData);

            File.WriteAllText(m_savePath, json);
        }

        public static GameSaveData LoadGameData()
        {
            if (DoesSaveGameExist())
            {
                string json = File.ReadAllText(m_savePath);

                GameSaveData gameSaveData = JsonUtility.FromJson<GameSaveData>(json);

                return gameSaveData;
            }
            else
            {
                Debug.Log("No Save Data Found");
                return null;
            }
        }

        public static bool DoesSaveGameExist()
        {
            return File.Exists(m_savePath);
        }
    }
}