using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.Draw;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "Alexander - The Cuff of the Son", territorys: [521], guid: "E6835ED0-D91C-4946-B07E-3634337311D7", version: "0.0.0.1", author: "Due")]
    public class The_Cuff_of_the_Son
    {
        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        [ScriptMethod(name: "Blaster Mirage", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:5580"])]
        public void BlasterMirage(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["SourceId"], out var sid)) return;
            var dp = accessory.Data.GetDefaultDrawProperties();

            dp.Name = $"Blaster Mirage - {sid}";
            dp.Color = accessory.Data.DefaultDangerColor;
            dp.Owner = sid;
            dp.Scale = new(5, 70);
            dp.Delay = 5000;

            accessory.Method.SendDraw(DrawModeEnum.Default, DrawTypeEnum.Rect, dp);
        }

        [ScriptMethod(name: "Low/High Arithmeticks", eventType: EventTypeEnum.StatusAdd, eventCondition: ["StatusID:regex:^(102[12])$"])]
        public void LowHighArithmeticks(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (accessory.Data.Me != tid) return;

            if (@event["StatusID"] == "1021")
            {
                accessory.Method.TextInfo("上高台", 2000, true);
            }
            else if (@event["StatusID"] == "1022")
            {
                accessory.Method.TextInfo("下低台", 2000, true);
            }
        }

        [ScriptMethod(name: "Wave", eventType: EventTypeEnum.TargetIcon, eventCondition: ["Id:0043"])]
        public void Wave(Event @event, ScriptAccessory accessory)
        {
            if (!ParseObjectId(@event["TargetId"], out var tid)) return;
            if (accessory.Data.Me != tid) return;

            accessory.Method.TextInfo("去炸冰圈", 2000, true);
        }

        [ScriptMethod(name: "Ultra Flash", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:5922"])]
        public void UltraFlash(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("躲在冰块后", 2000, true);
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
