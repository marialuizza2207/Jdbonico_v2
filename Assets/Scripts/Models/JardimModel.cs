public class JardimModel
{
    public int  Pontuacao       { get; private set; }
    public int  FloresColetadas { get; private set; }
    public int  TotalFlores     { get; }
    public bool FonteAtivada    { get; private set; }

    public JardimModel(int totalFlores)
    {
        TotalFlores = totalFlores;
    }

    public void RegistrarColeta(int pontos)
    {
        Pontuacao += pontos;
        FloresColetadas++;
    }

    public void AtivarFonte()
    {
        FonteAtivada = true;
    }

    public bool JardimCompleto => FloresColetadas >= TotalFlores;
}
