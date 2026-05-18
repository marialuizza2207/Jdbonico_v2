using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Exibe informações botânicas quando o jogador se aproxima da planta
public class PainelBotanicoController : MonoBehaviour
{
    [SerializeField] public PlantaModel dadosPlanta;
    [SerializeField] public PainelBotanicoView painel;

    private bool painelVisivel = false;

    public void AoEntrarProximidade()
    {
        GerenciadorJardim.Instancia?.ExibirInfoPlanta(dadosPlanta);
        painel?.Mostrar(dadosPlanta);
        painelVisivel = true;
    }

    public void AoSairProximidade()
    {
        painel?.Esconder();
        painelVisivel = false;
        GerenciadorJardim.Instancia?.hud?.ExibirMensagem("Explore o jardim...");
    }

    // Pressionar E alterna visibilidade do painel quando próximo
    public void AoAtivar()
    {
        if (painelVisivel) painel?.Esconder();
        else               painel?.Mostrar(dadosPlanta);
        painelVisivel = !painelVisivel;
    }

    // Evento XR
    public void AoAtivarXR(SelectEnterEventArgs args) => AoAtivar();
}
