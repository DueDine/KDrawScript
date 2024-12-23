using System;
using System.Threading;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Dalamud.Utility.Numerics;
using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Module.Draw;
using ECommons.ExcelServices.TerritoryEnumeration;
using System.Reflection.Metadata;
using System.Windows.Forms;

namespace KDrawScript.Dev
{

    [ScriptType(name: "Yuweyawata Field Station", territorys: [1242], guid: "ad5876c4-31b1-43f6-9e5c-d8ab47c66027", version: "0.0.0.1", author: "Due")]
    public class Yuweyawata
    {

        [UserSetting(note: "Enable TTS")]
        public bool EnableTTS { get; set; } = false;

        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        // Boss 1: Lindblum Zaghnal
        [ScriptMethod(name: "Electrical Overload", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40635"])]
        public void ElectricalOverload(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Lightning Storm", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40637"])]
        public void LightningStorm(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("分散", duration: 2000, true);
        }

        [ScriptMethod(name: "Gore", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40630"])]
        public void Gore(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("间断 AOE & 转火小怪", duration: 2000, true);
        }

        [ScriptMethod(name: "Caber Toss", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40624"])]
        public void CaberToss(Event @event, ScriptAccessory accessory)
        {
            // Thread.Sleep(14000);
            accessory.Method.TextInfo("远离落点！", duration: 2000, true);
        }


        // Boss 2: Overseer Kanilokka
        [ScriptMethod(name: "Dark Souls", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00DA"])]
        public void DarkSouls(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("T死刑", duration: 2000, true);
        }

        [ScriptMethod(name: "Free Spirits", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40640"])]
        public void FreeSpirits(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE & 缩小场地", duration: 2000, true);
        }

        [ScriptMethod(name: "Phantom Flood", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40644"])]
        public void PhantomFlood(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("月环", duration: 2000, true);
        }

        [ScriptMethod(name: "Lost Hope", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40645"])]
        public void LostHope(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("准备目押", duration: 2000, true);
        }

        [ScriptMethod(name: "Bloodbrust", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40647"])]
        public void Bloodbrust(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Soul Douse", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40651"])]
        public void SoulDouse(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("分摊", duration: 2000, true);
        }


        // Boss 3: Lunipyati
        [ScriptMethod(name: "Raging Claw", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40613"])]
        public void RagingClaw(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("Boss 身后躲避", duration: 2000, true);
        }

        [ScriptMethod(name: "Leporine Loaf", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40603"])]
        public void LeporineLoaf(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE & 缩小场地", duration: 2000, true);
        }

        [ScriptMethod(name: "Crater Carve", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40605"])]
        public void CraterCarve(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("大钢铁", duration: 2000, true);
        }

        [ScriptMethod(name: "Sonic Howl", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40618"])]
        public void SonicHowl(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }
    }
}
