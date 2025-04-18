using ECommons;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KDrawScript.Dev
{
    [ScriptType(name: "The Cloud of Darkness (Chaotic)", territorys: [1241], guid: "436effd2-a350-4c67-b341-b4fe5a4ac233", version: "0.0.0.2", author: "Due")]
    public class Cloud_of_Darkness_Chaotic
    {
        private string Embrace = string.Empty;
        private string DelayWhat = string.Empty;
        private bool HaveLoomingChaos = false;
        private readonly List<Vector3> FlarePoint = [];
        private readonly List<Vector3> SeedPoint = []; // Only A, C Party Each have two points
        private readonly List<Vector3> TetherPoint = []; // Only A, C Party Each have four points
        private readonly List<uint> SeedTarget = [];

        [UserSetting(note: "是否开启文字提醒")]
        public bool EnableTextInfo { get; set; } = true;

        // [UserSetting(note: "是否开启额外提示。请确保小队排序正确。")]
        public bool EnableGuidance { get; set; } = false;

        // [UserSetting(note: "请选择你的队伍。")]
        public PartyEnum Party { get; set; } = PartyEnum.None;

        [UserSetting(note: "特殊提醒 不知道是什么绝对不要开")]
        public bool SpecialText { get; set; } = false;

        public enum PartyEnum
        {
            None = -1,
            A = 0,
            B = 1,
            C = 2
        }

        public void Init(ScriptAccessory accessory)
        {
            Embrace = string.Empty;
            DelayWhat = string.Empty;
            HaveLoomingChaos = false;
            SeedTarget.Clear();
            accessory.Method.RemoveDraw(".*");
        }
        #region P1
        [ScriptMethod(name: "Blade of Darkness 左右小月环及钢铁", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4044[468])$"])]
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

        [ScriptMethod(name: "AOE 提醒", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40509|40510|40456)$"])]
        public void DelugeofDarkness(Event @event, ScriptAccessory accessory)
        {
            SendText("AOE", accessory);
        }

        [ScriptMethod(name: "Razing-volley Particle Beam 场外车轮激光", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40511"])]
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
            SendText("小队直线分摊", accessory);
        }

        [ScriptMethod(name: "Unholy Darkness 治疗分组分摊", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0064"])]
        public void UnholyDarkness(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"Unholy Darkness - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            if (EnableGuidance)
                if (IsInSameParty(accessory, tid))
                    if (IsInSameStack(accessory, accessory.Data.Me, tid))
                        dp.Color = accessory.Data.DefaultSafeColor;
            dp.Scale = new(6);
            dp.Owner = tid;
            dp.DestoryAt = 7000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Flare 核爆绘制", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:015A"])]
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

            if (accessory.Data.Me != tid || !EnableGuidance) return;
            var index = Party switch
            {
                PartyEnum.A => 0,
                PartyEnum.B => 1,
                PartyEnum.C => 2,
                _ => -1
            };
            if (index == -1) index = InWhichParty(accessory, tid);
            if (index == -1) return;

            dp.Name = $"Flare Guide";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.DestoryAt = 5000;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.TargetPosition = FlarePoint[index];

            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
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
                    SendText("存储前方", accessory);
                    break;
                case "012D":
                    Embrace = "Backward";
                    SendText("存储后方", accessory);
                    break;
            }
        }

        [ScriptMethod(name: "Embrace AOE 放手前后绘制", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0228"])]
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

            Task.Delay(6000).ContinueWith(t =>
            {
                if (Embrace == "Backward")
                    SendText("向前走", accessory);
                else
                    SendText("向后走", accessory);
            }
            );

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Endeath 吸引提醒", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40515|40531)$"])]
        public void Endeath(Event @event, ScriptAccessory accessory)
        {
            if (@event["ActionId"] == "40515")
            {
                SendText("准备吸引", accessory);
            }
            else if (@event["ActionId"] == "40531")
            {
                DelayWhat = "Endeath";
                SendText("存储吸引", accessory);
            }
        }

        [ScriptMethod(name: "Delay Death & Aero 延时提醒", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:4182"])]
        public void DelayDeathAero(Event @event, ScriptAccessory accessory)
        {
            if (string.IsNullOrEmpty(DelayWhat)) return;

            if (DelayWhat == "Endeath")
            {
                SendText("准备吸引", accessory);
            }
            else if (DelayWhat == "Enaero")
            {
                SendText("准备击退", accessory);
            }

            DelayWhat = string.Empty;
        }

        [ScriptMethod(name: "Enaero 击退提醒", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40524|40532)$"])]
        public void Enaero(Event @event, ScriptAccessory accessory)
        {
            if (@event["ActionId"] == "40524")
            {
                SendText("准备击退", accessory);
            }
            else if (@event["ActionId"] == "40532")
            {
                DelayWhat = "Enaero";
                SendText("存储击退", accessory);
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

        [ScriptMethod(name: "Break IV 背对提醒", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40527"])]
        public void BreakIV(Event @event, ScriptAccessory accessory)
        {
            SendText("背对", accessory);
        }
        #endregion
        #region P2

        [ScriptMethod(name: "Ghastly Gloom 大云月环十字绘制", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40458|40460)$"])]
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

        [ScriptMethod(name: "Dark Energy Particle Beam 附身激光绘制", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:2387"])]
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

        [ScriptMethod(name: "Third Art Of Darkness 小云三连", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:regex:^(00EF|00F[012])$"])]
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

                    if (SpecialText)
                        Task.Delay(5000).ContinueWith(t =>
                        {
                            if (!HaveLoomingChaos)
                                SendText("左侧分摊", accessory);
                            else
                                SendText("北侧分摊", accessory);
                        }
                        );

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

                    if (SpecialText)
                        Task.Delay(5000).ContinueWith(t =>
                        {
                            SendText("左起第二分散", accessory);
                        }
                        );

                    break;
            }
        }

        [ScriptMethod(name: "Evil Seed 放种子绘制", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0227"])]
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

            if (!EnableGuidance) return;
            if (Party == PartyEnum.None || Party == PartyEnum.B) return; // No support for B Party
            if (IsInSameParty(accessory, tid)) SeedTarget.Add(tid); // Me included of course
            if (SeedTarget.Count != 2) return;
            if (!SeedTarget.Contains(accessory.Data.Me)) return; // Not in the list
            var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            var otherIndex = accessory.Data.PartyList.IndexOf(SeedTarget.First(x => x != accessory.Data.Me));
            var offset = (int)Party + (myIndex < otherIndex ? 1 : 2); // A, C Party Index at 1, 2, 3, 4

            dp.Name = $"Evil Seed Guide";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.DestoryAt = 8000;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.TargetPosition = SeedPoint[offset];

            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }

        [ScriptMethod(name: "Evil Seed Tether 拉线站位", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40492"])]
        public void EvilSeedTether(Event @event, ScriptAccessory accessory)
        {
            if (SpecialText) SendText("右上接种子", accessory);
            if (!EnableGuidance) return;
            if (Party == PartyEnum.None || Party == PartyEnum.B) return; // No support for B Party
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (tid != accessory.Data.Me) return;
            var index = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (index == 1 || index == 2) return; // ST and H1 inner platform

            var priority = new int[] { -1, -1, -1, 1, -1, 2, 3, 4 };
            var offset = (int)Party + priority[index];

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"Evil Seed Tether Guide";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.DestoryAt = 8000;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.TargetPosition = TetherPoint[offset];

            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }

        [ScriptMethod(name: "Particle Concentration 踩塔指引", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40472"])]
        public void ParticleConcentration(Event @event, ScriptAccessory accessory)
        {
            if (SpecialText)
                if (!HaveLoomingChaos) SendText("左/上踩塔", accessory);
                else SendText("右/下踩塔", accessory);
            if (!EnableGuidance) return;
            if (Party == PartyEnum.None || Party == PartyEnum.B) return; // No support for B Party
            if (HaveLoomingChaos) return; // No support after position swap
            // H1 Team take north / west. H2 Team take south / east
            var index = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (index == 1 || index == 2) return; // ST and H1 inner platform
        }

        [ScriptMethod(name: "Diffusive Force Particle Beam 分散点名绘制", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40464"])]
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
        /* Conflict with in-game notice
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
        [ScriptMethod(name: "Phaser AOE 小云扇形绘制", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40497"])]
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

        [ScriptMethod(name: "Active Pivot Particle Beam 90度前后炮(实验版)", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4046[79])$"])]
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

        [ScriptMethod(name: "Looming Chaos", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41673"], userControl: false)]
        public void LoomingChaos(Event @event, ScriptAccessory accessory) => HaveLoomingChaos = true;

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

        private static Vector3 ParsePosition(Event @event, string type) => JsonConvert.DeserializeObject<Vector3>(@event[type]);

        private void SendText(string text, ScriptAccessory accessory, int duration = 2000, bool isImportant = true)
        {
            if (!EnableTextInfo) return;
            accessory.Method.TextInfo(text, duration, isImportant);
        }

        private bool IsInSameParty(ScriptAccessory accessory, uint target) => accessory.Data.PartyList.Contains(target);
        private bool IsInSameStack(ScriptAccessory accessory, uint source, uint target)
        {
            var sourceIndex = accessory.Data.PartyList.IndexOf(source);
            var targetIndex = accessory.Data.PartyList.IndexOf(target);
            return (sourceIndex % 2) == (targetIndex % 2);
        }

        private unsafe int InWhichParty(ScriptAccessory accessory, uint target)
        {
            var group = GroupManager.Instance()->MainGroup;
            for (var index = 0; index <= 2; index++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var id = group.GetAllianceMemberByGroupAndIndex(index, j)->EntityId;
                    if (id == target) return index;
                }
            }
            return -1;
        }

        #endregion
    }
}