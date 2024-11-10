

namespace GTRC_Database_Viewer.Migrations.V1
{
    public class EntryDatetime : GTRC_Basics.Models.EntryDatetime
    {
        public new bool IsPermanent
        {
            get
            {
                foreach (Entry obj in V1.Entry.List)
                {
                    if (obj.Id == EntryId) { return obj.IsPermanent; }
                }
                return new EntryDatetime().IsPermanent;
            }
            set { }
        }
    }
}
