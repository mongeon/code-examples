using Bunit;

namespace Arbitres.Web.Tests;

public class GameCardTests : BunitContext
{
    private static readonly Game FullGame = new(1, "Expos", "Blue Jays", new DateTime(2026, 10, 3, 19, 0, 0), 2, 2);
    private static readonly Game GameMissingOne = new(2, "Expos", "Blue Jays", new DateTime(2026, 10, 4, 13, 0, 0), 2, 1);

    [Fact]
    public void Affiche_les_equipes_du_match()
    {
        // On passe les paramètres du composant avec des expressions typées
        var cut = Render<GameCard>(parameters => parameters
            .Add(p => p.Game, FullGame));

        cut.Find("h3").MarkupMatches("<h3>Blue Jays @ Expos</h3>");
    }

    [Fact]
    public void Match_complet_n_affiche_pas_le_bouton()
    {
        var cut = Render<GameCard>(parameters => parameters
            .Add(p => p.Game, FullGame));

        Assert.Empty(cut.FindAll("button.volunteer"));
    }

    [Fact]
    public void Match_incomplet_affiche_le_nombre_manquant()
    {
        var cut = Render<GameCard>(parameters => parameters
            .Add(p => p.Game, GameMissingOne));

        cut.Find(".missing").MarkupMatches("<span class=\"missing\">Manque 1 arbitre(s)</span>");
    }

    [Fact]
    public void Clic_sur_me_proposer_envoie_l_id_du_match()
    {
        int? volunteeredGameId = null;
        var cut = Render<GameCard>(parameters => parameters
            .Add(p => p.Game, GameMissingOne)
            .Add(p => p.OnVolunteer, id => volunteeredGameId = id));

        cut.Find("button.volunteer").Click();

        Assert.Equal(2, volunteeredGameId);
    }

    [Fact]
    public void Copier_le_lien_appelle_le_presse_papiers()
    {
        // L'appel attendu, avec ses arguments
        JSInterop.SetupVoid("navigator.clipboard.writeText", "https://arbitres.ca/matchs/1");
        var cut = Render<GameCard>(parameters => parameters
            .Add(p => p.Game, FullGame));

        cut.Find("button.copy-link").Click();

        JSInterop.VerifyInvoke("navigator.clipboard.writeText");
    }
}
