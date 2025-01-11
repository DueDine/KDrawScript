using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "The Copied Factory", territorys: [882], guid: "182DCBAA-AF3F-4576-BFA1-0F91CD3FEB04", version: "0.0.0.1", author: "Due")]
    public class The_Copied_Factory
    {
        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        // Boss 1: Serial-Jointed Command Model
        [ScriptMethod(name: "Energy Assault", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18613"])]
        public void EnergyAssault(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Energy Assault - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(30);
            dp.DestoryAt = 5000;
            dp.Radian = float.Pi * (1 / 3);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Centrifugal Spin", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18633"])]
        public void CentrifugalSpin(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Centrifugal Spin - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(8, 30);
            dp.DestoryAt = 5000;
            dp.Offset = new(0, 0, 15);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
        /* Find its owner
        [ScriptMethod(name: "High-powered Laser", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:00A4"])]
        public void HighPoweredLaser(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"High-powered Laser - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = tid;
            dp.Scale = new(4, 70);
            dp.DestoryAt = 5000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
        */
        [ScriptMethod(name: "Sidestriking Spin", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(1863[56])$"])]
        public void SidestrikingSpin(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Sidestriking Spin - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(12, 30);
            dp.DestoryAt = 6000;

            if (@event["ActionId"] == "18635")
                dp.Offset = new(-9, 0, 15);
            else if (@event["ActionId"] == "18636")
                dp.Offset = new(9, 0, 15);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        // Boss 2: Hobbes
        // Use cactbot 

        // Boss 3: Engels
        [ScriptMethod(name: "Marx Smash", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(1821[458]|18222)$"])]
        public void MarxSmash(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Marx Smash - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(30, 60);
            dp.DestoryAt = 6000;

            if (@event["ActionId"] == "18214")
                dp.Offset = new(-15, 0, 0);
            else if (@event["ActionId"] == "18215")
                dp.Offset = new(15, 0, 0);
            else if (@event["ActionId"] == "18218")
            {
                dp.Scale = new(60, 30);
            }
            else if (@event["ActionId"] == "18222")
            {
                dp.Scale = new(60, 35);
                dp.Offset = new(0, 0, -25);
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Marx Thrust", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18684"])]
        public void MarxThrust(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Marx Thrust - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(20, 30);
            dp.DestoryAt = 5500;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        // Boss 4: 9S-Operated Walking Fortress
        [ScriptMethod(name: "Laser Turret", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:19060"])]
        public void LaserTurret(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Laser Turret - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(8, 90);
            dp.DestoryAt = 4000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Laser Suppression", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18656"])]
        public void LaserSuppression(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Laser Suppression - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Rotation = rot;
            dp.FixRotation = true;
            dp.Scale = new(60);
            dp.DestoryAt = 5000;
            dp.Radian = float.Pi * (1 / 2);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Marx Impact", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:18644"])]
        public void MarxImpact(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Marx Impact - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(22);
            dp.DestoryAt = 5000;

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
