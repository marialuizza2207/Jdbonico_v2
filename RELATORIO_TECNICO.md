# RELATÓRIO TÉCNICO — PROJETO VR NO METAVERSO

**Web 3.0 | Residência em TIC 29 — Atividade Avaliativa**

---

## SEÇÃO 1 — IDENTIFICAÇÃO

**Nome Completo:** Maria Luiza de Moraes Mazon

**Turma / Residência:** Web 3.0 — Residência em TIC 29

**Limitação de Hardware Relatada:** Desenvolvimento realizado no Linux, onde o Meta Quest Link (streaming para PC) não é suportado pela Meta. O build para APK Android funciona normalmente. Para testes no Editor, foi utilizado o WASD + mouse em substituição ao XR Device Simulator (que apresenta erros de inicialização do OVRPlugin no Linux).

---

## SEÇÃO 2 — CONCEITO DO PROJETO

**2.1 Nome do Projeto:** EcoBotanica — Jardim Botânico Virtual

**2.2 Contexto e Objetivo no Metaverso:**

Criar um ambiente educacional imersivo em Realidade Virtual que simule um jardim botânico, permitindo que usuários explorem plantas, aprendam sobre espécies botânicas e interajam com o ambiente de forma natural. O projeto resolve o problema de desengajamento em aulas teóricas de botânica, tornando o aprendizado lúdico e acessível a estudantes de qualquer localidade através do Metaverso.

**2.3 Descrição Geral do Ambiente Virtual:**

Um jardim ao ar livre com gramado texturizado (PBR), árvores 3D, céu com nuvens (Fantasy Skybox FREE), pavilhão central com colunas e telhado, banco de jardim, estufa com porta animada, poste de luz e fonte d'água. O ambiente contém cinco flores coletáveis espalhadas pelo jardim, três painéis botânicos informativos e a fonte como elemento central interativo. A iluminação usa luz direcional dourada simulando fim de tarde. O jogador se move livremente com colisão física nos elementos sólidos.

---

## SEÇÃO 3 — CONFIGURAÇÃO TÉCNICA DO PROJETO

**3.1 Versão do Unity e Porquê:**
Unity 6000.3.14f1 (LTS) — versão estável de longo prazo com suporte ao Meta XR SDK 201.0.0 e ao Universal Render Pipeline (URP) 17.0.3.

**3.2 Instalação do Meta XR SDK (Passo a Passo):**
1. Acesso ao **Window → Package Manager**
2. Adição do escopo `https://npm.developer.oculus.com/` para pacotes Meta
3. Download e importação do **Meta XR SDK Core 201.0.0** e **Meta XR SDK Interaction 201.0.0**
4. Correção manual do bug Linux em `OVRProjectConfig.cs` (Enumerable.Range com count negativo quando `wrapperVersion == 0.0.0`)

**3.3 Configurações de Build para Android/Meta Quest:**
- Switch Platform → Android
- Minimum API Level: Android 10 (Level 29)
- Target API Level: Android 12 (Level 31)
- Texture Compression: ASTC
- Scripting Backend: IL2CPP
- Target Architecture: ARM64

**3.4 Configuração do XR Plugin Management:**
- Aba Android: loader **OpenXR** habilitado + **Oculus Touch Controller Profile** adicionado em Interaction Profiles
- Aba PC/Linux: todos os loaders desabilitados (evita erro de inicialização do OVRPlugin no Linux)

**3.5 Movimentação no PC (Editor):**
Implementada via `JogadorController.cs` com Unity Input System: teclas WASD para movimento e botão direito do mouse para rotação horizontal. Funciona completamente no Unity Editor sem necessidade de headset ou XR Device Simulator.

---

## SEÇÃO 4 — ASSETS E ELEMENTOS DA CENA

**ASSET 1**
- Nome: Fantasy Skybox FREE
- Tipo: Skybox / Textura
- Origem: Asset Store (Gratuito)
- Função: Criar o céu imersivo com nuvens ao redor do jardim

