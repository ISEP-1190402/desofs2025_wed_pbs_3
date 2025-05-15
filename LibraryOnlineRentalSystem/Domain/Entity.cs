namespace LibraryOnlineRentalSystem.Domain;

public class Entity<IDEntity> where IDEntity : EntityId
{
    public IDEntity Id { get; protected set; }
}
