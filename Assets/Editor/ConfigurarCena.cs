using UnityEngine;
using UnityEditor;

// Configura bolinhas coletáveis e a nova fonte na cena
public class ConfigurarCena : EditorWindow
{
    private static GameObject novaFonte;

    [MenuItem("EcoBotanica/7. Configurar Bolinhas e Nova Fonte")]
    public static void Abrir() => GetWindow<ConfigurarCena>("Configurar Cena");

    void OnGUI()
    {
        GUILayout.Label("━━ Nova Fonte ━━", EditorStyles.boldLabel);
        novaFonte = (GameObject)EditorGUILayout.ObjectField(
            "Objeto da Fonte", novaFonte, typeof(GameObject), allowSceneObjects: true);

        EditorGUILayout.HelpBox(
            "Arraste aqui o GameObject da sua nova fonte (o modelo 3D que você importou).",
            MessageType.Info);

        if (GUILayout.Button("Configurar Fonte") && novaFonte != null)
            ConfigurarFonte(novaFonte);

        EditorGUILayout.Space(10);
        GUILayout.Label("━━ Bolinhas Coletáveis ━━", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "Selecione as bolinhas na Hierarchy e clique no botão abaixo, " +
            "ou clique em 'Auto-detectar' para encontrar todas as esferas da cena.",
            MessageType.Info);

        if (GUILayout.Button("Configurar Selecionados como Bolinhas"))
            ConfigurarSelecionados();

        if (GUILayout.Button("Auto-detectar e Configurar Todas as Bolinhas"))
            AutoDetectarBolinhas();
    }

    static void ConfigurarFonte(GameObject fonte)
    {
        // Remove fonte antiga se existir
        var antigaFonte = GameObject.Find("Fonte_Principal");
        if (antigaFonte != null && antigaFonte != fonte)
        {
            Undo.DestroyObjectImmediate(antigaFonte);
            Debug.Log("[ConfigurarCena] Fonte antiga removida.");
        }

        fonte.name = "Fonte_Principal";

        if (!fonte.GetComponent<FonteController>())
            Undo.AddComponent<FonteController>(fonte);

        if (!fonte.GetComponent<FonteView>())
            Undo.AddComponent<FonteView>(fonte);

        if (!fonte.GetComponent<CapsuleCollider>())
        {
            var col = Undo.AddComponent<CapsuleCollider>(fonte);
            col.isTrigger = false;
        }

        EditorUtility.SetDirty(fonte);
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"[ConfigurarCena] Fonte configurada: {fonte.name}");
    }

    static void ConfigurarSelecionados()
    {
        int count = 0;
        foreach (var go in Selection.gameObjects)
        {
            ConfigurarBolinha(go);
            count++;
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"[ConfigurarCena] {count} bolinha(s) configurada(s).");
    }

    static void AutoDetectarBolinhas()
    {
        // Encontra objetos com SphereCollider OU mesh "Sphere" que não sejam câmera, luz, etc.
        int count = 0;
        var todos = GameObject.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var go in todos)
        {
            if (go.GetComponent<FlorescenteController>() != null) continue; // já configurado
            if (go.GetComponent<Camera>() != null) continue;
            if (go.GetComponent<Light>() != null) continue;
            if (go.name.StartsWith("[---")) continue;

            var mf = go.GetComponent<MeshFilter>();
            bool ehEsfera = go.GetComponent<SphereCollider>() != null
                         || (mf != null && mf.sharedMesh != null && mf.sharedMesh.name.Contains("Sphere"));

            if (!ehEsfera) continue;

            // Ignora HUD, flores já existentes, o próprio player
            if (go.GetComponentInParent<Canvas>() != null) continue;
            if (go.name.StartsWith("Flor_Coletavel")) { ConfigurarBolinha(go); count++; continue; }

            // Bolinhas soltas — adiciona só se tiver renderer (objeto visível)
            if (go.GetComponent<Renderer>() == null) continue;
            if (go.GetComponent<JogadorController>() != null) continue;
            if (go.GetComponentInParent<JogadorController>() != null) continue;

            ConfigurarBolinha(go);
            count++;
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log($"[ConfigurarCena] Auto-detecção: {count} bolinha(s) configurada(s).");
    }

    static void ConfigurarBolinha(GameObject go)
    {
        if (!go.GetComponent<SphereCollider>())
        {
            var col = Undo.AddComponent<SphereCollider>(go);
            col.radius = 0.5f;
        }

        if (!go.GetComponent<FlorescenteView>())
            Undo.AddComponent<FlorescenteView>(go);

        if (!go.GetComponent<FlorescenteController>())
            Undo.AddComponent<FlorescenteController>(go);

        EditorUtility.SetDirty(go);
    }
}
