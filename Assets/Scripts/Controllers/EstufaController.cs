using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Abre/fecha a porta da estufa quando o jogador pressiona E ou usa XR
public class EstufaController : MonoBehaviour
{
    private EstufaView view;
    private bool aberta = false;

    void Awake() => view = GetComponentInChildren<EstufaView>();

    public void AoEntrarProximidade()
    {
        GerenciadorJardim.Instancia?.hud?.ExibirMensagem("Pressione E para abrir/fechar a estufa!");
    }

    public void AoSairProximidade()
    {
        GerenciadorJardim.Instancia?.hud?.ExibirMensagem("Explore o jardim...");
    }

    public void AoAtivar()
    {
        aberta = !aberta;
        if (aberta) view?.Abrir();
        else        view?.Fechar();
        GerenciadorJardim.Instancia?.AtualizarEstadoEstufa(aberta);
    }

    // Evento XR
    public void AoAtivarXR(SelectEnterEventArgs args) => AoAtivar();
}
