namespace Arbitres.Web;

public record Game(int Id, string HomeTeam, string AwayTeam, DateTime StartTime, int RequiredUmpires, int AssignedUmpires)
{
    public int MissingUmpires => RequiredUmpires - AssignedUmpires;
}
