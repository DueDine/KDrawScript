using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "AAC Cruiserweight M3", territorys: [1260], guid: "E30016AA-1FD4-4E87-8152-15282D46B806", version: "0.0.0.1", author: "Due")]
    public class CruiserweightM3
    {
        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        [ScriptMethod(name: "Brutish Swing Donut", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(42271)$"])]
        public void BrutishSwing(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Brutish Swing";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.InnerScale = new(12);
            dp.Scale = new(60);
            dp.DestoryAt = 4500;
            dp.Radian = float.Pi * 2;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
        }

        [ScriptMethod(name: "Brutish Swing Char", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(42270)$"])]
        public void BrutishSwingChar(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Brutish Swing Char";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(12);
            dp.DestoryAt = 4500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Brutish Swing Fan", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(42293|42303|4231[79])$"])]
        public void BrutishSwingFan(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Brutish Swing Fan";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;

            if (@event["ActionId"] == "42293" || @event["ActionId"] == "42317")
            {
                dp.Scale = new(25);
                dp.Radian = float.Pi;
                dp.DestoryAt = 3500;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
            }
            else if (@event["ActionId"] == "42303" || @event["ActionId"] == "42319")
            {
                dp.InnerScale = new(20);
                dp.Scale = new(88);
                dp.DestoryAt = 6500;
                dp.Radian = float.Pi * 2;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
            }
        }

        [ScriptMethod(name: "Neo Bombarian Special", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:42287"])]
        public void NeoBombarianSpecial(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Neo Bombarian Special";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Owner = accessory.Data.Me;
            dp.Scale = new(2);
            dp.DestoryAt = 6000;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.TargetPosition = new(100, 0, 85);
            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Arrow, dp);
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
