# EcoBotanica — Jardim Botânico Virtual

> Web 3.0 | Residência em TIC 29 — Unidade 1 / Capítulo 3
> Aluna: Maria Luiza de Moraes Mazon | Professora: Ana Beatriz

Ambiente VR interativo feito em Unity 6 com Meta XR SDK. O jogador explora um jardim botânico virtual, coleta flores espalhadas pelo jardim e ativa uma fonte d'água. Funciona no Unity Editor via teclado e mouse, sem precisar de headset.

---

## Requisitos

Desenvolvido com Unity 6000.3.14f1 (LTS), Meta XR SDK Core 201.0.0, XR Interaction Toolkit 3.0.7, URP 17.0.3 e TextMesh Pro. Build configurado para Android (Meta Quest).

---

## Como abrir o projeto

1. Abra o Unity Hub
2. Clique em Add → Add project from disk e selecione a pasta `Trabalho_avancado_Maria/`
3. Aguarde o Unity importar os pacotes (pode levar alguns minutos)
4. Abra a cena: `Assets/scene1.unity`

---

## Controles (Unity Editor — teclado e mouse)

- `W` / `↑` — frente
- `S` / `↓` — atrás
- `A` / `←` — esquerda
- `D` / `→` — direita
- Botão direito do mouse + arrastar — virar
- `E` — ativar a fonte d'água (quando próximo)

---

## Mecânicas

A coleta de flores é automática por proximidade: o `JogadorController` chama `Physics.OverlapSphere` a cada frame e qualquer `FlorescenteController` dentro do raio é coletado. O HUD World Space acompanha a câmera via Lerp e mostra pontuação, progresso de flores e mensagens contextuais.

A fonte funciona em dois passos: ao se aproximar o HUD avisa, e ao pressionar `E` um `ParticleSystem` é criado por código (partículas azuis com gravidade simulando jato d'água). Flores e fonte têm `XRSimpleInteractable` para funcionar com os controles do Meta Quest.

---

## Como foi feito

O projeto usa arquitetura MVC: `JardimModel` guarda o estado (pontuação, flores), as Views cuidam da apresentação visual e os Controllers têm a lógica. O `GerenciadorJardim` é o Singleton central que conecta tudo.

Os scripts em `Assets/Editor/` automatizam a montagem da cena via menus `EcoBotanica` no Unity — dá pra recriar toda a hierarquia em alguns cliques sem arrastar nada manualmente.

**Dificuldades:** configurar o Meta XR SDK no Linux foi trabalhoso — o SDK não foi pensado pra Linux e havia um bug no `OVRProjectConfig.cs` que causava crash no Editor (Enumerable.Range com count negativo). Precisei corrigir direto no arquivo do pacote. Os shaders do NatureStarterKit2 também eram incompatíveis com URP e tive que substituir por árvores 3D de outro pacote.

---

## Hierarquia da cena

A cena `scene1.unity` está organizada em quatro grupos:

- `[--- MANAGEMENT ---]` — GerenciadorJardim (Singleton), EventSystem e HUD_Canvas WorldSpace
- `[--- PLAYER ---]` — XROrigin com JogadorController e CharacterController
- `[--- ENVIRONMENT ---]` — gramado PBR, luz direcional, pavilhão, banco, árvores decorativas e poste
- `[--- INTERACTABLES ---]` — cinco flores coletáveis (Rosa 10pts, Tulipa 15pts, Orquídea 25pts, Girassol 20pts, Lavanda 30pts) e Fonte_Principal

---

## Estrutura de scripts

```
Assets/Scripts/
├── GerenciadorJardim.cs
├── JogadorController.cs
├── Models/
│   ├── PlantaModel.cs
│   └── JardimModel.cs
├── Views/
│   ├── HUDView.cs
│   ├── FlorescenteView.cs
│   └── FonteView.cs
└── Controllers/
    ├── FlorescenteController.cs
    └── FonteController.cs
```

---

## Build para Meta Quest

File → Build Settings → Android → Switch Platform. No Player Settings: API Level mínimo Android 10 (Level 29), IL2CPP, ARM64, textura ASTC. Em XR Plug-in Management → Android habilitar OpenXR + Oculus Touch Controller Profile. Na aba PC/Linux desabilitar todos os loaders (evita erro de inicialização no Linux).
