using UnityEngine;
using UnityEditor;

public class ReconstruirArvores : EditorWindow
{
    [MenuItem("EcoBotanica/6. Reconstruir Árvores Estilizadas")]
    public static void Reconstruir()
    {
        LimparNatureKit();
        CriarArvores();
        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
        Debug.Log("[ReconstruirArvores] Árvores reconstruídas! Salve com Ctrl+S.");
    }

    static void LimparNatureKit()
    {
        string[] nomesParaRemover = {
            "tree01","tree02","tree03","tree04",
            "bush01","bush02","bush03","bush04","bush05","bush06"
        };
        foreach (var nome in nomesParaRemover)
        {
            var go = GameObject.Find(nome);
            if (go != null)
            {
                Undo.DestroyObjectImmediate(go);
                Debug.Log($"[ReconstruirArvores] Removido: {nome}");
            }
        }
    }

    static readonly (Vector3 pos, float altTronco, float escCopa, Color corCopa)[] configs =
    {
        (new Vector3(-6f, 0f, -5f), 2.2f, 2.4f, new Color(0.13f, 0.52f, 0.13f)),
        (new Vector3( 6f, 0f, -5f), 2.8f, 2.0f, new Color(0.10f, 0.45f, 0.10f)),
        (new Vector3(-6f, 0f,  8f), 2.0f, 2.6f, new Color(0.20f, 0.58f, 0.15f)),
        (new Vector3( 6f, 0f,  8f), 2.5f, 2.2f, new Color(0.12f, 0.48f, 0.18f)),
    };

    static void CriarArvores()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var matTronco = CriarMaterial(shader, new Color(0.35f, 0.20f, 0.08f));

        var arvoresGO = GameObject.Find("Arvores");

        for (int i = 0; i < 4; i++)
        {
            var (pos, altTronco, escCopa, corCopa) = configs[i];
            string nome = $"Arvore_0{i + 1}";

            var arv = GameObject.Find(nome);
            if (arv == null)
            {
                arv = new GameObject(nome);
                if (arvoresGO != null) arv.transform.SetParent(arvoresGO.transform);
            }
            else
            {
                for (int c = arv.transform.childCount - 1; c >= 0; c--)
                    Undo.DestroyObjectImmediate(arv.transform.GetChild(c).gameObject);
            }

            arv.transform.position = pos;
            Undo.RegisterCreatedObjectUndo(arv, nome);

            var tronco = Cylinder("Tronco_Mesh", arv.transform,
                localPos: new Vector3(0f, altTronco * 0.5f, 0f),
                scale:    new Vector3(0.28f, altTronco * 0.5f, 0.28f),
                mat:      matTronco);

            var matCopa = CriarMaterial(shader, corCopa);
            float base3D = altTronco + escCopa * 0.45f;

            Sphere("Copa_Base", arv.transform,
                localPos: new Vector3(0f, base3D, 0f),
                scale:    new Vector3(escCopa, escCopa * 0.85f, escCopa),
                mat:      matCopa);

            var matCopaMid = CriarMaterial(shader, Clarear(corCopa, 0.06f));
            Sphere("Copa_Meio", arv.transform,
                localPos: new Vector3(0.15f, base3D + escCopa * 0.55f, 0.05f),
                scale:    new Vector3(escCopa * 0.72f, escCopa * 0.72f, escCopa * 0.72f),
                mat:      matCopaMid);

            var matCopaTop = CriarMaterial(shader, Clarear(corCopa, 0.12f));
            Sphere("Copa_Topo", arv.transform,
                localPos: new Vector3(-0.1f, base3D + escCopa * 0.95f, -0.1f),
                scale:    new Vector3(escCopa * 0.48f, escCopa * 0.52f, escCopa * 0.48f),
                mat:      matCopaTop);

            EditorUtility.SetDirty(arv);
        }
    }

    static Color Clarear(Color c, float amount) =>
        new Color(Mathf.Clamp01(c.r + amount), Mathf.Clamp01(c.g + amount), Mathf.Clamp01(c.b + amount));

    static Material CriarMaterial(Shader shader, Color cor)
    {
        var m = new Material(shader);
        m.color = cor;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", cor);
        return m;
    }

    static GameObject Cylinder(string nome, Transform pai, Vector3 localPos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = nome;
        go.transform.SetParent(pai);
        go.transform.localPosition = localPos;
        go.transform.localScale    = scale;
        go.GetComponent<Renderer>().material = mat;
        return go;
    }

    static GameObject Sphere(string nome, Transform pai, Vector3 localPos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = nome;
        go.transform.SetParent(pai);
        go.transform.localPosition = localPos;
        go.transform.localScale    = scale;
        go.GetComponent<Renderer>().material = mat;
        return go;
    }
}
