using Bunit;
using Bunit.TestDoubles;
using Microsoft.Extensions.DependencyInjection;

namespace Arbitres.Web.Tests;

public class GameListTests : BunitContext
{
    private readonly FakeGameService gameService = new();
    private readonly BunitAuthorizationContext auth;

    public GameListTests()
    {
        Services.AddSingleton<IGameService>(gameService);
        auth = AddAuthorization();
    }

    [Fact]
    public void Affiche_chargement_pendant_l_appel()
    {
        var cut = Render<GameList>();

        cut.Find(".loading").MarkupMatches("<p class=\"loading\">Chargement...</p>");
    }

    [Fact]
    public void Affiche_une_carte_par_match()
    {
        var cut = Render<GameList>();

        gameService.Complete(
            new Game(1, "Expos", "Blue Jays", DateTime.Today, 2, 2),
            new Game(2, "Capitales", "Aigles", DateTime.Today, 2, 0));

        // Le rendu se fait après la fin de la tâche, on attend qu'il arrive
        cut.WaitForAssertion(() => Assert.Equal(2, cut.FindComponents<GameCard>().Count));
    }

    [Fact]
    public void Affiche_un_message_si_aucun_match()
    {
        var cut = Render<GameList>();

        gameService.Complete();

        cut.WaitForAssertion(() => cut.Find(".empty").MarkupMatches("<p class=\"empty\">Aucun match à venir.</p>"));
    }

    [Fact]
    public void Visiteur_voit_le_lien_de_connexion()
    {
        var cut = Render<GameList>();

        Assert.NotNull(cut.Find("a.login"));
    }

    [Fact]
    public void Utilisateur_connecte_voit_son_nom()
    {
        auth.SetAuthorized("Gabriel");

        var cut = Render<GameList>();

        cut.Find(".welcome").MarkupMatches("<p class=\"welcome\">Bonjour Gabriel</p>");
    }
}
