using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "AAC Cruiserweight M2", territorys: [1258], guid: "1EF7A8D1-1D46-4E4A-9051-CC392724CA18", version: "0.0.0.1", author: "Due")]
    public class CruiserweightM2
    {
        public ulong DartId = 0;

        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
            DartId = 0;
        }

        [ScriptMethod(name: "Dart", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:18332"])]
        public void Dart(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            DartId = sid;
        }

        // SourceDataId: 18330 Bomb 18331 Fly Bomb 18333 Shot 18332 Target
        [ScriptMethod(name: "Single / Double Style", eventType: EventTypeEnum.SetObjPos, eventCondition: ["SourceDataId:regex:^(1833[013])$"])]
        public void SingleStyle(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Single Style";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;

            if (@event["SourceDataId"] == "18330")
            {
                dp.Scale = new(15);
                dp.DestoryAt = 3000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
            else if (@event["SourceDataId"] == "18331")
            {
                dp.Scale = new(15);
                dp.DestoryAt = 3000;
                var pos = ParsePosition(@event, "SourcePosition");
                if (pos.X >= 100)
                    dp.Position = new(pos.X - 16, pos.Y, pos.Z);
                else
                    dp.Position = new(pos.X + 16, pos.Y, pos.Z);
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            }
            /*
            else if (@event["SourceDataId"] == "18333")
            {
                if (IsDartVisible(accessory)) dp.TargetPosition = new(100, 0, 100);
                dp.Scale = new(7, 50);
                dp.DestoryAt = 3000;
                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
            }
            */

        }

        [ScriptMethod(name: "Spray Pain", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(42603)$"])]
        public void SprayPain(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Spray Pain";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(10);
            dp.Delay = 2000;
            dp.DestoryAt = 4500;
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

        private unsafe bool IsDartVisible(ScriptAccessory accessory)
        {
            if (DartId == 0) return false;
            var dartObject = GameObjectManager.Instance()->Objects.GetObjectByGameObjectId(DartId);
            return dartObject != null && dartObject->DrawObject->IsVisible;
        }
        #endregion
    }
}
