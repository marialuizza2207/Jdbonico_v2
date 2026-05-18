using UnityEngine;

// Feedback visual da fonte: efeito de água via ParticleSystem
public class FonteView : MonoBehaviour
{
    [Tooltip("Posição local de onde a água sai (topo da fonte). Deixe (0,0,0) para auto.")]
    [SerializeField] Vector3 offsetAgua = Vector3.zero;

    private ParticleSystem agua;
    private bool ativa = false;

    void Start() => agua = CriarEfeitoAgua();

    public void SetHover(bool hover) { } // feedback via HUD, não visual aqui

    public void SetAtivo()
    {
        if (ativa) return;
        ativa = true;
        if (agua != null) agua.Play();
    }

    public void Desativar()
    {
        ativa = false;
        if (agua != null) agua.Stop();
    }

    ParticleSystem CriarEfeitoAgua()
    {
        // Calcula o topo do collider/renderer como ponto de emissão
        Vector3 posEmissao = transform.position + offsetAgua;
        if (offsetAgua == Vector3.zero)
        {
            var col = GetComponentInChildren<Collider>();
            if (col != null) posEmissao = col.bounds.center + Vector3.up * col.bounds.extents.y;
            else
            {
                var rend = GetComponentInChildren<Renderer>();
                if (rend != null) posEmissao = rend.bounds.center + Vector3.up * rend.bounds.extents.y;
            }
        }

        var go = new GameObject("EfeitoAgua");
        go.transform.SetParent(transform);
        go.transform.position = posEmissao;

        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // Main
        var main = ps.main;
        main.loop            = true;
        main.startLifetime   = new ParticleSystem.MinMaxCurve(2.2f, 3.0f);
        main.startSpeed      = new ParticleSystem.MinMaxCurve(6.0f, 9.0f);
        main.startSize       = new ParticleSystem.MinMaxCurve(0.04f, 0.08f);
        main.startColor      = new ParticleSystem.MinMaxGradient(
                                   new Color(0.5f, 0.85f, 1.0f, 0.85f),
                                   new Color(0.2f, 0.6f,  1.0f, 0.70f));
        main.gravityModifier = 1.2f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles    = 300;

        // Emissão
        var em = ps.emission;
        em.rateOverTime = 80f;

        // Forma: cone largo simulando jato de água
        var shape = ps.shape;
        shape.enabled   = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle     = 25f;
        shape.radius    = 0.05f;

        // Tamanho decresce ao cair
        var sizeOverLife = ps.sizeOverLifetime;
        sizeOverLife.enabled = true;
        var sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f, 1f);
        sizeCurve.AddKey(1f, 0.3f);
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // Opacidade: some ao final
        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        var grad = new Gradient();
        grad.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(new Color(0.6f, 0.9f, 1f), 0f),
                new GradientColorKey(new Color(0.3f, 0.7f, 1f), 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.9f, 0f),
                new GradientAlphaKey(0.0f, 1f)
            });
        colorOverLife.color = new ParticleSystem.MinMaxGradient(grad);

        // Renderer — shader URP particles
        var pr = go.GetComponent<ParticleSystemRenderer>();
        pr.material = new Material(
            Shader.Find("Universal Render Pipeline/Particles/Lit")
            ?? Shader.Find("Universal Render Pipeline/Lit")
            ?? Shader.Find("Standard"));

        return ps;
    }
}
