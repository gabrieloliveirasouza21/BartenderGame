using Bartender.Adapters.Input.UI.Interfaces;
using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Events;
using Bartender.GameCore.Events.Events;
using Bartender.GameCore.UseCases;

namespace Bartender.Adapters.Input.UI
{
    public class GamePresenter
    {
        private readonly IGameView _gameView;
        private readonly GameLoopUseCase _gameLoopUseCase;
        private readonly PrepareDrinkUseCase _prepareDrinkUseCase;
        private readonly ServeClientUseCase _serveClientUseCase;
        private readonly ShopUseCase _shopUseCase;
        private readonly MatchUseCase _matchUseCase;
        private readonly IInventoryService _inventoryService;
        private readonly EventBus _eventBus;

        public GamePresenter(
            IGameView gameView,
            GameLoopUseCase gameLoopUseCase,
            PrepareDrinkUseCase prepareDrinkUseCase,
            ServeClientUseCase serveClientUseCase,
            ShopUseCase shopUseCase,
            MatchUseCase matchUseCase,
            IInventoryService inventoryService,
            EventBus eventBus)
        {
            _gameView = gameView;
            _gameLoopUseCase = gameLoopUseCase;
            _prepareDrinkUseCase = prepareDrinkUseCase;
            _serveClientUseCase = serveClientUseCase;
            _shopUseCase = shopUseCase;
            _matchUseCase = matchUseCase;
            _inventoryService = inventoryService;
            _eventBus = eventBus;

            SubscribeToEvents();
        }

