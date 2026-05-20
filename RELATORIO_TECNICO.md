# Relatório Técnico — EcoBotanica

**Web 3.0 | Residência em TIC 29 — Atividade Avaliativa**
**Nome:** Maria Luiza de Moraes Mazon
**Turma:** Web 3.0 — Residência em TIC 29

---

## Sobre o projeto

O EcoBotanica é um jardim botânico virtual desenvolvido em Unity 6 com Meta XR SDK. A ideia veio da proposta de criar um ambiente VR educacional dentro do tema Metaverso: o usuário explora um jardim ao ar livre, coleta flores coletáveis e ativa uma fonte d'água. Sem headset, tudo funciona via teclado e mouse no Unity Editor.

---

## Configuração técnica

Usei Unity 6000.3.14f1 (LTS) com URP 17.0.3 e Meta XR SDK Core 201.0.0. Desenvolvi no Linux, o que causou alguns problemas — o Meta Quest Link não funciona no Linux, e o XR Device Simulator apresentava erros na inicialização do OVRPlugin. A solução foi usar WASD + mouse diretamente no Editor mesmo.

Para instalar o Meta XR SDK, precisei adicionar o escopo `https://npm.developer.oculus.com/` manualmente no Package Manager. Isso não está documentado de forma clara em lugar nenhum — fui achando por tentativa e erro até funcionar.

**Bug Linux no SDK:** o `OVRProjectConfig.cs` chamava `Enumerable.Range` com count negativo quando `wrapperVersion` era `0.0.0`, causando `ArgumentOutOfRangeException` e crash. Precisei corrigir direto no arquivo do pacote. Não é a solução mais elegante mas foi o que funcionou.

O build para Android (Meta Quest) ficou com: API Level 29 mínimo, IL2CPP, ARM64, textura ASTC, loader OpenXR habilitado no Android e todos os loaders desabilitados no PC/Linux.

---

## Assets e cena

Usei alguns assets gratuitos da Asset Store:

- **Fantasy Skybox FREE** — céu com nuvens ao redor do jardim
- **PBR Grass Textures** — textura do gramado com tiling configurado
- **Árvores 3D (URP_Tree)** — vegetação decorativa sem colisão
- **TextMesh Pro** (built-in) — HUD World Space
- **Primitivos Unity** — pavilhão, banco, poste e flores feitos com Cylinder/Cube/Sphere

O NatureStarterKit2 que eu tinha baixado não funcionou — os shaders são incompatíveis com URP e as árvores ficaram com material rosa. Tive que descartar e importar outro pacote de árvores.

A cena tem quatro grupos na Hierarchy: `[--- MANAGEMENT ---]`, `[--- PLAYER ---]`, `[--- ENVIRONMENT ---]` e `[--- INTERACTABLES ---]`. Dentro do management fica o `GerenciadorJardim` (Singleton), o `EventSystem` e o `HUD_Canvas` WorldSpace que segue a câmera.

---

## Interações implementadas

**Coleta de flores:**
O `JogadorController` chama `Physics.OverlapSphere` a cada frame com raio de 1,5m. Quando detecta um collider com `FlorescenteController`, chama `TentarColetar()`. O Controller verifica a flag `coletada`, registra no `GerenciadorJardim` e avisa a View para desativar o GameObject. O `GerenciadorJardim` atualiza o `JardimModel` (pontuação e contagem) e chama o `HUDView`.

Há cinco flores com pontuações diferentes: Rosa (10), Tulipa (15), Orquídea (25), Girassol (20) e Lavanda (30).

**Fonte d'água:**
A fonte usa detecção de proximidade separada com raio de 4m. Ao entrar no range, o HUD mostra "Pressione E para ativar a fonte!". Ao pressionar E, a `FonteController` chama `AoAtivar()` na `FonteView`, que cria um `ParticleSystem` por código — partículas azul/ciano com velocidade e gravidade simulando jato d'água.

**Suporte XR:**
Flores e fonte têm `XRSimpleInteractable`. O `FlorescenteController` tem um método `OnInteracaoXR` conectado ao evento `selectEntered` — funciona tanto com o trigger do Quest quanto com a detecção por proximidade no Editor.

---

## Arquitetura

Usei MVC separando dados, apresentação e lógica:

- `JardimModel` e `PlantaModel` — só dados, sem MonoBehaviour
- `FlorescenteView` e `FonteView` — feedback visual (oscilação senoidal, ParticleSystem)
- `FlorescenteController` e `FonteController` — lógica de coleta e ativação
- `GerenciadorJardim` — Singleton central, conecta tudo
- `JogadorController` — movimento WASD, detecção por proximidade

Os scripts em `Assets/Editor/` automatizam a montagem da cena via menus `EcoBotanica`. Isso foi útil porque recriei a hierarquia várias vezes durante o desenvolvimento.

---

## O que deu trabalho

O Linux foi o maior problema. Além do bug do OVRProjectConfig, o XR Plug-in Management tem um comportamento estranho: se os loaders do PC ficam habilitados, o OVRPlugin tenta inicializar e trava o Editor. Demorei pra entender que a solução era simplesmente desativar tudo na aba PC.

Outro ponto complicado foi o `CharacterController`. No começo o jogador afundava no chão ou ficava preso em objetos. Precisei ajustar `height`, `radius` e `center` manualmente várias vezes e criar um script Editor (`ConfigurarColisoes.cs`) para remover os colliders das árvores, que estavam bloqueando a passagem.

O HUD WorldSpace seguir a câmera com suavidade também não foi óbvio — a primeira versão usava `transform.position = cam.position + forward * dist` direto, sem Lerp, e ficava tremendo muito. A interpolação via `Slerp`/`Lerp` no `LateUpdate` resolveu.

---

## Repositório

**Nome:** Jdbonico_v2
**URL:** https://github.com/marialuizza2207/Jdbonico_v2

Estrutura: `/Assets` com scripts e cena, `/ProjectSettings` com config de build e XR, `/Packages` com manifest.json. Assets grandes da Asset Store estão no `.gitignore`.

---

## Reflexão

No começo achei que a parte mais difícil seria programar as interações, mas na prática o que consumiu mais tempo foi a configuração do ambiente — Unity, SDK, Linux, shaders incompatíveis. Até o projeto compilar sem erros levou um bom tempo.

O XR Interaction Toolkit abstrai bem a diferença entre input de teclado e controles do Quest, mas a documentação é confusa sobre qual versão usar com qual SDK. Fui no trial and error até achar uma combinação que funcionasse.

Ainda tenho dúvida se o build do APK vai funcionar corretamente no hardware real — testei só no Editor. Uma melhoria que faria seria adicionar feedback de áudio ao coletar as flores, e talvez expandir o jardim com mais plantas e um sistema de missões simples.
