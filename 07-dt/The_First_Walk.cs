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

        #region 1
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
        /*
        [ScriptMethod(name: "Banish Storm", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40946"])]
        public void BanishStorm(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }
        */
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

            var dp2 = accessory.Data.GetDefaultDrawProperties();
            dp2.Name = "Knuckle Sandwich - In";
            dp2.Radian = float.Pi * 2;
            dp2.Scale = new(50);
            dp2.Color = accessory.Data.DefaultDangerColor;
            dp2.Owner = sid;
            dp2.DestoryAt = 1000;
            dp2.Delay = 12500;

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
                    dp.Scale = new(1.5f, 38);
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
        #endregion

        #region 2
        // Boss 2: Fafnir the Forgotten

        [ScriptMethod(name: "Dark Matter Blast", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40854"])]
        public void DarkMatterBlast(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Offensive Posture", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4081[146])$"])]
        public void OffensivePosture(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            // 40811 Tail 2/3 40816 Chariot 40814 Donut

            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = "Offensive Posture";
            dp.Owner = sid;
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.DestoryAt = 10000;

            switch (@event["ActionId"])
            {
                case "40811":
                    if (!float.TryParse(@event["SourceRotation"], out var rot)) return;
                    dp.Rotation = rot;
                    dp.FixRotation = true;
                    dp.Radian = float.Pi * 0.50f;
                    dp.Scale = new(40);
                    dp.Color = accessory.Data.DefaultSafeColor;

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
                    break;
                case "40814":
                    dp.Radian = float.Pi * 2;
                    dp.Scale = new(35);
                    dp.InnerScale = new(17);

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
                    accessory.Method.TextInfo("To Front", duration: 2000, true);
                    break;
                case "40816":
                    dp.Scale = new(24);

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    break;
            }

        }

        [ScriptMethod(name: "Baleful Breath", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:39922"])]
        public void BalefulBreath(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("Alliance Line Stack", duration: 2000, true);
        }

        [ScriptMethod(name: "Hurricane Wing", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40817"])]
        public void HurricaneWing(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("Multiple AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Absolute Terror", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40846"])]
        public void AbsoluteTerror(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Absolute Terror";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 7000;
            dp.Scale = new(20, 70);
            dp.Position = new(-500, -500, 570);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Winged Terror", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40848"])]
        public void WingedTerror(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Winged Terror - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 7000;
            dp.Scale = new(25, 70);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Horrid Roar", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40850"])]
        public void HorridRoar(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Horrid Roar - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 4000;
            dp.Scale = new(4);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        #endregion

        #region 3
        // Boss 3: Ark Angel
        [ScriptMethod(name: "The Decisive Battle", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:regex:^(4105[789])$"])]
        public void TheDecisiveBattle(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (tid != accessory.Data.Me) return;

            switch (@event["ActionId"])
            {
                case "41057":
                    accessory.Method.TextInfo("Attack MR", duration: 2000, true);
                    break;
                case "41058":
                    accessory.Method.TextInfo("Attack TT", duration: 2000, true);
                    break;
                case "41059":
                    accessory.Method.TextInfo("Attack GK", duration: 2000, true);
                    break;
            }
        }

        // Cross + Gaze + Out -> Fan -> In
        /*
        [ScriptMethod(name: "Tachi: Yukikaze", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41081"])]
        public void TachiYukikaze(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Tachi: Yukikaze - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 2500;
            dp.Scale = new(5, 50);
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }
        */
        [ScriptMethod(name: "Tachi: Kasha", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41083"])]
        public void TachiKasha(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Tachi: Kasha";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Delay = 3000;
            dp.DestoryAt = 8000;
            dp.Scale = new(20);
            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Concerted Dissolution", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41084"])]
        public void ConcertedDissolution(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Concerted Dissolution - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 6000;
            dp.Scale = new(40);
            dp.Radian = float.Pi * 0.23f;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Light's Chain", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41085"])]
        public void LightsChain(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Light's Chain";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Delay = 2000;
            dp.DestoryAt = 6000;
            dp.Scale = new(40);
            dp.InnerScale = new(4);
            dp.Radian = float.Pi * 2;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
        }

        [ScriptMethod(name: "Dragonfall", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41086"])]
        public void Dragonfall(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("Party Stack", duration: 2000, true);
        }

        [ScriptMethod(name: "Guillotine", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41063"])]
        public void Guillotine(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Guillotine";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 10000;
            dp.Scale = new(40);
            dp.Radian = float.Pi * 1.35f;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Divine Dominion", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41094"])]
        public void DivineDominion(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Divine Dominion - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 2000;
            dp.Scale = new(6);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Ark Shield", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:18260"])]
        public void ArkShield(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("Attack Shield", duration: 2000, true);
        }

        [ScriptMethod(name: "Rampage", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4107[34])$"])]
        public void Rampage(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Rampage - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 7000;

            switch (@event["ActionId"])
            {
                case "41073":
                    dp.Scale = new(10, 60);

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
                    break;
                case "41074":
                    dp.Scale = new(20);

                    accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
                    break;
            }

        }
        #endregion

        #region 4
        // Boss 4: Shadow Lord

        [ScriptMethod(name: "Giga Slash Text", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4076[67])$"])]
        public void GigaSlash(Event @event, ScriptAccessory accessory)
        {
            switch (@event["ActionId"])
            {
                case "40767":
                    accessory.Method.TextInfo("Left then Right", duration: 2000, true);
                    break;
                case "40766":
                    accessory.Method.TextInfo("Right then Left", duration: 2000, true);
                    break;
            }
        }
        // Until I can get the offset of the rotation for 2-3 swings
        [ScriptMethod(name: "Giga Slash", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4076[89]|4077[01])$"])]
        public void GigaSlashDraw(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            if (!float.TryParse(@event["SourceRotation"], out var rot)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Giga Slash - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(60);
            dp.Rotation = rot;
            dp.FixRotation = true;

            switch (@event["ActionId"])
            {
                case "40768":
                    dp.DestoryAt = 11500;
                    dp.Radian = float.Pi * 1.25f;
                    break;
                case "40769":
                    dp.Rotation = rot - float.Pi / 2;
                    dp.DestoryAt = 1000;
                    dp.Radian = float.Pi * 1.5f;
                    break;
                case "40770":
                    dp.DestoryAt = 11500;
                    dp.Radian = float.Pi * 1.25f;
                    break;
                case "40771":
                    dp.Rotation = rot + float.Pi / 2;
                    dp.DestoryAt = 1000;
                    dp.Radian = float.Pi * 1.5f;
                    break;
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }
        
        [ScriptMethod(name: "Umbra Wave", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40801"])]
        public void UmbraWave(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Umbra Wave - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 1700;
            dp.Scale = new(60, 5);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Flames of Hatred", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40809"])]
        public void FlamesOfHatred(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE", duration: 2000, true);
        }

        [ScriptMethod(name: "Implosion", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4077[4567])$"])]
        public void Implosion(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Implosion - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 9000;
            dp.Scale = new(12);
            dp.Radian = float.Pi;

            // 774 L L 776 L R
            switch (@event["ActionId"])
            {
                case "40774":
                    dp.Rotation = float.Pi / 2;
                    dp.Scale = new(50);
                    break;
                case "40775":
                    dp.Rotation = -float.Pi / 2;
                    break;
                case "40776":
                    dp.Rotation = float.Pi / 2;
                    dp.Scale = new(50);
                    break;
                case "40777":
                    dp.Rotation = -float.Pi / 2;
                    break;
            }

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Fan, dp);
        }

        [ScriptMethod(name: "Cthonic Fury", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4077[89])$"])]
        public void CthonicFury(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("AOE & Arena Transition", duration: 2000, true);
        }

        [ScriptMethod(name: "Burning Moat", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40781"])]
        public void BurningMoat(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Burning Moat - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 7000;
            dp.Scale = new(15);
            dp.InnerScale = new(5);
            dp.Radian = float.Pi * 2;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Donut, dp);
        }

        [ScriptMethod(name: "Burning Court", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40780"])]
        public void BurningCourt(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Burning Court - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 7000;
            dp.Scale = new(8);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Giga Slash: Nightfall Text", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:regex:^(4202[0123])$"])]
        public void GigaSlashNightfall(Event @event, ScriptAccessory accessory)
        {
            switch (@event["ActionId"])
            {
                case "42020":
                    accessory.Method.TextInfo("Right - Left - Back", duration: 5000, true);
                    break;
                case "42021":
                    accessory.Method.TextInfo("Right - Left - Front", duration: 5000, true);
                    break;
                case "42022":
                    accessory.Method.TextInfo("Left - Right - Back", duration: 5000, true);
                    break;
                case "42023":
                    accessory.Method.TextInfo("Left - Right - Front", duration: 5000, true);
                    break;
            }
        }

        [ScriptMethod(name: "Binding Sigil", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:41513"])]
        public void BindingSigil(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = $"Binding Sigil - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.DestoryAt = 10000;
            dp.Scale = new(9);

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Circle, dp);
        }

        [ScriptMethod(name: "Soul Binding", eventType: EventTypeEnum.ActionEffect, eventCondition: ["ActionId:41514"])]
        public void SoulBinding(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;

            accessory.Method.RemoveDraw($"Binding Sigil - {sid}");
        }

        [ScriptMethod(name: "Damning Strikes", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:40791"])]
        public void DamningStrikes(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("Take Towers", duration: 2000, true);
        }
        #endregion

        #region Utils
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
        #endregion
    }
}
