using FluentAssertions;

namespace APIScrutins.Specs.Steps;

[Binding]
public sealed class ScrutinStepDefinitions
{
    // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

    private readonly ScenarioContext _scenarioContext;
    private Scrutin scrutin;
    
    public ScrutinStepDefinitions(ScenarioContext scenarioContext)
    {
        _scenarioContext = scenarioContext;
        scrutin = new Scrutin();
    }
    

    [Then(@"le gagnant doit être (.*)")]
    public void ThenLeGagnantDoitEtreCandidat(string candidat)
    {
        candidat.Should().Be(scrutin.Resultats().Gagnant!.Nom); 
    }

    [When(@"on debute le scrutin")]
    public void WhenOnDebuteLeScrutin()
    {
        scrutin.Debuter();
    }
    
    
    [When(@"on cloture le tour")]
    public void WhenOnClotureLeTour()
    {
        scrutin.CloturerTour();
    }

    [Given(@"candidats")]
    public void GivenCandidatsEtVotes(Table table)
    {
        foreach (var row in table.Rows)
        {
            Candidat candidat = new Candidat(row["Candidats"]);
            scrutin.AddCandidat(candidat);
        }
    }

    [When(@"on vote (.*) fois pour le (.*)")]
    public void WhenOnVoteFoisPourLeCandidat(int votes, string candidat)
    {
        scrutin.Voter(scrutin.GetCandidat(candidat)!, votes);
    }

    [When(@"on cloture le premier tour")]
    public void WhenOnClotureLePremierTour()
    {
        WhenOnClotureLeTour();
    }
    
    
    [When(@"on cloture le second tour")]
    public void WhenOnClotureLeSecondTour()
    {
        WhenOnClotureLeTour();
    }

    [Then(@"il n'y a pas de gagnant")]
    public void ThenIlNyAPasDeGagnant()
    {
        scrutin.Resultats().Gagnant.Should().BeNull(); 
    }

    [Then(@"obtenir les resultats du tour (.*) donne une erreur")]
    public void ThenObtenirLesResultatsDuTourDonneUneErreur(int tour)
    {
        scrutin.Invoking(x => x.Resultats(tour)).Should().Throw<Exception>()
            .WithMessage("Impossible d'obtenir le résultat avant la fin du tour");
    }

    [Then(@"seulement le (.*) ou le (.*) passe au second tour")]
    public void ThenSeulementLeCandidatOuLeCandidatPasseAuSecondTour(string candidat1, string candidat2)
    {
        var candidats = scrutin.Candidats();
        var candidat1Found = candidats.Contains(scrutin.GetCandidat(candidat1)); 
        var candidat2Found = candidats.Contains(scrutin.GetCandidat(candidat2));

        if (candidat1Found && candidat2Found)
        {
            throw new Exception("Les deux candidats sont présents au second tour");
        }
        
        if (!candidat1Found && !candidat2Found)
        {
            throw new Exception("aucun des deux candidats ne sont présents au second tour");
        }
    }
}