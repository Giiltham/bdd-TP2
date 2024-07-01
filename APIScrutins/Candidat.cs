namespace APIScrutins;

public class Candidat
{
    public string Nom { get; }

    public Candidat(string nom)
    {
        this.Nom = nom;
    }

    public override string ToString()
    {
        return Nom;
    }
}