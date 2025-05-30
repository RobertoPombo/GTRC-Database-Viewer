﻿namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Carclass : GTRC_Basics.Models.Carclass
    {
        public static List<Carclass> List = [];

        private int id = GTRC_Basics.GlobalValues.NoId;

        public new int Id
        {
            get { return id; }
            set
            {
                id = value;
                bool isInList = false;
                foreach (Carclass obj in List)
                {
                    if (obj.Id == value) { isInList = true; break; }
                }
                if (!isInList) { List.Add(this); }
            }
        }
    }
}
