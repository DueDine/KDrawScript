using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "AAC Cruiserweight M4", territorys: [1262], guid: "73D227EB-D2E2-40B6-8107-16E36A09FB8D", version: "0.0.0.1", author: "Due")]
    public class CruiserweightM4
    {
        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        [ScriptMethod(name: "Wolves' Reign", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4337[6789]|4330[234]|43311)$"])]
        public void WolvesReign(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Wolves' Reign";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(6);
            dp.Owner = sid;
            dp.DestoryAt = 5500;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Wolves' Reign Line", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(43384|43368)$"])]
        public void WolvesReignLine(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Wolves' Reign Line";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(10, 36);
            dp.Owner = sid;
            dp.DestoryAt = 1500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Wolves' Reign Fan", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(43386|42928)$"])]
        public void WolvesReignFan(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Wolves' Reign Fan";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(40);
            dp.Owner = sid;
            dp.DestoryAt = 1500;
            dp.Radian = float.Pi / 3 * 2;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Moonbeam's Bite", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4338[89]|4184[67])$"])]
        public void MoonbeamsBite(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Moonbeam's Bite";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(20, 40);
            dp.Owner = sid;
            dp.Delay = 3000;
            dp.DestoryAt = 2000;
            dp.Position = ParsePosition(@event, "EffectPosition");
            dp.Rotation = float.Parse(@event["SourceRotation"]);
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Shadowchase & Roaring Wind", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(43393|41844|43397)$"])]
        public void Shadowchase(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Shadowchase";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(8, 40);
            dp.Owner = sid;
            dp.DestoryAt = 2500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Fanged Charge", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41861"])]
        public void FangedCharge(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Fanged Charge";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(6, 46);
            dp.Owner = sid;
            dp.DestoryAt = 3500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
            dp.Rotation = float.Pi;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Weal of Stone", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(43401)$"])]
        public void WealOfStone(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Weal of Stone";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(6, 40);
            dp.Owner = sid;
            dp.DestoryAt = 2000;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Towerfall", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(43315)$"])]
        public void Towerfall(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Towerfall";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(6, 30);
            dp.Owner = sid;
            dp.DestoryAt = 3500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }


        #region Utility
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

        private static Vector3 ParsePosition(Event @event, string type)
        {
            return JsonConvert.DeserializeObject<Vector3>(@event[type]);
        }
        #endregion
    }
}
