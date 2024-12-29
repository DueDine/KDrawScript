using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using System.Threading;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "The Cloud of Darkness (Chaotic)", territorys: [1241], guid: "436effd2-a350-4c67-b341-b4fe5a4ac233", version: "0.0.0.1")]
    public class Cloud_of_Darkness_Chaotic
    {
        private string Embrace = string.Empty;
        private string DelayWhat = string.Empty;

        public void Init(ScriptAccessory accessory)
        {
            Embrace = string.Empty;
            DelayWhat = string.Empty;
            accessory.Method.RemoveDraw(".*");
        }
        #region P1
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
            accessory.Method.TextInfo("AOE", 2000, true);
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
            dp.Delay = 5000;
            dp.DestoryAt = 3000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Razing-volley Particle Beam Cancel", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40511"], UserControl = false)]
        public void RazingvolleyParticleBeamCancel(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            accessory.Method.RemoveDraw($"Razing-volley Particle Beam - {sid}");
        }


        [ScriptMethod(name: "Rapid-sequence Particle Beam", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40512"])]
        public void RapidsequenceParticleBeam(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("小队直线分摊", 2000, true);
        }

        [ScriptMethod(name: "Unholy Darkness", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0064"])]
        public void UnholyDarkness(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"Rapid-sequence Particle Beam AOE - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(6);
            dp.Owner = tid;
            dp.DestoryAt = 7000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Flare", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:015A"])]
        public void Flare(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"Flare AOE - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(25);
            dp.Owner = tid;
            dp.Delay = 1500;
            dp.DestoryAt = 6000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
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
                    accessory.Method.TextInfo("存储前方", 2000, true);
                    break;
                case "012D":
                    Embrace = "Backward";
                    accessory.Method.TextInfo("存储后方", 2000, true);
                    break;
            }

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
            dp.DestoryAt = 9000;

            if (Embrace == "Backward")
            {
                dp.Rotation = float.Pi;
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
        
        [ScriptMethod(name: "Endeath", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40515|40531)$"])]
        public void Endeath(Event @event, ScriptAccessory accessory)
        {
            if (@event["ActionId"] == "40515")
            {
                accessory.Method.TextInfo("准备吸引", 2000, true);
            }
            else if (@event["ActionId"] == "40531")
            {
                DelayWhat = "Endeath";
                accessory.Method.TextInfo("存储吸引", 2000, true);
            }
        }

        [ScriptMethod(name: "Delay Death & Aero", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:4182"])]
        public void DelayDeathAero(Event @event, ScriptAccessory accessory)
        {
            if (string.IsNullOrEmpty(DelayWhat)) return;

            if (DelayWhat == "Endeath")
            {
                accessory.Method.TextInfo("准备吸引", 2000, true);
            }
            else if (DelayWhat == "Enaero")
            {
                accessory.Method.TextInfo("准备击退", 2000, true);
            }

            DelayWhat = string.Empty;
        }

        [ScriptMethod(name: "Enaero", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40524|40532)$"])]
        public void Enaero(Event @event, ScriptAccessory accessory)
        {
            if (@event["ActionId"] == "40524")
            {
                accessory.Method.TextInfo("准备击退", 2000, true);
            }
            else if (@event["ActionId"] == "40532")
            {
                DelayWhat = "Enaero";
                accessory.Method.TextInfo("存储击退", 2000, true);
            }
        }

        [ScriptMethod(name: "Enaero AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4052[36])$"])]
        public void EnaeroAOE(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = "Enaero AOE";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;

            switch (@event["ActionId"])
            {
                case "40523":
                    dp.Scale = new(8);
                    dp.DestoryAt = 2000;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    break;
                case "40526":
                    dp.Scale = new(8);
                    dp.DestoryAt = 1000;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    break;
            }
        }

        [ScriptMethod(name: "Endeath AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4052[01]|4051[78])$"])]
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
                    dp.Delay = 3000;
                    dp.DestoryAt = 2000;
                    dp.Radian = float.Pi * 2;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
                    break;
                case "40517":
                    dp.Scale = new(6);
                    dp.DestoryAt = 4000;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    break;
                case "40518":
                    dp.Scale = new(40);
                    dp.InnerScale = new(6);
                    dp.Delay = 4000;
                    dp.DestoryAt = 2000;
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
        #endregion
        #region P2
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

        [ScriptMethod(name: "Dark Energy Particle Beam", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2387"])]
        public void DarkEnergyParticleBeam(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Dark Energy Particle Beam - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(25);
            dp.Owner = tid;
            dp.Radian = float.Pi * 0.09f;
            dp.Delay = int.Parse(@event["DurationMilliseconds"]) - 5000;
            dp.DestoryAt = 5500;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Dark Energy Particle Beam Cancel", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:2387"], UserControl = false)]
        public void DarkEnergyParticleBeamCancel(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            accessory.Method.RemoveDraw($"Dark Energy Particle Beam - {tid}");
        }

        /*
         * TargetIcon 00EF Left 00F0 Right 00F2 6 Spread 00F1 2 Stack
         */

        [ScriptMethod(name: "Third Art Of Darkness", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:regex:^(00EF|00F[012])$"])]
        public void ThirdArtOfDarkness(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Third Art Of Darkness - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = tid;
            dp.Delay = 5000;
            dp.DestoryAt = 5000;

            switch (@event["Id"])
            {
                case "00EF":
                    dp.Scale = new(15);
                    dp.Radian = float.Pi;
                    dp.Rotation = float.Pi / 2;
                    dp.Name += " - Left";

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
                    break;
                case "00F0":
                    dp.Scale = new(15);
                    dp.Radian = float.Pi;
                    dp.Rotation = -float.Pi / 2;
                    dp.Name += " - Right";

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
                    break;
                case "00F1":
                    dp.Scale = new(3);
                    dp.Name += " - 2 Stack";

                    for (var i = 1; i <= 3; i++)
                    {
                        dp.CentreResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
                        dp.CentreOrderIndex = (uint)i;
                        dp.Name += $" - {i}";

                        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    }
                    break;
                case "00F2":
                    dp.Scale = new(5, 22);
                    dp.Name += " - 6 Spread";

                    for (var i = 1; i <= 6; i++)
                    {
                        dp.TargetResolvePattern = PositionResolvePatternEnum.PlayerNearestOrder;
                        dp.TargetOrderIndex = (uint)i;
                        dp.Name += $" - {i}";
                        accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
                    }
                    break;
            }
        }

        [ScriptMethod(name: "Evil Seed", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0227"])]
        public void EvilSeed(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"Evil Seed AOE - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(5);
            dp.Owner = tid;
            dp.DestoryAt = 8000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Evil Seed Tether", eventType: EventTypeEnum.Tether, eventCondition: ["Id:0012"])]
        public void EvilSeedTether(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (tid != accessory.Data.Me) return;

            accessory.Method.TextInfo("拉线", 2000, true);
        }

        [ScriptMethod(name: "Diffusive Force Particle Beam", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40464"])]
        public void DiffusiveForceParticleBeam(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Diffusive Force Particle Beam";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(7); // Secone one would be smaller but for simplicity we use same scale
            dp.Owner = accessory.Data.Me;
            dp.DestoryAt = 10000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }
        /*
        [ScriptMethod(name: "Phaser Text", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4049[56])$"])]
        public void PhaserText(Event @event, ScriptAccessory accessory)
        {
            if (@event["ActionId"] == "40495")
            {
                accessory.Method.TextInfo("侧面 -> 前面", 2000, true);
            }
            else if (@event["ActionId"] == "40496")
            {
                accessory.Method.TextInfo("前面 -> 侧面", 2000, true);
            }
        }
        */
        [ScriptMethod(name: "Phaser AOE", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40497"])]
        public void PhaserAOE(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Phaser AOE - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(23);
            dp.Owner = sid;
            dp.Delay = 6000;
            dp.DestoryAt = 3000;
            dp.Radian = float.Pi * 0.3f;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Phaser AOE Cancel", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40497"], UserControl = false)]
        public void PhaserAOECancel(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            accessory.Method.RemoveDraw($"Phaser AOE - {sid}");
        }

        [ScriptMethod(name: "Active Pivot Particle Beam", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4046[79])$"])]
        public void ActivePivotParticleBeam(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            
            var dp = accessory.Data.GetDefaultDrawProperties();
            var rot = -float.Pi / 2;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(18, 80);
            dp.Owner = sid;


            var change = @event["ActionId"] == "40467" ? -1 : 1;
            for (var i = 0; i < 5; i++)
            {
                dp.Name = $"Active Pivot Particle Beam - {i}";
                dp.Rotation = i * change * float.Pi * 0.125f + rot;
                dp.FixRotation = true;
                dp.DestoryAt = 14500 + i * 1500;

                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
            }
        }


        #endregion

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