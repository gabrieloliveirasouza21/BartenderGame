# ?? Bartender Game

Um jogo de simulação de bar desenvolvido em C# (.NET 8) onde você atua como bartender, preparando drinks para clientes com diferentes preferências e gerenciando pagamentos baseados na satisfação dos clientes. **Nova Feature**: Loja de ingredientes que abre a cada 3 clientes servidos!

## ?? Índice

- [Visão Geral](#-visão-geral)
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

O jogo implementa uma mecânica de **compatibilidade de efeitos** onde clientes reagem diferentemente dependendo de quão próximo o drink servido está do que eles desejavam.

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
4. **UI Layer**: Interface com o usuário

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

#### ?? Novos Ingredientes
| Ingrediente | Tags | Preço | Quantidade |
|-------------|------|-------|------------|
| **Baunilha** | Doce, Aromático | $25 | 3 unidades |
| **Gengibre** | Picante, Estimulante | $30 | 3 unidades |
| **Mel** | Doce, Natural | $20 | 4 unidades |
| **Limão** | Ácido, Refrescante | $15 | 5 unidades |
| **Hortelã** | Refrescante, Mentolado | $18 | 4 unidades |

#### ?? Reposição de Ingredientes
| Ingrediente | Preço | Quantidade |
|-------------|-------|------------|
| **Café** | $10 | 5 unidades |
| **Leite** | $8 | 5 unidades |
| **Canela** | $12 | 3 unidades |
| **Açúcar** | $5 | 5 unidades |
| **Chocolate** | $15 | 3 unidades |
| **Água** | $2 | 10 unidades |

### ?? Estratégia de Compras
- **Novos ingredientes** permitem criar drinks mais variados
- **Novas tags** podem resultar em melhores combinações
- **Reposição** mantém o estoque para receitas conhecidas
- Gerencie seu dinheiro sabiamente!

### ?? Fluxo da Loja
1. **Abertura Automática**: Após 3 clientes servidos
2. **Exibição de Itens**: Lista com preços e quantidades
3. **Seleção**: Escolha os itens desejados
4. **Compra**: Gasta dinheiro e atualiza inventário
5. **Continuação**: Opção de comprar mais itens
6. **Fechamento**: Volta ao jogo normal

## ??? Tecnologias

- **Framework**: .NET 8
- **Linguagem**: C# 12.0
- **Arquitetura**: Clean Architecture
- **Padrões**: DDD, Event-Driven Architecture
- **Testes**: NUnit, Moq
- **Interface**: Console Application

## ?? Como Executar

### Pré-requisitos
- .NET 8 SDK instalado

### Executando o Jogo# Clone o repositório
git clone [repository-url]

# Navegue para o diretório
cd Bartender

# Execute o projeto
dotnet run
### Como Jogar
1. **Início**: O jogo mostra uma mensagem de boas-vindas
2. **Cliente Chega**: Um cliente chega com uma preferência específica
3. **Seleção de Ingredientes**: Escolha ingredientes digitando números separados por vírgula (ex: 1,2,3)
4. **Confirmação**: Confirme se deseja servir o drink preparado
5. **Reação**: Veja a reação do cliente e o pagamento recebido
6. **?? Loja**: A cada 3 clientes, a loja abre automaticamente
7. **?? Compras**: Compre novos ingredientes ou reponha estoque
8. **Continuação**: Escolha se deseja continuar jogando

### ?? Como Usar a Loja
1. **Abertura Automática**: A loja abre após servir 3 clientes
2. **Visualização**: Veja seus fundos e itens disponíveis
3. **Seleção**: Digite o número do item desejado
4. **Compra**: Confirme a compra (se tiver dinheiro suficiente)
5. **Continuação**: Escolha continuar comprando ou sair da loja

## ?? Testes

O projeto possui cobertura completa de testes unitários:
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
### Estrutura de TestesBartenderTests/
??? Tests/
?   ??? Domain/
?   ?   ??? Models/          # Inclui ShopItemTests
?   ?   ??? Services/        # Inclui ShopServiceTests
?   ??? UseCases/            # Inclui ShopUseCaseTests
?   ??? Events/              # Inclui testes dos novos eventos
?   ??? Adapters/Input/UI/
### Testes Destacados
- **PaymentServiceConsistencyTests**: Verifica comportamento aleatório do sistema de pagamento
- **ServeClientUseCaseTests**: Testa lógica de avaliação de clientes
- **GameLoopUseCaseTests**: Verifica fluxo completo do jogo
- **CraftServiceTests**: Testa criação de drinks
- **?? ShopServiceTests**: Testa funcionalidades da loja
- **?? ShopUseCaseTests**: Testa casos de uso da loja
- **?? GameStateTests**: Inclui testes para SpendMoney e ShouldOpenShop
- **?? InventoryServiceShopTests**: Testa adição e reposição de ingredientes

## ?? Documentação Técnica

### Interfaces Principais

#### `IGameView`
Interface para abstração da camada de apresentação:public interface IGameView
{
    void DisplayWelcomeMessage();
    void DisplayClientArrival(Client client);
    List<Ingredient> GetSelectedIngredients(List<Ingredient> availableIngredients);
    bool ConfirmServeDrink(Drink drink);
    void DisplayClientReaction(string reactionMessage);
    void DisplayPaymentResult(PaymentResult paymentResult);
    bool AskToContinue();
    
    // ?? Métodos da loja
    void DisplayShopOpening();
    void DisplayShopItems(List<ShopItem> items, int playerMoney);
    ShopItem? GetSelectedShopItem(List<ShopItem> items);
    void DisplayPurchaseResult(bool success, ShopItem item, int remainingMoney);
    bool AskToContinueShopping();
}
#### `IShopService` ??public interface IShopService
{
    List<ShopItem> GetAvailableItems();
    bool CanPurchase(ShopItem item, int playerMoney);
    void PurchaseItem(ShopItem item);
}
#### `IPaymentService`public interface IPaymentService
{
    PaymentResult CalculatePayment(ClientReaction reaction, int baseDrinkPrice);
}
### Fluxo Principal do Jogo

1. **Inicialização**: `Program.cs` configura dependências
2. **Loop Principal**: `GamePresenter.StartGame()`
3. **Nova Rodada**: `GameLoopUseCase.StartNewRound()`
4. **Preparação**: `PrepareDrinkUseCase.Execute()`
5. **Servir**: `ServeClientUseCase.Execute()`
6. **Finalização**: `GameLoopUseCase.CompleteRound()`
7. **?? Verificação da Loja**: `GameState.ShouldOpenShop()`
8. **?? Abertura da Loja**: `GamePresenter.OpenShop()`

### Padrões de Design Utilizados

- **Repository Pattern**: Para abstração de dados
- **Observer Pattern**: Sistema de eventos
- **Strategy Pattern**: Diferentes estratégias de pagamento
- **Dependency Injection**: Inversão de controle
- **MVP Pattern**: Separação de apresentação e lógica

### Extensibilidade

O projeto foi desenhado para fácil extensão:

- **Novos Ingredientes**: Adicionar ao `ShopService`
- **Novas Receitas**: Estender `RecipeBook`
- **Novos Efeitos**: Expandir mapa de compatibilidade
- **Nova UI**: Implementar `IGameView`
- **Novos Eventos**: Implementar `IEvent`
- **?? Novos Itens da Loja**: Estender `ShopService.InitializeShopItems()`
- **?? Preços Dinâmicos**: Implementar estratégias de precificação
- **?? Ciclos da Loja**: Modificar `GameState.ShouldOpenShop()`

---

## ?? Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudanças
4. Push para a branch
5. Abra um Pull Request

## ?? Licença

Este projeto está sob a licença MIT. Veja o arquivo `LICENSE` para detalhes.

---

*Desenvolvido com ? e muito carinho! Agora com loja de ingredientes para expandir suas possibilidades! ??*