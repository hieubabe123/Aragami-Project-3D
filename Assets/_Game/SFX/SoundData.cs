using System.IO;
using UnityEngine;



    [CreateAssetMenu(fileName = "SoundData", menuName = "_GAME/SFX/SoundData")]
    public partial class SoundData : SingletonResourcesScriptable<SoundData> {
        public SoundInfo[] data;
        protected override void Initialize() {

        }

        public AudioClip GetSound(int eventId) { 
            return GetSound(data, eventId);
        }

        public AudioClip GetSound(SoundInfo[] arrayData, int eventId) {
            if (arrayData == null || arrayData.Length == 0) return null;
            var sound = System.Array.Find(arrayData, i => i.eventId == eventId);
            if (sound == null) return null;
            return sound.clip;
        }


#if UNITY_EDITOR
        [UnityEditor.MenuItem("GAME/OpenSoundData")]
        public static void OpenSoundDataFile() {
            if (SoundData.Instance == null) {
                string path = $"Assets/_GAME/SFX/Resources/{typeof(SoundData).Name}/";
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<SoundData>();
                string assetPath = Path.Combine(path, $"{typeof(SoundData).Name}.asset");
                UnityEditor.AssetDatabase.CreateAsset(ins, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            UnityEditor.Selection.activeObject = SoundData.Instance;
        }

#endif
    }

    [System.Serializable]
    public class SoundInfo {
        public int eventId;
        public AudioClip clip;
    }

    public partial class SFX {
        /// <summary> eventId in the class SoundEvent.cs </summary>
        public void PlaySound(int eventId, float volume = 1, bool loop = false, bool overrideCurrent = false) {
            SFX.Instance.PlaySound(SoundData.Instance.GetSound(eventId), volume, loop, overrideCurrent);
        }

        /// <summary> eventId in the class SoundEvent.cs </summary>
        public void PlayBgMusic(int eventId, float volume) {
            SFX.Instance.PlayBgMusic(SoundData.Instance.GetSound(eventId), volume);
        }
    }

