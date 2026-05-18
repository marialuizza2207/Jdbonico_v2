#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

// Liga automaticamente todas as referências serializadas entre componentes
public class ConectarReferencias : EditorWindow
{
    [MenuItem("EcoBotanica/4. Conectar Referências")]
    public static void ConectarMenu() => Conectar();

    public static void Conectar()
    {
        // GerenciadorJardim → HUDView
        var gj  = Object.FindFirstObjectByType<GerenciadorJardim>();
        var hud = Object.FindFirstObjectByType<HUDView>();

        if (gj && hud)
        {
            gj.hud = hud;
            EditorUtility.SetDirty(gj);
        }

        // HUDView → TextMeshPro
        if (hud)
        {
            hud.textoPontuacao = FindTMP("Texto_Pontuacao");
            hud.textoFlores    = FindTMP("Texto_Flores");
            hud.textoMensagem  = FindTMP("Texto_Mensagem");
            hud.textoEstado    = FindTMP("Texto_Estado");
            EditorUtility.SetDirty(hud);
        }

        // JogadorController → câmera principal
        var jc  = Object.FindFirstObjectByType<JogadorController>();
        var cam = Camera.main;
        if (jc && cam)
        {
            jc.referenciaCamera = cam;
            EditorUtility.SetDirty(jc);
        }

        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[ConectarReferencias] Referências conectadas! Salve com Ctrl+S.");
    }

    static TextMeshProUGUI FindTMP(string nome)
    {
        var go = GameObject.Find(nome);
        return go != null ? go.GetComponent<TextMeshProUGUI>() : null;
    }
}
#endif
