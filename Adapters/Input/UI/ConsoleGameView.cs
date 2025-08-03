using Bartender.Adapters.Input.UI.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Domain.Interfaces;

namespace Bartender.Adapters.Input.UI
{
    public class ConsoleGameView : IGameView
    {
        public void DisplayWelcomeMessage()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("       BEM-VINDO AO BARTENDER GAME!");
            Console.WriteLine("========================================");
            Console.WriteLine("Prepare drinks para satisfazer seus clientes!");
            Console.WriteLine("Pressione qualquer tecla para começar...");
            Console.ReadKey();
        }

        public void DisplayClientArrival(Client client)
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine($"CLIENTE: {client.Name}");
            Console.WriteLine("========================================");
            Console.WriteLine($"\"Olá! Eu gostaria de algo {client.DesiredEffect}, por favor.\"");
            Console.WriteLine();
        }

        public void DisplayAvailableIngredients(List<Ingredient> ingredients)
        {
            Console.WriteLine("INGREDIENTES DISPONÍVEIS:");
            Console.WriteLine("----------------------------");
            for (int i = 0; i < ingredients.Count; i++)
            {
                var ingredient = ingredients[i];
                Console.WriteLine($"{i + 1}. {ingredient.Name} (Tags: {string.Join(", ", ingredient.Tags)})");
            }
            Console.WriteLine();
        }

        public void DisplayAvailableIngredients(List<Ingredient> ingredients, IInventoryService inventoryService)
        {
            Console.WriteLine("INGREDIENTES DISPONÍVEIS:");
            Console.WriteLine("----------------------------");
            for (int i = 0; i < ingredients.Count; i++)
            {
                var ingredient = ingredients[i];
                var quantity = inventoryService.GetIngredientStock(ingredient.Name);
                Console.WriteLine($"{i + 1}. {ingredient.Name} (Tags: {string.Join(", ", ingredient.Tags)}) - Quantidade: {quantity}");
            }
            Console.WriteLine();
        }

        public List<Ingredient> GetSelectedIngredients(List<Ingredient> availableIngredients)
        {
            var selectedIngredients = new List<Ingredient>();
            
            Console.WriteLine("PREPARANDO DRINK:");
            Console.WriteLine("Digite os números dos ingredientes que deseja usar (separados por vírgula).");
            Console.WriteLine("Exemplo: 1,3,5 para usar ingredientes 1, 3 e 5");
            Console.Write("Ingredientes selecionados: ");
            
            var input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
                return selectedIngredients;

            var indices = input.Split(',')
                              .Select(s => s.Trim())
                              .Where(s => int.TryParse(s, out _))
                              .Select(int.Parse)
                              .Where(i => i >= 1 && i <= availableIngredients.Count)
                              .Select(i => i - 1)
                              .Distinct();

            foreach (var index in indices)
            {
                selectedIngredients.Add(availableIngredients[index]);
            }

            return selectedIngredients;
        }

        public bool ConfirmServeDrink(Drink drink)
        {
            Console.WriteLine();
            Console.WriteLine("DRINK PREPARADO:");
            Console.WriteLine($"   Nome: {drink.Name}");
            Console.WriteLine($"   Efeito: {drink.Effect}");
            Console.WriteLine($"   Ingredientes: {string.Join(", ", drink.Ingredients.Select(i => i.Name))}");
            Console.WriteLine();
            Console.Write("Deseja servir este drink? (s/n): ");
            
            var response = Console.ReadLine()?.ToLower();
            return response == "s" || response == "sim" || response == "y" || response == "yes";
        }

        public void DisplayClientReaction(string reactionMessage)
        {
            Console.WriteLine();
            Console.WriteLine("REAÇÃO DO CLIENTE:");
            Console.WriteLine("----------------------------");
            Console.WriteLine(reactionMessage);
            Console.WriteLine();
        }

        public void DisplayPaymentResult(PaymentResult paymentResult)
        {
            Console.WriteLine();
            Console.WriteLine("?? PAGAMENTO:");
            Console.WriteLine("========================================");
            Console.WriteLine(paymentResult.PaymentMessage);
            Console.WriteLine();
            
            if (paymentResult.Status == PaymentStatus.Paid)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"? Total recebido: ${paymentResult.TotalAmount}");
                if (paymentResult.TipAmount > 0)
                {
                    Console.WriteLine($"?? Gorjeta: ${paymentResult.TipAmount}");
                }
            }
            else if (paymentResult.Status == PaymentStatus.PartiallyPaid)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"??  Valor parcial recebido: ${paymentResult.TotalAmount}");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("? Nenhum pagamento recebido!");
            }
            
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("Pressione qualquer tecla para continuar...");
            Console.ReadKey();
        }

        public void DisplayGameScore(int score, int round)
        {
            Console.WriteLine();
            Console.WriteLine("STATUS DO JOGO:");
            Console.WriteLine($"   Rodada: {round}");
            Console.WriteLine($"   ?? Dinheiro total: ${score}");
            Console.WriteLine();
        }

        public bool AskToContinue()
        {
            Console.Write("Deseja continuar jogando? (s/n): ");
            var response = Console.ReadLine()?.ToLower();
            return response == "s" || response == "sim" || response == "y" || response == "yes";
        }

        public void DisplayGameOver()
        {
            Console.Clear();
            Console.WriteLine("========================================");
            Console.WriteLine("           GAME OVER!");
            Console.WriteLine("========================================");
            Console.WriteLine("Obrigado por jogar Bartender Game!");
            Console.WriteLine("Pressione qualquer tecla para sair...");
            Console.ReadKey();
        }
    }
}