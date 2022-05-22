#if Release
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

/*==========================
* 创建时间: 2022/5/18 0:11:12
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.扩展计划
{
    [Serializable]
    public class RecipeMod
    {
        
        public string modid;

        public List<菜单和食物> 菜单和食物list;
        public override string ToString()
        {
            return modid;
        }

    }
}
#endif