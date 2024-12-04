using Helpers;

namespace BO;
public enum Roles { Volunteer, Management };
public enum DistanceType { Aerial, Car, Walking };
public enum CallType { RegularVehicle, Ambulance, IntensiveCareAmbulance, None };
//public enum CallStatus { Treatment, TreatmentOfRisk };
public enum EndType { Processed, AdminCancellation, SelfCancellation, ExpiredCancellation };
public enum CallStatus { Open, Treatment, OpenAtRisk, TreatmentOfRisk, Expired, Close };
//**********************************************
public enum TimeUnit { Minute, Hour, Day, Month, Year };
public enum VolunteerField { Id, Name , Active, Phone, Mail , Address , Role , MaximumDistance , Type }
