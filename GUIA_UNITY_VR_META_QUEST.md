# Guia Técnico — Projeto Unity VR para Meta Quest
**Referência prática baseada no projeto MetaMuseu (TIC 29 — Web 3.0)**
Aluno: Filipe Mazon | 2025

---

## 1. Stack Técnica

| Componente | Versão | Observação |
|---|---|---|
| Unity | 6000.3.14f1 (LTS) | Usar obrigatoriamente via Unity Hub |
| Universal Render Pipeline (URP) | 17.0.3 | Selecionar no template de criação |
| Meta XR SDK Core | 201.0.0 | Via scoped registry |
| Meta XR SDK Interaction | 201.0.0 | Via scoped registry |
| Meta XR SDK Interaction OVR | 201.0.0 | Via scoped registry |
| XR Interaction Toolkit | 3.0.7 | Via Package Manager oficial |
| TextMeshPro | 3.0.6 | Já incluso no Unity 6 |

---

## 2. Configuração Inicial do Projeto

### 2.1 Criar o projeto
1. Unity Hub → **New Project → 3D (URP)** → dar nome sem acentos ou espaços
2. **IMPORTANTE:** o caminho do projeto não pode ter caracteres especiais (`ã`, `ç`, espaços). O Android SDK recusa caminhos não-ASCII na hora do build.

### 2.2 Instalar Meta XR SDK
Editar `Packages/manifest.json` e adicionar antes de `"dependencies"`:

```json
"scopedRegistries": [
  {
    "name": "Meta XR",
    "url": "https://npm.developer.oculus.com",
    "scopes": ["com.meta.xr"]
  }
],
```

Depois no **Package Manager → My Registries** instalar:
- `com.meta.xr.sdk.core`
- `com.meta.xr.sdk.interaction`
- `com.meta.xr.sdk.interaction.ovr`

Instalar também pelo Package Manager oficial:
- `com.unity.xr.interaction.toolkit` (v3.0.7)
- Samples → **XR Device Simulator** (para testes no PC)

### 2.3 XR Plugin Management
**Edit → Project Settings → XR Plugin Management:**
- Aba **PC**: ✅ OpenXR + Meta Quest Feature Group
- Aba **Android**: ✅ OpenXR + Meta Quest Feature Group
- Em **OpenXR → Interaction Profiles**: adicionar **Oculus Touch Controller Profile**

### 2.4 Build Settings para Android (Meta Quest)
**File → Build Settings → Android → Switch Platform**

**Player Settings → Other Settings:**
- Minimum API Level: **Android 10 (API 29)**
- Target API Level: **Android 12 (API 31)**
- Texture Compression: **ASTC**
- Scripting Backend: **IL2CPP**
- Target Architectures: ✅ **ARM64** (desmarcar x86/x86_64)
- Graphics APIs: remover Vulkan se a GPU não suportar (Intel UHD integrada não suporta)

**Edit → Preferences → External Tools:**
- Marcar todos os 4 "Installed with Unity" (JDK, Android SDK, NDK, Gradle)

---

## 3. Problema Conhecido: NDK no Linux

No Linux, os symlinks do NDK dentro do Unity podem estar apontando para um subdiretório inexistente. Se o build falhar com `clang++ not found` ou `-fuse-ld=lld invalid`, corrigir manualmente:

```bash
cd /var/home/<usuario>/Unity/Hub/Editor/6000.3.14f1/Editor/Data/PlaybackEngines/AndroidPlayer/NDK/toolchains/llvm/prebuilt/linux-x86_64/bin/

ln -sf clang-18 clang
ln -sf clang-18 clang++
ln -sf lld ld.lld
ln -sf ld.lld ld
# repetir para demais symlinks quebrados (verificar com: ls -la | grep "->")
```

---

## 4. Problema Conhecido: OVRProjectConfig no Linux

O Meta XR SDK pode travar o editor no Linux com `ArgumentOutOfRangeException` em `OVRProjectConfig.cs` quando `OVRPlugin` retorna versão `0.0.0`. A linha problemática chama `Enumerable.Range` com contagem negativa.

**Fix:** em `Library/PackageCache/com.meta.xr.sdk.core@.../Editor/OVRProjectConfig.cs`, substituir a linha `horizonOsSdkVersions` por:

```csharp
public static int[] horizonOsSdkVersions = Enumerable.Range(minSdkVersion, finalSdkVersion - minSdkVersion + 1)
    .Concat(currentSdkVersion >= version2Start
        ? Enumerable.Range(version2Start, currentSdkVersion - version2Start + 1)
        : Enumerable.Empty<int>())
    .Except(skippedSdkVersions)
    .ToArray();
```

---

## 5. Arquitetura MVC para Projetos VR

Organizar os scripts em três camadas dentro de `Assets/Scripts/`:

```
Scripts/
├── GaleriaManager.cs        ← Singleton central (coordena tudo)
├── PlayerController.cs      ← Movimentação + detecção por proximidade
├── Models/                  ← Dados puros (sem MonoBehaviour)
│   ├── ExibitoModel.cs
│   ├── JogadorModel.cs
│   └── AmbienteModel.cs
├── Views/                   ← Apenas visual (sem lógica de negócio)
│   ├── HUDView.cs
│   ├── ExibitoView.cs
│   ├── PainelInfoView.cs
│   └── BotaoPrincipalView.cs
└── Controllers/             ← Lógica, input e coordenação
    ├── ExibitoController.cs
    ├── PainelInfoController.cs
    ├── PortaAnimadaController.cs
    └── BotaoPrincipalController.cs
```

