using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;

namespace Bartender.GameCore.Domain.Services
{
    public class GameModeService : IGameModeService
    {
        private readonly List<GameMode> _gameModes;

        public GameModeService()
        {
            _gameModes = InitializeGameModes();
        }

        public List<GameMode> GetAvailableGameModes()
        {
            return _gameModes.Where(gm => gm.IsUnlocked).ToList();
        }

        public GameMode GetGameModeById(string id)
        {
            var gameMode = _gameModes.FirstOrDefault(gm => gm.Id == id);
            if (gameMode == null)
                throw new ArgumentException($"Game mode with ID '{id}' not found.");
            
            return gameMode;
        }

        public MatchState CreateNewMatch(string gameModeId)
        {
            var gameMode = GetGameModeById(gameModeId);
            return new MatchState(gameMode);
        }

        public void UnlockNextLocation(MatchState completedMatch)
        {
            if (!completedMatch.CanUnlockNextLocation())
                return;

            // Encontra o próximo local bloqueado na lista
            var nextLockedGameMode = _gameModes.FirstOrDefault(gm => !gm.IsUnlocked);
            if (nextLockedGameMode != null)
            {
                nextLockedGameMode.Unlock();
            }
        }

        public void ApplyStartingModifiers(MatchState matchState, IInventoryService inventoryService)
        {
            foreach (var modifier in matchState.GameMode.Modifiers)
            {
                if (modifier.Type == ModifierType.StartingIngredient)
                {
                    var ingredient = modifier.GetParameter<string>("ingredient");
                    var quantity = modifier.GetParameter<int>("quantity");
                    
                    if (!string.IsNullOrEmpty(ingredient))
                    {
                        inventoryService.AddNewIngredient(ingredient, new List<string>(), quantity);
                    }
                }
            }
        }

        public PaymentResult ApplyPaymentModifiers(MatchState matchState, PaymentResult basePayment, ClientReaction reaction)
        {
            var modifiedAmount = basePayment.TotalAmount;

            foreach (var modifier in matchState.GameMode.Modifiers)
            {
                if (modifier.Type == ModifierType.PaymentBonus && IsPositiveReaction(reaction))
                {
                    var bonusPercentage = modifier.GetParameter<double>("percentage");
                    modifiedAmount = (int)(modifiedAmount * (1 + bonusPercentage / 100));
                }
                else if (modifier.Type == ModifierType.PaymentPenalty && IsNegativeReaction(reaction))
                {
                    var penaltyPercentage = modifier.GetParameter<double>("percentage");
                    modifiedAmount = (int)(modifiedAmount * (1 - penaltyPercentage / 100));
                }
            }

            return new PaymentResult(
                basePayment.BaseAmount,
                basePayment.TipAmount,
                modifiedAmount,
                basePayment.Status,
                basePayment.PaymentMessage
            );
        }

        public string GetBossBonus(MatchState matchState, int dayNumber)
        {
            if (dayNumber < 1 || dayNumber > matchState.BossSatisfactionByDay.Count)
                return "";

            if (matchState.BossSatisfactionByDay[dayNumber - 1])
            {
                return matchState.GameMode.Id switch
                {
                    "cafe_local" => "O cliente VIP recomendou os drinks! Clientes pagam 5% a mais hoje.",
                    "bar_noturno" => "O cliente VIP ficou impressionado! Gorjetas aumentaram em 10% hoje.",
                    "hotel_luxo" => "O cliente VIP deixou uma avaliação 5 estrelas! Pagamentos aumentaram em 15% hoje.",
                    _ => "O cliente VIP ficou satisfeito! Bônus de 5% nos pagamentos hoje."
                };
            }

            return "";
        }

        private bool IsPositiveReaction(ClientReaction reaction)
        {
            return reaction == ClientReaction.VeryHappy || reaction == ClientReaction.Happy;
        }

        private bool IsNegativeReaction(ClientReaction reaction)
        {
            return reaction == ClientReaction.Disappointed || reaction == ClientReaction.Angry;
        }

        private List<GameMode> InitializeGameModes()
        {
            return new List<GameMode>
            {
                // Café Local - Modo básico (desbloqueado)
                new GameMode(
                    "cafe_local",
                    "? Café Local",
                    "Um café aconchegante no centro da cidade. Perfeito para iniciantes!\nHorário: 18:00 - 02:00 | 3 dias de trabalho",
                    true,
                    new List<GameModeModifier>
                    {
                        new GameModeModifier(ModifierType.StartingIngredient, "Baunilha Extra", "Começa com 3 Baunilhas", 
                            new Dictionary<string, object> { { "ingredient", "Baunilha" }, { "quantity", 3 } }),
                        new GameModeModifier(ModifierType.PaymentBonus, "Clientes Simpáticos", "Ao acertar um drink ganha +3% do total", 
                            new Dictionary<string, object> { { "percentage", 3.0 } })
                    }
                ),

                // Bar Noturno - Modo intermediário (bloqueado)
                new GameMode(
                    "bar_noturno",
                    "?? Bar Noturno",
                    "Um bar sofisticado onde os clientes são mais exigentes.\nHorário: 18:00 - 02:00 | 3 dias de trabalho",
                    false,
                    new List<GameModeModifier>
                    {
                        new GameModeModifier(ModifierType.PaymentBonus, "Gorjetas Generosas", "Ao acertar um drink ganha +5% do total", 
                            new Dictionary<string, object> { { "percentage", 5.0 } }),
                        new GameModeModifier(ModifierType.PaymentPenalty, "Clientes Exigentes", "Ao errar um drink perde -5% do total", 
                            new Dictionary<string, object> { { "percentage", 5.0 } })
                    }
                ),

                // Hotel de Luxo - Modo avançado (bloqueado)
                new GameMode(
                    "hotel_luxo",
                    "?? Hotel de Luxo",
                    "Um hotel 5 estrelas com clientes muito sofisticados.\nHorário: 18:00 - 02:00 | 3 dias de trabalho",
                    false,
                    new List<GameModeModifier>
                    {
                        new GameModeModifier(ModifierType.StartingIngredient, "Kit Premium", "Começa com ingredientes especiais", 
                            new Dictionary<string, object> { { "ingredient", "Ouro Comestível" }, { "quantity", 2 } }),
                        new GameModeModifier(ModifierType.PaymentBonus, "Padrão de Luxo", "Ao acertar um drink ganha +8% do total", 
                            new Dictionary<string, object> { { "percentage", 8.0 } }),
                        new GameModeModifier(ModifierType.PaymentPenalty, "Expectativas Altas", "Ao errar um drink perde -8% do total", 
                            new Dictionary<string, object> { { "percentage", 8.0 } })
                    }
                )
            };
        }
    }
}