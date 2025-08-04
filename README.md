# ?? Bartender Game

Um jogo de simulação de bar desenvolvido em C# (.NET 8) onde você atua como bartender, preparando drinks para clientes com diferentes preferências e gerenciando pagamentos baseados na satisfação dos clientes. **Nova Feature**: Loja de ingredientes que abre a cada 3 clientes servidos!

**? Interface totalmente redesenhada com Spectre.Console para uma experiência visual incrível!**

## ?? Índice

- [Visão Geral](#-visão-geral)
- [Interface Visual](#-interface-visual)
- [Arquitetura](#-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Mecânicas do Jogo](#-mecânicas-do-jogo)
- [Sistema de Loja](#-sistema-de-loja)
- [Tecnologias](#-tecnologias)
- [Como Executar](#-como-executar)
- [Testes](#-testes)
- [Documentação Técnica](#-documentação-técnica)

## ?? Visão Geral

O **Bartender Game** é um jogo de console onde você:
- Recebe clientes com preferências específicas de drinks
- Seleciona ingredientes do inventário
- Prepara drinks baseados em receitas ou improvisação
- Recebe pagamentos baseados na satisfação do cliente
- Gerencia um inventário limitado de ingredientes
- **?? Compra novos ingredientes na loja a cada 3 clientes servidos**
- **? Desfruta de uma interface visual rica e colorida**

O jogo implementa uma mecânica de **compatibilidade de efeitos** onde clientes reagem diferentemente dependendo de quão próximo o drink servido está do que eles desejavam.

## ? Interface Visual

O jogo utiliza **Spectre.Console** para criar uma experiência visual rica:

### ?? Características da Interface

- **??? Títulos ASCII Art**: Logos estilizados para seções importantes
- **?? Tabelas Dinâmicas**: Informações organizadas em tabelas coloridas
- **?? Painéis Temáticos**: Cada seção tem seu próprio estilo visual
- **?? Cores Contextuais**: 
  - ?? Verde para sucesso e dinheiro alto
  - ?? Amarelo para avisos e dinheiro médio
  - ?? Vermelho para erros e dinheiro baixo
  - ?? Azul para informações dos clientes
  - ?? Roxo para status do jogo
- **?? Inputs Intuitivos**: Prompts claros e validação automática
- **?? Animações de Texto**: Efeitos visuais para experiência imersiva

### ??? Telas Principais

#### ?? Tela Inicial??????   ?????  ??????  ???????? ??????? ???    ?? ??????  ??????? ??????      ??????   ?????  ???    ??? ??????? 
??   ?? ??   ?? ??   ??    ??    ??      ????   ?? ??   ?? ??      ??   ??    ??       ??   ?? ????  ???? ??      
??????  ??????? ??????     ??    ?????   ?? ??  ?? ??   ?? ?????   ??????     ??   ??? ??????? ?? ???? ?? ?????   
??   ?? ??   ?? ??   ??    ??    ??      ??  ?? ?? ??   ?? ??      ??   ??    ??    ?? ??   ?? ??  ??  ?? ??      
??????  ??   ?? ??   ??    ??    ??????? ??   ???? ??????  ??????? ??   ??     ??????  ??   ?? ??      ?? ??????? 

???? BEM-VINDO ??????????????????????????????????????????????????????????????????????????????????
?                                                                                               ?
?  ?? Prepare drinks para satisfazer seus clientes! ??                                         ?
?                                                                                               ?
?  • Misture ingredientes para criar drinks únicos                                             ?
?  • Satisfaça as preferências dos clientes                                                    ?
?  • Ganhe dinheiro e expanda seu bar                                                          ?
?  • Visite a loja a cada 3 clientes!                                                          ?
?                                                                                               ?
?????????????????????????????????????????????????????????????????????????????????????????????????
#### ?? Cliente?????????????????????????????????????????? ?? CLIENTE: MARIA ??????????????????????????????????????????

???? Pedido?????????????????????????????????????????????????????????????????????????????????????????
?                                                                                                   ?
?  "Olá! Eu gostaria de algo Reconfortante, por favor."                                            ?
?                                                                                                   ?
?????????????????????????????????????????????????????????????????????????????????????????????????????
#### ?? Inventário           ?? INGREDIENTES DISPONÍVEIS           
?????????????????????????????????????????
? # ? Nome       ? Tags    ? Quantidade ?
?????????????????????????????????????????
? 1 ? Café       ? Amargo  ?     10     ?
? 2 ? Leite      ? Doce    ?     8      ?
? 3 ? Canela     ? Doce    ?     5      ?
? 4 ? Açúcar     ? Nenhuma ?     7      ?
? 5 ? Chocolate  ? Nenhuma ?     4      ?
? 6 ? Água       ? Nenhuma ?     15     ?
?????????????????????????????????????????
#### ?? Loja??     ??????      ??  ?????  
??    ??    ??     ?? ??   ?? 
??    ??    ??     ?? ??????? 
??    ??    ????   ?? ??   ?? 
?????  ?????? ????? ?? ??   ?? 

???? LOJA DE INGREDIENTES????????????????????????????????????????????????????????????????????????????
?                                                                                                   ?
?  ?? Parabéns! Você serviu 3 clientes! ??                                                         ?
?                                                                                                   ?
?  ?? A loja está aberta para compras!                                                             ?
?  Aproveite para expandir seu arsenal de ingredientes                                             ?
?                                                                                                   ?
?????????????????????????????????????????????????????????????????????????????????????????????????????
## ??? Arquitetura

O projeto segue os princípios de **Clean Architecture** e **Domain-Driven Design**:
??? GameCore/               # Núcleo da aplicação
?   ??? Domain/            # Modelos de domínio e regras de negócio
?   ?   ??? Models/        # Entidades principais
?   ?   ??? Services/      # Serviços de domínio
?   ?   ??? Interfaces/    # Contratos de interface
?   ??? UseCases/          # Casos de uso (Application Layer)
?   ??? Events/            # Sistema de eventos
??? Adapters/              # Camada de adaptadores
    ??? Input/UI/          # Interface do usuário (Console)
### Camadas da Arquitetura

1. **Domain Layer**: Contém as regras de negócio puras
2. **Application Layer**: Orquestra os casos de uso
3. **Infrastructure Layer**: Implementações concretas
4. **UI Layer**: Interface com o usuário (Spectre.Console)

## ?? Estrutura do Projeto

### ?? Domain Models

#### `Client`public class Client
{
    public string Name { get; }
    public string DesiredEffect { get; } // Ex: "Reconfortante", "Energizante"
}
#### `Drink`public class Drink
{
    public string Name { get; }
    public List<Ingredient> Ingredients { get; }
    public string Effect { get; } // Efeito resultante do drink
}
#### `Ingredient`public class Ingredient
{
    public string Name { get; }
    public List<string> Tags { get; } // Ex: ["Doce"], ["Amargo"]
}
#### `ShopItem` ??public class ShopItem
{
    public string Name { get; }
    public List<string> Tags { get; }
    public int Price { get; }
    public int Quantity { get; }
    public ShopItemType Type { get; } // NewIngredient, IngredientRestock
}
#### `PaymentResult`public class PaymentResult
{
    public int BaseAmount { get; }
    public int TipAmount { get; }
    public int TotalAmount { get; }
    public PaymentStatus Status { get; } // Paid, PartiallyPaid, NotPaid
    public string PaymentMessage { get; }
}
#### `GameState`public class GameState
{
    public int CurrentRound { get; }
    public int Money { get; }
    public int TotalTipsEarned { get; }
    public Client? CurrentClient { get; }
    public Drink? PreparedDrink { get; }
    public bool IsRoundActive { get; }
    
    // ?? Novos métodos para loja
    public void SpendMoney(int amount);
    public bool ShouldOpenShop(); // True a cada 3 clientes
}
### ?? Domain Services

#### `ClientService`
- **Responsabilidade**: Gerencia a sequência de clientes
- **Método Principal**: `GetNextClient()` - Retorna o próximo cliente na fila

#### `CraftService`
- **Responsabilidade**: Criação de drinks baseado em ingredientes
- **Lógica**: 
  - Verifica receitas conhecidas primeiro
  - Se não encontrar, cria drink dinâmico baseado nas tags dominantes
- **Método Principal**: `Craft(List<Ingredient> ingredients)`

#### `InventoryService` ??
- **Responsabilidade**: Gerenciamento do estoque de ingredientes
- **Funcionalidades**:
  - `GetAvailableIngredients()` - Lista ingredientes disponíveis
  - `ConsumeIngredients()` - Consome ingredientes utilizados
  - `HasIngredients()` - Verifica disponibilidade
  - **?? `AddNewIngredient()` - Adiciona novos ingredientes da loja**
  - **?? `RestockIngredient()` - Repõe ingredientes existentes**

#### `PaymentService`
- **Responsabilidade**: Calcula pagamentos baseado na reação do cliente
- **Sistema de Pagamento**:
  - **VeryHappy/Happy**: 100% + 20% gorjeta
  - **Neutral**: 50% do valor
  - **Disappointed**: 10% (90% chance) ou 0% (10% chance)
  - **Angry**: 0%

#### `ShopService` ??
- **Responsabilidade**: Gerencia itens da loja e compras
- **Funcionalidades**:
  - `GetAvailableItems()` - Lista itens disponíveis para compra
  - `CanPurchase()` - Verifica se o jogador pode comprar
  - `PurchaseItem()` - Executa a compra do item

### ?? Use Cases

#### `GameLoopUseCase`
- **Responsabilidade**: Orquestra o fluxo principal do jogo
- **Funcionalidades**:
  - `StartNewRound()` - Inicia nova rodada com cliente
  - `SetPreparedDrink()` - Define drink preparado
  - `CompleteRound()` - Finaliza rodada e processa pagamento

#### `PrepareDrinkUseCase`
- **Responsabilidade**: Orquestra a preparação de drinks
- **Fluxo**:
  1. Verifica disponibilidade de ingredientes
  2. Chama o CraftService para criar o drink
  3. Consome ingredientes do inventário
  4. Publica evento de drink preparado

#### `ServeClientUseCase`
- **Responsabilidade**: Gerencia a interação entre cliente e drink servido
- **Sistema de Avaliação**:
  1. **Match Exato**: Cliente fica `VeryHappy`
  2. **Compatibilidade**: Calcula baseado em mapa de efeitos
     - \> 80% = `Happy`
     - \> 50% = `Neutral`
     - \> 20% = `Disappointed`
     - ? 20% = `Angry`

#### `ShopUseCase` ??
- **Responsabilidade**: Orquestra operações da loja
- **Funcionalidades**:
  - `GetAvailableItems()` - Obtém itens disponíveis
  - `CanPurchase()` - Verifica poder de compra
  - `PurchaseItem()` - Executa compra e atualiza estado do jogo

### ?? Sistema de Eventos

O jogo utiliza um **EventBus** para comunicação desacoplada:

#### Eventos Disponíveis:
- `ClientArrivedEvent` - Cliente chegou ao bar
- `DrinkPreparedEvent` - Drink foi preparado
- `DrinkServedEvent` - Drink foi servido ao cliente
- `ClientReactionEvent` - Cliente reagiu ao drink
- `PaymentProcessedEvent` - Pagamento foi processado
- `GameRoundCompletedEvent` - Rodada foi finalizada
- **?? `ShopOpenedEvent` - Loja foi aberta**
- **?? `ItemPurchasedEvent` - Item foi comprado na loja**
// Exemplo de uso
_eventBus.Subscribe<ItemPurchasedEvent>(OnItemPurchased);
_eventBus.Publish(new ItemPurchasedEvent(item, remainingMoney));
## ?? Mecânicas do Jogo

### ?? Receitas Disponíveis
1. **Latte Canela**: Café + Leite + Canela ? Reconfortante
2. **Espresso**: Café ? Energizante  
3. **Café Doce**: Café + Açúcar ? Reconfortante
4. **Chocolate Quente**: Chocolate + Leite ? Reconfortante
5. **Água**: Água ? Neutro

### ?? Sistema de Compatibilidade de Efeitosvar compatibilityMap = new Dictionary<string, Dictionary<string, double>>
{
    ["Reconfortante"] = { ["Energizante"] = 0.6, ["Doce"] = 0.8, ["Relaxante"] = 0.9 },
    ["Energizante"] = { ["Reconfortante"] = 0.6, ["Estimulante"] = 0.9, ["Forte"] = 0.8 },
    ["Relaxante"] = { ["Reconfortante"] = 0.9, ["Doce"] = 0.7, ["Suave"] = 0.8 }
};
### ?? Sistema de Pagamento
| Reação Cliente | Pagamento Base | Gorjeta | Status |
|----------------|----------------|---------|---------|
| VeryHappy | 100% | 20% | Paid |
| Happy | 100% | 20% | Paid |
| Neutral | 50% | 0% | PartiallyPaid |
| Disappointed | 10%* | 0% | PartiallyPaid/NotPaid |
| Angry | 0% | 0% | NotPaid |

*\*10% com 90% de chance, 0% com 10% de chance*

### ?? Inventário Inicial
- **Café**: 10 unidades (Tag: Amargo)
- **Leite**: 8 unidades (Tag: Doce)
- **Canela**: 5 unidades (Tag: Doce)
- **Açúcar**: 7 unidades
- **Chocolate**: 4 unidades
- **Água**: 15 unidades

## ?? Sistema de Loja

### ?? Quando a Loja Abre
A loja abre automaticamente **a cada 3 clientes servidos**:
- Após o 3º cliente ? Loja abre
- Após o 6º cliente ? Loja abre novamente
- E assim por diante...

### ??? Itens Disponíveis na Loja

#### ?? Novos Ingredientes (Compra Única)
| Ingrediente | Tags | Preço | Quantidade |
|-------------|------|-------|------------|
| **Baunilha** | Doce, Aromático | $25 | 3 unidades |
| **Gengibre** | Picante, Estimulante | $30 | 3 unidades |
| **Mel** | Doce, Natural | $20 | 4 unidades |
| **Limão** | Ácido, Refrescante | $15 | 5 unidades |
| **Hortelã** | Refrescante, Mentolado | $18 | 4 unidades |

> **?? Importante**: Cada novo ingrediente pode ser comprado apenas **uma vez**. Após a compra, ele **sai da loja permanentemente**.

#### ?? Reposição de Ingredientes (Compra Ilimitada)
| Ingrediente | Preço | Quantidade |
|-------------|-------|------------|
| **Café** | $10 | 5 unidades |
| **Leite** | $8 | 5 unidades |
| **Canela** | $12 | 3 unidades |
| **Açúcar** | $5 | 5 unidades |
| **Chocolate** | $15 | 3 unidades |
| **Água** | $2 | 10 unidades |

> **?? Reposição**: Estes itens **permanecem na loja** após compra e podem ser comprados múltiplas vezes.

### ?? Estratégia de Compras
- **Novos ingredientes** permitem criar drinks mais variados
- **Novas tags** podem resultar em melhores combinações
- **Reposição** mantém o estoque para receitas conhecidas
- **Planeje bem**: novos ingredientes só podem ser comprados uma vez!

### ?? Fluxo da Loja
1. **Abertura Automática**: Após 3 clientes servidos
2. **Exibição de Itens**: Lista com preços e quantidades
   - ?? = Novo ingrediente (compra única) - aparece com **NEW!** na primeira vez
   - ?? = Reposição (compra ilimitada) - ingredientes originais e previamente comprados
3. **Seleção**: Escolha os itens desejados
4. **Compra**: Gasta dinheiro e atualiza inventário
5. **Remoção Automática**: Novos ingredientes saem da categoria de novos
6. **Conversão**: Ingredientes comprados aparecem como reposição nas próximas lojas
7. **Continuação**: Opção de comprar mais itens
8. **Fechamento**: Volta ao jogo normal

### ?? Mecânica de Compra Única e Reposição```csharp
// Novos ingredientes são rastreados para evitar re-compra
private readonly HashSet<string> _purchasedNewIngredients;

public void PurchaseItem(ShopItem item)
{
    if (item.Type == ShopItemType.NewIngredient)
    {
        _inventoryService.AddNewIngredient(item.Name, item.Tags, item.Quantity);
        _purchasedNewIngredients.Add(item.Name); // Remove da categoria de novos
    }
    // Reposição pode ser comprada infinitamente
}

// Ingredientes comprados aparecem como reposição em lojas subsequentes
foreach (var purchasedIngredientName in _purchasedNewIngredients)
{
    if (_inventoryService.GetIngredientStock(purchasedIngredientName) >= 0)
    {
        // Cria item de reposição com preço reduzido (60% do original)
        var restockItem = new ShopItem(/* ... */);
        availableItems.Add(restockItem);
    }
}
### ?? Sistema de Progressão da Loja

#### **Primeira Loja (Rodada 3)**
- **?? Novos Ingredientes**: Baunilha, Gengibre, Mel, Limão, Hortelã (com **NEW!**)
- **?? Reposição**: Café, Leite, Canela, Açúcar, Chocolate, Água

#### **Segunda Loja (Rodada 6)**
- **?? Novos Ingredientes**: Apenas os que não foram comprados na primeira loja
- **?? Reposição**: 
  - Ingredientes originais (Café, Leite, etc.)
  - **+ Ingredientes comprados** na primeira loja (preço reduzido)

#### **Terceira Loja (Rodada 9)**
- **?? Novos Ingredientes**: Apenas os que nunca foram comprados
- **?? Reposição**: 
  - Ingredientes originais
  - **+ Todos os ingredientes comprados** nas lojas anteriores

### ?? Sistema de Preços da Loja
| Tipo | Preço | Quantidade | Disponibilidade |
|------|-------|------------|-----------------|
| **Novo Ingrediente** | 100% | Original | Uma vez apenas |
| **Reposição Original** | Fixo | Fixo | Ilimitada |
| **Reposição Comprada** | 60% do original | Original - 1 | Ilimitada |

> **?? Estratégia**: Ingredientes comprados como novos têm preço reduzido na reposição!