**ASSET 2**
- Nome: PBR Grass Textures
- Tipo: Textura PBR
- Origem: Asset Store (Gratuito)
- Função: Textura realista do gramado com tiling configurado

**ASSET 3**
- Nome: Árvores 3D (URP_Tree)
- Tipo: Objeto 3D + Materiais URP
- Origem: Asset Store (Gratuito)
- Função: Vegetação realista do jardim sem colisão física

**ASSET 4**
- Nome: Primitivos Unity (Cylinder, Cube, Sphere)
- Tipo: Objeto 3D
- Origem: Unity Built-in
- Função: Pavilhão, banco, estufa, poste e flores coletáveis

**ASSET 5**
- Nome: TextMesh Pro
- Tipo: Sistema de texto UI
- Origem: Unity Package (built-in)
- Função: HUD World Space com pontuação, flores coletadas e mensagens

---

## SEÇÃO 5 — HIERARQUIA DE GAME OBJECTS

**Scene: scene1**

```
scene1
│
├── [--- MANAGEMENT ---]
│   ├── GerenciadorJardim       ← Singleton, coordena estado global
│   ├── EventSystem             ← UI event system
│   └── HUD_Canvas              ← Canvas WorldSpace (segue câmera)
│       ├── Texto_Pontuacao
│       ├── Texto_Flores
│       ├── Texto_Mensagem
│       └── Texto_Estado
│
├── [--- PLAYER ---]
│   └── XROrigin                ← JogadorController + CharacterController
│       └── Main Camera
│
├── [--- ENVIRONMENT ---]
│   ├── Plano_Gramado
│   ├── Directional Light
│   ├── Pavilhao
│   │   ├── Colunas (Coluna_FE/FD/TE/TD)
│   │   └── Telhado_Mesh
│   ├── Banco
│   │   └── Assento / Pes
│   ├── Arvores (URP_Tree_1..N)
│   ├── Poste_Luz
│   └── Estufa
│       ├── Estrutura (paredes + teto)
│       └── Porta_Pivot ← EstufaView (animação)
│
└── [--- INTERACTABLES ---]
    ├── Flor_Coletavel_01  (Rosa — 10 pts)
    ├── Flor_Coletavel_02  (Tulipa — 15 pts)
    ├── Flor_Coletavel_03  (Orquídea — 25 pts)
    ├── Flor_Coletavel_04  (Girassol — 20 pts)
    ├── Flor_Coletavel_05  (Lavanda — 30 pts)
    ├── Fonte_Principal    ← FonteController + FonteView (partículas)
    ├── Planta_Info_01     ← PainelBotanicoController (Bambu)
    ├── Planta_Info_02     ← PainelBotanicoController (Helicônia)
    └── Planta_Info_03     ← PainelBotanicoController (Bromélia)
```

---

## SEÇÃO 6 — SISTEMA DE INTERAÇÃO

**6.1 Descrição da Interação Principal:**

O jogador se aproxima de uma flor coletável e ela é coletada automaticamente por detecção de proximidade (`Physics.OverlapSphere`). O HUD atualiza a pontuação e o contador de flores em tempo real. Ao se aproximar dos painéis botânicos, um painel flutuante com informações científicas da planta aparece automaticamente. A fonte é ativada com a tecla `E` ao se aproximar, gerando um sistema de partículas que simula jato d'água. A estufa também é ativada com `E`, animando a abertura e fechamento da porta. Todos os interativos possuem `XRSimpleInteractable` para funcionar com os controles do Meta Quest.

**6.2 Lógica da Interação — Coleta de Flores:**

1. `JogadorController.Update()` chama `DetectarProximidade()` a cada frame
2. `Physics.OverlapSphere(posição, raio: 1.5m)` detecta colliders próximos
3. Se encontrar `FlorescenteController`, chama `TentarColetar()`
4. `FlorescenteController` verifica flag `coletada`, registra no `GerenciadorJardim`
5. `GerenciadorJardim.RegistrarColeta(pontos)` atualiza `JardimModel` e chama `HUDView`
6. `FlorescenteView.Coletar()` desativa o GameObject

