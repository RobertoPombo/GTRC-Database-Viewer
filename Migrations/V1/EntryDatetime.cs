

namespace GTRC_Database_Viewer.Migrations.V1
{
    public class EntryDatetime : GTRC_Basics.Models.EntryDatetime
    {
        public new bool IsPermanent
        {
            get
            {
                foreach (Entry entry in V1.Entry.List)
                {
                    if (entry.Id == EntryId) { return entry.IsPermanent; }
                }
                return new EntryDatetime().IsPermanent;
            }
            set { }
        }
    }
}
