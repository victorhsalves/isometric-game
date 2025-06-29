# Grid Visualization System - README

## Visão Geral
Este sistema fornece visualização em grid para terrenos no Unity, com funcionalidade de preview ao passar o mouse sobre as células.

## Componentes Principais

### 1. TerrainController
- **Função**: Controla a criação e gerenciamento das células visuais do grid
- **Funcionalidades**:
  - Criação automática de células baseadas no tamanho do terreno
  - Sistema de preview com materiais diferentes ao passar o mouse
  - Detecção automática de colliders para interação
  - Sistema de debug integrado

### 2. GridVisualizationController
- **Função**: Interface para controlar a visibilidade e configurações do grid
- **Funcionalidades**:
  - Toggle de visibilidade do grid (tecla G por padrão)
  - Validação da configuração do sistema
  - Controles de debug

### 3. TerrainPreviewDebugger
- **Função**: Ferramenta de debug para solucionar problemas
- **Funcionalidades**:
  - Visualização de raycasts
  - Teste de detecção de células
  - Verificação de materiais e configurações

## Configuração Rápida

### Passo 1: Configurar o Terreno
1. Adicione um objeto `Terrain` à sua cena
2. Adicione um componente `Grid` ao objeto do terreno
3. Configure o `cellSize` do Grid (ex: 1x1x1)

### Passo 2: Configurar o TerrainController
1. Adicione o script `TerrainController` a um GameObject
2. Configure os seguintes campos no Inspector:
   - **Terrain**: Arraste seu objeto Terrain
   - **Cell Prefab**: (Opcional) Prefab para representar cada célula
   - **Cell Material**: Material padrão das células
   - **Preview Material**: Material aplicado ao fazer hover
   - **Cell Color**: Cor das células (se não usar material)
   - **Cell Height**: Altura das células visuais

### Passo 3: Configurar Materiais
1. Crie dois materiais:
   - **Material padrão**: Para as células normais
   - **Material de preview**: Para destaque ao passar o mouse
2. Atribua estes materiais nos campos correspondentes do TerrainController

### Passo 4: Testar o Sistema
1. Execute a cena
2. O grid deve aparecer automaticamente
3. Passe o mouse sobre as células para ver o preview

## Sistema de Preview

### Vantagens do Sistema Baseado em Grid
- **Performance Superior**: Não depende de colliders físicos nas células
- **Detecção Precisa**: Baseado em matemática do grid, sempre preciso
- **Simplicidade**: Não requer configuração de colliders ou tags
- **Flexibilidade**: Funciona independente do tipo de prefab usado
- **Memória**: Menor uso de memória (sem colliders extras)

### Como Funciona
1. O sistema faz raycast da câmera contra o terreno para obter a posição do mouse no mundo
2. Converte a posição mundial para coordenadas do grid usando matemática
3. Identifica qual célula do grid corresponde àquela posição
4. Aplica o material de preview à célula correspondente
5. Restaura o material original quando o mouse sai da célula

### Requisitos para Funcionar
- **Câmera**: Camera.main deve existir ou uma câmera deve estar na cena
- **Terreno**: Deve ter collider para o raycast detectar a posição
- **Grid Component**: Configurado no terreno com cellSize apropriado
- **Preview Material**: Deve estar configurado no TerrainController

## Solução de Problemas

### Preview Não Funciona

**Problema**: Mouse hover não aplica o material de preview

**Soluções**:
1. **Verificar Preview Material**: Certifique-se que está configurado no Inspector
2. **Verificar Câmera**: Camera.main deve existir
3. **Verificar Terreno**: Deve ter collider para o raycast funcionar
4. **Verificar Grid Component**: Deve estar configurado no terreno com cellSize adequado

**Debug**:
```csharp
// Ativar debug mode no TerrainController
TerrainController.SetDebugMode(true);

// Ou usar o TerrainPreviewDebugger
// F1 - Toggle debug mode
// F2 - Toggle raycast visualization
// F3 - Toggle grid debug mode
```

### Células Não Aparecem

**Problema**: Grid não é visível na cena

**Soluções**:
1. Verificar se o Grid component está no Terrain
2. Verificar se cellSize está configurado corretamente
3. Usar GridVisualizationController.RefreshGrid()
4. Verificar se as células estão ativas (GameObject.SetActive)

### Performance

**Para terrenos grandes**:
- Use prefabs simples para as células
- Configure cellSize maior para menos células
- Desative células não visíveis quando necessário

## Controles de Debug

### Teclas de Atalho
- **B**: Toggle visibilidade do grid (configurável)
- **F1**: Toggle debug mode do TerrainController
- **F2**: Toggle visualização de raycast
- **F3**: Toggle debug mode do GridVisualizationController

### Context Menus
**TerrainController**:
- Show/Hide Grid
- Toggle Grid
- Refresh Grid
- Validate Setup
- Force Sync Visibility

**TerrainPreviewDebugger**:
- Test Raycast
- List All Grid Cells
- Check Preview Material

## API Pública

### TerrainController
```csharp
// Controlar visibilidade
public void ToggleCellVisibility(bool visible)

// Recriar grid
public void RefreshCellVisualization()

// Ativar debug
public void SetDebugMode(bool enable)
```

### GridVisualizationController
```csharp
// Controles básicos
public void ShowGrid()
public void HideGrid()
public void ToggleGrid()
public void RefreshGrid()

// Configurações
public void SetGridColor(Color color)
public void SetGridOpacity(float opacity)
public void SetCellHeight(float height)
```

## Notas Importantes

1. **Detecção por Grid**: Sistema usa coordenadas matemáticas do grid, não colliders físicos
2. **Performance**: Muito mais eficiente que raycast contra colliders individuais
3. **Terreno**: Deve ter collider (TerrainCollider) para detecção da posição do mouse
4. **Materiais**: Preview material deve ser diferente do material padrão para efeito visível

## Exemplo de Configuração Mínima

```csharp
// No Inspector do TerrainController:
// - Terrain: [Seu objeto Terrain]
// - Preview Material: [Material com cor/shader diferente]
// - Cell Material: [Material padrão das células]
// - Enable Debug Logs: true (para debug)
``` 