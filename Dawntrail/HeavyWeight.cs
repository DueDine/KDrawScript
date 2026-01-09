using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "Heavy Weight Savage", territorys: [1323], guid: "A5689105-A7FD-4B84-9674-67610285768C", version: "0.0.0.1", author: "Due")]
    public class HeavyWeightSavage
    {
        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        [ScriptMethod(name: "Cutback Blaze", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:46537"])]
        public void CutbackBlaze(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Cutback Blaze";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(60);
            dp.Owner = sid;
            dp.DestoryAt = 4300;
            dp.Radian = float.Pi * 2 * 330 / 360;
            dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerFarestOrder;
            dp.TargetOrderIndex = 1;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Deep Impact", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:46519"])]
        public void DeepImpact(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Deep Impact";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(6);
            dp.Owner = sid;
            dp.DestoryAt = 4900;
            dp.CentreResolvePattern = PositionResolvePatternEnum.PlayerFarestOrder;
            dp.CentreOrderIndex = 1;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Hot Aerial", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:46532"])]
        public void HotAerial(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Hot Aerial";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(6);
            dp.Owner = sid;
            dp.DestoryAt = 4700;
            dp.CentreResolvePattern = PositionResolvePatternEnum.PlayerFarestOrder;
            dp.CentreOrderIndex = 1;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Hot Aerial 2", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:47389"])]
        public void HotAerial_(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Hot Aerial";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(6);
            dp.Owner = sid;
            dp.DestoryAt = 1700;
            dp.Delay = 500;
            dp.CentreResolvePattern = PositionResolvePatternEnum.PlayerFarestOrder;
            dp.CentreOrderIndex = 1;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
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
