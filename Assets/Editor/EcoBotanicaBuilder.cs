#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Constrói toda a hierarquia da cena EcoBotanica via menu
public class EcoBotanicaBuilder : EditorWindow
{
    [MenuItem("EcoBotanica/1. Construir Cena Completa")]
    public static void ConstruirCena()
    {
        CriarManagement();
        CriarJogador();
        CriarAmbiente();
        CriarInterativos();

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[EcoBotanicaBuilder] Hierarquia criada! Execute os demais menus em ordem.");
    }

    // ── [--- MANAGEMENT ---] ───────────────────────────────────────

    static void CriarManagement()
    {
        var raiz = new GameObject("[--- MANAGEMENT ---]");

        // GerenciadorJardim (Singleton)
        var gjObj = new GameObject("GerenciadorJardim");
        gjObj.transform.SetParent(raiz.transform);
        gjObj.AddComponent<GerenciadorJardim>();

        // EventSystem
        var esObj = new GameObject("EventSystem");
        esObj.transform.SetParent(raiz.transform);
        esObj.AddComponent<EventSystem>();
        esObj.AddComponent<StandaloneInputModule>();

        // HUD_Canvas (WorldSpace — segue a câmera via HUDView.LateUpdate)
        var canvasObj = new GameObject("HUD_Canvas");
        canvasObj.transform.SetParent(raiz.transform);
        var canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.WorldSpace;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.transform.localPosition = new Vector3(0f, 1.5f, 2f);
        canvasObj.transform.localScale    = Vector3.one * 0.002f;

        CriarTMP(canvasObj, "Texto_Pontuacao",  new Vector2(0,  90), "Pontos: 0");
        CriarTMP(canvasObj, "Texto_Flores",     new Vector2(0,  30), "Flores: 0/5");
        CriarTMP(canvasObj, "Texto_Mensagem",   new Vector2(0, -30), "Bem-vindo ao EcoBotanica!");
        CriarTMP(canvasObj, "Texto_Estado",     new Vector2(0, -90), "");

        canvasObj.AddComponent<HUDView>();
    }

    static void CriarTMP(GameObject pai, string nome, Vector2 pos, string conteudo)
    {
        var obj = new GameObject(nome);
        obj.transform.SetParent(pai.transform, false);
        var tmp = obj.AddComponent<TextMeshProUGUI>();
        tmp.text      = conteudo;
        tmp.fontSize  = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color     = Color.white;
        var rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = pos;
        rt.sizeDelta        = new Vector2(500, 60);
    }

    // ── [--- PLAYER ---] ───────────────────────────────────────────

    static void CriarJogador()
    {
        var camPadrao = GameObject.Find("Main Camera");
        if (camPadrao != null) Object.DestroyImmediate(camPadrao);

        var raiz = new GameObject("[--- PLAYER ---]");

        var xrOrigin = new GameObject("XROrigin");
        xrOrigin.transform.SetParent(raiz.transform);
        xrOrigin.transform.position = Vector3.zero;
        xrOrigin.tag = "Player";
        xrOrigin.AddComponent<JogadorController>();

        var camObj = new GameObject("Main Camera");
        camObj.transform.SetParent(xrOrigin.transform);
        camObj.transform.localPosition = new Vector3(0f, 1.7f, 0f);
        camObj.transform.localRotation = Quaternion.identity;
        var cam = camObj.AddComponent<Camera>();
        cam.tag = "MainCamera";
        camObj.AddComponent<AudioListener>();
    }

    // ── [--- ENVIRONMENT ---] ──────────────────────────────────────

