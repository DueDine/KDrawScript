using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using System;
using KodakkuAssist.Module.Draw;
using Newtonsoft.Json;
using FFXIVClientStructs.FFXIV.Common.Math;

namespace KDrawScript.Dev
{

    [ScriptType(name: "Yuweyawata Field Station 废弃据点玉韦亚瓦塔实验站 (Deprecated)", territorys: [1242], guid: "6b354054-8066-4717-85b0-5ee7d44273a5", version: "0.0.0.3", author: "Due")]
    public class Yuweyawata
    {

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
            accessory.Method.TextInfo("AOE & 击杀小怪", duration: 2000, true);
        }

        [ScriptMethod(name: "Sparking Fissure", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41258"])]
        public void SparkingFissure(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Caber Toss", eventType: EventTypeEnum.EnvControl, eventCondition: ["Id:00020001"])]
        public void CaberToss(Event @event, ScriptAccessory accessory)
        {
            if (@event["Index"] == "0000000D")
            {
                accessory.Method.TextInfo("西南角", duration: 2000, true);
            }
            if (@event["Index"] == "0000000E")
            {
                accessory.Method.TextInfo("西北角", duration: 2000, true);
            }
        }


        // Boss 2: Overseer Kanilokka
        [ScriptMethod(name: "Dark Souls", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00DA"])]
        public void DarkSouls(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("T 死刑", duration: 1500, true);
        }

        [ScriptMethod(name: "Free Spirits", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40640"])]
        public void FreeSpirits(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
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
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "RagingClaw";
            dp.Scale = new(45);
            dp.Radian = float.Pi;
            dp.FixRotation = true;
            dp.Rotation = rot;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 4700;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
            accessory.Method.TextInfo("Boss 身后躲避", duration: 2000, true);
        }

        [ScriptMethod(name: "Leporine Loaf", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40603"])]
        public void LeporineLoaf(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Boulder Dance", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4060[78])$"])]
        public void BoulderDance(Event @event, ScriptAccessory accessory)
        {
            // From 40607 to 40608
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var pos = JsonConvert.DeserializeObject<Vector3>(@event["EffectPosition"]);
            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"BoulderDance-{sid}";
            dp.Scale = new(7);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 6000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Jagged Edge", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40615"])]
        public void JaggedEdge(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("分散", duration: 2000, true);
        }

        [ScriptMethod(name: "Crater Carve", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40605"])]
        public void CraterCarve(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = "CraterCarve";
            dp.Scale = new(11);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 7000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            accessory.Method.TextInfo("远离场中", duration: 2000, true);
        }

        [ScriptMethod(name: "Turali Stone IV", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40616"])]
        public void TuraliStoneIV(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("分摊", duration: 2000, true);
        }

        [ScriptMethod(name: "Sonic Howl", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40618"])]
        public void SonicHowl(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        // Utility
        private static bool ParseObjectId(string? idStr, out uint id)
        {
            id = 0;
            if (string.IsNullOrEmpty(idStr)) return false;
            try
            {
                var idStr2 = idStr.Replace("0x", "");
                id = uint.Parse(idStr2, System.Globalization.NumberStyles.HexNumber);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
