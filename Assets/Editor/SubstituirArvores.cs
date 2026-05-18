using UnityEngine;
using UnityEditor;
using System.IO;

public class SubstituirArvores : EditorWindow
{
    [MenuItem("EcoBotanica/5. Substituir Árvores (NatureKit2)")]
    public static void Substituir()
    {
        SubstituirNaCena();
        ConverterMateriaisURP();
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[SubstituirArvores] Concluído! Salve com Ctrl+S.");
    }

    static void SubstituirNaCena()
    {
        // Mapeamento: nome do objeto na cena → prefab do NatureStarterKit2
        var mapa = new (string nome, string prefabPath)[]
        {
            ("Arvore_01", "Assets/NatureStarterKit2/Nature/tree01.prefab"),
            ("Arvore_02", "Assets/NatureStarterKit2/Nature/tree02.prefab"),
            ("Arvore_03", "Assets/NatureStarterKit2/Nature/tree03.prefab"),
            ("Arvore_04", "Assets/NatureStarterKit2/Nature/tree04.prefab"),
        };

        foreach (var (nome, prefabPath) in mapa)
        {
            var antigo = GameObject.Find(nome);
            if (antigo == null)
            {
                Debug.LogWarning($"[SubstituirArvores] '{nome}' não encontrado na cena.");
                continue;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"[SubstituirArvores] Prefab não encontrado: {prefabPath}");
                continue;
            }

            var pos    = antigo.transform.position;
            var rot    = antigo.transform.rotation;
            var pai    = antigo.transform.parent;

            var novo = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            novo.name                  = nome;
            novo.transform.parent      = pai;
            novo.transform.position    = pos;
            novo.transform.rotation    = rot;
            novo.transform.localScale  = new Vector3(1f, 1f, 1f);

            Undo.RegisterCreatedObjectUndo(novo, $"Substituir {nome}");
            Undo.DestroyObjectImmediate(antigo);

            EditorUtility.SetDirty(novo);
            Debug.Log($"[SubstituirArvores] '{nome}' substituído por {Path.GetFileNameWithoutExtension(prefabPath)}.");
        }
    }

    static void ConverterMateriaisURP()
    {
        var shader     = Shader.Find("Universal Render Pipeline/Lit");
        var shaderFoil = Shader.Find("Universal Render Pipeline/Lit"); // folhas com alpha clip

        if (shader == null)
        {
            Debug.LogWarning("[SubstituirArvores] Shader URP/Lit não encontrado. Configure o URP primeiro.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { "Assets/NatureStarterKit2" });
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat  = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            var shaderName = mat.shader.name;
            // Somente converte materiais Built-in (Nature/* ou Standard)
            if (!shaderName.StartsWith("Nature/") && shaderName != "Standard") continue;

            bool ehFolha = shaderName.Contains("Leaves") || path.Contains("branch") || path.Contains("bush");

            mat.shader = shader;

            if (ehFolha)
            {
                // Ativa alpha clipping para as folhas
                mat.SetFloat("_AlphaClip", 1f);
                mat.SetFloat("_Cutoff", 0.4f);
                mat.EnableKeyword("_ALPHATEST_ON");
                mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.AlphaTest;
                mat.SetOverrideTag("RenderType", "TransparentCutout");
            }

            EditorUtility.SetDirty(mat);
            Debug.Log($"[SubstituirArvores] Material convertido para URP: {path}");
        }

        AssetDatabase.SaveAssets();
    }
}
