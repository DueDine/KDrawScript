using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "AAC Cruiserweight M1", territorys: [1256], guid: "6376CF47-0403-459E-9350-D168AB9119B5", version: "0.0.0.1", author: "Due")]
    public class CruiserweightM1
    {
        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        [ScriptMethod(name: "Do the Hustle", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4269[78])$"])]
        public void DoTheHustle(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Do the Hustle";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(50, 25);
            dp.DestoryAt = 4500;
            dp.Position = new(100, 0, 100);
            if (@event["ActionId"] == "42697")
            {
                dp.Rotation = float.Pi / 2;
            }
            else if (@event["ActionId"] == "42698")
            {
                dp.Rotation = -float.Pi / 2;
            }
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "2-snap Twist", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(42706)$"])]
        public void SnapTwist2(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "2-snap Twist";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(50, 25);
            dp.Delay = 2000;
            dp.DestoryAt = 1500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "4-snap Twist", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(42725)$"])]
        public void SnapTwist4(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "4-snap Twist";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(50, 25);
            dp.Delay = 2000;
            dp.DestoryAt = 1500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Moonburn", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4278[34])$"])]
        public void Moonburn(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Moonburn";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(15, 40);
            dp.DestoryAt = 9000;
            dp.Position = ParsePosition(@event, "EffectPosition");
            dp.Rotation = float.Parse(@event["SourceRotation"]);
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Frogtourage", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(4276[45])$"])]
        public void Frogtourage(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Frogtourage";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(90, 25);
            dp.Position = new(100, 0, 100);
            if (@event["ActionId"] == "42764")
            {
                dp.Rotation = float.Pi / 2;
            }
            else if (@event["ActionId"] == "42765")
            {
                dp.Rotation = -float.Pi / 2;
            }
            dp.Delay = 18500;
            dp.DestoryAt = 1900;
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
