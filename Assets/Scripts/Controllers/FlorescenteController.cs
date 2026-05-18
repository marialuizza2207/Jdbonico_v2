using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Lógica de coleta da flor — registra no Gerenciador e aciona a View
public class FlorescenteController : MonoBehaviour
{
    [SerializeField] public PlantaModel dados;

    private bool coletada = false;

    void Reset()
    {
        // Dados padrão ao adicionar o componente
        dados = new PlantaModel("Flor", "Species sp.", "Planta ornamental do jardim.", 10, Color.white);
    }

    public void TentarColetar()
    {
        if (coletada) return;
        coletada = true;

        GerenciadorJardim.Instancia?.RegistrarColeta(dados?.pontos ?? 10);
        GetComponent<FlorescenteView>()?.Coletar();
        if (GetComponent<FlorescenteView>() == null) gameObject.SetActive(false);
    }

    // Chamado pelo XRSimpleInteractable (controllers do Quest)
    public void OnInteracaoXR(SelectEnterEventArgs args) => TentarColetar();
}
