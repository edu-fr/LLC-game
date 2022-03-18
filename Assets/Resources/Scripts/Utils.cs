using System;
using System.Collections;
using System.Collections.Generic;
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

        public static int SortProductions(GrammarScript.Production p1, GrammarScript.Production p2)
        {
            if (p1._in != p2._in)
            {
                if (p1._in == 'S') 
                    return -1;
                if (p2._in == 'S')
                    return 1;
                return p1._in.CompareTo(p2._in);
            }

            foreach (var letterP1 in p1._out)
            {
                foreach (var letterP2 in p2._out)
                {
                    if (!Char.IsUpper(letterP1)) break;
                    if (!Char.IsUpper(letterP2)) continue;
                    
                    if (letterP1 != letterP2)
                    {
                        if (letterP1 == 'S') return -1;
                        if (letterP2 == 'S') return 1;
                    }
                }
            } 
            return String.Compare(p1._out, p2._out, StringComparison.Ordinal);
        }
        
        public static int SortVariables(char p1, char p2)
        {
            if (p1 == 'S') 
                return -1;
            return p2 == 'S' ? 1 : p1.CompareTo(p2);
        }
    }
    
    
    
    
}