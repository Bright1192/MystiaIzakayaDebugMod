using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnhollowerRuntimeLib;
using UnityEngine;

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
        public static void Init()
        {
            Setup();
        }
        internal static DebugMono Instance { get; private set; }

        internal static void Setup()
        {
            if(Instance)
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