        public virtual void StartGame()
        {
            _gameView.DisplayWelcomeMessage();
            
            bool gameRunning = true;
            while (gameRunning)
            {
                try
                {
                    // Sele��o do modo de jogo/partida
                    var selectedGameMode = SelectGameMode();
                    if (selectedGameMode == null)
                    {
                        break; // Jogador escolheu sair
                    }

                    // Iniciar nova partida
                    var gameState = _gameLoopUseCase.GetGameState();
                    var matchState = _matchUseCase.StartNewMatch(selectedGameMode.Id, gameState);
                    
                    // Loop principal da partida
                    bool matchRunning = true;
                    while (matchRunning && !matchState.IsCompleted)
                    {
                        try
                        {
                            // Mostrar progresso da partida
                            _gameView.DisplayMatchProgress(matchState);
                            
                            var client = _gameLoopUseCase.StartNewRound();
                            _gameView.DisplayClientArrival(client);

                            var availableIngredients = _inventoryService.GetAvailableIngredients();
                            _gameView.DisplayAvailableIngredients(availableIngredients, _inventoryService);

                            var selectedIngredients = _gameView.GetSelectedIngredients(availableIngredients);

                            if (selectedIngredients.Count == 0)
                            {
                                Console.WriteLine("Nenhum ingrediente selecionado. Tente novamente.");
                                continue;
                            }

                            if (!_inventoryService.HasIngredients(selectedIngredients))
                            {
                                Console.WriteLine("Ingredientes insuficientes!");
                                continue;
                            }

                            _prepareDrinkUseCase.Execute(selectedIngredients);
                            
                            if (gameState.PreparedDrink != null)
                            {
                                bool confirmServe = _gameView.ConfirmServeDrink(gameState.PreparedDrink);
                                if (confirmServe)
                                {
                                    var reaction = _serveClientUseCase.Execute(client, gameState.PreparedDrink);
                                    _gameLoopUseCase.CompleteRound(reaction);
                                    
                                    // Check if shop should open
                                    if (gameState.ShouldOpenShop())
                                    {
                                        OpenShop(gameState);
                                    }
                                    
                                    // Verificar se a partida foi completada
                                    if (matchState.IsCompleted)
                                    {
                                        _gameView.DisplayMatchCompleted(matchState);
                                        _matchUseCase.EndMatch(gameState);
                                        matchRunning = false;
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("Drink n�o foi servido. Tente novamente.");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Erro durante a partida: {ex.Message}");
                            matchRunning = _gameView.AskToContinue();
                        }
                    }
                    
                    // Perguntar se quer continuar jogando (nova partida)
                    gameRunning = _gameView.AskToContinue();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                    gameRunning = _gameView.AskToContinue();
                }
            }

            _gameView.DisplayGameOver();
        }

        private GameMode? SelectGameMode()
        {
            var availableGameModes = _matchUseCase.GetAvailableGameModes();
            return _gameView.DisplayGameModeSelection(availableGameModes);
        }

        private void OpenShop(GameState gameState)
        {
            _gameView.DisplayShopOpening();
            _eventBus.Publish(new ShopOpenedEvent(gameState.CurrentRound - 1));
            
            bool continueShopping = true;
            while (continueShopping && gameState.Money > 0)
            {
                var availableItems = _shopUseCase.GetAvailableItems();
                
                if (!availableItems.Any())
                {
                    Console.WriteLine("Nenhum item dispon�vel na loja!");
                    break;
                }

                _gameView.DisplayShopItems(availableItems, gameState.Money);
                var selectedItem = _gameView.GetSelectedShopItem(availableItems);
                
                if (selectedItem == null)
                {
                    break; // Player chose to exit shop
                }

                var success = _shopUseCase.PurchaseItem(selectedItem, gameState);
                _gameView.DisplayPurchaseResult(success, selectedItem, gameState.Money);
                
                if (gameState.Money > 0)
                {
                    continueShopping = _gameView.AskToContinueShopping();
                }
                else
                {
                    Console.WriteLine("Voc� n�o tem mais dinheiro para comprar!");
                    continueShopping = false;
                }
            }
        }

        private void SubscribeToEvents()
        {
            _eventBus.Subscribe<DrinkPreparedEvent>(OnDrinkPrepared);
            _eventBus.Subscribe<ClientReactionEvent>(OnClientReaction);
            _eventBus.Subscribe<PaymentProcessedEvent>(OnPaymentProcessed);
            _eventBus.Subscribe<ItemPurchasedEvent>(OnItemPurchased);
            _eventBus.Subscribe<ShopOpenedEvent>(OnShopOpened);
            _eventBus.Subscribe<MatchStartedEvent>(OnMatchStarted);
            _eventBus.Subscribe<MatchCompletedEvent>(OnMatchCompleted);
            _eventBus.Subscribe<BossClientArrivedEvent>(OnBossClientArrived);
        }

        private void OnDrinkPrepared(IEvent eventData)
        {
            var drinkEvent = (DrinkPreparedEvent)eventData;
            _gameLoopUseCase.SetPreparedDrink(drinkEvent.Drink);
        }

        private void OnClientReaction(IEvent eventData)
        {
            var reactionEvent = (ClientReactionEvent)eventData;
            _gameView.DisplayClientReaction(reactionEvent.Message);
        }

        private void OnPaymentProcessed(IEvent eventData)
        {
            var paymentEvent = (PaymentProcessedEvent)eventData;
            _gameView.DisplayPaymentResult(paymentEvent.PaymentResult);
        }

        private void OnItemPurchased(IEvent eventData)
        {
            var purchaseEvent = (ItemPurchasedEvent)eventData;
            // Additional logging or processing can be added here
        }

        private void OnShopOpened(IEvent eventData)
        {
            var shopEvent = (ShopOpenedEvent)eventData;
            // Additional logging or processing can be added here
        }

        private void OnMatchStarted(IEvent eventData)
        {
            var matchEvent = (MatchStartedEvent)eventData;
            // Additional logging or processing can be added here
        }

        private void OnMatchCompleted(IEvent eventData)
        {
            var matchEvent = (MatchCompletedEvent)eventData;
            // Additional logging or processing can be added here
        }

        private void OnBossClientArrived(IEvent eventData)
        {
            var bossEvent = (BossClientArrivedEvent)eventData;
            // Additional logging or processing can be added here
        }
    }
}