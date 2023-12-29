using System.Reflection;

namespace GTRC_Database_Client
{
    public class SortState
    {
        public SortState() { }

        public bool SortAscending = true;
        public PropertyInfo? Property = null;
    }
}
