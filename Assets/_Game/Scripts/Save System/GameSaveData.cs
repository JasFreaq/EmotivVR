namespace ChatGPT_Detective
{
    [System.Serializable]
    public class GameSaveData
    {
        public string mPlayerProfile;

        public GameSaveData(string playerProfile)
        {
            mPlayerProfile = playerProfile;
        }
    }
}