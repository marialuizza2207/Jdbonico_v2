#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using TMPro;

public class ConectarReferencias : EditorWindow
{
    [MenuItem("EcoBotanica/4. Conectar Referências")]
    public static void ConectarMenu() => Conectar();

    public static void Conectar()
    {
        var gj  = Object.FindObjectOfType<GerenciadorJardim>();
        var hud = Object.FindObjectOfType<HUDView>();

        if (gj && hud)
        {
            gj.hud = hud;
            EditorUtility.SetDirty(gj);
        }

        if (hud)
        {
            hud.textoPontuacao = FindTMP("Texto_Pontuacao");
            hud.textoFlores    = FindTMP("Texto_Flores");
            hud.textoMensagem  = FindTMP("Texto_Mensagem");
            hud.textoEstado    = FindTMP("Texto_Estado");
            EditorUtility.SetDirty(hud);
        }

        var jc  = Object.FindObjectOfType<JogadorController>();
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
