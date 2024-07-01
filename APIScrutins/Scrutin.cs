using System.Collections.ObjectModel;

namespace APIScrutins;

public class Scrutin
{
    public int Tour { get; private set; }
    public bool Cloture { get; private set; }
    public bool Debute { get; private set; }
    
    
    private Dictionary<Candidat, int> _votesTour1 = new ();
    private Dictionary<Candidat, int> _votesTour2 = new ();

    private Dictionary<Candidat, int> _votes(int tour = -1)
    {
        if (tour == -1) tour = Tour;

        if (tour == 1) return _votesTour1;
        return _votesTour2;
    }
    
    public IReadOnlyDictionary<Candidat, int> Votes(int tour = -1)
    {
        return new ReadOnlyDictionary<Candidat, int>(_votes(tour));
    }

    private List<Candidat> _candidatsTour1 = new ();
    private List<Candidat> _candidatsTour2 = new ();
    private List<Candidat> _candidats(int tour = -1)
    {
        if (tour == -1) tour = Tour;

        if (tour == 1) return _candidatsTour1;
        return _candidatsTour2;
    }

    public IReadOnlyList<Candidat> Candidats(int tour = -1)
    {
        return new ReadOnlyCollection<Candidat>(_candidats(tour));
    }
    
    public Scrutin()
    {
        Cloture = false;
        Tour = 1;
    }
    
    public void AddCandidat(Candidat candidat)
    {
        if (Debute) throw new Exception("Impossible d'ajouter de nouveaux candidats après le début du scrutin");
        
        _candidats(1).Add(candidat);
        _votes(1).Add(candidat, 0);
    }
    
    public void RemoveCandidat(Candidat candidat)
    {
        if (Debute) throw new Exception("Impossible de retirer un candidats après le début du scrutin");
        
        _candidats(1).Remove(candidat);
        _votes(1).Remove(candidat);
    }

    private float PourcentageCandidat(Candidat candidat, int tour = -1)
    {
        if (tour == -1) tour = Tour;

        var votesTour = _votes(tour);
        
        int votesCandidat = votesTour[candidat];
        int total = votesTour.Values.Sum();
        float pourcentageCandidat = ((float) votesCandidat / total) * 100.0f;
        return pourcentageCandidat;
    }
    
    private Dictionary<Candidat, float> PourcentagesAll(int tour = -1)
    {
        if (tour == -1) tour = Tour;

        Dictionary<Candidat, float> pourcentages = new Dictionary<Candidat, float>();
        foreach (var candidat in _candidats(tour))
        {
            pourcentages.Add(candidat, PourcentageCandidat(candidat, tour));
        }

        return pourcentages;
    }
    
    public void Voter(Candidat candidat, int votes = 1)
    {
        if (Cloture) throw new Exception("Impossible de voter après la fin du scrutin");
        
        _votes()[candidat] += votes;
    }

    public void Debuter()
    {
        Debute = true;
    }

    public Candidat? GetCandidat(string nom)
    {
        foreach (var candidat in Candidats(1))
        {
            if(candidat.Nom == nom) return candidat;
        }

        return null;
    }
    
    public void CloturerTour()
    {
        if (Cloture) throw new Exception("Impossible de cloturer le tour d'un scrutin qui l'est déjà");
        if (Tour == 1) _cloturerPremierTour();
        else if (Tour == 2)_cloturerSecondTour();
    }

    private void _cloturerPremierTour()
    {
        if (Gagnant(1) != null)
        {
            Cloture = true;
            return;
        }

        var sortedCandidats = _votes(1)
            .OrderBy(x=>x.Value)
            .Select(x=>x.Key)
            .ToList();
        sortedCandidats.Reverse();
        
        _candidatsTour2.Add(sortedCandidats[0]);
        _votesTour2.Add(sortedCandidats[0], 0);

        var votesCandidat2 = Votes(1)[sortedCandidats[1]];
        var votesCandidat3 = Votes(1)[sortedCandidats[2]];
        
        //Candidats 2 & 3 à égalités, on départage aléatoirement
        if (votesCandidat2 == votesCandidat3)
        {
            Random random = new Random();
            if (random.Next(2) == 0)
            {
                _candidatsTour2.Add(sortedCandidats[1]);
                _votesTour2.Add(sortedCandidats[1], 0);
            }
            else
            {
                _candidatsTour2.Add(sortedCandidats[2]);
                _votesTour2.Add(sortedCandidats[2], 0);
            }
        }
        else if (votesCandidat2 > votesCandidat3)
        {
            _candidatsTour2.Add(sortedCandidats[1]);
            _votesTour2.Add(sortedCandidats[1], 0);
        }
        else
        {
            _candidatsTour2.Add(sortedCandidats[2]);
            _votesTour2.Add(sortedCandidats[2], 0);
        }

        
        Tour += 1;
    }

    private void _cloturerSecondTour()
    {
        Cloture = true;
    }

    private Candidat? Gagnant(int tour)
    {
        if (tour == -1) tour = Tour;

        if (tour == 1)
        {
            foreach (var candidat in PourcentagesAll(tour))
            {
                if (candidat.Value > 50.0f) return candidat.Key;
            }
            return null;
        }
        
        if(tour == 2)
        {
            Candidat candidat1 = _candidats(tour)[0];
            Candidat candidat2 = _candidats(tour)[1];

            int votesCandidat1 = _votes(tour)[candidat1];
            int votesCandidat2 = _votes(tour)[candidat2];

            if (votesCandidat1 > votesCandidat2) return candidat1;
            if (votesCandidat2 > votesCandidat1) return candidat2;
            return null;
        }

        throw new Exception("Tour should be 1 or 2");
    }
    
    public ResultatTourScrutin Resultats(int tour = -1)
    {
        if (tour == -1) tour = Tour;
        
        //Les résultat du tour demandé ne sont pas encore disponible
        if ((tour == 1 && Tour == 1 && !Cloture) || (tour == 2 && Tour == 2 && !Cloture))
        {
            throw new Exception("Impossible d'obtenir le résultat avant la fin du tour"); 
        }

        return new ResultatTourScrutin(Gagnant(tour)!, PourcentagesAll(tour), tour);
    }
}