using UnityEngine;
using TMPro;

// Exibe pontuação, progresso e mensagens no HUD World Space (segue a câmera)
public class HUDView : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI textoPontuacao;
    [SerializeField] public TextMeshProUGUI textoFlores;
    [SerializeField] public TextMeshProUGUI textoMensagem;
    [SerializeField] public TextMeshProUGUI textoEstado;

    [SerializeField] float distancia    = 2f;
    [SerializeField] float alturaOffset = -0.15f;
    [SerializeField] float suavizacao   = 8f;

    private Transform camTransform;

    void Start()
    {
        BuscarCamera();
    }

    void BuscarCamera()
    {
        var cam = Camera.main;
        if (cam == null) cam = FindFirstObjectByType<Camera>();
        if (cam != null) camTransform = cam.transform;
    }

    void LateUpdate()
    {
        if (camTransform == null) { BuscarCamera(); return; }

        Vector3 forward = camTransform.forward;
        forward.y = 0f;
        if (forward.sqrMagnitude < 0.01f) forward = camTransform.forward; // olhando para cima/baixo
        forward.Normalize();

        Vector3 alvo = camTransform.position
            + forward * distancia
            + Vector3.up * alturaOffset;

        transform.position = Vector3.Lerp(transform.position, alvo, suavizacao * Time.deltaTime);
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(forward), suavizacao * Time.deltaTime);
    }

    public void AtualizarPontuacao(int pts)
    {
        if (textoPontuacao) textoPontuacao.text = $"Pontos: {pts}";
    }

    public void AtualizarFlores(int coletadas, int total)
    {
        if (textoFlores) textoFlores.text = $"Flores: {coletadas}/{total}";
    }

    public void ExibirMensagem(string msg)
    {
        if (textoMensagem) textoMensagem.text = msg;
    }

    public void AtualizarEstado(string estado)
    {
        if (textoEstado) textoEstado.text = estado;
    }
}
