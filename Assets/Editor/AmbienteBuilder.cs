#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class AmbienteBuilder : EditorWindow
{
    [MenuItem("EcoBotanica/2. Construir Ambiente (Pavilhão)")]
    public static void Build()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit")
                  ?? Shader.Find("Standard");

        Material Mat(Color c) { var m = new Material(shader); m.color = c; if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c); return m; }

        var matColuna   = Mat(new Color(0.95f, 0.93f, 0.88f));
        var matTelhado  = Mat(new Color(0.55f, 0.35f, 0.15f));
        var matTronco   = Mat(new Color(0.42f, 0.26f, 0.12f));
        var matCopa     = Mat(new Color(0.15f, 0.55f, 0.15f));

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

}

#endif
