#if Release
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

/*==========================
* 创建时间: 2022/5/17 20:11:23
* 作者: Bright
*==========================*/
namespace MystiaIzakayaDebugMod.扩展计划
{
    [Serializable]
    public class 菜单和食物
    {

        public int ID;

        public int Money;

        public int Level;

        public float CookTime;

        public string Name;

        public string Description;

        public List<int> Tags;

        public List<int> BanTags;

        public List<int> Ingredients;

        public GameData.Core.Collections.Cooker.CookerType CookerType;
        [XmlIgnore]
        private UnityEngine.Sprite foodsprite;

        public string foodspritepath;
        [XmlIgnore]
        public UnityEngine.Sprite FoodSprite
        {
            get
            {
                if (!foodsprite)
                {

                    foodsprite = 扩展计划Manager.GetArtWork(foodspritepath);

                }
                return foodsprite;
            }
            set
            {
                if (!value)
                {
                    foodsprite = value;
                }
                else
                {
                    foodsprite = GameData.CoreLanguage.ObjectLanguageBase.defaultNull;
                }
            }
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
#endif