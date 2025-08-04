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
        private readonly IInventoryService _inventoryService;
        private readonly EventBus _eventBus;

        public GamePresenter(
            IGameView gameView,
            GameLoopUseCase gameLoopUseCase,
            PrepareDrinkUseCase prepareDrinkUseCase,
            ServeClientUseCase serveClientUseCase,
            ShopUseCase shopUseCase,
            IInventoryService inventoryService,
            EventBus eventBus)
        {
            _gameView = gameView;
            _gameLoopUseCase = gameLoopUseCase;
            _prepareDrinkUseCase = prepareDrinkUseCase;
            _serveClientUseCase = serveClientUseCase;
            _shopUseCase = shopUseCase;
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
                    
                    var gameState = _gameLoopUseCase.GetGameState();
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
                            
                            _gameView.DisplayGameScore(gameState.Score, gameState.CurrentRound);
                            gameRunning = _gameView.AskToContinue();
                        }
                        else
                        {
                            Console.WriteLine("Drink não foi servido. Tente novamente.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro: {ex.Message}");
                    gameRunning = _gameView.AskToContinue();
                }
            }

            _gameView.DisplayGameOver();
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
                    Console.WriteLine("Nenhum item disponível na loja!");
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
                    Console.WriteLine("Você não tem mais dinheiro para comprar!");
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
    }
}