**Regra:** Models não conhecem Views nem Controllers. Views não chamam Controllers. Só Controllers e o Singleton chamam as outras camadas.

---

## 6. Player VR (XR Origin)

Hierarquia mínima do player:

```
XROrigin  [CharacterController + PlayerController.cs | tag: "Player"]
  └─ CameraOffset  [localPos: (0, 1.7, 0)]
       └─ MainCamera  [Camera + AudioListener | tag: "MainCamera"]
```

**PlayerController.cs** — movimentação por WASD + detecção de interativos por proximidade:
- `CharacterController.Move()` para deslocamento
- `Physics.OverlapSphere()` no `Update()` para detectar objetos interativos próximos
- Gravidade manual: acumular velocidade Y negativa quando `!controller.isGrounded`

---

## 7. Construção da Cena via Editor Script

Criar um `Editor Script` (pasta `Assets/Editor/`) com `[MenuItem]` para construir toda a hierarquia da cena programaticamente. Vantagens:
- Reprodutível: qualquer um pode recriar a cena rodando o menu
- Sem dependência de prefabs externos
- Conecta todas as referências automaticamente via `ConectarReferencias()`

Estrutura básica:

```csharp
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public static class SceneBuilder
{
    [MenuItem("MeuProjeto/► Construir Cena Completa")]
    public static void ConstruirCena()
    {
        // 1. Limpar cena
        // 2. Criar environment (chão, teto, paredes)
        // 3. Criar player (XROrigin)
        // 4. Criar objetos interativos
        // 5. Criar management (Singleton, HUD)
        // 6. Conectar referências
        // 7. Salvar cena
    }
}
#endif
```

---

## 8. Iluminação Baked (LightingSetup.cs)

Criar editor script para configurar e disparar o bake:

```csharp
[MenuItem("MeuProjeto/► Configurar e Iniciar Bake")]
public static void ConfigurarEBake()
{
    // Marcar objetos estáticos (exceto player e objetos animados)
    // Configurar LightingSettings: ProgressiveCPU, AO ligado
    Lightmapping.bakeCompleted += OnBakeDone;
    Lightmapping.BakeAsync();
}
```

**API correta no Unity 6:**
- Usar `LightingSettings` (asset) + `Lightmapping.TryGetLightingSettings()`
- NÃO usar `LightmapEditorSettings` (deprecated)
- NÃO usar `Lightmapping.BakeAsync(callback)` (não existe no Unity 6; usar o evento `bakeCompleted`)
- Objetos dinâmicos (player, porta, animados) NÃO devem ter `ContributeGI`

---

## 9. XRSimpleInteractable (interação com controllers)

Para cada objeto que deve responder a controllers do Quest:

1. **Add Component → XRSimpleInteractable** no objeto filho que tem o Collider
2. Conectar eventos no Inspector:
   - `Hover Entered → Controller / OnXRHoverEntered`
   - `Hover Exited  → Controller / OnXRHoverExited`
   - `Select Entered → Controller / OnXRSelectEntered`

Criar editor script para automatizar:

```csharp
[MenuItem("MeuProjeto/► Configurar XRSimpleInteractable")]
public static void ConfigurarXR()
{
    // FindObjectsByType<ExibitoController>()
    // Para cada um: GetComponentInChildren<Renderer>()
    // AddComponent<XRSimpleInteractable>() se não existir
    // Conectar UnityEvents via SerializedObject
}
```

---

## 10. Simulador XR no Linux

O **Meta XR Simulator** é exclusivo para Windows. No Linux usar o **XR Device Simulator** do XRI:

1. Package Manager → XR Interaction Toolkit → Samples → **XR Device Simulator** → Import
2. Adicionar o prefab `XR Device Simulator` na cena (em Assets/Samples/...)
3. Controles em Play Mode:
   - Mouse: olhar em volta
   - WASD: mover
   - Shift esquerdo: controlar mão esquerda
   - Shift direito: controlar mão direita
   - Botão direito do mouse: ativar look/rotate

---

## 11. .gitignore para Unity

```gitignore
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Bb]uilds/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
/Assets/Oculus/
/Assets/MetaXR/
*.apk
*.aab
*.keystore
.vs/
.idea/
.vscode/
*.csproj
*.sln
.DS_Store

# Asset packs grandes (acima do limite de 100MB do GitHub)
# Listar aqui os pacotes de terceiros baixados
```

---

## 12. Checklist de Entrega

- [ ] Caminho do projeto sem acentos ou espaços
- [ ] URP configurado como pipeline ativo
- [ ] Meta XR SDK instalado via scoped registry
- [ ] XR Plugin Management: OpenXR ativo nas abas PC e Android
- [ ] Build Settings: Android, ARM64, IL2CPP, API 29 mínimo
- [ ] External Tools: todos os 4 "Installed with Unity" marcados
- [ ] Cena construída e salva (Ctrl+S)
- [ ] Lightmap baked (3+ mapas gerados na pasta da cena)
- [ ] XRSimpleInteractable configurado nos objetos interativos
- [ ] APK gerado com sucesso e testado no Editor
- [ ] .gitignore cobrindo Library/, Temp/, Builds/, assets grandes
- [ ] README.md descrevendo apenas o que o projeto **realmente faz**
- [ ] Relatório técnico condizente com a implementação real

---

*Guia gerado a partir do projeto MetaMuseu — TIC 29 | Filipe Mazon | 2025*
