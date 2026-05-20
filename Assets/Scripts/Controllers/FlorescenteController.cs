using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FlorescenteController : MonoBehaviour
{
    public PlantaModel dados;

    private bool coletada = false;

    void Reset()
    {
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

    public void OnInteracaoXR(SelectEnterEventArgs args)
    {
        TentarColetar();
    }
}
