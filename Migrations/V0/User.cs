using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class User : GTRC_Basics.Models.User
    {
        public new int CommunityId { get { if (V0.Community.List.Count > 0) { return V0.Community.List[0].Id; } else { return GlobalValues.NoId; } } }

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
    }
}
