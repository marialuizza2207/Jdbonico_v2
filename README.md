# EcoBotanica — Jardim Botânico Virtual

> **Web 3.0 | Residência em TIC 29 — Unidade 1 / Capítulo 3**
> Aluna: Maria Luiza de Moraes Mazon | Professora: Ana Beatriz

Ambiente VR interativo criado em Unity 6 com Meta XR SDK. O jogador explora um jardim botânico virtual, coleta flores luminosas espalhadas pelo jardim, consulta painéis botânicos flutuantes com informações científicas sobre plantas, ativa uma fonte d'água e interage com uma estufa animada. Totalmente testável no Unity Editor via teclado e mouse, sem necessidade de headset.

---

## Requisitos

| Item | Versão |
|---|---|
| Unity | 6000.3.14f1 (LTS) |
| Meta XR SDK Core | 201.0.0 |
| XR Interaction Toolkit | 3.0.7 |
| Universal Render Pipeline (URP) | 17.0.3 |
| TextMesh Pro | 3.0.6 |
| Plataforma de build | Android (Meta Quest) |

---

## Como abrir o projeto

1. Abra o **Unity Hub**
2. Clique em **Add → Add project from disk** e selecione a pasta `Trabalho_avancado_Maria/`
3. Aguarde o Unity importar os pacotes
4. Abra a cena: `Assets/scene1.unity`

---

## Controles (Unity Editor — teclado e mouse)

| Tecla / Input | Ação |
|---|---|
| `W` / `↑` | Mover para frente |
| `S` / `↓` | Mover para trás |
| `A` / `←` | Mover para esquerda |
| `D` / `→` | Mover para direita |
| Botão direito do mouse + arrastar | Virar horizontalmente |
| `E` | Interagir (fonte / estufa / painel botânico) |

---

## Mecânicas implementadas

| Mecânica | Como funciona |
|---|---|
| **Coleta de flores** | Ao se aproximar de uma flor/bolinha coletável, a coleta ocorre automaticamente via `Physics.OverlapSphere`. O HUD atualiza a pontuação e o contador de flores coletadas em tempo real. |
| **Painéis botânicos** | Ao se aproximar de `Planta_Info_01/02/03` (Bambu, Helicônia, Bromélia), um painel flutuante com nome comum, nome científico e descrição aparece voltado para o jogador. Pressionar `E` alterna a visibilidade do painel. |
| **Fonte d'água** | Ao se aproximar da fonte, o HUD exibe "Pressione E para ativar a fonte!". Ao pressionar `E`, um sistema de partículas simula o jato d'água (partículas azul/ciano que sobem com velocidade e caem com gravidade). O HUD confirma "Fonte ativada! A água jorra!". |
| **Estufa** | Ao se aproximar, o HUD exibe "Pressione E para abrir/fechar a estufa!". Ao pressionar `E`, a porta anima abertura ou fechamento com rotação suave (pivot na dobradiça). |
| **HUD World Space** | Sempre visível, segue a câmera com interpolação suave (Lerp/Slerp), exibindo pontuação, contagem de flores, mensagem contextual e estado atual. |
| **Colisão física** | O jogador (CharacterController) colide com banco, fonte e barracão, mas atravessa as árvores livremente. |
| **Suporte XR (Quest)** | Todos os objetos interativos possuem `XRSimpleInteractable`. Flores, fonte, estufa e painéis respondem ao trigger dos controles do Meta Quest. |

---

## Apresentando o Projeto

EcoBotanica é um jardim botânico virtual para o Metaverso, com foco em educação ambiental imersiva. O jogador percorre um ambiente ao ar livre com vegetação, estruturas e elementos interativos, aprendendo sobre espécies botânicas enquanto coleta flores e explora o espaço.

---

## Contexto e Objetivos

No Metaverso, ambientes educacionais imersivos superam a barreira do desengajamento das aulas tradicionais. Um jardim botânico virtual permite que estudantes de qualquer lugar explorem plantas, aprendam taxonomia de forma lúdica e vivenciem a natureza sem limitações geográficas. O projeto conecta **educação**, **entretenimento** e **meio ambiente** no contexto Web 3.0.

