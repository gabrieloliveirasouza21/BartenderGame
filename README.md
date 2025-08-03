# ?? Bartender Game

Um jogo de simula��o de bar desenvolvido em C# (.NET 8) onde voc� atua como bartender, preparando drinks para clientes com diferentes prefer�ncias e gerenciando pagamentos baseados na satisfa��o dos clientes.

## ?? �ndice

- [Vis�o Geral](#-vis�o-geral)
- [Arquitetura](#-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Mec�nicas do Jogo](#-mec�nicas-do-jogo)
- [Tecnologias](#-tecnologias)
- [Como Executar](#-como-executar)
- [Testes](#-testes)
- [Documenta��o T�cnica](#-documenta��o-t�cnica)

## ?? Vis�o Geral

O **Bartender Game** � um jogo de console onde voc�:
- Recebe clientes com prefer�ncias espec�ficas de drinks
- Seleciona ingredientes do invent�rio
- Prepara drinks baseados em receitas ou improvisa��o
- Recebe pagamentos baseados na satisfa��o do cliente
- Gerencia um invent�rio limitado de ingredientes

O jogo implementa uma mec�nica de **compatibilidade de efeitos** onde clientes reagem diferentemente dependendo de qu�o pr�ximo o drink servido est� do que eles desejavam.

## ??? Arquitetura

O projeto segue os princ�pios de **Clean Architecture** e **Domain-Driven Design**:
??? GameCore/               # N�cleo da aplica��o
?   ??? Domain/            # Modelos de dom�nio e regras de neg�cio
?   ?   ??? Models/        # Entidades principais
?   ?   ??? Services/      # Servi�os de dom�nio
?   ?   ??? Interfaces/    # Contratos de interface
?   ??? UseCases/          # Casos de uso (Application Layer)
?   ??? Events/            # Sistema de eventos
??? Adapters/              # Camada de adaptadores
    ??? Input/UI/          # Interface do usu�rio (Console)
### Camadas da Arquitetura

1. **Domain Layer**: Cont�m as regras de neg�cio puras
2. **Application Layer**: Orquestra os casos de uso
3. **Infrastructure Layer**: Implementa��es concretas
4. **UI Layer**: Interface com o usu�rio

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
- **Responsabilidade**: Gerencia a sequ�ncia de clientes
- **M�todo Principal**: `GetNextClient()` - Retorna o pr�ximo cliente na fila

#### `CraftService`
- **Responsabilidade**: Cria��o de drinks baseado em ingredientes
- **L�gica**: 
  - Verifica receitas conhecidas primeiro
  - Se n�o encontrar, cria drink din�mico baseado nas tags dominantes
- **M�todo Principal**: `Craft(List<Ingredient> ingredients)`

#### `InventoryService`
- **Responsabilidade**: Gerenciamento do estoque de ingredientes
- **Funcionalidades**:
  - `GetAvailableIngredients()` - Lista ingredientes dispon�veis
  - `ConsumeIngredients()` - Consome ingredientes utilizados
  - `HasIngredients()` - Verifica disponibilidade

#### `PaymentService`
- **Responsabilidade**: Calcula pagamentos baseado na rea��o do cliente
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
- **Responsabilidade**: Orquestra a prepara��o de drinks
- **Fluxo**:
  1. Verifica disponibilidade de ingredientes
  2. Chama o CraftService para criar o drink
  3. Consome ingredientes do invent�rio
  4. Publica evento de drink preparado

#### `ServeClientUseCase`
- **Responsabilidade**: Gerencia a intera��o entre cliente e drink servido
- **Sistema de Avalia��o**:
  1. **Match Exato**: Cliente fica `VeryHappy`
  2. **Compatibilidade**: Calcula baseado em mapa de efeitos
     - \> 80% = `Happy`
     - \> 50% = `Neutral`
     - \> 20% = `Disappointed`
     - ? 20% = `Angry`

### ?? Sistema de Eventos

O jogo utiliza um **EventBus** para comunica��o desacoplada:

#### Eventos Dispon�veis:
- `ClientArrivedEvent` - Cliente chegou ao bar
- `DrinkPreparedEvent` - Drink foi preparado
- `DrinkServedEvent` - Drink foi servido ao cliente
- `ClientReactionEvent` - Cliente reagiu ao drink
- `PaymentProcessedEvent` - Pagamento foi processado
- `GameRoundCompletedEvent` - Rodada foi finalizada
// Exemplo de uso
_eventBus.Subscribe<ClientReactionEvent>(OnClientReaction);
_eventBus.Publish(new ClientReactionEvent(client, drink, reaction, message));
## ?? Mec�nicas do Jogo

### ?? Receitas Dispon�veis
1. **Latte Canela**: Caf� + Leite + Canela ? Reconfortante
2. **Espresso**: Caf� ? Energizante  
3. **Caf� Doce**: Caf� + A��car ? Reconfortante
4. **Chocolate Quente**: Chocolate + Leite ? Reconfortante
5. **�gua**: �gua ? Neutro

### ?? Sistema de Compatibilidade de Efeitosvar compatibilityMap = new Dictionary<string, Dictionary<string, double>>
{
    ["Reconfortante"] = { ["Energizante"] = 0.6, ["Doce"] = 0.8, ["Relaxante"] = 0.9 },
    ["Energizante"] = { ["Reconfortante"] = 0.6, ["Estimulante"] = 0.9, ["Forte"] = 0.8 },
    ["Relaxante"] = { ["Reconfortante"] = 0.9, ["Doce"] = 0.7, ["Suave"] = 0.8 }
};
### ?? Sistema de Pagamento
| Rea��o Cliente | Pagamento Base | Gorjeta | Status |
|----------------|----------------|---------|---------|
| VeryHappy | 100% | 20% | Paid |
| Happy | 100% | 20% | Paid |
| Neutral | 50% | 0% | PartiallyPaid |
| Disappointed | 10%* | 0% | PartiallyPaid/NotPaid |
| Angry | 0% | 0% | NotPaid |

*\*10% com 90% de chance, 0% com 10% de chance*

### ?? Invent�rio Inicial
- **Caf�**: 10 unidades (Tag: Amargo)
- **Leite**: 8 unidades (Tag: Doce)
- **Canela**: 5 unidades (Tag: Doce)
- **A��car**: 7 unidades
- **Chocolate**: 4 unidades
- **�gua**: 15 unidades

## ??? Tecnologias

- **Framework**: .NET 8
- **Linguagem**: C# 12.0
- **Arquitetura**: Clean Architecture
- **Padr�es**: DDD, Event-Driven Architecture
- **Testes**: NUnit, Moq
- **Interface**: Console Application

## ?? Como Executar

### Pr�-requisitos
- .NET 8 SDK instalado

### Executando o Jogo# Clone o reposit�rio
git clone [repository-url]

# Navegue para o diret�rio
cd Bartender

# Execute o projeto
dotnet run
### Como Jogar
1. **In�cio**: O jogo mostra uma mensagem de boas-vindas
2. **Cliente Chega**: Um cliente chega com uma prefer�ncia espec�fica
3. **Sele��o de Ingredientes**: Escolha ingredientes digitando n�meros separados por v�rgula (ex: 1,2,3)
4. **Confirma��o**: Confirme se deseja servir o drink preparado
5. **Rea��o**: Veja a rea��o do cliente e o pagamento recebido
6. **Continua��o**: Escolha se deseja continuar jogando

## ?? Testes

O projeto possui cobertura completa de testes unit�rios:
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
- **PaymentServiceConsistencyTests**: Verifica comportamento aleat�rio do sistema de pagamento
- **ServeClientUseCaseTests**: Testa l�gica de avalia��o de clientes
- **GameLoopUseCaseTests**: Verifica fluxo completo do jogo
- **CraftServiceTests**: Testa cria��o de drinks

## ?? Documenta��o T�cnica

### Interfaces Principais

#### `IGameView`
Interface para abstra��o da camada de apresenta��o:public interface IGameView
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

1. **Inicializa��o**: `Program.cs` configura depend�ncias
2. **Loop Principal**: `GamePresenter.StartGame()`
3. **Nova Rodada**: `GameLoopUseCase.StartNewRound()`
4. **Prepara��o**: `PrepareDrinkUseCase.Execute()`
5. **Servir**: `ServeClientUseCase.Execute()`
6. **Finaliza��o**: `GameLoopUseCase.CompleteRound()`

### Padr�es de Design Utilizados

- **Repository Pattern**: Para abstra��o de dados
- **Observer Pattern**: Sistema de eventos
- **Strategy Pattern**: Diferentes estrat�gias de pagamento
- **Dependency Injection**: Invers�o de controle
- **MVP Pattern**: Separa��o de apresenta��o e l�gica

### Extensibilidade

O projeto foi desenhado para f�cil extens�o:

- **Novos Ingredientes**: Adicionar ao `InventoryService`
- **Novas Receitas**: Estender `RecipeBook`
- **Novos Efeitos**: Expandir mapa de compatibilidade
- **Nova UI**: Implementar `IGameView`
- **Novos Eventos**: Implementar `IEvent`

---

## ?? Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature
3. Commit suas mudan�as
4. Push para a branch
5. Abra um Pull Request

## ?? Licen�a

Este projeto est� sob a licen�a MIT. Veja o arquivo `LICENSE` para detalhes.

---

*Desenvolvido com ? e muito carinho!*