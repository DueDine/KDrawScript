using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "Alexander - The Eyes of the Creator (A9)", territorys: [580], guid: "31204d48-0846-41fc-a389-20c8fb5327b1", version: "0.0.0.1", author: "Due")]
    public class TheEyesoftheCreator
    {
        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        [ScriptMethod(name: "Doll Scarp", eventType: EventTypeEnum.AddCombatant, eventCondition: ["DataId:regex:^(635[012])$"])]
        public void DollScarp(Event @event, ScriptAccessory accessory)
        {
            var position = ParsePosition(@event, "SourcePosition");
            var location = PositionToLocation(position);

            accessory.Method.TextInfo($"ST {location} 放炸弹 拉至发光处", duration: 4000, true);
        }

        [ScriptMethod(name: "Scrap Burst", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:6926"])]
        public void ScrapBurst(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("石头后躲避", duration: 2000, true);
        }

        [ScriptMethod(name: "Scrapline", eventType: EventTypeEnum.StartCasting, eventCondition: ["ActionId:6928"])]
        public void Scrapline(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("身后躲避", duration: 2000, true);
        }

        [ScriptMethod(name: "Faust", eventType: EventTypeEnum.Tether, eventCondition: ["Id:000C"])]
        public void Faust(Event @event, ScriptAccessory accessory)
        {
            accessory.Method.TextInfo("攻击小怪", duration: 2000, true);
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

        private static string PositionToLocation(Vector3 position)
        {
            // Center at 0, -250, -250
            if (position.X < 0)
            {
                if (position.Z > -250)
                {
                    return "左下";
                }
                else
                {
                    return "左上";
                }
            }
            else
            {
                if (position.Z > -250)
                {
                    return "右下";
                }
                else
                {
                    return "右上";
                }
            }
        }

        #endregion
    }
}