**6.3 Script C# — FlorescenteController.cs**

```csharp
public class FlorescenteController : MonoBehaviour
{
    [SerializeField] public PlantaModel dados;
    private bool coletada = false;

    public void TentarColetar()
    {
        if (coletada) return;
        coletada = true;
        GerenciadorJardim.Instancia?.RegistrarColeta(dados?.pontos ?? 10);
        GetComponent<FlorescenteView>()?.Coletar();
        if (GetComponent<FlorescenteView>() == null) gameObject.SetActive(false);
    }

    public void OnInteracaoXR(SelectEnterEventArgs args) => TentarColetar();
}
```

---

## SEÇÃO 7 — PLANEJAMENTO DO REPOSITÓRIO GITHUB

**7.1 Nome do Repositório:** Jdbonico_v2

**7.2 URL:** https://github.com/marialuizza2207/Jdbonico_v2

**7.3 Estrutura de Pastas:**
- `/Assets` — Scripts, Prefabs, Materiais, Cenas, Texturas
- `/ProjectSettings` — Configurações de build, XR, qualidade
- `/Packages` — manifest.json com dependências
- `.gitignore` — ignora `/Library`, `/Temp`, `/Builds`, `/Logs`

---

## SEÇÃO 8 — PLANO DE EXECUÇÃO PASSO A PASSO

1. **Etapa 1:** Instalação do Unity 6000.3.14f1 e criação do projeto 3D com URP
2. **Etapa 2:** Importação do Meta XR SDK 201.0.0 via Package Manager (escopo Meta)
3. **Etapa 3:** Configuração do XR Origin, câmera principal e `JogadorController` com WASD
4. **Etapa 4:** Modelagem da cena com primitivos Unity (pavilhão, estufa, banco) e assets importados (árvores, gramado, skybox)
5. **Etapa 5:** Aplicação de materiais URP, textura PBR do gramado (tiling 20×20) e configuração da Directional Light
6. **Etapa 6:** Programação dos scripts em C# com arquitetura MVC — Models, Views, Controllers e GerenciadorJardim Singleton
7. **Etapa 7:** Adição de `XRSimpleInteractable`, `SphereCollider` e `CharacterController` nos objetos; configuração dos Editor Scripts no menu EcoBotanica
8. **Etapa 8:** Correção do bug `OVRProjectConfig` no Linux, configuração do Build Profile Android e geração do APK para Meta Quest

---

## SEÇÃO 9 — REFLEXÃO FINAL

**9.1 Aprendizado:**
Compreendi na prática como o XR Interaction Toolkit abstrai a diferença entre input de teclado e controles do Quest. A arquitetura MVC foi essencial para manter o projeto organizado — cada script tem responsabilidade única. Aprendi também que builds para hardware mobile (Quest) exigem atenção especial a API levels, compressão de textura (ASTC) e backend IL2CPP.

**9.2 Dificuldades Encontradas:**
O maior desafio foi o ambiente Linux: o Meta XR SDK não foi projetado para Linux, causando crashes no Editor (`OVRProjectConfig` / `ArgumentOutOfRangeException`) e impossibilidade de usar o Quest Link. A solução foi corrigir o bug diretamente no arquivo do pacote e desabilitar os loaders XR na plataforma PC. Outro desafio foi a incompatibilidade total do NatureStarterKit2 (Tree Creator) com URP, que resultou na migração para árvores 3D de outro pacote.

**9.3 Melhorias Futuras:**
- Adicionar narração em áudio ao se aproximar de cada planta
- Implementar modo multiplayer com Photon PUN2 para visitas guiadas em grupo
- Expandir o jardim com mais biomas (aquático, deserto, floresta tropical)
- Adicionar um sistema de missões com recompensas progressivas
