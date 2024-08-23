using Newtonsoft.Json;
using System.IO;
using System.Text;

using GTRC_Basics;
using GTRC_Database_Viewer.ViewModels;
using GTRC_Basics.Models.Common;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class EntryUserEvent : GTRC_Basics.Models.EntryUserEvent
    {
        private static int nextId = GlobalValues.Id0;
        private static int currentJsonFileListCount = 0;
        public static List<EntryUserEvent> List = [];

        private int id = GlobalValues.NoId;
        private int entryId = GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set { }
        }

        public new int EntryId { get; set; }

        public new string Name3Digits { get; set; } = string.Empty;

        public int DriverID
        {
            set
            {
                UserId = value;
                int seasonId = GlobalValues.NoId;
                foreach (Entry _entry in V0.Entry.List) { if (_entry.Id == EntryId) { seasonId = _entry.SeasonId; break; } }
                List<Event> listEvents = [];
                foreach (Event _event in V0.Event.List) { if (_event.SeasonId == seasonId) { listEvents.Add(_event); } }
                for (int index1 = 0; index1 < listEvents.Count - 1; index1++)
                {
                    for (int index2 = index1; index2 < listEvents.Count; index2++)
                    {
                        if (listEvents[index1].Date > listEvents[index2].Date)
                        {
                            (listEvents[index2], listEvents[index1]) = (listEvents[index1], listEvents[index2]);
                        }
                    }
                }
                EntryUserEvent? eue = null;
                foreach (Event _event in listEvents)
                {
                    foreach (EntryUserEvent _eue in List) { if (_eue.EntryId == EntryId && _eue.UserId == UserId && _eue.EventId == _event.Id) { eue = _eue; break; } }
                    if (eue is null) { EventId = _event.Id; List.Add(this); id = nextId; nextId++; break; }
                }
                if (eue is not null) { id = eue.Id; EventId = eue.EventId; }
                else
                {
                    foreach (Event _event in listEvents)
                    {
                        bool exists = false;
                        foreach (EntryUserEvent _eue in List) { if (_eue.EntryId == EntryId && _eue.UserId == UserId && _eue.EventId == _event.Id) { exists = true; break; } }
                        if (!exists)
                        {
                            List.Add(new EntryUserEvent() { id = nextId, EntryId = EntryId, UserId = UserId, EventId = _event.Id, Name3Digits = Name3Digits });
                            nextId++;
                        }
                    }
                }
                if (eue is not null) { id = eue.Id; EventId = eue.EventId; }
                if (List.Count > currentJsonFileListCount)
                {
                    List<EntryUserEventJson> list = [];
                    foreach (EntryUserEvent _eue in List)
                    {
                        EntryUserEventJson _euej = new() { Id = _eue.Id, EntryId = _eue.EntryId, UserId = _eue.UserId, EventId = _eue.EventId, Name3Digits = _eue.Name3Digits };
                        list.Add(_euej);
                    }
                    Type modelType = typeof(GTRC_Basics.Models.EntryUserEvent);
                    string path = Directories.DbMigrations + "//" + DatabaseVM.DictDatabaseTableVM[modelType].DbVersion + "//" + modelType.Name + " - ALLEVENTS.json";
                    File.WriteAllText(path, JsonConvert.SerializeObject(list, Formatting.Indented), Encoding.Unicode);
                    currentJsonFileListCount = list.Count;
                }
            }
        }
    }



    public class EntryUserEventJson : IBaseModel
    {
        public int Id { get; set; }
        public int EntryId { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string Name3Digits { get; set; } = string.Empty;
    }
}
