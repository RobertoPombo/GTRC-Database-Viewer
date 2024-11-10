

using GTRC_Basics;

namespace GTRC_Database_Viewer.Migrations.V1
{
    public class Prequalifying : GTRC_Basics.Models.Prequalifying
    {
        public static readonly string prefixPreQualifying = "Pre-Qualifying #";
        public static bool alternate = false;

        public new SessionPurpose SessionPurpose
        {
            get { return SessionPurpose.PreQualifying; }
            set { }
        }

        private int eventId = GlobalValues.NoId;

        public new int EventId
        {
            get { return eventId; }
            set
            {
                eventId = value;
                foreach (Event obj in V1.Event.List)
                {
                    if (obj.Id == eventId)
                    {
                        if (obj.Name.Length > prefixPreQualifying.Length && obj.Name[..prefixPreQualifying.Length] == prefixPreQualifying) { break; }
                        foreach (Season season in V1.Season.List)
                        {
                            if (season.Id == obj.SeasonId)
                            {
                                List<Session> preqSessions = [];
                                foreach (Event preqEvent in V1.Event.List)
                                {
                                    if (season.Id == preqEvent.SeasonId &&
                                        preqEvent.Name.Length > prefixPreQualifying.Length && preqEvent.Name[..prefixPreQualifying.Length] == prefixPreQualifying)
                                    {
                                        foreach (Session preqSession in V1.Session.List)
                                        {
                                            if (preqSession.EventId == preqEvent.Id)
                                            {
                                                preqSessions.Add(preqSession);
                                            }
                                        }
                                    }
                                }
                                if (alternate && preqSessions.Count > 1) { sessionId = preqSessions[1].Id; alternate = !alternate; }
                                else if (preqSessions.Count > 0) { sessionId = preqSessions[0].Id; alternate = !alternate; }
                                break;
                            }
                        }
                        break;
                    }
                }
            }
        }

        private int sessionId = GlobalValues.NoId;

        public new int SessionId
        {
            get { return sessionId; }
            set { }
        }

        public new int PerformancerequirementId
        {
            get { if (V1.Performancerequirement.List.Count == 0) { return GlobalValues.NoId; } else { return V1.Performancerequirement.List[0].Id; } }
            set { }
        }
    }
}
