namespace SaveData
{
    [System.Serializable]
    public class PlayerProfile
    {
        public int playerRecord;
        public int playerCurrentPoints;

        public int[] field3 = new int[3];
        public int[] field4 = new int[4];
        public int[] field5 = new int[5];
        
        public bool GameStarted;
        public string resultText;
        public bool FieldIsCreated;
    }
}