using FFXIVClientStructs.FFXIV.Client.UI.Info;
using FFXIVClientStructs.FFXIV.Common.Math;
using KodakkuAssist.Module.GameEvent;
using KodakkuAssist.Script;
using Newtonsoft.Json;
using System;

namespace KDrawScript.Dev
{
    [ScriptType(name: "TemplateTest", territorys: [], guid: "ACFA875F-5F97-48F7-A0A2-A338C5450AF1", version: "0.0.0.1", author: "Due")]
    public class TemplateTest
    {
        private static readonly bool DebugMode = true;

        public void Init(ScriptAccessory accessory)
        {
            accessory.Method.RemoveDraw(".*");
        }

        [ScriptMethod(name: "随时DEBUG用", eventType: EventTypeEnum.Chat, eventCondition: ["Type:Echo"], userControl: false)]
        public unsafe void EchoDebugActive(Event @event, ScriptAccessory accessory)
        {
            if (!DebugMode) return;
            var msg = @event["Message"];
            if (msg == null) return;

            if (msg.Contains("debug"))
            {
                var proxy = InfoProxyCrossRealm.Instance();
                var groupNum = proxy->GroupCount;
                if (groupNum > 0)
                {
                    var groups = proxy->CrossRealmGroups;
                    foreach (var group in groups)
                    {
                        var memberNum = group.GroupMemberCount;
                        if (memberNum > 0)
                        {
                            var members = group.GroupMembers;
                            foreach (var member in members)
                            {
                                var name = member.Name;
                                var cid = member.ContentId;
                                var groupIdx = member.GroupIndex;
                                var memberIdx = member.MemberIndex;
                                accessory.Log.Debug($"Debug: {name.ToString()} {cid:X} {groupIdx} {memberIdx}");
                            }
                        }
                    }
                }
                accessory.Log.Debug($"Debug: Trigger");
            }
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
