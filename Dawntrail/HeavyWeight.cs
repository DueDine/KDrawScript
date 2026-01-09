using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Extensions;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace KDrawScript.Dev
{
    [ScriptType(name: "Heavy Weight Savage", territorys: [1323], guid: "A5689105-A7FD-4B84-9674-67610285768C", version: "0.0.0.1", author: "Due")]
    public class HeavyWeightSavage
    {

        [UserSetting(note: "实验性开关。")]
        public bool EnableGuidance { get; set; } = false;

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

        // 4 Fire 5 Water
        [ScriptMethod(name: "Firesnaking & Watersnaking", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:regex:^(497[45])$"])]
        public void FiresnakingWatersnaking(Event @event, ScriptAccessory accessory)
        {
            if (!EnableGuidance) return;
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (accessory.Data.Me != tid) return;

            var StatusID = @event.StatusId;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Firesnaking & Watersnaking";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.DestoryAt = 7000;

            if (StatusID == 4975)
                dp.TargetObject = accessory.Data.Objects.GetByDataId(19288).FirstOrDefault().EntityId;
            else
                dp.TargetPosition = new Vector3(118.50f, 0, 88.50f);

            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
        }

        [ScriptMethod(name: "Deep Varial", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:46547"])]
        public void DeepVarial(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out _)) return;
            
            var pos = @event.SourcePosition;

            var dp = accessory.Data.GetDefaultDrawProperties();
            dp.Name = "Deep Varial";
            dp.Color = accessory.Data.DefaultSafeColor;
            dp.Scale = new(2);
            dp.Owner = accessory.Data.Me;
            dp.ScaleMode |= ScaleMode.YByDistance;
            dp.DestoryAt = 6000;

            if (accessory.Data.MyObject.HasStatus(4975))
            {
                if (pos.Z == 120)
                    dp.TargetPosition = new Vector3(84.60f, 0, 112.39f);
                else if (pos.Z == 80)
                    dp.TargetPosition = new Vector3(84.60f, 0, 87.5f);

            }
            else if (accessory.Data.MyObject.HasStatus(4974))
            {
                if (pos.Z == 120)
                    dp.TargetPosition = new Vector3(115.16f, 0, 118.83f);
                else if (pos.Z == 80)
                    dp.TargetPosition = new Vector3(112.27f, 0, 82.00f);
            }

            accessory.Method.SendDraw(DrawModeEnum.Imgui, DrawTypeEnum.Displacement, dp);
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
