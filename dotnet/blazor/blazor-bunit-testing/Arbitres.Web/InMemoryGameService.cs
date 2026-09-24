namespace Arbitres.Web;

// Dans l'application réelle, ce service appelle une API.
// Ici, il retourne quelques matchs en mémoire pour pouvoir lancer l'application.
public class InMemoryGameService : IGameService
{
    public async Task<IReadOnlyList<Game>> GetUpcomingGamesAsync()
    {
        await Task.Delay(500);

        return
        [
            new Game(1, "Expos", "Blue Jays", DateTime.Today.AddDays(1).AddHours(19), 2, 2),
            new Game(2, "Capitales", "Aigles", DateTime.Today.AddDays(2).AddHours(13), 2, 1),
            new Game(3, "Expos", "Capitales", DateTime.Today.AddDays(3).AddHours(19), 3, 0),
        ];
    }
}
