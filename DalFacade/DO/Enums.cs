namespace DO;

public enum Roles { Volunteer, Management  };
public enum DistanceType { Aerial, Car, Walking };
public enum CallType { RegularVehicle, Ambulance, IntensiveCareAmbulance };
public enum CallStatus { Open, Treatment, OpenAtRisk, TreatmentOfRisk, Expired, Close };
public enum EndType { Processed, AdminCancellation, SelfCancellation, ExpiredCancellation }