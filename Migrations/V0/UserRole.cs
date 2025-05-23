﻿using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V0
{
    public class UserRole : GTRC_Basics.Models.UserRole
    {
        private static int nextId = GlobalValues.Id0;
        private int id = GlobalValues.NoId;
        public new int Id { get { return id; } set { if (value == GlobalValues.Id0) { nextId = GlobalValues.Id0; } id = nextId; nextId++; } }

        public int DriverID { set { UserId = value; RoleId = 1; } }
    }
}
