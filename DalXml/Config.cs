namespace Dal;
internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";//אמרו בלי סיומת ובדוגמה כתבו עם
  
    internal const string s_volunteer_xml = "volunteer.xml";

    internal const string s_call_xml = "call.xml";

    internal const string s_assignment_xml = "assignment.xml";


    internal static int NextCallId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    internal static int NextAssignmentId
    {
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }


    internal static DateTime Clock
    {
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    internal static TimeSpan RiskRange
    {
        get;
        set;
    }
    internal static void Reset()
    {
        NextCallId = 0;
        NextAssignmentId = 0;
        Clock = DateTime.Now;
       
    }
}
