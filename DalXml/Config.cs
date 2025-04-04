﻿using System.Runtime.CompilerServices;

namespace Dal;
internal static class Config
{
    internal const string s_data_config_xml = "data-config.xml";
  
    internal const string s_volunteers_xml = "volunteers.xml";

    internal const string s_calls_xml = "calls.xml";

    internal const string s_assignments_xml = "assignments.xml";

    /// <summary>
    /// Manages the configuration value for the next unique Call ID.
    /// </summary>
    internal static int NextCallId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextCallId");
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextCallId", value);
    }

    /// <summary>
    /// Manages the configuration value for the next unique assignment ID.
    /// </summary>
    internal static int NextAssignmentId
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetAndIncreaseConfigIntVal(s_data_config_xml, "NextAssignmentId");
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        private set => XMLTools.SetConfigIntVal(s_data_config_xml, "NextAssignmentId", value);
    }

    /// <summary>
    /// Manages the current simulation clock value.
    /// </summary>
    internal static DateTime Clock
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetConfigDateVal(s_data_config_xml, "Clock");
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set => XMLTools.SetConfigDateVal(s_data_config_xml, "Clock", value);
    }
    
    /// <summary>
    /// Manages the risk range threshold as a time span.
    /// </summary>
    internal static TimeSpan RiskRange
    {
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        get => XMLTools.GetConfigRiskRange(s_data_config_xml, "RiskRange");
        [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
        set => XMLTools.SetConfigTimeSpan(s_data_config_xml, "RiskRange", value);
    }
    
    /// <summary>
    /// Resets the configuration settings to their initial states.
    /// </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7
    internal static void Reset()
    {
        NextCallId = 0;
        NextAssignmentId = 0;
        Clock = DateTime.Now;
        RiskRange = TimeSpan.FromMinutes(5);
    }
}
