namespace BO;
public enum Roles { Volunteer, Management };
public enum DistanceType { Aerial, Car, Walking };
public enum CallType { RegularVehicle, Ambulance, IntensiveCareAmbulance, None };
//public enum CallStatus { Treatment, TreatmentOfRisk };
public enum EndType { Processed, AdminCancellation, SelfCancellation, ExpiredCancellation };
public enum CallStatus { Open, Treatment, OpenAtRisk, TreatmentOfRisk, Expired, Close };

