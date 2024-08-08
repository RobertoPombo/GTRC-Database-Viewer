using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Organization : GTRC_Basics.Models.Organization
    {
        private static readonly List<string> del = [" - ", " #", " | ", " (",];
        private static readonly List<string> apx = ["1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX", "X"];
        private static readonly List<string> rep = [" ", "-", "_", ".", ",", "(", ")", "team", "simracing", "eracing", "racing", "motorsports", "motorsport", "esports", "esport", "sport", "performance", "junior", "gold", "silver", "bronze"];
        private static int nextId = GlobalValues.Id0;
        public static List<Organization> List = [];

        private int id = GlobalValues.NoId;
        private string? name = null;

        public new int Id
        {
            get { return id; }
            set { if (!TeamIds.Contains(value)) { TeamIds.Add(value); } }
        }

        public List<int> TeamIds = [];

        public new string Name
        {
            get { if (name is not null) { return name; } else { return DefaultName; } }
            set
            {
                name = string.Empty;
                string _name0 = value;
                foreach (string _del in del)
                {
                    string[] _names = _name0.Split(_del);
                    if (_names.Length > 1)
                    {
                        string _name1 = _names[0];
                        if (_name1.Length > name.Length) { name = _name1; }
                    }
                }
                if (name.Length == 0) { name = _name0; }
                foreach (string _apx in apx)
                {
                    if (_name0.Length > _apx.Length && _name0[^_apx.Length..] == _apx && (_name0.Length - _apx.Length) < name.Length) { name = _name0[..^_apx.Length]; }
                }
                name = name.ToLower();
                name = name[..1].ToUpper() + name[1..];
                foreach (string _rep in rep) { name = name.Replace(_rep.ToLower(), ""); }
                Organization? organization = null;
                foreach (Organization _organization in List) { if (_organization.Name == name) { organization = _organization; } }
                if (organization is not null)
                {
                    id = organization.Id;
                    foreach (int teamId in TeamIds) { if (!organization.TeamIds.Contains(teamId)) { organization.TeamIds.Add(teamId); } }
                    foreach (int teamId in organization.TeamIds) { if (!TeamIds.Contains(teamId)) { TeamIds.Add(teamId); } }
                }
                else
                {
                    List.Add(this);
                    id = nextId;
                    nextId++;
                }
                List<int> newTeamIds = [];
                foreach (int _oldTeamId in TeamIds) { foreach (Team _team in V0.Team.List) { if (_oldTeamId == _team.OldId) { newTeamIds.Add(_team.Id); } } }
                foreach (Entry _entry in V0.Entry.List)
                {
                    if (newTeamIds.Contains(_entry.TeamId))
                    {
                        if (_entry.RegisterDate < RegisterDate) { RegisterDate = _entry.RegisterDate; }
                    }
                }
            }
        }

        private DateTime registerDate = DateTime.UtcNow;
        public new DateTime RegisterDate
        {
            get { return registerDate; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { registerDate = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { registerDate = GlobalValues.DateTimeMinValue; }
                else { registerDate = value.ToUniversalTime(); }
            }
        }
    }
}
