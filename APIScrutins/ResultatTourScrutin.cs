namespace APIScrutins;

public class ResultatTourScrutin
{
    public Candidat? Gagnant { get; }
    public Dictionary<Candidat, float> Pourcentages { get; }
    public int Tour { get; }

    public ResultatTourScrutin(Candidat gagnant, Dictionary<Candidat, float> pourcentages, int tour = -1)
    {
        Gagnant = gagnant;
        Pourcentages = pourcentages;
        Tour = tour;
    }
}