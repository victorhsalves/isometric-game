# Sistema de Visualização da Grid

Este sistema permite visualizar as células da grid do terreno de forma visual, facilitando o desenvolvimento e debug do jogo de farming isométrico.

## Componentes

### TerrainController.cs
- **Função**: Gerencia a criação e visualização das células da grid
- **Funcionalidades**:
  - Cria objetos visuais para cada célula da grid
  - Ajusta a posição das células baseado na altura do terreno
  - Permite alternar visibilidade das células
  - Suporta prefabs customizados ou cria cubos automáticos

### GridVisualizationController.cs
- **Função**: Controla a visualização da grid através do Inspector e teclas
- **Funcionalidades**:
  - Botões no Inspector para mostrar/ocultar grid
  - Tecla G para alternar visibilidade (configurável)
  - Configurações de cor, opacidade e altura das células

## Configuração

### 1. Setup Básico
1. Adicione o componente `TerrainController` ao GameObject que contém o terreno
2. Configure as referências no Inspector:
   - **Terrain**: Referência ao componente Terrain
   - **Cell Prefab** (opcional): Prefab customizado para as células
   - **Cell Material**: Material para as células (use GridCellMaterial)
   - **Cell Color**: Cor das células
   - **Cell Height**: Altura das células visuais

### 2. Controle da Visualização
1. Adicione o componente `GridVisualizationController` a um GameObject
2. Configure as referências:
   - **Terrain Controller**: Referência ao TerrainController
   - **Show Grid On Start**: Se deve mostrar a grid ao iniciar
   - **Toggle Key**: Tecla para alternar visibilidade (padrão: G)

## Uso

### No Editor
- Use os botões no Inspector do `GridVisualizationController`
- Clique com botão direito no componente para acessar o menu de contexto
- Use "Refresh Grid" para recriar a visualização

### No Jogo
- Pressione a tecla configurada (padrão: G) para alternar visibilidade
- As células são criadas automaticamente ao iniciar

## Personalização

### Cores e Materiais
- O material `GridCellMaterial` é transparente e branco por padrão
- Você pode criar materiais customizados e aplicá-los
- A cor pode ser alterada via código ou Inspector

### Prefabs Customizados
- Crie um prefab para representar as células
- Configure-o no campo "Cell Prefab" do TerrainController
- O prefab será instanciado para cada célula

### Tamanho das Células
- O tamanho é baseado no componente Grid do terreno
- Ajuste o `cellSize` no componente Grid para alterar o tamanho das células

## Debug

### Logs
- O sistema gera logs informativos sobre a criação das células
- Verifique o Console para informações sobre erros ou sucesso

### Problemas Comuns
1. **Grid não aparece**: Verifique se o Terrain tem componente Grid
2. **Células fora de posição**: Verifique se o terreno está na posição correta
3. **Performance**: Reduza o número de células ou use LOD para terrenos grandes

## Exemplo de Uso

```csharp
// Acessar o TerrainController via código
TerrainController controller = FindObjectOfType<TerrainController>();

// Mostrar/ocultar grid
controller.ToggleCellVisibility(true);

// Recriar visualização
controller.RefreshCellVisualization();
``` 