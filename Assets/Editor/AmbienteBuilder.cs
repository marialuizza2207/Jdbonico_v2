#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

// Constrói pavilhão, estufa, árvores e poste com geometria real (meshes)
public class AmbienteBuilder : EditorWindow
{
    [MenuItem("EcoBotanica/2. Construir Ambiente (Pavilhão + Estufa)")]
    public static void Build()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit")
                  ?? Shader.Find("Standard");

        Material Mat(Color c) { var m = new Material(shader); m.color = c; if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c); return m; }

        var matColuna   = Mat(new Color(0.95f, 0.93f, 0.88f));
        var matTelhado  = Mat(new Color(0.55f, 0.35f, 0.15f));
        var matEstufa   = Mat(new Color(0.70f, 0.90f, 0.70f));
        var matVidro    = Mat(new Color(0.60f, 0.85f, 0.80f));
        var matTronco   = Mat(new Color(0.42f, 0.26f, 0.12f));
        var matCopa     = Mat(new Color(0.15f, 0.55f, 0.15f));

        // ── Pavilhão ──────────────────────────────────────────────
        var pavilhao = GameObject.Find("Pavilhao");
        if (pavilhao != null)
        {
            pavilhao.transform.position = new Vector3(0f, 0f, 10f);
            float larg = 8f, prof = 6f, alt = 3f, esp = 0.3f;
            Vector3 c = pavilhao.transform.position;

            var colunas = ObterOuCriar(pavilhao, "Colunas");
            Vector3[] posCols = {
                c + new Vector3(-larg/2, alt/2, -prof/2),
                c + new Vector3( larg/2, alt/2, -prof/2),
                c + new Vector3(-larg/2, alt/2,  prof/2),
                c + new Vector3( larg/2, alt/2,  prof/2),
            };
            string[] nomeCols = { "Coluna_FE", "Coluna_FD", "Coluna_TE", "Coluna_TD" };

            for (int i = 0; i < 4; i++)
            {
                if (colunas.transform.Find(nomeCols[i]) != null) continue;
                var col = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                col.name = nomeCols[i];
                col.transform.SetParent(colunas.transform);
                col.transform.position   = posCols[i];
                col.transform.localScale = new Vector3(esp, alt / 2f, esp);
                col.GetComponent<Renderer>().material = matColuna;
            }

            var telhado = ObterOuCriar(pavilhao, "Telhado");
            if (telhado.transform.Find("Telhado_Mesh") == null)
            {
                var tm = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tm.name = "Telhado_Mesh";
                tm.transform.SetParent(telhado.transform);
                tm.transform.position   = c + new Vector3(0, alt + 0.1f, 0);
                tm.transform.localScale = new Vector3(larg + esp, 0.2f, prof + esp);
                tm.GetComponent<Renderer>().material = matTelhado;
            }
        }

        // ── Banco ──────────────────────────────────────────────────
        var banco = GameObject.Find("Banco");
        if (banco != null)
        {
            banco.transform.position = new Vector3(0f, 0f, 5f);
            var assento = ObterOuCriar(banco, "Assento");
            if (assento.transform.Find("Assento_Mesh") == null)
            {
                var m = GameObject.CreatePrimitive(PrimitiveType.Cube);
                m.name = "Assento_Mesh";
                m.transform.SetParent(assento.transform);
                m.transform.localPosition = new Vector3(0, 0.5f, 0);
                m.transform.localScale    = new Vector3(2f, 0.1f, 0.5f);
                m.GetComponent<Renderer>().material = Mat(new Color(0.55f, 0.35f, 0.15f));
            }
        }

        // ── Árvores ───────────────────────────────────────────────
        Vector3[] posArv = {
            new Vector3(-6f, 0f, -5f),
            new Vector3( 6f, 0f, -5f),
            new Vector3(-6f, 0f,  8f),
            new Vector3( 6f, 0f,  8f),
        };

        for (int i = 1; i <= 4; i++)
        {
            var arv = GameObject.Find($"Arvore_0{i}");
            if (arv == null) continue;
            arv.transform.position = posArv[i - 1];

            var tronco = ObterOuCriar(arv, "Tronco");
            if (tronco.transform.Find("Tronco_Mesh") == null)
            {
                var m = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                m.name = "Tronco_Mesh";
                m.transform.SetParent(tronco.transform);
                m.transform.localPosition = new Vector3(0, 1f, 0);
                m.transform.localScale    = new Vector3(0.3f, 1f, 0.3f);
                m.GetComponent<Renderer>().material = matTronco;
            }

            var copa = ObterOuCriar(arv, "Copa");
            if (copa.transform.Find("Copa_Mesh") == null)
            {
                var m = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                m.name = "Copa_Mesh";
                m.transform.SetParent(copa.transform);
                m.transform.localPosition = new Vector3(0, 2.8f, 0);
                m.transform.localScale    = new Vector3(2f, 2f, 2f);
                m.GetComponent<Renderer>().material = matCopa;
            }
        }

        // ── Poste de Luz ──────────────────────────────────────────
        var poste = GameObject.Find("Poste_Luz");
        if (poste != null)
        {
            poste.transform.position = new Vector3(6f, 0f, 0f);
            var haste = ObterOuCriar(poste, "Haste");
            if (haste.transform.Find("Haste_Mesh") == null)
            {
                var hm = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                hm.name = "Haste_Mesh";
                hm.transform.SetParent(haste.transform);
                hm.transform.localPosition = new Vector3(0, 1.5f, 0);
                hm.transform.localScale    = new Vector3(0.08f, 1.5f, 0.08f);
                hm.GetComponent<Renderer>().material = Mat(new Color(0.2f, 0.2f, 0.2f));
            }

            var lampada = ObterOuCriar(poste, "Lampada");
            if (lampada.GetComponent<Light>() == null)
            {
                var luz = lampada.AddComponent<Light>();
                luz.type      = LightType.Point;
                luz.color     = new Color(1f, 0.95f, 0.75f);
                luz.intensity = 2f;
                luz.range     = 10f;
            }
            lampada.transform.localPosition = new Vector3(0, 3f, 0);
        }

        // ── Estufa ────────────────────────────────────────────────
        var estufa = GameObject.Find("Estufa");
        if (estufa != null)
        {
            estufa.transform.position = new Vector3(-8f, 0f, 5f);
            estufa.AddComponent<EstufaController>();

            var estrutura = ObterOuCriar(estufa, "Estrutura");

            // Paredes da estufa
            CriarParedeEstufa(estrutura, "Parede_F",  new Vector3(0, 1.5f, -1.5f), new Vector3(3f, 3f, 0.1f), matEstufa);
            CriarParedeEstufa(estrutura, "Parede_T",  new Vector3(0, 1.5f,  1.5f), new Vector3(3f, 3f, 0.1f), matEstufa);
            CriarParedeEstufa(estrutura, "Parede_E",  new Vector3(-1.5f, 1.5f, 0), new Vector3(0.1f, 3f, 3f), matEstufa);
            CriarParedeEstufa(estrutura, "Parede_D",  new Vector3( 1.5f, 1.5f, 0), new Vector3(0.1f, 3f, 3f), matEstufa);
            CriarParedeEstufa(estrutura, "Teto",      new Vector3(0, 3f, 0),       new Vector3(3f, 0.1f, 3f), matVidro);

            // Porta com pivot para animação
            var portaPivot = ObterOuCriar(estufa, "Porta_Pivot");
            portaPivot.transform.localPosition = new Vector3(-1.5f, 0, -1.5f);
            portaPivot.AddComponent<EstufaView>();

            if (portaPivot.transform.Find("Porta_Mesh") == null)
            {
                var pm = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pm.name = "Porta_Mesh";
                pm.transform.SetParent(portaPivot.transform);
                pm.transform.localPosition = new Vector3(0.5f, 1f, 0);
                pm.transform.localScale    = new Vector3(1f, 2f, 0.05f);
                pm.GetComponent<Renderer>().material = Mat(new Color(0.55f, 0.35f, 0.15f));
            }
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[AmbienteBuilder] Ambiente construído! Salve com Ctrl+S.");
    }

    static GameObject ObterOuCriar(GameObject pai, string nome)
    {
        var t = pai.transform.Find(nome);
        if (t != null) return t.gameObject;
        var go = new GameObject(nome);
        go.transform.SetParent(pai.transform);
        go.transform.localPosition = Vector3.zero;
        return go;
    }

    static void CriarParedeEstufa(GameObject pai, string nome, Vector3 posLocal, Vector3 escala, Material mat)
    {
        if (pai.transform.Find(nome) != null) return;
        var p = GameObject.CreatePrimitive(PrimitiveType.Cube);
        p.name = nome;
        p.transform.SetParent(pai.transform);
        p.transform.localPosition = posLocal;
        p.transform.localScale    = escala;
        p.GetComponent<Renderer>().material = mat;
    }
}
#endif
