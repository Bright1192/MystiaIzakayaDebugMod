using BepInEx.IL2CPP.Utils.Collections;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;
using UnityEngine.Networking;

/*==========================
* 创建时间: 2022/5/27 6:34:37
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.IL2CPPUnityCall
{
    public class DebugMono : MonoBehaviour
    {
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F8))
            {
                Provider.DayScene.LoadScene();
            }
            if (Input.GetKeyDown(KeyCode.F9))
            {
                Provider.DoSave();
            }
            if (Input.GetKeyDown(KeyCode.F10))
            {
                Provider.MainScene.LoadScene();
            }
        }
#if false
        //无法调用UnityWebRequestMultimedia..ccor
        public Dictionary<string, AudioClip> AudioClips = new Dictionary<string, AudioClip>();
        public void AddAudio(string path)
        {
            StartCoroutine(WrapToIl2Cpp( AddAudioCoroutine(path)) );
        }
        public static Il2CppSystem.Collections.IEnumerator WrapToIl2Cpp(System.Collections.IEnumerator self)
        {
            return new Il2CppSystem.Collections.IEnumerator(new Il2CppManagedEnumerator(self).Pointer);
        }
        private IEnumerator AddAudioCoroutine(string path)
        {
            if (File.Exists(path))
            {
                string Name = Path.GetFileName(path);
                AudioType audioType;
                if (path.EndsWith(".wav"))
                {
                    audioType = AudioType.WAV;
                }
                else if (path.EndsWith(".ogg"))
                {
                    audioType = AudioType.OGGVORBIS;
                }
                else
                {
                    audioType = AudioType.UNKNOWN;
                }
                UnityWebRequest audioClip = UnityWebRequestMultimedia.GetAudioClip("file://" + path, audioType);
                yield return audioClip.SendWebRequest();
                if (!audioClip.isDone)
                {
                    UnityEngine.Debug.LogError("未成功加载AudioClip,原因:" + audioClip.error);
                }
                else
                {
                    AudioClip content = DownloadHandlerAudioClip.GetContent(audioClip);
                    content.name = Name;
                    AudioClips[Name] = content;
                }
            }
            yield break;
        }
#endif
        public static void Init()
        {
            Setup();
        }
        internal static DebugMono Instance { get; private set; }

        internal static void Setup()
        {
            if (Instance)
            {
                UnityEngine.Object.Destroy(Instance.gameObject);
            }
            ClassInjector.RegisterTypeInIl2Cpp<DebugMono>();
            GameObject gameObject = new GameObject("[Debug]MystiaIzakayaDebugMod");
            UnityEngine.Object.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags |= HideFlags.HideAndDontSave;
            Instance = gameObject.AddComponent<DebugMono>();
        }
        public DebugMono(IntPtr ptr)
        : base(ptr)
        {
        }
    }

}
