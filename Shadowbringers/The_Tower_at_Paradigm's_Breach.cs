using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "The Tower at Paradigm's Breach", territorys: [966], guid: "C5E49762-7E9A-4CCC-8842-1912842420F4", version: "0.0.0.1", author: "Due")]
    public class The_Tower_at_Paradigms_Breach
    {
        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        // Boss 1: Knave of Hearts
        [ScriptMethod(name: "Colossal Impact", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(24229|2423[01]|2477[456])$"])]
        public void ColossalImpact(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Colossal Impact - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Position = ParsePosition(@event, "EffectPosition");
            dp.Rotation = rot;
            dp.FixRotation = true;
            dp.Scale = new(20, 61);
            dp.DestoryAt = 8000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Magic Artillery Beta", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:24243"])]
        public void MagicArtilleryBeta(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Magic Artillery Beta - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = tid;
            dp.Scale = new(3);
            dp.DestoryAt = 5000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        // Boss 2: Hansel and Gretel
        [ScriptMethod(name: "Upgraded Shield", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(2365[79])$"])]
        public void UpgradedShield(Event @event, ScriptAccessory accessory)
        {
            if (@event["ActionId"] == "23657")
                accessory.Method.TextInfo("攻击韩塞尔", 2000, true);
            else if (@event["ActionId"] == "23659")
                accessory.Method.TextInfo("攻击格雷特", 2000, true);
        }

        [ScriptMethod(name: "Bloody Sweep", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(2366[0123])$"])]
        public void BloodySweep(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Bloody Sweep - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Position = ParsePosition(@event, "EffectPosition");
            dp.Rotation = rot;
            dp.FixRotation = true;
            dp.Scale = new(25, 50);
            dp.DestoryAt = 8000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        // Boss 3: Red Girl
        // Nothing Interesting

        // Boss 4: Xun Zi and Meng Zi
        [ScriptMethod(name: "Deploy Armaments", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(2355[47]|2469[67])$"])]
        public void DeployArmaments(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Deploy Armaments - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(18, 50);
            dp.DestoryAt = 6000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        // Boss 5: False Idol
        [ScriptMethod(name: "Made Magic", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(2351[01])$"])]
        public void MadeMagic(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Made Magic - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Position = ParsePosition(@event, "EffectPosition");
            dp.Scale = new(30, 50);
            dp.DestoryAt = 7000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        // Boss 6: Her Inflorescence
        /*
        [ScriptMethod(name: "Uneven Footing", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:23522"])]
        public void RecreateStructure(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Recreate Structure - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Rotation = rot;
            dp.FixRotation = true;
            dp.Scale = new(30, 80);
            dp.DestoryAt = 2000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
        */
        [ScriptMethod(name: "Heavy Arms", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(2353[35])$"])]
        public void HeavyArms(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Heavy Arms - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Rotation = rot;
            dp.FixRotation = true;
            dp.DestoryAt = 7000;
            if (@event["ActionId"] == "23533")
                dp.Scale = new(12, 100);
            else
            {
                dp.Scale = new(100, 44);
                dp.Position = ParsePosition(@event, "EffectPosition");
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Towerfall", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:23540"])]
        public void Towerfall(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Towerfall - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Rotation = rot;
            dp.FixRotation = true;
            dp.Scale = new(14, 70);
            dp.DestoryAt = 3000;

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
