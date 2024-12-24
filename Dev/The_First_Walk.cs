using KodakkuAssist.Script;
using KodakkuAssist.Module.GameEvent;
using System;
using System.Threading;
using KodakkuAssist.Module.Draw;
using Newtonsoft.Json;
using FFXIVClientStructs.FFXIV.Common.Math;

namespace KDrawScript.Dev
{

    [ScriptType(name: "Jeuno: The First Walk", territorys: [1248], guid: "69c6613b-0d45-48d5-adcf-bc90075cc0ba  ", version: "0.0.0.1", author: "Due")]
    public class FirstWalk
    {

        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        // Boss 1: Prishe of the Distant Chains
        [ScriptMethod(name: "Banishga", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40935"])]
        public void Banishga(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Banishga IV", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40954"])]
        public void BanishgaIV(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Banish Storm", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40946"])]
        public void BanishStorm(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Knuckle Sandwich", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4094[01]|40939)$"])]
        public void KnuckleSandwich(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Knuckle Sandwich";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 12000;
            switch (@event["ActionId"])
            {
                case "40940":
                    dp.Scale = new(18);
                    break;
                case "40941":
                    dp.Scale = new(27);
                    break;
                case "40939":
                    dp.Scale = new(9);
                    break;
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            accessory.Method.TextInfo("Out -> In", duration: 2000, true);
            /*
            var dp2 = accessory.Data.GetDefaultDrawProperties();
            dp2.Name = "Knuckle Sandwich - In";
            dp2.Radian = float.Pi * 2;
            dp2.Scale = new(50);
            dp2.Color = accessory.Data.DefaultDangerColor;
            dp2.Owner = sid;
            dp2.DestoryAt = 1000;

            switch (@event["ActionId"])
            {
                case "40940":
                    dp2.InnerScale = new(18);
                    break;
                case "40941":
                    dp2.InnerScale = new(27);
                    break;
                case "40939":
                    dp2.InnerScale = new(9);
                    break;
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp2);
            */
        }

        [ScriptMethod(name: "Nullifying Dropkick", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40957"])]
        public void NullifyingDropkick(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Nullifying Dropkick";
            dp.Scale = new(6);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = tid;
            dp.DestoryAt = 5000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            accessory.Method.TextInfo("Tank Stack", duration: 2000, true);
        }

        [ScriptMethod(name: "Banish", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40947"])]
        public void Banish(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Banish-{sid}";
            dp.Scale = new(6);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 500;
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Holy", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40963"])]
        public void Holy(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Holy-{tid}";
            dp.Scale = new(6);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = tid;
            dp.DestoryAt = 4500;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
            accessory.Method.TextInfo("Spread & Do not overlap", duration: 2000, true);
        }

        [ScriptMethod(name: "Auroral Uppercut", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4095[012])$"])]
        public void AuroralUppercut(Event @event, ScriptAccessory accessory)
        {
            var level = 0;

            switch (@event["ActionId"])
            {
                case "40950":
                    level = 1;
                    break;
                case "40951":
                    level = 2;
                    break;
                case "40952":
                    level = 3;
                    break;
            }

            if (level == 0) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            accessory.Log.Debug($"Auroral Uppercut: {level}");
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Auroral Uppercut-{level}";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Owner = accessory.Data.Me;
            dp.TargetObject = sid;
            dp.Rotation = float.Pi;
            dp.DestoryAt = 10000;

            switch (level)
            {
                case 1:
                    dp.Scale = new(1.5f, 12); // Need more test
                    break;
                case 2:
                    dp.Scale = new(1.5f, 25);
                    break;
                case 3:
                    dp.Scale = new(1.5f, 35);
                    break;
            }

            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }

        [ScriptMethod(name: "Explosion", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40955"])]
        public void Explosion(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Explosion-{sid}";
            dp.Scale = new(8);
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 5000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Asuran Fists", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40956"])]
        public void AsuranFists(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("Alliance Stack", duration: 2000, true);
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
