# ?? Bartender Game

Um jogo de simula��o de bar desenvolvido em C# (.NET 8) onde voc� atua como bartender, preparando drinks para clientes com diferentes prefer�ncias e gerenciando pagamentos baseados na satisfa��o dos clientes. **Nova Feature**: Loja de ingredientes que abre a cada 3 clientes servidos!

**? Interface totalmente redesenhada com Spectre.Console para uma experi�ncia visual incr�vel!**

## ?? �ndice

- [Vis�o Geral](#-vis�o-geral)
- [Interface Visual](#-interface-visual)
- [Arquitetura](#-arquitetura)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Mec�nicas do Jogo](#-mec�nicas-do-jogo)
- [Sistema de Loja](#-sistema-de-loja)
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
- **?? Compra novos ingredientes na loja a cada 3 clientes servidos**
- **? Desfruta de uma interface visual rica e colorida**

O jogo implementa uma mec�nica de **compatibilidade de efeitos** onde clientes reagem diferentemente dependendo de qu�o pr�ximo o drink servido est� do que eles desejavam.

## ? Interface Visual

O jogo utiliza **Spectre.Console** para criar uma experi�ncia visual rica:

### ?? Caracter�sticas da Interface

- **??? T�tulos ASCII Art**: Logos estilizados para se��es importantes
- **?? Tabelas Din�micas**: Informa��es organizadas em tabelas coloridas
- **?? Pain�is Tem�ticos**: Cada se��o tem seu pr�prio estilo visual
- **?? Cores Contextuais**: 
  - ?? Verde para sucesso e dinheiro alto
  - ?? Amarelo para avisos e dinheiro m�dio
  - ?? Vermelho para erros e dinheiro baixo
  - ?? Azul para informa��es dos clientes
  - ?? Roxo para status do jogo
- **?? Inputs Intuitivos**: Prompts claros e valida��o autom�tica
- **?? Anima��es de Texto**: Efeitos visuais para experi�ncia imersiva

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
?  � Misture ingredientes para criar drinks �nicos                                             ?
?  � Satisfa�a as prefer�ncias dos clientes                                                    ?
?  � Ganhe dinheiro e expanda seu bar                                                          ?
?  � Visite a loja a cada 3 clientes!                                                          ?
?                                                                                               ?
?????????????????????????????????????????????????????????????????????????????????????????????????
#### ?? Cliente?????????????????????????????????????????? ?? CLIENTE: MARIA ??????????????????????????????????????????

???? Pedido?????????????????????????????????????????????????????????????????????????????????????????
?                                                                                                   ?
?  "Ol�! Eu gostaria de algo Reconfortante, por favor."                                            ?
?                                                                                                   ?
?????????????????????????????????????????????????????????????????????????????????????????????????????
#### ?? Invent�rio           ?? INGREDIENTES DISPON�VEIS           
?????????????????????????????????????????
? # ? Nome       ? Tags    ? Quantidade ?
?????????????????????????????????????????
? 1 ? Caf�       ? Amargo  ?     10     ?
? 2 ? Leite      ? Doce    ?     8      ?
? 3 ? Canela     ? Doce    ?     5      ?
? 4 ? A��car     ? Nenhuma ?     7      ?
? 5 ? Chocolate  ? Nenhuma ?     4      ?
? 6 ? �gua       ? Nenhuma ?     15     ?
?????????????????????????????????????????
#### ?? Loja??     ??????      ??  ?????  
??    ??    ??     ?? ??   ?? 
??    ??    ??     ?? ??????? 
??    ??    ????   ?? ??   ?? 
?????  ?????? ????? ?? ??   ?? 

???? LOJA DE INGREDIENTES????????????????????????????????????????????????????????????????????????????
?                                                                                                   ?
?  ?? Parab�ns! Voc� serviu 3 clientes! ??                                                         ?
?                                                                                                   ?
?  ?? A loja est� aberta para compras!                                                             ?
?  Aproveite para expandir seu arsenal de ingredientes                                             ?
?                                                                                                   ?
?????????????????????????????????????????????????????????????????????????????????????????????????????
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
4. **UI Layer**: Interface com o usu�rio (Spectre.Console)

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
    
    // ?? Novos m�todos para loja
    public void SpendMoney(int amount);
    public bool ShouldOpenShop(); // True a cada 3 clientes
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

#### `InventoryService` ??
- **Responsabilidade**: Gerenciamento do estoque de ingredientes
- **Funcionalidades**:
  - `GetAvailableIngredients()` - Lista ingredientes dispon�veis
  - `ConsumeIngredients()` - Consome ingredientes utilizados
  - `HasIngredients()` - Verifica disponibilidade
  - **?? `AddNewIngredient()` - Adiciona novos ingredientes da loja**
  - **?? `RestockIngredient()` - Rep�e ingredientes existentes**

#### `PaymentService`
- **Responsabilidade**: Calcula pagamentos baseado na rea��o do cliente
- **Sistema de Pagamento**:
  - **VeryHappy/Happy**: 100% + 20% gorjeta
  - **Neutral**: 50% do valor
  - **Disappointed**: 10% (90% chance) ou 0% (10% chance)
  - **Angry**: 0%

#### `ShopService` ??
- **Responsabilidade**: Gerencia itens da loja e compras
- **Funcionalidades**:
  - `GetAvailableItems()` - Lista itens dispon�veis para compra
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

#### `ShopUseCase` ??
- **Responsabilidade**: Orquestra opera��es da loja
- **Funcionalidades**:
  - `GetAvailableItems()` - Obt�m itens dispon�veis
  - `CanPurchase()` - Verifica poder de compra
  - `PurchaseItem()` - Executa compra e atualiza estado do jogo

### ?? Sistema de Eventos

O jogo utiliza um **EventBus** para comunica��o desacoplada:

#### Eventos Dispon�veis:
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

## ?? Sistema de Loja

### ?? Quando a Loja Abre
A loja abre automaticamente **a cada 3 clientes servidos**:
- Ap�s o 3� cliente ? Loja abre
- Ap�s o 6� cliente ? Loja abre novamente
- E assim por diante...

### ??? Itens Dispon�veis na Loja

#### ?? Novos Ingredientes (Compra �nica)
| Ingrediente | Tags | Pre�o | Quantidade |
|-------------|------|-------|------------|
| **Baunilha** | Doce, Arom�tico | $25 | 3 unidades |
| **Gengibre** | Picante, Estimulante | $30 | 3 unidades |
| **Mel** | Doce, Natural | $20 | 4 unidades |
| **Lim�o** | �cido, Refrescante | $15 | 5 unidades |
| **Hortel�** | Refrescante, Mentolado | $18 | 4 unidades |

> **?? Importante**: Cada novo ingrediente pode ser comprado apenas **uma vez**. Ap�s a compra, ele **sai da loja permanentemente**.

#### ?? Reposi��o de Ingredientes (Compra Ilimitada)
| Ingrediente | Pre�o | Quantidade |
|-------------|-------|------------|
| **Caf�** | $10 | 5 unidades |
| **Leite** | $8 | 5 unidades |
| **Canela** | $12 | 3 unidades |
| **A��car** | $5 | 5 unidades |
| **Chocolate** | $15 | 3 unidades |
| **�gua** | $2 | 10 unidades |

> **?? Reposi��o**: Estes itens **permanecem na loja** ap�s compra e podem ser comprados m�ltiplas vezes.

### ?? Estrat�gia de Compras
- **Novos ingredientes** permitem criar drinks mais variados
- **Novas tags** podem resultar em melhores combina��es
- **Reposi��o** mant�m o estoque para receitas conhecidas
- **Planeje bem**: novos ingredientes s� podem ser comprados uma vez!

### ?? Fluxo da Loja
1. **Abertura Autom�tica**: Ap�s 3 clientes servidos
2. **Exibi��o de Itens**: Lista com pre�os e quantidades
   - ?? = Novo ingrediente (compra �nica) - aparece com **NEW!** na primeira vez
   - ?? = Reposi��o (compra ilimitada) - ingredientes originais e previamente comprados
3. **Sele��o**: Escolha os itens desejados
4. **Compra**: Gasta dinheiro e atualiza invent�rio
5. **Remo��o Autom�tica**: Novos ingredientes saem da categoria de novos
6. **Convers�o**: Ingredientes comprados aparecem como reposi��o nas pr�ximas lojas
7. **Continua��o**: Op��o de comprar mais itens
8. **Fechamento**: Volta ao jogo normal

### ?? Mec�nica de Compra �nica e Reposi��o```csharp
// Novos ingredientes s�o rastreados para evitar re-compra
private readonly HashSet<string> _purchasedNewIngredients;

public void PurchaseItem(ShopItem item)
{
    if (item.Type == ShopItemType.NewIngredient)
    {
        _inventoryService.AddNewIngredient(item.Name, item.Tags, item.Quantity);
        _purchasedNewIngredients.Add(item.Name); // Remove da categoria de novos
    }
    // Reposi��o pode ser comprada infinitamente
}

// Ingredientes comprados aparecem como reposi��o em lojas subsequentes
foreach (var purchasedIngredientName in _purchasedNewIngredients)
{
    if (_inventoryService.GetIngredientStock(purchasedIngredientName) >= 0)
    {
        // Cria item de reposi��o com pre�o reduzido (60% do original)
        var restockItem = new ShopItem(/* ... */);
        availableItems.Add(restockItem);
    }
}
### ?? Sistema de Progress�o da Loja

#### **Primeira Loja (Rodada 3)**
- **?? Novos Ingredientes**: Baunilha, Gengibre, Mel, Lim�o, Hortel� (com **NEW!**)
- **?? Reposi��o**: Caf�, Leite, Canela, A��car, Chocolate, �gua

#### **Segunda Loja (Rodada 6)**
- **?? Novos Ingredientes**: Apenas os que n�o foram comprados na primeira loja
- **?? Reposi��o**: 
  - Ingredientes originais (Caf�, Leite, etc.)
  - **+ Ingredientes comprados** na primeira loja (pre�o reduzido)

#### **Terceira Loja (Rodada 9)**
- **?? Novos Ingredientes**: Apenas os que nunca foram comprados
- **?? Reposi��o**: 
  - Ingredientes originais
  - **+ Todos os ingredientes comprados** nas lojas anteriores

### ?? Sistema de Pre�os da Loja
| Tipo | Pre�o | Quantidade | Disponibilidade |
|------|-------|------------|-----------------|
| **Novo Ingrediente** | 100% | Original | Uma vez apenas |
| **Reposi��o Original** | Fixo | Fixo | Ilimitada |
| **Reposi��o Comprada** | 60% do original | Original - 1 | Ilimitada |

> **?? Estrat�gia**: Ingredientes comprados como novos t�m pre�o reduzido na reposi��o!