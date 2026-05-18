using UnityEngine;
using UnityEngine.InputSystem;

// Movimentação WASD + detecção de objetos interativos por proximidade
public class JogadorController : MonoBehaviour
{
    [SerializeField] public Camera referenciaCamera;
    [SerializeField] float velocidade         = 3f;
    [SerializeField] float raioColeta         = 1.5f;
    [SerializeField] float raioInter          = 1.8f;
    [SerializeField] float sensibilidadeMouse = 120f;
    [SerializeField] float raioFonte          = 4f;

    private CharacterController      cc           = null;
    private FonteController          fonteAtual   = null;
    private PainelBotanicoController painelAtual  = null;
    private EstufaController         estufaAtual  = null;
    private float                    velocidadeY  = 0f;

    void Awake() => cc = GetComponent<CharacterController>();

    void Update()
    {
        Virar();
        Mover();
        DetectarProximidade();
        ProcessarInputInteracao();
    }

    void Virar()
    {
        var mouse = Mouse.current;
        if (mouse == null || !mouse.rightButton.isPressed) return;

        float deltaX = mouse.delta.x.ReadValue();
        transform.Rotate(Vector3.up, deltaX * sensibilidadeMouse * Time.deltaTime, Space.World);
    }

    void Mover()
    {
        float h = 0f, v = 0f;
        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed)  h += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)   h -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)     v += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)   v -= 1f;

        if (cc != null)
        {
            // Gravidade para manter o player no chão
            velocidadeY = cc.isGrounded ? -0.5f : velocidadeY + Physics.gravity.y * Time.deltaTime;

            Vector3 movimento = (transform.right * h + transform.forward * v).normalized
                                * velocidade
                                + Vector3.up * velocidadeY;
            cc.Move(movimento * Time.deltaTime);
        }
        else
        {
            // Fallback sem CharacterController
            transform.Translate(new Vector3(h, 0f, v) * velocidade * Time.deltaTime, Space.Self);
        }
    }

    void DetectarProximidade()
    {
        // Coleta automática de flores
        foreach (var col in Physics.OverlapSphere(transform.position, raioColeta))
            col.GetComponent<FlorescenteController>()?.TentarColetar();

        // Hover da fonte
        FonteController fonteEncontrada = null;
        PainelBotanicoController painelEncontrado = null;
        EstufaController estufaEncontrada = null;

        foreach (var col in Physics.OverlapSphere(transform.position, raioFonte))
            if (fonteEncontrada == null) fonteEncontrada = col.GetComponent<FonteController>();

        foreach (var col in Physics.OverlapSphere(transform.position, raioInter))
        {
            if (painelEncontrado == null) painelEncontrado = col.GetComponent<PainelBotanicoController>();
            if (estufaEncontrada == null) estufaEncontrada = col.GetComponent<EstufaController>();
        }

        TrocarAlvo(ref fonteAtual, fonteEncontrada,
            () => fonteAtual.AoEntrarHover(),
            () => fonteAtual.AoSairHover());

        TrocarAlvo(ref painelAtual, painelEncontrado,
            () => painelAtual.AoEntrarProximidade(),
            () => painelAtual.AoSairProximidade());

        TrocarAlvo(ref estufaAtual, estufaEncontrada,
            () => estufaAtual.AoEntrarProximidade(),
            () => estufaAtual.AoSairProximidade());
    }

    void ProcessarInputInteracao()
    {
        var kb = Keyboard.current;
        if (kb == null || !kb.eKey.wasPressedThisFrame) return;

        fonteAtual?.AoAtivar();
        estufaAtual?.AoAtivar();
        painelAtual?.AoAtivar();
    }

    // Troca suavemente o alvo atual, acionando enter/exit
    static void TrocarAlvo<T>(ref T atual, T novo, System.Action onEnter, System.Action onExit)
        where T : class
    {
        if (novo == atual) return;
        if (atual != null) onExit();
        atual = novo;
        if (atual != null) onEnter();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, raioColeta);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, raioInter);
    }
}
