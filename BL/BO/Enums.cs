using Helpers;
namespace BO;

public enum Roles { Volunteer, Manager };
public enum DistanceType { Aerial, Car, Walking };
public enum CallType { RegularVehicle, Ambulance, IntensiveCareAmbulance, None };
public enum EndType { Treated, AdminCancellation, SelfCancellation, ExpiredCancellation };
public enum CallStatus { Open, Treatment, OpenAtRisk, TreatmentOfRisk, Expired, Close, None };
public enum TimeUnit { Minute, Hour, Day, Month, Year };
public enum VolunteerField { Id, Name, Active, None }
public enum CallField { Id, Address, CarTypeToSend }
public enum ClosedCallInListField { Id , CallType, Address, OpenTime, EnterTime, EndTime,TypeEndOfTreatment }
public enum OpenCallInListField { Id, CallType, Destination, Address, OpenTime, MaxTime, Distance }

public enum CallInListFields { Id, CallId, CallType, OpenTime, TimeLeftToFinish, LastVolunteer, TreatmentTimeLeft, Status, TotalAssignments, None };
public enum GroupBy { CallType,Status, None };

