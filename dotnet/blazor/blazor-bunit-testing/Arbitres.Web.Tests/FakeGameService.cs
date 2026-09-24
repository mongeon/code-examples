namespace Arbitres.Web.Tests;

public class FakeGameService : IGameService
{
    private readonly TaskCompletionSource<IReadOnlyList<Game>> tcs = new();

    public Task<IReadOnlyList<Game>> GetUpcomingGamesAsync() => tcs.Task;

    // Termine l'appel avec les matchs reçus
    public void Complete(params Game[] games) => tcs.SetResult(games);
}
