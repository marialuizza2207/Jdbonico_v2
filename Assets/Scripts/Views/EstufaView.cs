using UnityEngine;

// Anima a abertura e fechamento da porta da estufa
public class EstufaView : MonoBehaviour
{
    // A porta deve ser um filho deste objeto com pivot na dobradiça
    [SerializeField] public Transform porta;
    [SerializeField] float anguloAberto   = -90f;
    [SerializeField] float velocidadeAnim = 80f;

    private float anguloAlvo  = 0f;
    private float anguloAtual = 0f;

    void Update()
    {
        if (porta == null) return;

        anguloAtual = Mathf.MoveTowards(anguloAtual, anguloAlvo, velocidadeAnim * Time.deltaTime);
        porta.localRotation = Quaternion.Euler(0f, anguloAtual, 0f);
    }

    public void Abrir()   => anguloAlvo = anguloAberto;
    public void Fechar()  => anguloAlvo = 0f;
    public bool EstaAberta => Mathf.Abs(anguloAtual - anguloAberto) < 2f;
}
