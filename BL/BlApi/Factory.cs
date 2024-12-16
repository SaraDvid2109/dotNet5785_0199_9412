namespace BlApi;

/// <summary>
/// Factory class for creating an instance of the business logic layer (IBl).
/// </summary>
public static class Factory
{
    public static IBl Get() => new BlImplementation.Bl();

}
