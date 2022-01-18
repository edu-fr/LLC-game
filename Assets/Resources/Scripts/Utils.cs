using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Resources.Scripts
{
    public static class Utils
    {
        public static readonly float ScreenDif = Display.main.systemWidth / 1280;
    }
    
    public static class ExtensionMethods
    {
        // Deep clone
        public static T DeepClone<T>(this T a)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, a);
                stream.Position = 0;
                return (T) formatter.Deserialize(stream);
            }
        }
    }
}