using UnityEngine;

// Dados puros de uma planta — sem MonoBehaviour, sem lógica de UI
[System.Serializable]
public class PlantaModel
{
    public string nomeComum;
    public string nomeCientifico;
    public string descricao;
    public int pontos;
    public Color cor;

    public PlantaModel(string nomeComum, string nomeCientifico, string descricao, int pontos, Color cor)
    {
        this.nomeComum      = nomeComum;
        this.nomeCientifico = nomeCientifico;
        this.descricao      = descricao;
        this.pontos         = pontos;
        this.cor            = cor;
    }
}
