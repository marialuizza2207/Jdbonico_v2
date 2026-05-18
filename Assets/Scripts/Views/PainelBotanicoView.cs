using UnityEngine;
using TMPro;

// Painel flutuante que exibe informações botânicas quando o jogador se aproxima
public class PainelBotanicoView : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textoNome;
    [SerializeField] public TextMeshProUGUI textoCientifico;
    [SerializeField] public TextMeshProUGUI textoDescricao;

    private Transform camTransform;

    void Start()
    {
        var cam = Camera.main;
        if (cam != null) camTransform = cam.transform;
        gameObject.SetActive(false);
    }

    void LateUpdate()
    {
        // Painel sempre vira para a câmera
        if (camTransform != null)
            transform.LookAt(camTransform.position);
    }

    public void Mostrar(PlantaModel planta)
    {
        if (textoNome)       textoNome.text       = planta.nomeComum;
        if (textoCientifico) textoCientifico.text = planta.nomeCientifico;
        if (textoDescricao)  textoDescricao.text  = planta.descricao;
        gameObject.SetActive(true);
    }

    public void Esconder() => gameObject.SetActive(false);
}
