using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Resources.Scripts
{
    public static class SaveSystem
    {
        public static void SaveProgress(PlayerData playerData)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            string path = Application.persistentDataPath + "/player.sf";
            FileStream stream = new FileStream(path, FileMode.Create);

            formatter.Serialize(stream, playerData);
            stream.Close();
        }

        public static PlayerData LoadPlayer()
        {
            string path = Application.persistentDataPath + "/player.sf";
            if (File.Exists(path))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream fileStream = new FileStream(path, FileMode.Open);
                PlayerData data = formatter.Deserialize(fileStream) as PlayerData;
                fileStream.Close();

                return data;
            }
            else
            {
                Debug.Log("Save file not found in " + path);
                return null;
            }
        }
    }
}