---

## Processo de Criação e Dificuldades

O projeto foi desenvolvido com arquitetura **MVC** (Model-View-Controller), separando dados (`PlantaModel`, `JardimModel`), apresentação (Views) e lógica (Controllers). Scripts Editor automatizam a montagem da cena via menus `EcoBotanica`.

**Principais desafios:**
- Configurar o Meta XR SDK 201.0.0 no Linux com Unity 6 (correção de symlinks do NDK)
- Corrigir `ArgumentOutOfRangeException` no `OVRProjectConfig` (bug Linux no `Enumerable.Range` com SDK version < 200)
- Shaders do NatureStarterKit2 incompatíveis com URP — substituídos por árvores 3D importadas
- Fazer a porta da estufa animar com pivot correto na dobradiça (não no centro do mesh)
- Conectar referências serializadas entre scripts via Editor Script sem drag-and-drop manual

---

## Hierarquia da Cena

```
scene1 (Scene)
│
├── [--- MANAGEMENT ---]
│   ├── GerenciadorJardim       ← Singleton — pontuação, flores, estados
│   ├── EventSystem
│   └── HUD_Canvas              ← Canvas WorldSpace (segue a câmera com Lerp)
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
│   ├── Plano_Gramado           ← gramado com textura PBR
│   ├── Directional Light
│   ├── Pavilhao                ← estrutura central com colunas e telhado
│   ├── Banco                   ← banco de jardim (colisão física)
│   ├── Arvores                 ← árvores 3D importadas (sem colisão)
│   ├── Poste_Luz               ← Point Light
│   └── Estufa                  ← greenhouse com porta animada
│
└── [--- INTERACTABLES ---]
    ├── Flor_Coletavel_01       ← Rosa       — 10 pts
    ├── Flor_Coletavel_02       ← Tulipa     — 15 pts
    ├── Flor_Coletavel_03       ← Orquídea   — 25 pts
    ├── Flor_Coletavel_04       ← Girassol   — 20 pts
    ├── Flor_Coletavel_05       ← Lavanda    — 30 pts
    ├── Fonte_Principal         ← E → ativa ParticleSystem de água
    ├── Planta_Info_01          ← Bambu      — painel botânico flutuante
    ├── Planta_Info_02          ← Helicônia  — painel botânico flutuante
    └── Planta_Info_03          ← Bromélia   — painel botânico flutuante
```

---

## Arquitetura MVC

```
Assets/Scripts/
├── GerenciadorJardim.cs              ← Singleton central
├── JogadorController.cs              ← movimento + rotação + detecção por proximidade
├── Models/
│   ├── PlantaModel.cs                ← dados de uma planta
│   └── JardimModel.cs                ← estado global do jardim
├── Views/
│   ├── HUDView.cs                    ← HUD WorldSpace com câmera follow
│   ├── FlorescenteView.cs            ← oscilação senoidal + rotação
│   ├── FonteView.cs                  ← ParticleSystem de água
│   ├── PainelBotanicoView.cs         ← painel flutuante voltado para câmera
│   └── EstufaView.cs                 ← animação de abertura/fechamento da porta
└── Controllers/
    ├── FlorescenteController.cs      ← coleta automática + XR
    ├── FonteController.cs            ← hover + ativação + XR
    ├── PainelBotanicoController.cs   ← exibir/esconder painel + XR
    └── EstufaController.cs           ← abrir/fechar porta + XR
```

---

## Configuração de Build (Android / Meta Quest)

1. **File → Build Settings → Android → Switch Platform**
2. **Player Settings:**
   - Minimum API Level: Android 10 (Level 29)
   - Scripting Backend: IL2CPP
   - Target Architecture: ARM64
   - Texture Compression: ASTC
3. **Project Settings → XR Plug-in Management → Android:** marcar **OpenXR** + Oculus Touch Controller Profile
4. **Project Settings → XR Plug-in Management → PC:** desmarcar todos os loaders (evita erro no Linux)

---

*Web 3.0 | Residência em TIC 29 — Maria Luiza de Moraes Mazon*
