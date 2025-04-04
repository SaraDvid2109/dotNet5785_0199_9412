﻿namespace DO;
public enum Roles { Volunteer, Manager };
public enum DistanceType { Aerial, Car, Walking };
public enum CallType { RegularVehicle, Ambulance, IntensiveCareAmbulance };
public enum CallStatus { Open, Treatment, OpenAtRisk, TreatmentOfRisk, Expired, Close };
public enum EndType { Treated, AdminCancellation, SelfCancellation, ExpiredCancellation };
public enum MainMenuOptions { Exit, VolunteerMenu, CallMenu, AssignmentMenu, InitializeData, DisplayAllData, ConfigMenu, ResetDatabase };
public enum EntityMenuOptions { Exit, Create, Read, ReadAll, Update, Delete, DeleteAll };
public enum ConfigMenuOptions { Exit, AdvanceClockMinute, AdvanceClockHour, DisplayCurrentClock, SetConfigValue, DisplayConfigValue, ResetConfigValues }