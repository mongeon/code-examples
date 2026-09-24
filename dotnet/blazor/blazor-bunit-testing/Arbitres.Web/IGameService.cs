namespace Arbitres.Web;

public interface IGameService
{
    Task<IReadOnlyList<Game>> GetUpcomingGamesAsync();
}
