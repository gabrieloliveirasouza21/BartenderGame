# ?? Bartender Game

Um jogo de simulação de bar desenvolvido em C# (.NET 8) onde você atua como bartender, preparando drinks para clientes com diferentes preferências e gerenciando pagamentos baseados na satisfação dos clientes.

## ?? Índice

- [Visão Geral](#-visão-geral)
- [Arquitetura](#-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Mecânicas do Jogo](#-mecânicas-do-jogo)
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

#### `InventoryService`
- **Responsabilidade**: Gerenciamento do estoque de ingredientes
- **Funcionalidades**:
  - `GetAvailableIngredients()` - Lista ingredientes disponíveis
  - `ConsumeIngredients()` - Consome ingredientes utilizados
  - `HasIngredients()` - Verifica disponibilidade

#### `PaymentService`
- **Responsabilidade**: Calcula pagamentos baseado na reação do cliente
- **Sistema de Pagamento**:
  - **VeryHappy/Happy**: 100% + 20% gorjeta
  - **Neutral**: 50% do valor
  - **Disappointed**: 10% (90% chance) ou 0% (10% chance)
  - **Angry**: 0%

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

### ?? Sistema de Eventos

O jogo utiliza um **EventBus** para comunicação desacoplada:

#### Eventos Disponíveis:
- `ClientArrivedEvent` - Cliente chegou ao bar
- `DrinkPreparedEvent` - Drink foi preparado
- `DrinkServedEvent` - Drink foi servido ao cliente
- `ClientReactionEvent` - Cliente reagiu ao drink
- `PaymentProcessedEvent` - Pagamento foi processado
- `GameRoundCompletedEvent` - Rodada foi finalizada
// Exemplo de uso
_eventBus.Subscribe<ClientReactionEvent>(OnClientReaction);
_eventBus.Publish(new ClientReactionEvent(client, drink, reaction, message));
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
6. **Continuação**: Escolha se deseja continuar jogando

## ?? Testes

O projeto possui cobertura completa de testes unitários:
# Executar todos os testes
dotnet test

# Executar testes com cobertura
dotnet test --collect:"XPlat Code Coverage"
### Estrutura de TestesBartenderTests/
??? Tests/
?   ??? Domain/
?   ?   ??? Models/
?   ?   ??? Services/
?   ??? UseCases/
?   ??? Adapters/Input/UI/
### Testes Destacados
- **PaymentServiceConsistencyTests**: Verifica comportamento aleatório do sistema de pagamento
- **ServeClientUseCaseTests**: Testa lógica de avaliação de clientes
- **GameLoopUseCaseTests**: Verifica fluxo completo do jogo
- **CraftServiceTests**: Testa criação de drinks

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

### Padrões de Design Utilizados

- **Repository Pattern**: Para abstração de dados
- **Observer Pattern**: Sistema de eventos
- **Strategy Pattern**: Diferentes estratégias de pagamento
- **Dependency Injection**: Inversão de controle
- **MVP Pattern**: Separação de apresentação e lógica

### Extensibilidade

O projeto foi desenhado para fácil extensão:

- **Novos Ingredientes**: Adicionar ao `InventoryService`
- **Novas Receitas**: Estender `RecipeBook`
- **Novos Efeitos**: Expandir mapa de compatibilidade
- **Nova UI**: Implementar `IGameView`
- **Novos Eventos**: Implementar `IEvent`

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

*Desenvolvido com ? e muito carinho!*