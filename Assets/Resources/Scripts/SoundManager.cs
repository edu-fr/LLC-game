using System;
using UnityEditor.Audio;
using UnityEngine;

namespace Resources.Scripts
{
   public class SoundManager : MonoBehaviour
   {
      public static SoundManager instance;

      public Sound[] Sounds;

      private void Awake()
      {
         if (instance == null)
         {
            instance = this;
         }
         else
         {
            Destroy(gameObject);
            return;
         }
         
         foreach (var sound in Sounds)
         {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.volume = sound.volume;
            sound.source.loop = sound.loop;
         }
      }

      public void Play(string name)
      {
         var sound = Array.Find(Sounds, sound => sound.name == name);
         if (sound == null)
         {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
         }
         sound.source.Play();
      }
   }
}
