using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "The Cloud of Darkness (Chaotic)", territorys: [1241], guid: "436effd2-a350-4c67-b341-b4fe5a4ac233", version: "0.0.0.1")]
    public class Cloud_of_Darkness_Chaotic
    {
        private string Embrace = string.Empty;

        public void Init(ScriptAccessory accessory)
        {
            Embrace = string.Empty;
            accessory.Method.RemoveDraw(".*");
        }

        [ScriptMethod(name: "Blade of Darkness", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4044[468])$"])]
        public void BladeofDarkness(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            if (@event["ActionId"] != "40448")
            {
                dp.Name = "Blade of Darkness";
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.Radian = float.Pi * 2;
                dp.InnerScale = new(12);
                dp.Scale = new(60);
                dp.Position = ParsePosition(@event, "EffectPosition");
                dp.Owner = sid;
                dp.DestoryAt = 7000;

                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
            }
            else if (@event["ActionId"] == "40448")
            {
                dp.Name = "Blade of Darkness";
                dp.Color = accessory.Data.DefaultDangerColor;
                dp.Radian = float.Pi;
                dp.Scale = new(30);
                dp.Position = ParsePosition(@event, "EffectPosition");
                dp.Owner = sid;
                dp.DestoryAt = 7000;

                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
            }
        }

        [ScriptMethod(name: "Deluge of Darkness", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40509|40510|40456)$"])]
        public void DelugeofDarkness(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("转场 AOE", 2000, true);
        }

        [ScriptMethod(name: "Razing-volley Particle Beam", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40511"])]
        public void RazingvolleyParticleBeam(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"Razing-volley Particle Beam - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(8, 45);
            dp.Owner = sid;
            dp.DestoryAt = 4000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Rapid-sequence Particle Beam", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40512"])]
        public void RapidsequenceParticleBeam(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("小队直线分摊", 2000, true);
        }

        [ScriptMethod(name: "Grim Embrace", eventType: EventTypeEnum.Tether, eventCondition: ["Id:regex:^(012[CD])$"], UserControl = false)]
        public void GrimEmbrace(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (sid != accessory.Data.Me) return;

            switch (@event["Id"])
            {
                case "012C":
                    Embrace = "Forward";
                    break;
                case "012D":
                    Embrace = "Backward";
                    break;
            }

        }

        [ScriptMethod(name: "Aero Knockback", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40524"])]
        public void AeroKnockback(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("对角击退", 2000, true);
        }

        [ScriptMethod(name: "Embrace AOE", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0228"])]
        public void EmbraceAOE(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (tid != accessory.Data.Me) return;
            if (string.IsNullOrEmpty(Embrace)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Embrace AOE";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(8, 8);
            dp.Owner = tid;
            dp.DestoryAt = 8000;

            if (Embrace == "Backward")
            {
                dp.Rotation = float.Pi;
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
        /*
        [ScriptMethod(name: "Endeath", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40531"])]
        public void Endeath(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("靠边", 2000, true);
        }
        */
        [ScriptMethod(name: "Endeath AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4052[01])$"])]
        public void EndeathAOE(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Endeath AOE";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;

            switch (@event["ActionId"])
            {
                case "40520":
                    dp.Scale = new(6);
                    dp.DestoryAt = 3000;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    break;
                case "40521":
                    dp.Scale = new(40);
                    dp.InnerScale = new(6);
                    dp.DestoryAt = 5000;
                    dp.Radian = float.Pi * 2;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
                    break;
            }
        }
        
        [ScriptMethod(name: "Particle Concentration", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40472"])]
        public void ParticleConcentration(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("准备踩塔", 2000, true);
        }

        [ScriptMethod(name: "Ghastly Gloom", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40458|40460)$"])]
        public void GhastlyGloom(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Ghastly Gloom";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 8000;

            switch (@event["ActionId"])
            {
                case "40458":
                    dp.Scale = new(30, 80);

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);

                    dp.Rotation = float.Pi / 2;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);

                    break;
                case "40460":
                    dp.Scale = new(40);
                    dp.InnerScale = new(21);
                    dp.Radian = float.Pi * 2;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
                    break;
            }
        }

        [ScriptMethod(name: "Break IV", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40527"])]
        public void BreakIV(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("背对", 2000, true);
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