using Dalamud.Utility.Numerics;
using ECommons;
using ECommons.MathHelpers;
using FFXIVClientStructs.FFXIV.Client.Game.Group;
using FFXIVClientStructs.FFXIV.Client.Game.Object;
using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Data;
using KodakkuAssist.Extensions;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.Draw.Manager;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KDrawScript.Dev
{
    [ScriptType(name: "CoD (Chaotic) 暗黑之云诛灭战", territorys: [1241], guid: "436effd2-a350-4c67-b341-b4fe5a4ac233", version: "0.0.1.7", author: "Due", note: NoteStr, updateInfo: UpdateInfo)]
    public class Cloud_of_Darkness_Chaotic
    {
        private const string NoteStr =
        """
        当前仅有基础绘制。以及 A/C 队 D2 - 4 指路 （即固定换到对方平台组）。
        若发生问题请携ARR反馈。
        """;

        private const string UpdateInfo =
        """
        1. 修复了P3小云正侧炮。
        2. 内场组添加放种子前预站位。
        3. 内场组添加回旋式波动炮预占位与返回位。
        4. 若处于内场组，回旋式波动炮范围绘图直接出现。
        5. 加入了内场地板的持续时间绘制。
        6. 加入了踩塔指路。
        """;

        private const bool Debugging = false;
        private const bool ReplayGroup = false;

        private static List<string> _role = ["MT", "ST", "H1", "H2", "D1", "D2", "D3", "D4"];
        private static List<string> _alliance = ["A", "B", "C"];
        private int _partyMemberIdx = -1;
        private enum CodPhase
        {
            Init,
            Diamond,
            Tilt,
            // Exchange,    // 弃用，可以使用 bool HaveLoomingChaos;
        }

        private CodPhase _codPhase = CodPhase.Init;
        private static List<ManualResetEvent> _events = Enumerable
            .Range(0, 20)
            .Select(_ => new ManualResetEvent(false))
            .ToList();

        private List<(ulong, string)> Embrace = [];
        private string DelayWhat = string.Empty;
        private bool HaveLoomingChaos = false;
        private readonly List<Vector3> FlarePoint = [new(72, 0, 76), new(100, 0, 103), new(126, 0, 76)];
        private readonly List<Vector3> SeedPoint = [new(0, 0, 0), new(70, 0, 92), new(70, 0, 107), new(130, 0, 108), new(130, 0, 92)]; // Only A, C Party Each have two points
        private readonly Object SeedLock = new();
        private readonly List<Vector3> TetherPointA = [new(67, 0, 93), new(80, 0, 95), new(80, 0, 104), new(67, 0, 106)];
        private readonly List<Vector3> TetherPointC = [new(133, 0, 106), new(119, 0, 103), new(119, 0, 95), new(132, 0, 94)];
        private readonly List<Vector3> SpreadPointC = [new(126.89f, 0, 94.59f), new(129.41f, 0, 95.50f), new(131.86f, 0, 97.56f), new(131.69f, 0, 102.13f), new(130.01f, 0, 105.10f), new(125.94f, 0, 105.83f)];
        private readonly List<Vector3> SpreadPointA = [new(73.57f, 0, 105.46f), new(70.24f, 0, 105.06f), new(68.16f, 0, 103.41f), new(68.50f, 0, 98.08f), new(70.24f, 0, 96.12f), new(73.31f, 0, 96.32f)];
        private readonly Vector3 CenterC = new(126.50f, 0, 100);
        private readonly Vector3 CenterA = new(73.50f, 0, 100);
        private readonly Vector3 Center = new(100, 0, 100);
        private readonly Tiles TileInstance = new();
        private readonly Towers TowerInstance = new();
        private readonly List<uint> SeedTarget = [];
        private int RazingRecord = 0;
        private bool EverDrawPhaser = false;

        [UserSetting(note: "是否开启文字提醒")]
        public bool EnableTextInfo { get; set; } = true;

        [UserSetting(note: "是否开启额外提示。请确保小队排序正确。")]
        public bool EnableGuidance { get; set; } = false;

        [UserSetting(note: "请选择你的队伍。")]
        public PartyEnum Party { get; set; } = PartyEnum.None;

        [UserSetting(note: "在第二次 吸引 / 击退 时自动使用亲疏 / 沉稳 以及 打断暗之泛滥")]
        public bool UseAction { get; set; } = false;

        [UserSetting(note: "特殊提醒 不知道是什么绝对不要开")]
        public bool SpecialText { get; set; } = false;

        [UserSetting(note: "是否开启内场地板绘制")]
        public bool DrawTiles { get; set; } = false;

        [UserSetting(note: "是否开启实验性踩塔指路")]
        public bool DrawTowers { get; set; } = false;

        [UserSetting(note: "即使使用防击退也显示指示")]
        public bool AlwaysShowDisplacement { get; set; } = false;

        public enum PartyEnum
        {
            None = -1,
            A = 0,
            B = 1,
            C = 2
        }

        public void Init(ScriptAccessory accessory)
        {
            _codPhase = CodPhase.Init;
            List<ManualResetEvent> _events = Enumerable
                .Range(0, 20)
                .Select(_ => new ManualResetEvent(false))
                .ToList();

            Embrace.Clear();
            DelayWhat = string.Empty;
            HaveLoomingChaos = false;
            SeedTarget.Clear();
            RazingRecord = 0;
            EverDrawPhaser = false;
            accessory.Method.RemoveDraw(".*");
        }

        #region TestRegion

        [ScriptMethod(name: "---- 测试项 ----", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: Debugging)]
        public void SplitLine_TestRegion(Event ev, ScriptAccessory sa)
        {
        }

        [ScriptMethod(name: "测试 我在内场还是外场", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: Debugging)]
        public void LocateAtWhichPlatform(Event ev, ScriptAccessory sa)
        {
            var str = "";
            str += $"{(IsOnInnerPlatform(sa, sa.Data.Me) ? "在内场" : "不在内场")}";
            str += $"{(IsOnSidePlatform(sa, sa.Data.Me) ? "在外场" : "不在外场")}";
            sa.Log.Debug(str);
        }

        [ScriptMethod(name: "测试 我是谁", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: Debugging)]
        public void WhoAmI(Event ev, ScriptAccessory sa)
        {
            var myMemberIdx = GetMemberIdx(sa);
            sa.Log.Debug($"你的身份为，【{_alliance[myMemberIdx / 10]} 队 {_role[myMemberIdx % 10]}】");
        }

        [ScriptMethod(name: "测试 获得内场玩家", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: Debugging)]
        public void PrintInnerPlatformPlayers(Event ev, ScriptAccessory sa)
        {
            var players = GetInnerPlatformPlayers(sa);
            List<string> jobString = ["Tank", "Healer", "Dps"];
            sa.Log.Debug($"====== 内场玩家：======");
            foreach (var player in players)
            {
                sa.Log.Debug(
                    $"{player.Key}, 同组 {player.Value.Item1}, 职能 {jobString[player.Value.Item2]}," +
                    $" {player.Value.Item3}, eid {player.Value.Item4:x8}, 位置 {player.Value.Item5}");
            }
        }

        [ScriptMethod(name: "测试 获得外场玩家", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: Debugging)]
        public void PrintSidePlatformPlayers(Event ev, ScriptAccessory sa)
        {
            var players = GetSidePlatformPlayers(sa);
            List<string> jobString = ["Tank", "Healer", "Dps"];
            sa.Log.Debug($"====== 外场玩家：======");
            foreach (var player in players)
            {
                sa.Log.Debug(
                    $"{player.Key}, 同组 {player.Value.Item1}, 职能 {jobString[player.Value.Item2]}," +
                    $" {player.Value.Item3}, eid {player.Value.Item4:x8}, 位置 {player.Value.Item5}");
            }
        }

        [ScriptMethod(name: "测试 翻转大云可选中状态", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: Debugging)]
        public unsafe void ToggleCloudsTargetable(Event ev, ScriptAccessory sa)
        {
            var cloudCharaEnum = sa.Data.Objects.GetByDataId(0x461e);
            List<IGameObject> cloudCharaList = cloudCharaEnum.ToList();
            sa.Log.Debug($"获得 {cloudCharaList.Count} 个 0x461e 实体，为大云。");
            if (cloudCharaList.Count != 1) return;

            var cloudChara = cloudCharaList[0];
            SetTargetable(sa, cloudChara, !cloudChara.IsTargetable);
            sa.Log.Debug($"已翻转大云可选中状态。");
        }

        [ScriptMethod(name: "测试 翻转小云可选中状态", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: Debugging)]
        public unsafe void ToggleShadowsTargetable(Event ev, ScriptAccessory sa)
        {
            var shadowCharaEnum = sa.Data.Objects.GetByDataId(0x461f);
            List<IGameObject> shadowCharaList = shadowCharaEnum.ToList();
            sa.Log.Debug($"获得 {shadowCharaList.Count} 个 0x461f 实体，为小云。");
            if (shadowCharaList.Count != 2) return;

            foreach (var shadowChara in shadowCharaList)
                SetTargetable(sa, shadowChara, !shadowChara.IsTargetable);
            sa.Log.Debug($"已翻转小云可选中状态。");
        }

        #endregion TestRegion

        #region P1

        [ScriptMethod(name: "---- Phase Diamond ----", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: true)]
        public void SplitLine_Phase2(Event ev, ScriptAccessory sa)
        {
        }

        [ScriptMethod(name: "阶段转换 - 钻石", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40509"],
            userControl: Debugging)]
        public void PhaseChange_P2(Event ev, ScriptAccessory sa)
        {
            _codPhase = CodPhase.Diamond;
            sa.Log.Debug($"当前阶段为：{_codPhase}");
            var partyMemberIdxNew = GetMemberIdx(sa);
            if (_partyMemberIdx != partyMemberIdxNew)
            {
                _partyMemberIdx = partyMemberIdxNew;
                sa.Method.TextInfo($"你的身份为，【{_alliance[partyMemberIdxNew / 10]}队{_role[partyMemberIdxNew % 10]}】，若有误请及时于【用户设置】调整。",
                    5000, false);
            }
        }

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

        [ScriptMethod(name: "AOE 提醒", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(40510|40456)$"])]
        public void DelugeofDarkness(Event @event, ScriptAccessory accessory)
        {
            // Usami: 我把 [40509 暗之泛滥] AOE提醒的部分改成了玩家身份提醒，按理说AOE应该不会忘吧！
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
            if (RazingRecord < 2) dp.DestoryAt = 8000;
            else
            {
                dp.Delay = 4000;
                dp.DestoryAt = 4000;
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Razing-volley Particle Beam 场外车轮激光", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40511"], suppress: 500)]
        public void RazingvolleyParticleBeamRecord(Event @event, ScriptAccessory accessory)
        {
            Task.Delay(200).ContinueWith(t =>
            {
                if (!ParseObjectId(@event["SourceId"], out var sid)) return;
                RazingRecord++;
            });
        }

        [ScriptMethod(name: "Razing-volley Particle Beam Cancel", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40511"], UserControl = false)]
        public void RazingvolleyParticleBeamCancel(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (RazingRecord >= 4) RazingRecord = 0;
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
            dp.Color = accessory.Data.DefaultDangerColor.WithW(0.4f);
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
            if (index == -1) return;

            dp.Name = $"Flare Guide";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Scale = new(1.5f);
            dp.Owner = accessory.Data.Me;
            dp.DestoryAt = 5500;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.TargetPosition = FlarePoint[index];

            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }

        [ScriptMethod(name: "Grim Embrace", eventType: EventTypeEnum.Tether, eventCondition: ["Id:regex:^(012[CD])$"], UserControl = false)]
        public void GrimEmbrace(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            switch (@event["Id"])
            {
                case "012C":
                    Embrace.Add((sid, "Forward"));
                    if (sid == accessory.Data.Me) SendText("存储前方", accessory);
                    break;
                case "012D":
                    Embrace.Add((sid, "Backward"));
                    if (sid == accessory.Data.Me) SendText("存储后方", accessory);
                    break;
            }
        }

        [ScriptMethod(name: "Embrace AOE 放手前后绘制", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:4181"])]
        public void EmbraceAOE(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (Embrace.Count == 0) return;
            var embrace = Embrace.FirstOrDefault(x => x.Item1 == tid);
            if (string.IsNullOrEmpty(embrace.Item2)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Embrace AOE - {tid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(8, 8);
            dp.Owner = tid;
            dp.Delay = int.Parse(@event["DurationMilliseconds"]) - 7000;
            dp.DestoryAt = 7000;

            if (tid != accessory.Data.Me)
            {
                dp.Delay = int.Parse(@event["DurationMilliseconds"]) - 3000;
                dp.DestoryAt = 3000;
            }

            if (embrace.Item2 == "Backward")
                dp.Rotation = float.Pi;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Embrace AOE 放手前后绘制", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:4181"])]
        public void EmbraceAOECancel(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            accessory.Method.RemoveDraw($"Embrace AOE - {tid}");
            Embrace.RemoveAll(x => x.Item1 == tid);
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
            if (UseAction) AutoSCAL(accessory);
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

            Task.Delay(200).ContinueWith(t =>
                {
                    if (HaveMitigation(accessory)) return;
                    dp.Name = "Enaero - Knockback";
                    dp.Color = accessory.Data.DefaultSafeColor;
                    dp.Position = new(100, 0, 76.28f);
                    dp.TargetObject = accessory.Data.Me;
                    dp.Scale = new(1.5f, 21);
                    dp.DestoryAt = 2000;

                    accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                }
            );
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
                    return;
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
                    return;
            }

            Task.Delay(200).ContinueWith(t =>
            {
                if (HaveMitigation(accessory)) return;
                dp.Name = "Endeath - Attract";
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Owner = accessory.Data.Me;
                dp.TargetPosition = new(100, 0, 76.28f);
                dp.Scale = new(1.5f, 15);
                dp.DestoryAt = 2000;

                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            });
        }

        [ScriptMethod(name: "Break IV 背对提醒", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4052[79])$"])]
        public void BreakIV(Event @event, ScriptAccessory accessory)
        {
            SendText("背对", accessory);
            // if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            //
            // var dp = accessory.Data.GetDefaultDrawProperties();
            // dp.Name = $"Break IV - {sid}";
            // dp.Color = accessory.Data.DefaultDangerColor;
            // dp.Scale = new(1);
            // dp.Owner = sid;
            // dp.TargetObject = accessory.Data.Me;
            // dp.DestoryAt = 4000;
            //
            // accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);

            var sid = @event.SourceId;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "BreakEye";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = accessory.Data.Me;
            dp.TargetObject = sid;
            dp.Delay = 0;
            dp.DestoryAt = 4000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.SightAvoid, dp);
        }
        #endregion
        #region P2

        [ScriptMethod(name: "---- Phase Tilt ----", eventType: EventTypeEnum.NpcYell, eventCondition: ["HelloayaWorld"],
            userControl: true)]
        public void SplitLine_Phase3(Event ev, ScriptAccessory sa)
        {
        }

        [ScriptMethod(name: "阶段转换 - 三重", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40449"],
            userControl: Debugging)]
        public void PhaseChange_P3(Event ev, ScriptAccessory sa)
        {
            _codPhase = CodPhase.Tilt;
            sa.Log.Debug($"当前阶段为：{_codPhase}");
            var partyMemberIdxNew = GetMemberIdx(sa);
            if (_partyMemberIdx != partyMemberIdxNew)
            {
                _partyMemberIdx = partyMemberIdxNew;
                sa.Method.TextInfo($"你的身份为，【{_alliance[partyMemberIdxNew / 10]}队{_role[partyMemberIdxNew % 10]}】，若有误请及时于【用户设置】调整。",
                    5000, false);
            }
        }

        [ScriptMethod(name: "阶段转换 - 三重", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40449"], userControl: false)]
        public void RecordOwner(Event @event, ScriptAccessory accessory) => TileInstance.InitOwner(accessory);

        [ScriptMethod(name: "初始位置", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40449"],
        userControl: true)]
        public void P3InitField(Event ev, ScriptAccessory sa)
        {
            var myMemberIdx = GetMemberIdx(sa);
            if (myMemberIdx == -1)
            {
                sa.Log.Debug($"获得MemberIdx错误，可能该目标非我队队员");
                return;
            }

            Vector3 safePos = myMemberIdx switch
            {
                10 => GetBlockField(7, 3),  // B-MT
                11 => GetBlockField(7, 6),  // B-ST
                12 => GetBlockField(6, 2),  // B-H1
                13 => GetBlockField(6, 7),  // B-H2
                14 => GetBlockField(3, 2),  // B-D1
                15 => GetBlockField(3, 7),  // B-D2
                16 => GetBlockField(8, 2),  // B-D3
                17 => GetBlockField(8, 7),  // B-D4

                2 => GetBlockField(1, 2),   // A-H1
                1 => GetBlockField(2, 3),   // A-ST
                22 => GetBlockField(1, 7),  // C-H1
                21 => GetBlockField(2, 6),  // C-ST
                _ => new Vector3(0, 0, 0),
            };

            if (safePos != new Vector3(0, 0, 0))
            {
                var dp0 = sa.Data.GetDefaultDrawProperties();
                dp0.Name = $"方格{myMemberIdx}";
                dp0.Scale = new Vector2(6, 6);
                dp0.Position = safePos;
                dp0.Delay = 0;
                dp0.DestoryAt = 7500;
                dp0.Color = sa.Data.DefaultSafeColor;
                sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Straight, dp0);
            }

            else
            {
                safePos = (myMemberIdx / 10) switch
                {
                    0 => new Vector3(73.5f, 0, 100f),
                    2 => new Vector3(126.5f, 0, 100f),
                    _ => new Vector3(0, 0, 0),
                };
            }

            if (safePos == new Vector3(0, 0, 0)) return;

            var dp = DrawGuidance(sa, safePos, 0, 7500, "初始位置");
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            sa.Log.Debug($"获得{myMemberIdx}的初始位置{safePos}");
        }

        private int GetMemberIdx(ScriptAccessory sa)
        {
            try
            {
                var myParty = Party switch
                {
                    PartyEnum.A => 0,
                    PartyEnum.B => 10,
                    PartyEnum.C => 20,
                    _ => throw new ArgumentOutOfRangeException()
                };
                var myIndex = sa.Data.PartyList.IndexOf(sa.Data.Me);
                if (myIndex == -1) return -1;
                return myParty + myIndex;
            }
            catch { return -1; }
        }

        // 该功能未进行充分测试，先屏蔽。

        // [ScriptMethod(name: "等待小云出现并可选中", eventType: EventTypeEnum.PlayActionTimeline, eventCondition: ["SourceDataId:17951", "Id:7747"],
        //     userControl: false)]
        // public async void P3ShadowTimeline(Event ev, ScriptAccessory sa)
        // {
        //     if (_codPhase != CodPhase.Tilt) return;
        //     sa.Log.Debug($"检测到小云出现 PlayActionTimeline");
        //     await Task.Delay(2000);
        //     _events[0].Set();
        // }

        // [ScriptMethod(name: "根据内外场Buff设置可选中目标 - 1（请与下项一同开启）", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:regex:^(417[78])$"],
        //     userControl: true)]
        // public void P3DistargetableBoss(Event ev, ScriptAccessory sa)
        // {
        //     if (_codPhase != CodPhase.Tilt) return;
        //     if (ev.TargetId != sa.Data.Me) return;
        //     // 4177 inner platform
        //     // 4178 side platform
        //     sa.Log.Debug($"获得状态：{ev.StatusId}");
        //     _events[0].WaitOne();
        //     SetTargetableBoss(sa, ev.StatusId != 4177u, false);
        // }

        // [ScriptMethod(name: "根据内外场Buff设置可选中目标 - 2", eventType: EventTypeEnum.StatusRemove, eventCondition: ["StatusID:regex:^(417[78])$"],
        //     userControl: true)]
        // public void P3TargetableBoss(Event ev, ScriptAccessory sa)
        // {
        //     if (_codPhase != CodPhase.Tilt) return;
        //     if (ev.TargetId != sa.Data.Me) return;
        //     // 4177 inner platform
        //     // 4178 side platform
        //     sa.Log.Debug($"获得状态：{ev.StatusId}");
        //     _events[0].WaitOne();
        //     SetTargetableBoss(sa, ev.StatusId != 4177u, true);
        // }

        private void SetTargetableBoss(ScriptAccessory sa, bool isCloud, bool isTargetable)
        {
            if (isCloud)
            {
                var cloudCharaEnum = sa.Data.Objects.GetByDataId(0x461e);
                List<IGameObject> cloudCharaList = cloudCharaEnum.ToList();
                sa.Log.Debug($"获得 {cloudCharaList.Count} 个 0x461e 实体，为大云。");
                if (cloudCharaList.Count != 1) return;
                var cloudChara = cloudCharaList[0];
                SetTargetable(sa, cloudChara, isTargetable);
            }
            else
            {
                var shadowCharaEnum = sa.Data.Objects.GetByDataId(0x461f);
                List<IGameObject> shadowCharaList = shadowCharaEnum.ToList();
                sa.Log.Debug($"获得 {shadowCharaList.Count} 个 0x461f 实体，为小云。");
                if (shadowCharaList.Count != 2) return;

                foreach (var shadowChara in shadowCharaList)
                    SetTargetable(sa, shadowChara, isTargetable);
            }
        }

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
            dp.Radian = 7.5f.DegToRad();
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
            dp.Delay = 6000;
            dp.DestoryAt = 4000;

            var index = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
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

                    if (!EnableGuidance || !IsInSameSide(accessory, tid)) return;
                    dp = accessory.Data.GetDefaultDrawProperties();
                    dp.Name = $"Third Art Of Darkness - {tid}";
                    dp.Color = accessory.Data.DefaultDangerColor;
                    dp.Owner = tid;
                    dp.Delay = 6000;
                    dp.DestoryAt = 4000;
                    if (!HaveLoomingChaos)
                    {
                        dp.Name = $"Third Art Of Darkness - {index}";
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.Scale = new(1.5f, 5);
                        if (Party == PartyEnum.A)
                        {
                            dp.Position = CenterA;
                            if (index == 0 || index == 3) dp.TargetPosition = new(CenterA.X - 5, CenterA.Y, CenterA.Z);
                            else if (index == 4 || index == 6) dp.TargetPosition = new(CenterA.X, CenterA.Y, CenterA.Z + 5);
                            else if (index == 5 || index == 7) dp.TargetPosition = new(CenterA.X, CenterA.Y, CenterA.Z - 5);
                            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                        }
                        else if (Party == PartyEnum.C)
                        {
                            dp.Position = CenterC;
                            if (index == 0 || index == 3) dp.TargetPosition = new(CenterC.X + 5, CenterC.Y, CenterC.Z);
                            else if (index == 4 || index == 6) dp.TargetPosition = new(CenterC.X, CenterA.Y, CenterC.Z - 5);
                            else if (index == 5 || index == 7) dp.TargetPosition = new(CenterC.X, CenterC.Y, CenterC.Z + 5);
                            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                        }
                    }
                    if (HaveLoomingChaos && index >= 5) // Exchange
                    {
                        dp.Name = $"Third Art Of Darkness - {index}";
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.Scale = new(1.5f, 5);
                        if (Party == PartyEnum.A)
                        {
                            dp.Position = CenterC;
                            if (index == 5) dp.TargetPosition = new(CenterC.X, CenterA.Y, CenterC.Z - 5);
                            else if (index == 6) dp.TargetPosition = new(CenterC.X + 5, CenterC.Y, CenterC.Z);
                            else if (index == 7) dp.TargetPosition = new(CenterC.X, CenterC.Y, CenterC.Z + 5);
                            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                        }
                        else if (Party == PartyEnum.C)
                        {
                            dp.Position = CenterA;
                            if (index == 5) dp.TargetPosition = new(CenterA.X, CenterA.Y, CenterA.Z + 5);
                            else if (index == 6) dp.TargetPosition = new(CenterA.X - 5, CenterA.Y, CenterA.Z);
                            else if (index == 7) dp.TargetPosition = new(CenterA.X, CenterA.Y, CenterA.Z - 5);
                            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                        }
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

                    if (!EnableGuidance || !IsInSameSide(accessory, tid)) return;
                    var priority = new int[] { 2, -1, -1, 3, 0, 5, 1, 4 };
                    if (priority[index] != -1)
                    {
                        dp = accessory.Data.GetDefaultDrawProperties();
                        dp.Name = $"Third Art Of Darkness - {tid}";
                        dp.Color = accessory.Data.DefaultDangerColor;
                        dp.Owner = tid;
                        dp.Delay = 6000;
                        dp.DestoryAt = 4000;
                        dp.Name = $"Third Art Of Darkness - {priority[index]}";
                        dp.Color = accessory.Data.DefaultSafeColor;
                        dp.Scale = new(1.5f, 5);
                        if (Party == PartyEnum.A)
                            dp.TargetPosition = SpreadPointA[priority[index]];
                        else if (Party == PartyEnum.C)
                            dp.TargetPosition = SpreadPointC[priority[index]];
                        accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
                    }

                    break;
            }
        }

        [ScriptMethod(name: "Evil Seed Prepared Position 场内放种子前就位",
            eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40490"],
            suppress: 10000, userControl: true)]
        public void EvilSeedPreparedPosition(Event ev, ScriptAccessory sa)
        {
            if (_codPhase != CodPhase.Tilt) return;

            // suppress 10000，该技能为场边两只小云的读条，避免执行两次
            var myMemberIdx = GetMemberIdx(sa);
            if (myMemberIdx == -1)
            {
                sa.Log.Debug($"获得MemberIdx错误，可能该目标非我队队员");
                return;
            }

            // 获得玩家状态，是否带有InnerDarkness状态
            if (!IsOnInnerPlatform(sa, sa.Data.Me))
            {
                sa.Log.Debug($"玩家不在场中平台上，真可怜！");
                return;
            }

            Vector3 readyPos = myMemberIdx switch
            {
                10 => GetBlockField(7, 3),  // B-MT
                11 => GetBlockField(7, 6),  // B-ST
                12 => GetBlockField(7, 1),  // B-H1
                13 => GetBlockField(7, 8),  // B-H2
                14 => GetBlockField(2, 1),  // B-D1
                15 => GetBlockField(2, 8),  // B-D2
                16 => GetBlockField(8, 2),  // B-D3
                17 => GetBlockField(8, 7),  // B-D4

                2 => GetBlockField(1, 2),   // A-H1
                1 => GetBlockField(2, 3),   // A-ST
                22 => GetBlockField(1, 7),  // C-H1
                21 => GetBlockField(2, 6),  // C-ST
                _ => new Vector3(0, 0, 0),
            };

            sa.Method.TextInfo("放种子前就位", 4000, false);

            if (readyPos == new Vector3(0, 0, 0))
            {
                sa.Log.Debug($"不应在场内的玩家却出现在了场内，真可怜！");
                return;
            }

            // 画方格
            var dp0 = sa.Data.GetDefaultDrawProperties();
            dp0.Name = $"方格{myMemberIdx}";
            dp0.Scale = new Vector2(6, 6);
            dp0.Position = readyPos;
            dp0.Delay = 0;
            dp0.DestoryAt = 7000;
            dp0.Color = sa.Data.DefaultSafeColor;
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Straight, dp0);

            // 画指路线
            var dp = DrawGuidance(sa, readyPos, 0, 7000, "放种子就位位置");
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            sa.Log.Debug($"获得{myMemberIdx}的放种子就位位置{readyPos}");
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
            lock (SeedLock)
            {
                if (Party == PartyEnum.None || Party == PartyEnum.B) return; // No support for B Party
                if (IsInSameParty(accessory, tid)) SeedTarget.Add(tid); // Me included of course
                if (SeedTarget.Count != 2) return;
                if (!SeedTarget.Contains(accessory.Data.Me)) return; // Not in the list
                var myIndex = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                var otherIndex = accessory.Data.PartyList.IndexOf(SeedTarget.First(x => x != accessory.Data.Me));
                var offset = (int)Party + (myIndex < otherIndex ? 1 : 2); // A, C Party Index at 1, 2, 3, 4

                dp.Name = $"Evil Seed Guide";
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.Scale = new(1.5f);
                dp.Owner = accessory.Data.Me;
                dp.DestoryAt = 8000;
                dp.ScaleMode |= ScaleMode.YByDistance;
                dp.TargetPosition = SeedPoint[offset];

                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            }
        }

        [ScriptMethod(name: "Evil Seed Tether 拉线站位", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40492"])]
        public void EvilSeedTether(Event @event, ScriptAccessory accessory)
        {
            if (SpecialText) SendText("右上接种子", accessory);
            if (!EnableGuidance) return;
            if (Party == PartyEnum.None || Party == PartyEnum.B) return; // No support for B Party
            var index = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (index == 1 || index == 2) return; // ST and H1 inner platform

            var priority = new int[] { -1, -1, -1, 0, -1, 1, 2, 3 };
            if (priority[index] == -1) return;

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"Evil Seed Tether Guide";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.DestoryAt = 8000;
            dp.ScaleMode |= ScaleMode.YByDistance;
            if (Party == PartyEnum.A)
                dp.TargetPosition = TetherPointA[priority[index]];
            else if (Party == PartyEnum.C)
                dp.TargetPosition = TetherPointC[priority[index]];

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

        [ScriptMethod(name: "Flood of Darkness 暗之泛滥", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40503"])]
        public void FloodOfDarkness(Event @event, ScriptAccessory accessory)
        {
            if (!UseAction) return;
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!IsInSameSide(accessory, sid)) return;
            if (Party == PartyEnum.None || Party == PartyEnum.B) return;

            var index = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
            if (index != 0 && index != 6) return;
            AutoInterrupt(accessory, sid);
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

        [ScriptMethod(name: "Pivot Particle Beam 内场回旋式波动炮就位提示", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4046[79])$"],
            userControl: true)]
        public void P3PivotBeamGuidance(Event ev, ScriptAccessory sa)
        {
            if (_codPhase != CodPhase.Tilt) return;
            // 判断自身是否在内场
            if (!IsOnInnerPlatform(sa, sa.Data.Me)) return;

            var myMemberIdx = GetMemberIdx(sa);
            var bottomLeftSafe = ev.ActionId == 40467;  // clockwise 40467 顺时针
            List<Vector3> pos = myMemberIdx switch
            {
                // UP LEFT
                2 => bottomLeftSafe ? [GetBlockField(2, 6), GetBlockField(1, 2)] : [GetBlockField(1, 2), GetBlockField(1, 2)], // AH1
                1 => bottomLeftSafe ? [GetBlockField(2, 7), GetBlockField(2, 3)] : [GetBlockField(2, 1), GetBlockField(2, 3)], // AST
                14 => bottomLeftSafe ? [GetBlockField(2, 5), GetBlockField(3, 2)] : [GetBlockField(3, 2), GetBlockField(3, 2)], // BD1

                // BOTTOM RIGHT
                13 => bottomLeftSafe ? [GetBlockField(7, 3), GetBlockField(6, 7)] : [GetBlockField(6, 7), GetBlockField(6, 7)], // BH2
                11 => bottomLeftSafe ? [GetBlockField(7, 2), GetBlockField(7, 6)] : [GetBlockField(7, 8), GetBlockField(7, 6)], // BST
                17 => bottomLeftSafe ? [GetBlockField(7, 4), GetBlockField(8, 7)] : [GetBlockField(8, 7), GetBlockField(8, 7)], // BD4

                // UP RIGHT
                22 => bottomLeftSafe ? [GetBlockField(1, 7), GetBlockField(1, 7)] : [GetBlockField(2, 3), GetBlockField(1, 7)], // CH1
                21 => bottomLeftSafe ? [GetBlockField(2, 8), GetBlockField(2, 6)] : [GetBlockField(2, 2), GetBlockField(2, 6)], // CST
                15 => bottomLeftSafe ? [GetBlockField(3, 7), GetBlockField(3, 7)] : [GetBlockField(2, 4), GetBlockField(3, 7)], // BD2

                // BOTTOM LEFT
                12 => bottomLeftSafe ? [GetBlockField(6, 2), GetBlockField(6, 2)] : [GetBlockField(7, 6), GetBlockField(6, 2)], // BH1
                10 => bottomLeftSafe ? [GetBlockField(7, 1), GetBlockField(7, 3)] : [GetBlockField(7, 7), GetBlockField(7, 3)], // BMT
                16 => bottomLeftSafe ? [GetBlockField(8, 2), GetBlockField(8, 2)] : [GetBlockField(7, 5), GetBlockField(8, 2)], // BD3

                _ => []
            };

            if (pos.Count == 0)
            {
                sa.Log.Debug($"不属于场内人员却出现在了场内，需灵性处理，不作指路。");
                return;
            }

            // 第一轮绘图，就位位置
            // 画方格
            var dp0 = sa.Data.GetDefaultDrawProperties();
            dp0.Name = $"方格{myMemberIdx}";
            dp0.Scale = new Vector2(6, 6);
            dp0.Position = pos[0];
            dp0.Delay = 0;
            dp0.DestoryAt = 14500;
            dp0.Color = sa.Data.DefaultSafeColor;
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Straight, dp0);

            // 画指路线
            var dp = DrawGuidance(sa, pos[0], 0, 14500, "旋转波动炮就位位置");
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
            sa.Log.Debug($"获得波动炮{(bottomLeftSafe ? "左下/右上安全" : "右下/左上安全")}的就位位置{pos[0]}");

            // 第二轮绘图，就位位置
            // 画方格
            var dp01 = sa.Data.GetDefaultDrawProperties();
            dp01.Name = $"方格{myMemberIdx}";
            dp01.Scale = new Vector2(6, 6);
            dp01.Position = pos[1];
            dp01.Delay = 21000;
            dp01.DestoryAt = 7000;
            dp01.Color = sa.Data.DefaultSafeColor;
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Straight, dp01);

            // 画指路线
            var dp1 = DrawGuidance(sa, pos[1], 21000, 7000, "旋转波动炮返回位置");
            sa.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp1);
            sa.Log.Debug($"获得波动炮{(bottomLeftSafe ? "左下/右上安全" : "右下/左上安全")}的返回位置{pos[1]}");

        }

        [ScriptMethod(name: "Chaos Condensed Particle Beam", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40461"])]
        public void ChaosCondensedParticleBeam(Event @event, ScriptAccessory accessory)
        {
            SendText("大云直线挡枪分摊", accessory);
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
            var rot = @event.SourceRotation;
            dp.Name = $"Phaser AOE - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(23);
            dp.Owner = sid;
            dp.Radian = 60f.DegToRad(); // Need more testing
            if (!EverDrawPhaser)
            {
                dp.DestoryAt = 8000;
            }
            else
            {
                dp.Delay = 7000;
                dp.DestoryAt = 3000;
            }

            Task.Delay(200).ContinueWith(t =>
            {
                EverDrawPhaser = true;
            });

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Phaser AOE Cancel", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:40497"], UserControl = false)]
        public void PhaserAOECancel(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (EverDrawPhaser) EverDrawPhaser = false;
            accessory.Method.RemoveDraw($"Phaser AOE - {sid}");
        }

        [ScriptMethod(name: "Active Pivot Particle Beam 90度前后炮", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4046[79])$"])]
        public void ActivePivotParticleBeam(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            bool isOnInnerPlatform = IsOnInnerPlatform(accessory, accessory.Data.Me);

            var dp = accessory.Data.GetDefaultDrawProperties();
            var rot = -float.Pi / 2;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Scale = new(18, 80);
            dp.Owner = sid;
            dp.Delay = isOnInnerPlatform ? 0 : 10000;

            var change = @event["ActionId"] == "40467" ? -1 : 1;
            for (var i = 0; i < 5; i++)
            {
                dp.Name = $"Active Pivot Particle Beam - {i}";
                dp.Rotation = i * change * float.Pi * 0.125f + rot;
                dp.FixRotation = true;
                dp.DestoryAt = 4500 + i * 1500 + (isOnInnerPlatform ? 10000 : 0);

                accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Straight, dp);
            }
        }

        [ScriptMethod(name: "Looming Chaos", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41673"])]
        public void LoomingChaos(Event @event, ScriptAccessory accessory) => SendText("准备换位", accessory);

        [ScriptMethod(name: "Looming Chaos", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:41673"], userControl: false)]
        public void LoomingChaosMark(Event @event, ScriptAccessory accessory) => HaveLoomingChaos = true;

        [ScriptMethod(name: "EnvControl 杂烩", eventType: EventTypeEnum.EnvControl)]
        public void EnvControl(Event @event, ScriptAccessory accessory)
        {
            try
            {
                var Index = int.Parse(@event["Index"]);
                var Flag = int.Parse(@event["Flag"]);

                if (DrawTiles)
                {
                    if (accessory.Data.MyObject.HasStatus(4177))
                    {

                        if (Index < 3 || Index > 30) return; // Between 0x03 - 0x1E
                                                             // Flags: Init 2048 Occupied 32 Free 512 Danger 128 Break 8
                        if (Flag == 512 || Flag == 8) TileInstance.CancelDraw(Index, accessory);
                        if (Flag == 32) TileInstance.StartDraw(Index, accessory);
                        if (Flag == 128) TileInstance.StartDraw(Index, accessory, true);
                    }
                }
                if (DrawTowers)
                {
                    if (accessory.Data.MyObject.HasStatus(4178))
                    {
                        if (Index < 0x3F || Index > 0x46) return; // Between 63 - 70 4 Active Towers
                                                                  // Flags Appear 2 Disappear 8
                        if (Flag == 8) TowerInstance.CancelDraw(Index, accessory);
                        if (Flag == 2)
                        {
                            if (TowerInstance.IsMyTower(accessory, Index, Party, HaveLoomingChaos))
                                TowerInstance.StartDraw(Index, accessory);
                        }
                    }
                }
            }
            catch (Exception)
            {
                // ignore
            }
        }

        [ScriptMethod(name: "Cancel Tiles", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40455"], userControl: false)]
        public void CancelTiles(Event @event, ScriptAccessory accessory) => TileInstance.CancelAll(accessory);


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

        private void DisableGuide()
        {
            SpecialText = false;
            EnableGuidance = false;
            UseAction = false;
        }

        private void AutoSCAL(ScriptAccessory accessory)
        {
            // Sure Cast 7559 Arm's Length 7548
            var JobId = accessory.Data.MyObject.ClassJob.Value.ClassJobCategory.RowId;
            // 30 War 31 Magic
            if (JobId == 0) return;
            if (JobId == 30) accessory.Method.UseAction(0xE000_0000, 7548);
            else if (JobId == 31) accessory.Method.UseAction(0xE000_0000, 7559);
            else return;
        }

        private bool HaveMitigation(ScriptAccessory accessory)
        {
            if (AlwaysShowDisplacement) return false;
            return accessory.Data.MyObject.HasStatusAny(new uint[] { 160, 1209 });
        }

        private void AutoInterrupt(ScriptAccessory accessory, uint target)
        {
            // Head Graze 7551 Ranged Interject 7538 Tank
            var JobId = accessory.Data.MyObject.ClassJob.RowId;
            if (JobId == 0) return;

            var RangedId = new List<uint> { 31, 23, 38 };
            var TankId = new List<uint> { 19, 21, 32, 37 };
            if (RangedId.Contains(JobId)) accessory.Method.UseAction(target, 7551);
            else if (TankId.Contains(JobId)) accessory.Method.UseAction(target, 7538);
            else return;
        }

        private bool IsInSameSide(ScriptAccessory accessory, ulong tid)
        {
            var myPosition = accessory.Data.MyObject.Position;
            var targetPosition = accessory.Data.Objects.SearchById(tid).Position;
            var threshold = 20;
            return Vector3.Distance(myPosition, targetPosition) < threshold;
        }

        /// <summary>
        /// 获得P3场地第row排第col列的中心坐标
        /// </summary>
        /// <param name="row"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private Vector3 GetBlockField(int row, int col)
        {
            Vector3 centerTriple = new(100, 0, 100);
            return centerTriple + new Vector3(6 * (col - 4) - 3, 0, 6 * (row - 4) - 3);
        }

        /// <summary>
        /// 输入实体id，判断是否带有内场Buff，判断在内场
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="entityId"></param>
        /// <param name="innerPlatform">在内场</param>
        /// <returns></returns>
        private bool IsOnWhichPlatform(ScriptAccessory sa, ulong entityId, bool innerPlatform = true)
        {
            IPlayerCharacter? chara = (IPlayerCharacter?)sa.Data.Objects.SearchById(entityId);
            if (chara == null)
            {
                sa.Log.Error($"输入的entityId {entityId} 未找到实例");
                return false;
            }
            return chara.HasStatus(innerPlatform ? 4177u : 4178u);
        }

        private bool IsOnInnerPlatform(ScriptAccessory sa, ulong entityId) => IsOnWhichPlatform(sa, entityId, true);
        private bool IsOnSidePlatform(ScriptAccessory sa, ulong entityId) => IsOnWhichPlatform(sa, entityId, false);

        /// <summary>
        /// 获得对应场地玩家字典
        /// Key: EntityId
        /// Value: (bool sameParty, int job(0:Tank, 1:Healer, 2:Dps, -1:Unknown), string name)
        /// </summary>
        /// <param name="sa"></param>
        /// <param name="innerPlatform">是否是内场玩家</param>
        /// <returns></returns>
        private unsafe Dictionary<int, (bool, int, string, ulong, Vector3)> GetPlatformPlayers(ScriptAccessory sa, bool innerPlatform = true)
        {
            var innerPlayersDict = new Dictionary<int, (bool sameParty, int job, string name, ulong eid, Vector3 pos)>();
            // 先找本队
            for (int i = 0; i < sa.Data.PartyList.Count; i++)
            {
                // 不用foreach，避免/pdr leaveduty，导致IndexOutOfRange的崩游戏（？？？？？？）
                var entityId = sa.Data.PartyList[i];
                var chara = (IPlayerCharacter?)sa.Data.Objects.SearchById(entityId);
                if (chara == null) continue;
                if (innerPlatform ? !IsOnInnerPlatform(sa, entityId) : !IsOnSidePlatform(sa, entityId)) continue;

                var job = chara.IsTank() ? 0 :
                    chara.IsHealer() ? 1 :
                    chara.IsDps() ? 2 :
                    -1;

                // Dict的Key，为了与MemberIdx区分，+100
                innerPlayersDict.Add(i + 100, (true, job, chara.Name.ToString(), entityId, chara.Position));
            }

            // 再找团队
            var group = GroupManager.Instance()->GetGroup(ReplayGroup);
            for (var index = 0; index <= 1; index++)
            {
                for (var j = 0; j < 8; j++)
                {
                    var entityId = group->GetAllianceMemberByGroupAndIndex(index, j)->EntityId;
                    var chara = (IPlayerCharacter?)sa.Data.Objects.SearchById(entityId);
                    if (chara == null) continue;
                    if (innerPlatform ? !IsOnInnerPlatform(sa, entityId) : !IsOnSidePlatform(sa, entityId)) continue;

                    var job = chara.IsTank() ? 0 :
                        chara.IsHealer() ? 1 :
                        chara.IsDps() ? 2 :
                        -1;

                    innerPlayersDict.Add(j + 10 * (index + 1) + 100,
                        (false, job, chara.Name.ToString(), entityId, chara.Position));
                }
            }
            return innerPlayersDict;
        }

        private Dictionary<int, (bool, int, string, ulong, Vector3)> GetInnerPlatformPlayers(ScriptAccessory sa) => GetPlatformPlayers(sa, true);
        private Dictionary<int, (bool, int, string, ulong, Vector3)> GetSidePlatformPlayers(ScriptAccessory sa) => GetPlatformPlayers(sa, false);

        /// <summary>
        /// 返回箭头指引相关dp
        /// </summary>
        /// <param name="accessory"></param>
        /// <param name="ownerObj">箭头起始，可输入uint或Vector3</param>
        /// <param name="targetObj">箭头指向目标，可输入uint或Vector3，为0则无目标</param>
        /// <param name="delay">绘图出现延时</param>
        /// <param name="destroy">绘图消失时间</param>
        /// <param name="name">绘图名称</param>
        /// <param name="rotation">箭头旋转角度</param>
        /// <param name="scale">箭头宽度</param>
        /// <param name="isSafe">使用安全色</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static DrawPropertiesEdit DrawGuidance(ScriptAccessory accessory,
            object ownerObj, object targetObj, int delay, int destroy, string name, float rotation = 0, float scale = 1f, bool isSafe = true)
        {
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = name;
            dp.Scale = new Vector2(scale);
            dp.Rotation = rotation;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.Color = isSafe ? accessory.Data.DefaultSafeColor : accessory.Data.DefaultDangerColor;
            dp.Delay = delay;
            dp.DestoryAt = destroy;

            if (ownerObj is uint or ulong)
            {
                dp.Owner = (ulong)ownerObj;
            }
            else if (ownerObj is Vector3 spos)
            {
                dp.Position = spos;
            }
            else
            {
                throw new ArgumentException("ownerObj的目标类型输入错误");
            }

            if (targetObj is uint or ulong)
            {
                if ((ulong)targetObj != 0) dp.TargetObject = (ulong)targetObj;
            }
            else if (targetObj is Vector3 tpos)
            {
                dp.TargetPosition = tpos;
            }
            else
            {
                throw new ArgumentException("targetObj的目标类型输入错误");
            }

            return dp;
        }

        public static DrawPropertiesEdit DrawGuidance(ScriptAccessory accessory,
            object targetObj, int delay, int destroy, string name, float rotation = 0, float scale = 1f, bool isSafe = true)
            => DrawGuidance(accessory, (ulong)accessory.Data.Me, targetObj, delay, destroy, name, rotation, scale, isSafe);

        public class PriorityDict
        {
            // ReSharper disable once NullableWarningSuppressionIsUsed
            public ScriptAccessory sa { get; set; } = null!;

            // ReSharper disable once NullableWarningSuppressionIsUsed
            public Dictionary<int, int> Priorities { get; set; } = null!;
            public string Annotation { get; set; } = "";
            public int ActionCount { get; set; } = 0;

            public void Init(ScriptAccessory accessory, string annotation, int partyNum = 8)
            {
                sa = accessory;
                Priorities = new Dictionary<int, int>();
                for (var i = 0; i < partyNum; i++)
                {
                    Priorities.Add(i, 0);
                }

                Annotation = annotation;
                ActionCount = 0;
            }

            /// <summary>
            /// 为特定Key增加优先级
            /// </summary>
            /// <param name="idx">key</param>
            /// <param name="priority">优先级数值</param>
            public void AddPriority(int idx, int priority)
            {
                Priorities[idx] += priority;
            }

            /// <summary>
            /// 从Priorities中找到前num个数值最小的，得到新的Dict返回
            /// </summary>
            /// <param name="num"></param>
            /// <returns></returns>
            public List<KeyValuePair<int, int>> SelectSmallPriorityIndices(int num)
            {
                return SelectMiddlePriorityIndices(0, num);
            }

            /// <summary>
            /// 从Priorities中找到前num个数值最大的，得到新的Dict返回
            /// </summary>
            /// <param name="num"></param>
            /// <returns></returns>
            public List<KeyValuePair<int, int>> SelectLargePriorityIndices(int num)
            {
                return SelectMiddlePriorityIndices(0, num, true);
            }

            /// <summary>
            /// 从Priorities中找到升序排列中间的数值，得到新的Dict返回
            /// </summary>
            /// <param name="skip">跳过skip个元素。若从第二个开始取，skip=1</param>
            /// <param name="num"></param>
            /// <param name="descending">降序排列，默认为false</param>
            /// <returns></returns>
            public List<KeyValuePair<int, int>> SelectMiddlePriorityIndices(int skip, int num, bool descending = false)
            {
                if (Priorities.Count < skip + num)
                    return new List<KeyValuePair<int, int>>();

                IEnumerable<KeyValuePair<int, int>> sortedPriorities;
                if (descending)
                {
                    // 根据值从大到小降序排序，并取前num个键
                    sortedPriorities = Priorities
                        .OrderByDescending(pair => pair.Value) // 先根据值排列
                        .ThenBy(pair => pair.Key) // 再根据键排列
                        .Skip(skip) // 跳过前skip个元素
                        .Take(num); // 取前num个键值对
                }
                else
                {
                    // 根据值从小到大升序排序，并取前num个键
                    sortedPriorities = Priorities
                        .OrderBy(pair => pair.Value) // 先根据值排列
                        .ThenBy(pair => pair.Key) // 再根据键排列
                        .Skip(skip) // 跳过前skip个元素
                        .Take(num); // 取前num个键值对
                }

                return sortedPriorities.ToList();
            }

            /// <summary>
            /// 从Priorities中找到升序排列第idx位的数据，得到新的Dict返回
            /// </summary>
            /// <param name="idx"></param>
            /// <param name="descending">降序排列，默认为false</param>
            /// <returns></returns>
            public KeyValuePair<int, int> SelectSpecificPriorityIndex(int idx, bool descending = false)
            {
                var sortedPriorities = SelectMiddlePriorityIndices(0, 8, descending);
                return sortedPriorities[idx];
            }

            /// <summary>
            /// 从Priorities中找到对应key的数据，得到其Value排序后位置返回
            /// </summary>
            /// <param name="key"></param>
            /// <param name="descending">降序排列，默认为false</param>
            /// <returns></returns>
            public int FindPriorityIndexOfKey(int key, bool descending = false)
            {
                var sortedPriorities = SelectMiddlePriorityIndices(0, 8, descending);
                var i = 0;
                foreach (var dict in sortedPriorities)
                {
                    if (dict.Key == key) return i;
                    i++;
                }

                return i;
            }

            /// <summary>
            /// 一次性增加优先级数值
            /// 通常适用于特殊优先级（如H-T-D-H）
            /// </summary>
            /// <param name="priorities"></param>
            public void AddPriorities(List<int> priorities)
            {
                if (Priorities.Count != priorities.Count)
                    throw new ArgumentException("输入的列表与内部设置长度不同");

                for (var i = 0; i < Priorities.Count; i++)
                    AddPriority(i, priorities[i]);
            }

            /// <summary>
            /// 输出优先级字典的Key与优先级
            /// </summary>
            /// <returns></returns>
            public string ShowPriorities(bool showJob = true)
            {
                var str = $"{Annotation} 优先级字典：\n";
                foreach (var pair in Priorities)
                {
                    str += $"Key {pair.Key} {(showJob ? $"({_role[pair.Key]})" : "")}, Value {pair.Value}\n";
                }

                return str;
            }
            public PriorityDict DeepCopy()
            {
                return JsonConvert.DeserializeObject<PriorityDict>(JsonConvert.SerializeObject(this)) ??
                       new PriorityDict();
            }

        }

        public unsafe static void SetTargetable(ScriptAccessory sa, IGameObject? obj, bool targetable)
        {
            if (obj == null || !obj.IsValid())
            {
                sa.Log.Error($"传入的IGameObject不合法。");
                return;
            }

            GameObject* charaStruct = (GameObject*)obj.Address;
            if (targetable)
            {
                if (obj.IsDead || obj.IsTargetable) return;
                charaStruct->TargetableStatus |= ObjectTargetableFlags.IsTargetable;
            }
            else
            {
                if (!obj.IsTargetable) return;
                charaStruct->TargetableStatus &= ~ObjectTargetableFlags.IsTargetable;
            }
            sa.Log.Debug($"SetTargetable {targetable} => {obj.Name} {obj}");
        }

        // Courtesy of BMR
        public class Tiles
        {
            // - index arrangement:
            //      04             0B
            //   03 05 06 07 0E 0D 0C 0A
            //      08             0F
            //      09             10
            //      17             1E
            //      16             1D
            //   11 13 14 15 1C 1B 1A 18
            //      12             19
            // From 03 - 1E, total of 28 tiles

            private readonly Dictionary<int, (int x, int y)> _cellIndexToCoordinates = GenerateCellIndexToCoordinates();
            private uint BossId = 0;

            public void InitOwner(ScriptAccessory accessory) => BossId = accessory.Data.Objects.GetByDataId(0x461e).FirstOrDefault()?.EntityId ?? 0;

            private static int CellIndex(int x, int y) => (x, y) switch
            {
                (-4, -3) => 0x03,
                (-3, -4) => 0x04,
                (-3, -3) => 0x05,
                (-2, -3) => 0x06,
                (-1, -3) => 0x07,
                (-3, -2) => 0x08,
                (-3, -1) => 0x09,
                (+3, -3) => 0x0A,
                (+2, -4) => 0x0B,
                (+2, -3) => 0x0C,
                (+1, -3) => 0x0D,
                (+0, -3) => 0x0E,
                (+2, -2) => 0x0F,
                (+2, -1) => 0x10,
                (-4, +2) => 0x11,
                (-3, +3) => 0x12,
                (-3, +2) => 0x13,
                (-2, +2) => 0x14,
                (-1, +2) => 0x15,
                (-3, +1) => 0x16,
                (-3, +0) => 0x17,
                (+3, +2) => 0x18,
                (+2, +3) => 0x19,
                (+2, +2) => 0x1A,
                (+1, +2) => 0x1B,
                (+0, +2) => 0x1C,
                (+2, +1) => 0x1D,
                (+2, +0) => 0x1E,
                _ => 0
            };

            private static Dictionary<int, (int x, int y)> GenerateCellIndexToCoordinates()
            {
                var map = new Dictionary<int, (int x, int y)>();
                for (var x = -4; x <= 3; ++x)
                {
                    for (var y = -4; y <= 3; ++y)
                    {
                        var index = CellIndex(x, y);
                        if (index >= 0)
                            map[index] = (x, y);
                    }
                }
                return map;
            }

            private Vector3 CellCenter(int breakTimeIndex)
            {
                // var cellIndex = breakTimeIndex + 3; // We use it as-is.
                var cellIndex = breakTimeIndex;
                if (_cellIndexToCoordinates.TryGetValue(cellIndex, out var coordinates))
                {
                    var worldX = (coordinates.x + 0.5f) * 6f;
                    var worldZ = (coordinates.y + 0.5f) * 6f;
                    return new Vector3(100 + worldX, 0, 100 + worldZ);
                }
                else
                    return default;
            }

            private bool NeedToDraw(ScriptAccessory accessory, Vector3 target) => Vector3.Distance(accessory.Data.MyObject.Position, target) < 20f;

            public void StartDraw(int index, ScriptAccessory accessory, bool isDanger = false)
            {
                if (!NeedToDraw(accessory, CellCenter(index))) return;
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = $"Tiles - {index}";
                dp.Color = new Vector4(1.0f, 1.0f, 0.0f, 1.0f);
                if (isDanger) dp.Color = accessory.Data.DefaultDangerColor;
                dp.ScaleMode = ScaleMode.ByTime;
                dp.Scale = new(6, 6);
                dp.Owner = BossId;
                dp.DestoryAt = 38 * 1000;
                if (isDanger) dp.DestoryAt = 6000;
                dp.Position = CellCenter(index);
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Straight, dp);
            }

            public void CancelDraw(int index, ScriptAccessory accessory) => accessory.Method.RemoveDraw($"Tiles - {index}");

            public void CancelAll(ScriptAccessory accessory) => accessory.Method.RemoveDraw($"Tiles - .*");

        }

        // Also BMR
        public class Towers
        {
            // 3-man only
            // - arrangement:
            //     3F         43
            //   42  40     44  46
            //     41         45

            private readonly List<int> LeftSideTower = [0x3F, 0x40, 0x41, 0x42];
            private readonly List<int> RightSideTower = [0x43, 0x44, 0x45, 0x46];
            private readonly List<int> LeftMTGroup = [0x41, 0x42];
            private readonly List<int> RightMTGroup = [0x43, 0x46];
            private readonly List<int> LeftSTGroup = [0x3F, 0x40];
            private readonly List<int> RightSTGroup = [0x44, 0x45];

            private Vector3 TowerCenter(int index)
            {
                var offset = index switch
                {
                    0x3F => new Vector3(-26.5f, 0, -4.5f),
                    0x40 => new Vector3(-22f, 0, 0f),
                    0x41 => new Vector3(-26.5f, 0, 4.5f),
                    0x42 => new Vector3(-31f, 0, 0f),
                    0x43 => new Vector3(26.5f, 0, -4.5f),
                    0x44 => new Vector3(22f, 0, 0f),
                    0x45 => new Vector3(26.5f, 0, 4.5f),
                    0x46 => new Vector3(31f, 0, 0f),
                    _ => Vector3.Zero,
                };

                return new Vector3(100 + offset.X, 0, 100 + offset.Z);
            }

            public bool IsMyTower(ScriptAccessory accessory, int index, PartyEnum party, bool afterSwap)
            {
                if (party == PartyEnum.B || party == PartyEnum.None) return false;
                if ((party == PartyEnum.A && !afterSwap) ||
                    (party == PartyEnum.C && afterSwap))
                {
                    if (!LeftSideTower.Contains(index)) return false;
                    var idx = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                    if (idx < 5 && afterSwap) return false;
                    if (!afterSwap)
                    {
                        if (idx % 2 == 0 && idx != 2) return LeftMTGroup.Contains(index);
                        if (idx % 2 == 1 && idx != 3) return LeftSTGroup.Contains(index);
                    }
                    return LeftSTGroup.Contains(index);
                }

                if ((party == PartyEnum.A && afterSwap) ||
                    (party == PartyEnum.C && !afterSwap))
                {
                    if (!RightSideTower.Contains(index)) return false;
                    var idx = accessory.Data.PartyList.IndexOf(accessory.Data.Me);
                    if (idx < 5 && afterSwap) return false;
                    if (!afterSwap)
                    {
                        if (idx % 2 == 0 && idx != 2) return RightMTGroup.Contains(index);
                        if (idx % 2 == 1 && idx != 3) return RightSTGroup.Contains(index);
                    }
                    return RightSTGroup.Contains(index);
                }
                return false;
            }

            public void StartDraw(int index, ScriptAccessory accessory)
            {
                accessory.Log.Debug($"开始绘制塔{index}");
                var dp = accessory.Data.GetDefaultDrawProperties();
                dp.Name = $"Tower - {index}";
                dp.Color = accessory.Data.DefaultSafeColor;
                dp.ScaleMode = ScaleMode.ByTime;
                dp.Scale = new(3);
                dp.Owner = accessory.Data.Me;
                dp.DestoryAt = 10000;
                dp.Position = TowerCenter(index);
                accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Circle, dp);
            }

            public void CancelDraw(int index, ScriptAccessory accessory) => accessory.Method.RemoveDraw($"Tower - {index}");
        }

        #endregion
    }
}
