using UnityEngine;

// Feedback visual da flor coletável: oscilação senoidal + rotação contínua
public class FlorescenteView : MonoBehaviour
{
    [SerializeField] float velocidadeOscilacao = 1.5f;
    [SerializeField] float amplitudeOscilacao  = 0.15f;

    private Vector3 posicaoInicial;
    private bool ativa = true;

    void Start() => posicaoInicial = transform.position;

    void Update()
    {
        if (!ativa) return;
        float offsetY = Mathf.Sin(Time.time * velocidadeOscilacao) * amplitudeOscilacao;
        transform.position = posicaoInicial + new Vector3(0f, offsetY, 0f);
        transform.Rotate(Vector3.up, 45f * Time.deltaTime, Space.World);
    }

    public void Coletar()
    {
        ativa = false;
        gameObject.SetActive(false);
    }
}
