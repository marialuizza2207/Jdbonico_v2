using UnityEngine;
using UnityEngine.InputSystem;

public class JogadorController : MonoBehaviour
{
    public Camera referenciaCamera;
    [SerializeField] float velocidade         = 3f;
    [SerializeField] float raioColeta         = 1.5f;
    [SerializeField] float sensibilidadeMouse = 120f;
    [SerializeField] float raioFonte          = 4f;

    private CharacterController cc         = null;
    private FonteController     fonteAtual = null;
    private float               velocidadeY = 0f;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

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
            velocidadeY = cc.isGrounded ? -0.5f : velocidadeY + Physics.gravity.y * Time.deltaTime;

            Vector3 movimento = (transform.right * h + transform.forward * v).normalized
                                * velocidade
                                + Vector3.up * velocidadeY;
            cc.Move(movimento * Time.deltaTime);
        }
        else
        {
            transform.Translate(new Vector3(h, 0f, v) * velocidade * Time.deltaTime, Space.Self);
        }
    }

    void DetectarProximidade()
    {
        foreach (var col in Physics.OverlapSphere(transform.position, raioColeta))
            col.GetComponent<FlorescenteController>()?.TentarColetar();

        FonteController fonteEncontrada = null;
        foreach (var col in Physics.OverlapSphere(transform.position, raioFonte))
            if (fonteEncontrada == null) fonteEncontrada = col.GetComponent<FonteController>();

        TrocarAlvo(ref fonteAtual, fonteEncontrada,
            () => fonteAtual.AoEntrarHover(),
            () => fonteAtual.AoSairHover());
    }

    void ProcessarInputInteracao()
    {
        var kb = Keyboard.current;
        if (kb == null || !kb.eKey.wasPressedThisFrame) return;

        fonteAtual?.AoAtivar();
    }

    static void TrocarAlvo<T>(ref T atual, T novo, System.Action onEnter, System.Action onExit)
        where T : class
    {
        if (novo == atual) return;
        if (atual != null) onExit();
        atual = novo;
        if (atual != null) onEnter();
    }

}