    static void CriarAmbiente()
    {
        var raiz = new GameObject("[--- ENVIRONMENT ---]");

        // Gramado
        var gramado = GameObject.CreatePrimitive(PrimitiveType.Plane);
        gramado.name = "Plano_Gramado";
        gramado.transform.SetParent(raiz.transform);
        gramado.transform.localScale = new Vector3(5f, 1f, 5f);

        // Luz direcional (tom dourado, tarde)
        var luz = new GameObject("Directional Light");
        luz.transform.SetParent(raiz.transform);
        var lComp = luz.AddComponent<Light>();
        lComp.type      = LightType.Directional;
        lComp.intensity = 1.2f;
        lComp.color     = new Color(1f, 0.95f, 0.85f);
        luz.transform.rotation = Quaternion.Euler(45f, -35f, 0f);

        // Pavilhão central
        var pavilhao = new GameObject("Pavilhao");
        pavilhao.transform.SetParent(raiz.transform);
        new GameObject("Colunas") { transform = { parent = pavilhao.transform } };
        new GameObject("Telhado") { transform = { parent = pavilhao.transform } };

        // Banco de jardim
        var banco = new GameObject("Banco");
        banco.transform.SetParent(raiz.transform);
        new GameObject("Assento") { transform = { parent = banco.transform } };
        new GameObject("Pes")     { transform = { parent = banco.transform } };

        // Árvores
        var arvores = new GameObject("Arvores");
        arvores.transform.SetParent(raiz.transform);
        for (int i = 1; i <= 4; i++)
        {
            var arv = new GameObject($"Arvore_0{i}");
            arv.transform.SetParent(arvores.transform);
            new GameObject("Tronco") { transform = { parent = arv.transform } };
            new GameObject("Copa")   { transform = { parent = arv.transform } };
        }

        // Poste de luz
        var poste = new GameObject("Poste_Luz");
        poste.transform.SetParent(raiz.transform);
        new GameObject("Haste")   { transform = { parent = poste.transform } };
        new GameObject("Lampada") { transform = { parent = poste.transform } };

        // Estufa (greenhouse)
        var estufa = new GameObject("Estufa");
        estufa.transform.SetParent(raiz.transform);
        new GameObject("Estrutura")   { transform = { parent = estufa.transform } };
        new GameObject("Porta_Pivot") { transform = { parent = estufa.transform } };
    }

    // ── [--- INTERACTABLES ---] ────────────────────────────────────

    static void CriarInterativos()
    {
        var raiz = new GameObject("[--- INTERACTABLES ---]");

        // Cinco flores coletáveis
        string[] nomes = { "Rosa", "Tulipa", "Orquidea", "Girassol", "Lavanda" };
        Vector3[] pos  = {
            new Vector3(-3f, 0.5f,  2f),
            new Vector3( 3f, 0.5f,  2f),
            new Vector3(-3f, 0.5f, -2f),
            new Vector3( 3f, 0.5f, -2f),
            new Vector3( 0f, 0.5f,  4f),
        };

        for (int i = 0; i < 5; i++)
        {
            var flor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flor.name = $"Flor_Coletavel_0{i + 1}";
            flor.transform.SetParent(raiz.transform);
            flor.transform.position   = pos[i];
            flor.transform.localScale = Vector3.one * 0.25f;
            flor.AddComponent<FlorescenteView>();
            flor.AddComponent<FlorescenteController>();
        }

        // Fonte central interativa
        var fonte = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        fonte.name = "Fonte_Principal";
        fonte.transform.SetParent(raiz.transform);
        fonte.transform.position   = new Vector3(0f, 0.4f, 0f);
        fonte.transform.localScale = new Vector3(1.5f, 0.4f, 1.5f);
        fonte.AddComponent<FonteView>();
        fonte.AddComponent<FonteController>();

        // Painéis botânicos (3 plantas informativas separadas das coletáveis)
        string[] nomesInfo = { "Bambu", "Heliconia", "Bromelia" };
        Vector3[] posInfo  = {
            new Vector3( 6f, 1f,  3f),
            new Vector3(-6f, 1f,  3f),
            new Vector3( 0f, 1f, -6f),
        };

        for (int i = 0; i < 3; i++)
        {
            var planta = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            planta.name = $"Planta_Info_0{i + 1}";
            planta.transform.SetParent(raiz.transform);
            planta.transform.position   = posInfo[i];
            planta.transform.localScale = new Vector3(0.4f, 1f, 0.4f);
            planta.AddComponent<PainelBotanicoController>();

            // Painel flutuante acima da planta
            var painelObj = new GameObject($"Painel_0{i + 1}");
            painelObj.transform.SetParent(planta.transform);
            painelObj.transform.localPosition = new Vector3(0f, 2.5f, 0f);
            painelObj.transform.localScale    = Vector3.one * 0.01f;

            var canvas = painelObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            painelObj.AddComponent<CanvasScaler>();
            painelObj.AddComponent<GraphicRaycaster>();

            CriarTMP(painelObj, "Painel_Nome",       new Vector2(0,  60), nomesInfo[i]);
            CriarTMP(painelObj, "Painel_Cientifico", new Vector2(0,   0), "...");
            CriarTMP(painelObj, "Painel_Descricao",  new Vector2(0, -60), "...");

            painelObj.AddComponent<PainelBotanicoView>();
        }
    }
}
#endif
