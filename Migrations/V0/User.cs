using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class User : GTRC_Basics.Models.User
    {
        public new string NickName
        {
            get
            {
                string _shortName = string.Empty;
                List<char> cList = [' ', '-'];
                if (FirstName is not null)
                {
                    for (int index = 0; index < FirstName.Length - 1; index++)
                    {
                        if (_shortName.Length == 0 && !cList.Contains(FirstName[index]))
                        {
                            _shortName = FirstName[index].ToString() + ".";
                        }
                        else if (cList.Contains(FirstName[index]) && !cList.Contains(FirstName[index + 1]))
                        {
                            _shortName += FirstName[index].ToString() + FirstName[index + 1].ToString() + ".";
                        }
                    }
                    _shortName += " " + LastName;
                }
                else { _shortName = LastName; }
                return _shortName;
            }
        }

        private DateTime registerDate = DateTime.UtcNow;
        private DateTime banDate = GlobalValues.DateTimeMaxValue;
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

        public new DateTime BanDate
        {
            get { return banDate; }
            set
            {
                if (value > GlobalValues.DateTimeMaxValue) { banDate = GlobalValues.DateTimeMaxValue; }
                else if (value < new DateTime(DateTime.MinValue.Year + 1801, 1, 1, 0, 0, 0, 0, DateTime.MinValue.Kind)) { banDate = GlobalValues.DateTimeMinValue; }
                else { banDate = value.ToUniversalTime(); }
            }
        }
    }
}
