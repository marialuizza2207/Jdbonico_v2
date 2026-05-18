using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// Lógica da fonte: hover por proximidade e ativação por tecla E ou XR
public class FonteController : MonoBehaviour
{
    private FonteView view;
    private bool ativa = false;

    void Awake() => view = GetComponent<FonteView>();

    public void AoEntrarHover()
    {
        view?.SetHover(true);
        GerenciadorJardim.Instancia?.hud?.ExibirMensagem("Pressione E para ativar a fonte!");
    }

    public void AoSairHover()
    {
        view?.SetHover(false);
        GerenciadorJardim.Instancia?.hud?.ExibirMensagem("Explore o jardim e colete as flores!");
    }

    public void AoAtivar()
    {
        if (ativa) return;
        ativa = true;
        view?.SetAtivo();
        GerenciadorJardim.Instancia?.RegistrarAtivacaoFonte();
    }

    // Eventos XR (conectados via AdicionarComponentes.cs)
    public void AoEntrarHoverXR(HoverEnterEventArgs args) => AoEntrarHover();
    public void AoSairHoverXR(HoverExitEventArgs args)    => AoSairHover();
    public void AoAtivarXR(SelectEnterEventArgs args)     => AoAtivar();
}
