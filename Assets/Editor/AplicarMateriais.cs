#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

// Cria e aplica materiais coloridos, configura skybox e posições finais
public class AplicarMateriais : EditorWindow
{
    [MenuItem("EcoBotanica/5. Aplicar Materiais e Skybox")]
    public static void Aplicar()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit")
                  ?? Shader.Find("Standard");
        if (shader == null) { Debug.LogError("[AplicarMateriais] Nenhum shader encontrado!"); return; }

        System.IO.Directory.CreateDirectory("Assets/Materials");

        // Limpa materiais antigos
        foreach (var guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials" }))
            AssetDatabase.DeleteAsset(AssetDatabase.GUIDToAssetPath(guid));

        Material CriarMat(string nome, Color cor)
        {
            var mat = new Material(shader) { name = nome };
            mat.color = cor;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", cor);
            AssetDatabase.CreateAsset(mat, $"Assets/Materials/{nome}.mat");
            return mat;
        }

        var matGramado  = CriarMat("Mat_Gramado",   new Color(0.20f, 0.55f, 0.20f));
        var matColuna   = CriarMat("Mat_Coluna",    new Color(0.95f, 0.93f, 0.88f));
        var matTelhado  = CriarMat("Mat_Telhado",   new Color(0.55f, 0.35f, 0.15f));
        var matBanco    = CriarMat("Mat_Banco",     new Color(0.45f, 0.25f, 0.10f));
        var matTronco   = CriarMat("Mat_Tronco",    new Color(0.42f, 0.26f, 0.12f));
        var matCopa     = CriarMat("Mat_Copa",      new Color(0.15f, 0.55f, 0.15f));
        var matEstufa   = CriarMat("Mat_Estufa",    new Color(0.70f, 0.90f, 0.70f));
        var matVidro    = CriarMat("Mat_Vidro",     new Color(0.60f, 0.85f, 0.80f));
        var matPorta    = CriarMat("Mat_Porta",     new Color(0.55f, 0.35f, 0.15f));
        var matFonte    = CriarMat("Mat_Fonte",     new Color(0.20f, 0.60f, 1.00f));
        var matRosa     = CriarMat("Mat_Rosa",      new Color(1.00f, 0.40f, 0.60f));
        var matTulipa   = CriarMat("Mat_Tulipa",    new Color(1.00f, 0.65f, 0.00f));
        var matOrquidea = CriarMat("Mat_Orquidea",  new Color(0.70f, 0.00f, 0.80f));
        var matGirassol = CriarMat("Mat_Girassol",  new Color(1.00f, 0.85f, 0.00f));
        var matLavanda  = CriarMat("Mat_Lavanda",   new Color(0.65f, 0.50f, 0.90f));
        var matBambu    = CriarMat("Mat_Bambu",     new Color(0.40f, 0.70f, 0.20f));
        var matHeliconia= CriarMat("Mat_Heliconia", new Color(1.00f, 0.30f, 0.10f));
        var matBromelia = CriarMat("Mat_Bromelia",  new Color(0.85f, 0.15f, 0.50f));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        void Apr(Renderer r, Material m) { if (r && m) { r.sharedMaterial = m; EditorUtility.SetDirty(r.gameObject); } }

        // Gramado
        var gramado = GameObject.Find("Plano_Gramado");
        if (gramado) { gramado.transform.position = Vector3.zero; gramado.transform.localScale = new Vector3(4f, 1f, 4f); Apr(gramado.GetComponent<Renderer>(), matGramado); }

        // Pavilhão
        var pavilhao = GameObject.Find("Pavilhao");
        if (pavilhao)
        {
            foreach (var r in pavilhao.GetComponentsInChildren<Renderer>())
            {
                if      (r.name.Contains("Coluna"))  Apr(r, matColuna);
                else if (r.name.Contains("Telhado")) Apr(r, matTelhado);
            }
        }

        // Banco
        var banco = GameObject.Find("Banco");
        if (banco) foreach (var r in banco.GetComponentsInChildren<Renderer>()) Apr(r, matBanco);

        // Árvores
        for (int i = 1; i <= 4; i++)
        {
            var arv = GameObject.Find($"Arvore_0{i}");
            if (arv == null) continue;
            foreach (var r in arv.GetComponentsInChildren<Renderer>())
            {
                if      (r.name.Contains("Tronco")) Apr(r, matTronco);
                else if (r.name.Contains("Copa"))   Apr(r, matCopa);
            }
        }

        // Estufa
        var estufa = GameObject.Find("Estufa");
        if (estufa)
        {
            foreach (var r in estufa.GetComponentsInChildren<Renderer>())
            {
                if      (r.name.Contains("Porta"))  Apr(r, matPorta);
                else if (r.name.Contains("Teto"))   Apr(r, matVidro);
                else if (r.name.Contains("Parede")) Apr(r, matEstufa);
            }
        }

        // Fonte
        var fonte = GameObject.Find("Fonte_Principal");
        if (fonte) { fonte.transform.position = new Vector3(0f, 0.4f, 0f); fonte.transform.localScale = new Vector3(1.5f, 0.4f, 1.5f); Apr(fonte.GetComponent<Renderer>(), matFonte); }

        // Flores coletáveis
        Material[] matsFlores = { matRosa, matTulipa, matOrquidea, matGirassol, matLavanda };
        Vector3[] posFlores = { new Vector3(-3,0.5f,2), new Vector3(3,0.5f,2), new Vector3(-3,0.5f,-2), new Vector3(3,0.5f,-2), new Vector3(0,0.5f,4) };
        for (int i = 1; i <= 5; i++)
        {
            var f = GameObject.Find($"Flor_Coletavel_0{i}");
            if (f == null) continue;
            f.transform.position = posFlores[i - 1];
            f.transform.localScale = Vector3.one * 0.25f;
            Apr(f.GetComponent<Renderer>(), matsFlores[i - 1]);
        }

        // Plantas informativas
        Material[] matsPlantas = { matBambu, matHeliconia, matBromelia };
        for (int i = 1; i <= 3; i++)
        {
            var p = GameObject.Find($"Planta_Info_0{i}");
            if (p == null) continue;
            Apr(p.GetComponent<Renderer>(), matsPlantas[i - 1]);
        }

        // Skybox procedural (céu azul com sol)
        var skyShader = Shader.Find("Skybox/Procedural");
        if (skyShader != null)
        {
            var sky = new Material(skyShader);
            sky.SetFloat("_SunDisk", 2);
            sky.SetFloat("_SunSize", 0.04f);
            sky.SetFloat("_AtmosphereThickness", 1.0f);
            sky.SetColor("_SkyTint",    new Color(0.53f, 0.81f, 0.98f));
            sky.SetColor("_GroundColor", new Color(0.37f, 0.55f, 0.27f));
            sky.SetFloat("_Exposure", 1.3f);
            RenderSettings.skybox      = sky;
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
            DynamicGI.UpdateEnvironment();
        }

        // HUD_Canvas
        var hud = GameObject.Find("HUD_Canvas");
        if (hud) { hud.transform.position = new Vector3(0f, 1.8f, 3f); hud.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f); EditorUtility.SetDirty(hud); }

        // XROrigin
        var xr = GameObject.Find("XROrigin");
        if (xr) { xr.transform.position = Vector3.zero; EditorUtility.SetDirty(xr); }

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[AplicarMateriais] Materiais e skybox aplicados! Salve com Ctrl+S.");
    }
}
#endif
