using UnityEngine;
using UnityEditor;

public class ConfigurarColisoes : EditorWindow
{
    [MenuItem("EcoBotanica/8. Configurar Colisões do Player")]
    public static void Configurar()
    {
        ConfigurarCharacterController();
        RemoverColisoresArvores();
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[ConfigurarColisoes] Pronto! Salve com Ctrl+S.");
    }

    static void ConfigurarCharacterController()
    {
        var player = GameObject.Find("XROrigin")
                  ?? GameObject.FindWithTag("Player");

        var jc = Object.FindObjectOfType<JogadorController>();
        if (jc != null) player = jc.gameObject;

        if (player == null)
        {
            Debug.LogWarning("[ConfigurarColisoes] Player não encontrado. " +
                             "Verifique se o objeto tem a tag 'Player' ou o componente JogadorController.");
            return;
        }

        var cc = player.GetComponent<CharacterController>();
        if (cc == null)
        {
            cc = Undo.AddComponent<CharacterController>(player);
            Debug.Log($"[ConfigurarColisoes] CharacterController adicionado em '{player.name}'.");
        }

        cc.height = 1.8f;
        cc.radius = 0.3f;
        cc.center = new Vector3(0f, 0.9f, 0f);
        cc.slopeLimit   = 45f;
        cc.stepOffset   = 0.3f;
        cc.skinWidth    = 0.02f;

        EditorUtility.SetDirty(player);
        Debug.Log($"[ConfigurarColisoes] CharacterController configurado em '{player.name}'.");
    }

    static void RemoverColisoresArvores()
    {
        var arvoresGO = GameObject.Find("Arvores");
        if (arvoresGO == null)
        {
            for (int i = 1; i <= 10; i++)
            {
                var arv = GameObject.Find($"Arvore_0{i}");
                if (arv != null) RemoverColisoresRecursivo(arv);
            }
            return;
        }

        foreach (Transform filho in arvoresGO.transform)
            RemoverColisoresRecursivo(filho.gameObject);

        Debug.Log("[ConfigurarColisoes] Colisores das árvores removidos.");
    }

    static void RemoverColisoresRecursivo(GameObject go)
    {
        foreach (var col in go.GetComponentsInChildren<Collider>(includeInactive: true))
        {
            Undo.DestroyObjectImmediate(col);
        }
        EditorUtility.SetDirty(go);
    }
}
