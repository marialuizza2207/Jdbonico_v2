using UnityEngine;

// Singleton central — coordena estado global do jardim e atualiza o HUD
public class GerenciadorJardim : MonoBehaviour
{
    public static GerenciadorJardim Instancia { get; private set; }

    [SerializeField] public HUDView hud;

    private JardimModel modelo;

    void Awake()
    {
        if (Instancia != null && Instancia != this) { Destroy(gameObject); return; }
        Instancia = this;
    }

    void Start()
    {
        int total = FindObjectsByType<FlorescenteController>(FindObjectsSortMode.None).Length;
        if (total == 0) total = 5; // fallback
        modelo = new JardimModel(totalFlores: total);
        AtualizarHUD();
        hud?.ExibirMensagem("Explore o jardim e colete as flores!");
    }

    // Chamado por FlorescenteController ao coletar uma flor
    public void RegistrarColeta(int pontos)
    {
        modelo.RegistrarColeta(pontos);
        AtualizarHUD();

        string msg = modelo.JardimCompleto
            ? "Parabéns! Você coletou todas as flores do jardim!"
            : $"Flor coletada! +{pontos} pontos";

        hud?.ExibirMensagem(msg);
    }

    // Chamado por FonteController ao ativar a fonte
    public void RegistrarAtivacaoFonte()
    {
        modelo.AtivarFonte();
        hud?.ExibirMensagem("Fonte ativada! A água jorra!");
        hud?.AtualizarEstado("Fonte: ativa");
    }

    // Chamado por EstufaController ao abrir/fechar a estufa
    public void AtualizarEstadoEstufa(bool aberta)
    {
        if (aberta) modelo.AbrirEstufa(); else modelo.FecharEstufa();
        hud?.AtualizarEstado(aberta ? "Estufa: aberta" : "Estufa: fechada");
        hud?.ExibirMensagem(aberta ? "Porta da estufa aberta!" : "Porta da estufa fechada.");
    }

    // Chamado por PainelBotanicoController
    public void ExibirInfoPlanta(PlantaModel planta)
    {
        hud?.ExibirMensagem($"{planta.nomeComum} — pressione E para mais info");
    }

    private void AtualizarHUD()
    {
        hud?.AtualizarPontuacao(modelo.Pontuacao);
        hud?.AtualizarFlores(modelo.FloresColetadas, modelo.TotalFlores);
    }
}
