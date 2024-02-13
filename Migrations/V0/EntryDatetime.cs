using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class EntryDatetime : GTRC_Basics.Models.EntryDatetime
    {
        private static int nextId = GlobalValues.Id0;
        private int id = GlobalValues.NoId;
        public new int Id { get { return id; } set { if (value == GlobalValues.Id0) { nextId = GlobalValues.Id0; } id = nextId; nextId++; } }
    }
}
