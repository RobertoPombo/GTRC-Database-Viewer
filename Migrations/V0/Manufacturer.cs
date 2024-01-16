namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Manufacturer : GTRC_Basics.Models.Manufacturer
    {
        private static int nextId = GTRC_Basics.GlobalValues.Id0;
        public static List<Manufacturer> List = [];

        private int id = GTRC_Basics.GlobalValues.NoId;
        private string? name = null;

        public new int Id
        {
            get { return id; }
            set { }
        }

        public new string Name
        {
            get { if (name is not null) { return name; } else { return DefaultName; } }
            set { name ??= value; }
        }

        public string manufacturer
        {
            set
            {
                name = null;
                Name = value;
                Manufacturer? _manufacturer = null;
                foreach (Manufacturer m in List) { if (m.Name == name) { _manufacturer = m; } }
                if (name is not null && _manufacturer is not null) { id = _manufacturer.Id; }
                else if (name is not null)
                {
                    List.Add(this);
                    id = nextId;
                    nextId++;
                }
            }
        }
    }
}
