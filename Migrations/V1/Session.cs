

using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V1
{
    public class Session : GTRC_Basics.Models.Session
    {
        public static readonly string suffixDryPractice = "Trainingsserver Trocken";
        public static readonly string suffixWetPractice = "Trainingsserver Regen";
        public static List<Session> List = [];

        private int id = GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set
            {
                id = value;
                bool isInList = false;
                foreach (Session obj in List)
                {
                    if (obj.Id == value) { isInList = true; break; }
                }
                if (!isInList) { List.Add(this); }
            }
        }

        public new short AmbientTemp
        {
            get
            {
                foreach (Event obj in V1.Event.List)
                {
                    if (obj.Id == EventId) { return obj.AmbientTemp; }
                }
                return DefaultAmbientTemp;
            }
            set { }
        }

        public new byte CloudLevel
        {
            get
            {
                if (ServerName.Length > suffixWetPractice.Length && ServerName[^suffixWetPractice.Length..] == suffixWetPractice) { return 80; }
                foreach (Event obj in V1.Event.List)
                {
                    if (obj.Id == EventId) { return obj.CloudLevel; }
                }
                return byte.MinValue;
            }
            set { }
        }

        public new byte RainLevel
        {
            get
            {
                if (ServerName.Length > suffixDryPractice.Length && ServerName[^suffixDryPractice.Length..] == suffixDryPractice) { return 0; }
                if (ServerName.Length > suffixWetPractice.Length && ServerName[^suffixWetPractice.Length..] == suffixWetPractice) { return 20; }
                foreach (Event obj in V1.Event.List)
                {
                    if (obj.Id == EventId) { return obj.RainLevel; }
                }
                return byte.MinValue;
            }
            set { }
        }

        public new byte WeatherRandomness
        {
            get
            {
                if (ServerName.Length > suffixDryPractice.Length && ServerName[^suffixDryPractice.Length..] == suffixDryPractice) { return 0; }
                if (ServerName.Length > suffixWetPractice.Length && ServerName[^suffixWetPractice.Length..] == suffixWetPractice) { return 0; }
                foreach (Event obj in V1.Event.List)
                {
                    if (obj.Id == EventId) { return obj.WeatherRandomness; }
                }
                return byte.MinValue;
            }
            set { }
        }

        public new bool FixedConditions
        {
            get
            {
                foreach (Event obj in V1.Event.List)
                {
                    if (obj.Id == EventId) { return obj.FixedConditions; }
                }
                return false;
            }
            set { }
        }
    }
}
