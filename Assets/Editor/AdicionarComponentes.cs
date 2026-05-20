#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class AdicionarComponentes : EditorWindow
{
    [MenuItem("EcoBotanica/3. Adicionar Componentes")]
    public static void AddComponents()
    {
        ConfigurarManagement();
        ConfigurarJogador();
        ConfigurarFlores();
        ConfigurarFonte();

        ConectarReferencias.Conectar();

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[AdicionarComponentes] Tudo configurado! Salve com Ctrl+S.");
    }

    static void ConfigurarManagement()
    {
        var gjGO = GameObject.Find("GerenciadorJardim");
        if (gjGO != null) { AddIfMissing<GerenciadorJardim>(gjGO); EditorUtility.SetDirty(gjGO); }

        var esGO = GameObject.Find("EventSystem");
        if (esGO != null)
        {
            AddIfMissing<EventSystem>(esGO);
            AddIfMissing<StandaloneInputModule>(esGO);
            EditorUtility.SetDirty(esGO);
        }

        var hudGO = GameObject.Find("HUD_Canvas");
        if (hudGO != null)
        {
            var canvas = AddIfMissing<Canvas>(hudGO);
            canvas.renderMode = RenderMode.WorldSpace;
            AddIfMissing<CanvasScaler>(hudGO);
            AddIfMissing<GraphicRaycaster>(hudGO);
            var hud = AddIfMissing<HUDView>(hudGO);

            hudGO.transform.position   = new Vector3(0f, 1.6f, 2f);
            hudGO.transform.localScale = new Vector3(0.002f, 0.002f, 0.002f);

            ConfigurarTMP("Texto_Pontuacao", "Pontos: 0");
            ConfigurarTMP("Texto_Flores",    "Flores: 0/5");
            ConfigurarTMP("Texto_Mensagem",  "Bem-vindo ao EcoBotanica!");
            ConfigurarTMP("Texto_Estado",    "");

            EditorUtility.SetDirty(hudGO);
        }
    }

    static void ConfigurarJogador()
    {
        var xrGO = GameObject.Find("XROrigin");
        if (xrGO == null) return;

        AddIfMissing<JogadorController>(xrGO);
        xrGO.tag = "Player";

        var camFilha = xrGO.GetComponentInChildren<Camera>();
        if (camFilha == null)
        {
            var camObj = new GameObject("Main Camera");
            camObj.transform.SetParent(xrGO.transform);
            camObj.transform.localPosition = new Vector3(0f, 1.7f, 0f);
            var cam = camObj.AddComponent<Camera>();
            cam.tag = "MainCamera";
            if (!camObj.GetComponent<AudioListener>()) camObj.AddComponent<AudioListener>();
            EditorUtility.SetDirty(camObj);
        }

        EditorUtility.SetDirty(xrGO);
    }

    static readonly (string nome, string nomeCientifico, string descricao, int pts, Color cor)[] dadosFlores =
    {
        ("Rosa",     "Rosa × hybrida",         "Símbolo do amor, cultivada há milênios.",           10, new Color(1.00f, 0.40f, 0.60f)),
        ("Tulipa",   "Tulipa gesneriana",       "Originária da Pérsia, floresceu na Europa no s.XVII.", 15, new Color(1.00f, 0.65f, 0.00f)),
        ("Orquídea", "Orchidaceae sp.",         "Família com +25.000 espécies, símbolo do luxo.",    25, new Color(0.70f, 0.00f, 0.80f)),
        ("Girassol", "Helianthus annuus",       "Sempre vira o caule em direção ao sol (heliotrismo).", 20, new Color(1.00f, 0.85f, 0.00f)),
        ("Lavanda",  "Lavandula angustifolia",  "Usada em aromaterapia por seu efeito relaxante.",   30, new Color(0.65f, 0.50f, 0.90f)),
    };

    static void ConfigurarFlores()
    {
        for (int i = 0; i < 5; i++)
        {
            var go = GameObject.Find($"Flor_Coletavel_0{i + 1}");
            if (go == null) continue;

            AddIfMissing<SphereCollider>(go);
            AddIfMissing<FlorescenteView>(go);
            var ctrl = AddIfMissing<FlorescenteController>(go);

            var (nome, nomeCientifico, descricao, pts, cor) = dadosFlores[i];
            ctrl.dados = new PlantaModel(nome, nomeCientifico, descricao, pts, cor);

            var xr = AddIfMissing<XRSimpleInteractable>(go);
            xr.selectEntered.AddListener(ctrl.OnInteracaoXR);

            EditorUtility.SetDirty(go);
        }
    }

    static void ConfigurarFonte()
    {
        var go = GameObject.Find("Fonte_Principal");
        if (go == null) return;

        AddIfMissing<CapsuleCollider>(go);
        AddIfMissing<FonteView>(go);
        var ctrl = AddIfMissing<FonteController>(go);

        var xr = AddIfMissing<XRSimpleInteractable>(go);
        xr.hoverEntered.AddListener(ctrl.AoEntrarHoverXR);
        xr.hoverExited.AddListener(ctrl.AoSairHoverXR);
        xr.selectEntered.AddListener(ctrl.AoAtivarXR);

        EditorUtility.SetDirty(go);
    }

    static void ConfigurarTMP(string goName, string textoInicial)
    {
        var go = GameObject.Find(goName);
        if (go == null) return;
        var tmp = go.GetComponent<TMPro.TextMeshProUGUI>();
        if (tmp == null) tmp = go.AddComponent<TMPro.TextMeshProUGUI>();
        tmp.text      = textoInicial;
        tmp.fontSize  = 32;
        tmp.alignment = TMPro.TextAlignmentOptions.Center;
        tmp.color     = Color.white;
        var rt = go.GetComponent<RectTransform>();
        if (rt) rt.sizeDelta = new Vector2(400f, 60f);
        EditorUtility.SetDirty(go);
    }

    static T AddIfMissing<T>(GameObject go) where T : Component
    {
        var comp = go.GetComponent<T>();
        if (comp == null) comp = go.AddComponent<T>();
        return comp;
    }
}
#endif
