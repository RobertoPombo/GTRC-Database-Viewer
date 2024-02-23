namespace GTRC_Database_Viewer.Migrations.V0
{
    public class Track : GTRC_Basics.Models.Track
    {
        public int AccTimePenDT { set { AccTimePenDtS = (ushort)value; } }

        public string NameGtrc { set { NameGoogleSheets = value; } }

        public new string NameLfm
        {
            get
            {
                if (AccTrackId == "imola") { return "Autodromo Enzo e Dino Ferrari"; }
                else if (AccTrackId == "monza") { return "Autodromo Nazionale di Monza"; }
                else if (AccTrackId == "brands_hatch") { return "Brands Hatch Circuit"; }
                else if (AccTrackId == "barcelona") { return "Circuit de Catalunya"; }
                else if (AccTrackId == "paul_ricard") { return "Circuit de Paul Ricard"; }
                else if (AccTrackId == "spa") { return "Circuit de Spa Francorchamps"; }
                else if (AccTrackId == "cota") { return "Circuit Of The Americas"; }
                else if (AccTrackId == "valencia") { return "Circuit Ricardo Tormo"; }
                else if (AccTrackId == "donington") { return "Donington Park"; }
                else if (AccTrackId == "hungaroring") { return "Hungaroring"; }
                else if (AccTrackId == "indianapolis") { return "Indianapolis"; }
                else if (AccTrackId == "kyalami") { return "Kyalami"; }
                else if (AccTrackId == "laguna_seca") { return "Laguna Seca"; }
                else if (AccTrackId == "misano") { return "Misano"; }
                else if (AccTrackId == "mount_panorama") { return "Mount Panorama Circuit"; }
                else if (AccTrackId == "nurburgring") { return "Nürburgring"; }
                else if (AccTrackId == "oulton_park") { return "Oulton Park"; }
                else if (AccTrackId == "silverstone") { return "Silverstone"; }
                else if (AccTrackId == "snetterton") { return "Snetterton"; }
                else if (AccTrackId == "red_bull_ring") { return "Spielberg - Red Bull Ring"; }
                else if (AccTrackId == "suzuka") { return "Suzuka Circuit"; }
                else if (AccTrackId == "watkins_glen") { return "Watkins Glen"; }
                else if (AccTrackId == "zandvoort") { return "Zandvoort"; }
                else if (AccTrackId == "zolder") { return "Zolder"; }
                else { return string.Empty; }
            }
        }
    }
}
