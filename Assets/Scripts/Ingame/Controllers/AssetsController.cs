using System;
using System.Collections.Generic;
using UnityEngine;

public static class AssetsController {
    public static Dictionary<string, AudioClip> storedAudioClip = new Dictionary<string, AudioClip>();
    public static Dictionary<string, Shader> storedShader = new Dictionary<string, Shader>();

    // Asset loading and caching
    public static T GetResource<T>(string resourceID) {
        if (typeof(T) == typeof(AudioClip)) {
            if (storedAudioClip.ContainsKey(resourceID)) return (T)Convert.ChangeType(storedAudioClip[resourceID], typeof(T));
            object snd = Convert.ChangeType(Resources.Load(resourceID, typeof(T)), typeof(T));

            AssetsController.storedAudioClip.Add(resourceID, (AudioClip)snd);
            Debug.Log("[AssetsController] <color='red'>LOADED - AudioClip</color> {<color='blue'>" + resourceID + "</color>}");

            return (T)snd;
        } else if (typeof(T) == typeof(Shader)) {
            if (storedShader.ContainsKey(resourceID)) return (T)Convert.ChangeType(storedShader[resourceID], typeof(T));
            object shdr = Convert.ChangeType(Resources.Load(resourceID, typeof(T)), typeof(T));

            AssetsController.storedShader.Add(resourceID, (Shader)shdr);
            Debug.Log("[AssetsController] <color='red'>LOADED - Shader</color> {<color='blue'>" + resourceID + "</color>}");

            return (T)shdr;
        }

        return default(T);
    }
}