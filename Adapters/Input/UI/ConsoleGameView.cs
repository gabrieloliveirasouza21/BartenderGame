using Bartender.Adapters.Input.UI.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Domain.Interfaces;
using Spectre.Console;

namespace Bartender.Adapters.Input.UI
{
    public class ConsoleGameView : IGameView
    {
        public void DisplayWelcomeMessage()
        {
            AnsiConsole.Clear();
            
            // Título estilizado
            var title = new FigletText("BARTENDER GAME")
                .Centered()
                .Color(Color.Cyan1);
            
            AnsiConsole.Write(title);
            
            // Panel de boas-vindas
            var welcomePanel = new Panel(
                new Markup("[bold yellow]?? Prepare drinks para satisfazer seus clientes! ??[/]\n\n" +
                          "[dim]• Misture ingredientes para criar drinks únicos[/]\n" +
                          "[dim]• Satisfaça as preferências dos clientes[/]\n" +
                          "[dim]• Ganhe dinheiro e expanda seu bar[/]\n" +
                          "[dim]• Visite a loja a cada 3 clientes![/]\n" +
                          "[dim]• Complete 7 dias de trabalho em diferentes locais![/]\n" +
                          "[dim]• Enfrente clientes misteriosos como chefes![/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1),
                Header = new PanelHeader(" [bold gold1]?? BEM-VINDO AO BARTENDER GAME[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(welcomePanel);
            
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para começar sua jornada...[/]");
            Console.ReadKey();
        }

        public GameMode? DisplayGameModeSelection(List<GameMode> gameModes)
        {
            AnsiConsole.Clear();
            
            var title = new FigletText("SELECIONAR LOCAL")
                .Centered()
                .Color(Color.Gold1);
            
            AnsiConsole.Write(title);
            
            var introPanel = new Panel(
                new Markup("[bold yellow]??? Escolha onde você quer trabalhar como bartender! ???[/]\n\n" +
                          "[dim]Cada local tem suas próprias características:[/]\n" +
                          "[dim]• Bônus e penalidades únicos[/]\n" +
                          "[dim]• 7 dias de trabalho (semana completa)[/]\n" +
                          "[dim]• 10 clientes por dia + 1 cliente chefe no último dia[/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Cyan1),
                Header = new PanelHeader(" [bold cyan1]?? SISTEMA ROGUELIKE[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(introPanel);
            AnsiConsole.WriteLine();

            // Criar tabela de modos de jogo
            var gameModeTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Gold1)
                .Title("[bold gold1]?? LOCAIS DISPONÍVEIS[/]");

            gameModeTable.AddColumn(new TableColumn("[bold]#[/]").Centered());
            gameModeTable.AddColumn(new TableColumn("[bold]Status[/]").Centered());
            gameModeTable.AddColumn(new TableColumn("[bold]Local[/]").LeftAligned());
            gameModeTable.AddColumn(new TableColumn("[bold]Descrição[/]").LeftAligned());
            gameModeTable.AddColumn(new TableColumn("[bold]Modificadores[/]").LeftAligned());

            for (int i = 0; i < gameModes.Count; i++)
            {
                var gameMode = gameModes[i];
                var number = $"[bold cyan]{i + 1}[/]";
                var status = gameMode.IsUnlocked ? "[bold green]? Disponível[/]" : "[bold red]?? Bloqueado[/]";
                var name = gameMode.IsUnlocked ? $"[bold white]{gameMode.Name}[/]" : $"[dim]{gameMode.Name}[/]";
                var description = gameMode.IsUnlocked ? $"[dim]{gameMode.Description}[/]" : "[dim]???[/]";
                
                var modifiers = "";
                if (gameMode.IsUnlocked && gameMode.Modifiers.Any())
                {
                    modifiers = string.Join("\n", gameMode.Modifiers.Select(m => $"[dim yellow]• {m.Description}[/]"));
                }
                else if (!gameMode.IsUnlocked)
                {
                    modifiers = "[dim]???[/]";
                }

                gameModeTable.AddRow(number, status, name, description, modifiers);
            }

            AnsiConsole.Write(gameModeTable);
            AnsiConsole.WriteLine();

            var availableGameModes = gameModes.Where(gm => gm.IsUnlocked).ToList();
            if (!availableGameModes.Any())
            {
                AnsiConsole.MarkupLine("[bold red]? Nenhum local disponível![/]");
                AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para continuar...[/]");
                Console.ReadKey();
                return null;
            }

            var choice = AnsiConsole.Ask<int>(
                $"[bold cyan]Digite o número do local onde deseja trabalhar[/] [dim](1-{gameModes.Count}, 0 para sair):[/]");
            
            if (choice < 1 || choice > gameModes.Count)
                return null;
                
            var selectedGameMode = gameModes[choice - 1];
            
            if (!selectedGameMode.IsUnlocked)
            {
                AnsiConsole.MarkupLine("[bold red]? Este local ainda está bloqueado![/]");
                AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para continuar...[/]");
                Console.ReadKey();
                return null;
            }
            
            // Confirmação
            var confirmPanel = new Panel(
                new Markup($"[bold white]Você escolheu:[/] [bold yellow]{selectedGameMode.Name}[/]\n" +
                          $"[dim]{selectedGameMode.Description}[/]\n\n" +
                          $"[bold white]Modificadores ativos:[/]\n" +
                          string.Join("\n", selectedGameMode.Modifiers.Select(m => $"[dim yellow]• {m.Description}[/]"))))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader(" [bold yellow]?? CONFIRMAÇÃO[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(confirmPanel);
            
            if (AnsiConsole.Confirm("[bold cyan]Confirma sua escolha?[/]"))
            {
                return selectedGameMode;
            }
            
            return null;
        }

        public void DisplayMatchProgress(MatchState matchState)
        {
            var progressTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Purple)
                .Title($"[bold purple]?? PROGRESSO - {matchState.GameMode.Name}[/]");

            progressTable.AddColumn(new TableColumn("[bold]Informação[/]").LeftAligned());
            progressTable.AddColumn(new TableColumn("[bold]Valor[/]").RightAligned());

            progressTable.AddRow("[bold cyan]?? Dia[/]", $"[bold white]{matchState.CurrentDay} / {matchState.GameMode.DaysCount}[/]");
            progressTable.AddRow("[bold cyan]?? Cliente do Dia[/]", $"[bold white]{matchState.CurrentClientInDay} / {matchState.GameMode.ClientsPerDay}[/]");
            
            var moneyColor = matchState.TotalMoney >= 100 ? "green" : matchState.TotalMoney >= 50 ? "yellow" : "red";
            progressTable.AddRow("[bold gold1]?? Dinheiro Total[/]", $"[bold {moneyColor}]${matchState.TotalMoney}[/]");
            
            if (matchState.TotalTips > 0)
            {
                progressTable.AddRow("[bold yellow]?? Gorjetas[/]", $"[bold yellow]${matchState.TotalTips}[/]");
            }

            if (matchState.IsBossDay)
            {
                progressTable.AddRow("[bold red]?? Status[/]", "[bold red]?? DIA DO CHEFE![/]");
            }

            AnsiConsole.Write(progressTable);
            AnsiConsole.WriteLine();
        }

        public void DisplayMatchCompleted(MatchState matchState)
        {
            AnsiConsole.Clear();
            
            var completedTitle = new FigletText("PARTIDA CONCLUIDA")
                .Centered()
                .Color(Color.Green);
            
            AnsiConsole.Write(completedTitle);
            
            var finalScore = matchState.TotalMoney + matchState.TotalTips;
            var scoreColor = finalScore >= 500 ? "green" : finalScore >= 300 ? "yellow" : finalScore >= 100 ? "orange1" : "red";
            var performance = finalScore >= 500 ? "?? EXCELENTE!" : 
                             finalScore >= 300 ? "?? MUITO BOM!" : 
                             finalScore >= 100 ? "?? BOM!" : "?? PODE MELHORAR!";
            
            var resultsPanel = new Panel(
                new Markup($"[bold white]?? Local:[/] [bold yellow]{matchState.GameMode.Name}[/]\n" +
                          $"[bold white]?? Dias Trabalhados:[/] [bold cyan]{matchState.GameMode.DaysCount}[/]\n" +
                          $"[bold white]?? Clientes Atendidos:[/] [bold cyan]{matchState.GetTotalClientsServed()}[/]\n" +
                          $"[bold white]?? Dinheiro Ganho:[/] [bold {scoreColor}]${matchState.TotalMoney}[/]\n" +
                          $"[bold white]?? Gorjetas:[/] [bold yellow]${matchState.TotalTips}[/]\n" +
                          $"[bold white]?? Pontuação Final:[/] [bold {scoreColor}]${finalScore}[/]\n\n" +
                          $"[bold {scoreColor}]{performance}[/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1),
                Header = new PanelHeader(" [bold gold1]?? RESULTADOS FINAIS[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(resultsPanel);
            
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para continuar...[/]");
            Console.ReadKey();
        }

        public void DisplayBossClientArrival(BossClient bossClient)
        {
            AnsiConsole.Clear();
            
            // Título dramático para o chefe
            var bossTitle = new FigletText("CLIENTE CHEFE")
                .Centered()
                .Color(Color.Red);
            
            AnsiConsole.Write(bossTitle);
            
            // Aviso especial
            var warningPanel = new Panel(
                new Markup("[bold red]?? ATENÇÃO! ??[/]\n\n" +
                          "[bold yellow]Um cliente misterioso apareceu![/]\n" +
                          "[dim]Este é o último cliente do dia e o mais importante...[/]\n" +
                          "[dim]Ele não revela diretamente o que deseja - preste atenção às pistas![/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader(" [bold red]?? CLIENTE ESPECIAL[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(warningPanel);
            AnsiConsole.WriteLine();
            
            // Nome e fala do chefe
            var bossHeader = new Rule($"[bold red]?? CLIENTE MISTERIOSO: {bossClient.Name.ToUpper()}[/]")
            {
                Style = Style.Parse("red"),
                Justification = Justify.Center
            };
            
            AnsiConsole.Write(bossHeader);
            AnsiConsole.WriteLine();
            
            var bossSpeech = new Panel(
                new Markup($"[italic bold white]{bossClient.GetOrderMessage()}[/]"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader(" [bold red]?? PISTA MISTERIOSA[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(bossSpeech);
            
            var hintPanel = new Panel(
                new Markup("[bold yellow]?? DICA:[/] [dim]Analise as palavras do cliente para descobrir\n" +
                          "que tipo de drink ele realmente deseja![/]"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow),
                Padding = new Padding(1, 0, 1, 0)
            };
            
            AnsiConsole.Write(hintPanel);
            AnsiConsole.WriteLine();
        }

        public void DisplayClientArrival(Client client)
        {
            // Se é um cliente chefe, usar o método específico
            if (client is BossClient bossClient)
            {
                DisplayBossClientArrival(bossClient);
                return;
            }
            
            AnsiConsole.Clear();
            
            // Header do cliente
            var clientHeader = new Rule($"[bold yellow]?? CLIENTE: {client.Name.ToUpper()}[/]")
            {
                Style = Style.Parse("yellow"),
                Justification = Justify.Center
            };
            
            AnsiConsole.Write(clientHeader);
            AnsiConsole.WriteLine();
            
            // Fala do cliente em um painel bonito
            var clientSpeech = new Panel(
                new Markup($"[italic]\"{client.GetOrderMessage()}\"[/]"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Blue),
                Header = new PanelHeader(" [bold blue]?? Pedido[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(clientSpeech);
            AnsiConsole.WriteLine();
        }

        public void DisplayAvailableIngredients(List<Ingredient> ingredients, IInventoryService inventoryService)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Green)
                .Title("[bold green]?? INGREDIENTES DISPONÍVEIS[/]");

            table.AddColumn(new TableColumn("[bold]#[/]").Centered());
            table.AddColumn(new TableColumn("[bold]Nome[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold]Tags[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold]Quantidade[/]").Centered());

            for (int i = 0; i < ingredients.Count; i++)
            {
                var ingredient = ingredients[i];
                var number = $"[bold cyan]{i + 1}[/]";
                var name = $"[bold white]{ingredient.Name}[/]";
                var tags = ingredient.Tags.Any() 
                    ? string.Join(", ", ingredient.Tags.Select(tag => $"[dim yellow]{tag}[/]"))
                    : "[dim gray]Nenhuma[/]";

                var quantity = inventoryService.GetIngredientStock(ingredient.Name);
                var quantityColor = quantity > 5 ? "green" : quantity > 2 ? "yellow" : "red";
                var quantityText = $"[{quantityColor}]{quantity}[/]";
                table.AddRow(number, name, tags, quantityText);
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }

        public List<Ingredient> GetSelectedIngredients(List<Ingredient> availableIngredients)
        {
            var selectedIngredients = new List<Ingredient>();
            
            var preparingPanel = new Panel(
                new Markup("[bold cyan]?? Digite os números dos ingredientes que deseja usar[/]\n" +
                          "[dim](separados por vírgula)[/]\n\n" +
                          "[yellow]Exemplo:[/] [bold]1,3,5[/] para usar ingredientes 1, 3 e 5"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Cyan1),
                Header = new PanelHeader(" [bold cyan1]PREPARANDO DRINK[/] ")
            };
            
            AnsiConsole.Write(preparingPanel);
            
            var input = AnsiConsole.Ask<string>("[bold cyan]Ingredientes selecionados:[/]");
            
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

        public void DisplayIngredientsTable(List<Ingredient> ingredients, IInventoryService? inventoryService)
        {
            var table = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Green)
                .Title("[bold green]?? INGREDIENTES DISPONÍVEIS[/]");

            table.AddColumn(new TableColumn("[bold]#[/]").Centered());
            table.AddColumn(new TableColumn("[bold]Nome[/]").LeftAligned());
            table.AddColumn(new TableColumn("[bold]Tags[/]").LeftAligned());
            
            if (inventoryService != null)
            {
                table.AddColumn(new TableColumn("[bold]Quantidade[/]").Centered());
            }

            for (int i = 0; i < ingredients.Count; i++)
            {
                var ingredient = ingredients[i];
                var number = $"[bold cyan]{i + 1}[/]";
                var name = $"[bold white]{ingredient.Name}[/]";
                var tags = ingredient.Tags.Any() 
                    ? string.Join(", ", ingredient.Tags.Select(tag => $"[dim yellow]{tag}[/]"))
                    : "[dim gray]Nenhuma[/]";

                if (inventoryService != null)
                {
                    var quantity = inventoryService.GetIngredientStock(ingredient.Name);
                    var quantityColor = quantity > 5 ? "green" : quantity > 2 ? "yellow" : "red";
                    var quantityText = $"[{quantityColor}]{quantity}[/]";
                    table.AddRow(number, name, tags, quantityText);
                }
                else
                {
                    table.AddRow(number, name, tags);
                }
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine();
        }

        public bool ConfirmServeDrink(Drink drink)
        {
            AnsiConsole.WriteLine();
            
            var drinkPanel = new Panel(
                new Markup($"[bold white]?? Nome:[/] [bold yellow]{drink.Name}[/]\n" +
                          $"[bold white]? Efeito:[/] [bold {GetEffectColor(drink.Effect)}]{drink.Effect}[/]\n" +
                          $"[bold white]?? Ingredientes:[/] [dim]{string.Join(", ", drink.Ingredients.Select(i => i.Name))}[/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1),
                Header = new PanelHeader(" [bold gold1]?? DRINK PREPARADO[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(drinkPanel);
            
            return AnsiConsole.Confirm("[bold cyan]Deseja servir este drink?[/]");
        }

        public void DisplayClientReaction(string reactionMessage)
        {
            AnsiConsole.WriteLine();
            
            var reactionPanel = new Panel(
                new Markup($"[italic]{reactionMessage}[/]"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Blue),
                Header = new PanelHeader(" [bold blue]?? REAÇÃO DO CLIENTE[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(reactionPanel);
            AnsiConsole.WriteLine();
        }

        public void DisplayPaymentResult(PaymentResult paymentResult)
        {
            AnsiConsole.WriteLine();
            
            Color borderColor;
            string statusIcon;
            string statusColor;
            
            switch (paymentResult.Status)
            {
                case PaymentStatus.Paid:
                    borderColor = Color.Green;
                    statusIcon = "?";
                    statusColor = "green";
                    break;
                case PaymentStatus.PartiallyPaid:
                    borderColor = Color.Yellow;
                    statusIcon = "??";
                    statusColor = "yellow";
                    break;
                case PaymentStatus.NotPaid:
                    borderColor = Color.Red;
                    statusIcon = "?";
                    statusColor = "red";
                    break;
                default:
                    borderColor = Color.Grey;
                    statusIcon = "?";
                    statusColor = "grey";
                    break;
            }

            var paymentContent = new Markup($"{statusIcon} [bold {statusColor}]{paymentResult.PaymentMessage}[/]\n\n");
            
            if (paymentResult.Status == PaymentStatus.Paid)
            {
                paymentContent = new Markup(
                    $"{statusIcon} [bold {statusColor}]{paymentResult.PaymentMessage}[/]\n\n" +
                    $"[bold green]?? Total recebido: ${paymentResult.TotalAmount}[/]\n" +
                    (paymentResult.TipAmount > 0 ? $"[bold yellow]?? Gorjeta: ${paymentResult.TipAmount}[/]" : ""));
            }
            else if (paymentResult.Status == PaymentStatus.PartiallyPaid)
            {
                paymentContent = new Markup(
                    $"{statusIcon} [bold {statusColor}]{paymentResult.PaymentMessage}[/]\n\n" +
                    $"[bold yellow]?? Valor parcial recebido: ${paymentResult.TotalAmount}[/]");
            }
            else
            {
                paymentContent = new Markup(
                    $"{statusIcon} [bold {statusColor}]{paymentResult.PaymentMessage}[/]\n\n" +
                    $"[bold red]?? Nenhum pagamento recebido![/]");
            }

            var paymentPanel = new Panel(paymentContent)
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(borderColor),
                Header = new PanelHeader(" [bold gold1]?? PAGAMENTO[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(paymentPanel);
            
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para continuar...[/]");
            Console.ReadKey();
        }

        public void DisplayGameScore(int score, int round)
        {
            AnsiConsole.WriteLine();
            
            var statusTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Purple)
                .Title("[bold purple]?? STATUS DO JOGO[/]");

            statusTable.AddColumn(new TableColumn("[bold]Informação[/]").LeftAligned());
            statusTable.AddColumn(new TableColumn("[bold]Valor[/]").RightAligned());

            statusTable.AddRow("[bold cyan]?? Rodada[/]", $"[bold white]{round}[/]");
            
            var moneyColor = score >= 100 ? "green" : score >= 50 ? "yellow" : "red";
            statusTable.AddRow("[bold gold1]?? Dinheiro Total[/]", $"[bold {moneyColor}]${score}[/]");

            AnsiConsole.Write(statusTable);
            AnsiConsole.WriteLine();
        }

        public bool AskToContinue()
        {
            return AnsiConsole.Confirm("[bold cyan]Deseja continuar jogando?[/]");
        }

        public void DisplayGameOver()
        {
            AnsiConsole.Clear();
            
            var gameOverTitle = new FigletText("GAME OVER")
                .Centered()
                .Color(Color.Red);
            
            AnsiConsole.Write(gameOverTitle);
            
            var thanksPanel = new Panel(
                new Markup("[bold yellow]?? Obrigado por jogar Bartender Game! ??[/]\n\n" +
                          "[dim]Esperamos que tenha se divertido preparando drinks\n" +
                          "e atendendo seus clientes virtuais![/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1),
                Header = new PanelHeader(" [bold gold1]?? ATÉ A PRÓXIMA[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(thanksPanel);
            
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para sair...[/]");
            Console.ReadKey();
        }

        // Shop methods
        public void DisplayShopOpening()
        {
            AnsiConsole.Clear();
            
            var shopTitle = new FigletText("LOJA")
                .Centered()
                .Color(Color.Gold1);
            
            AnsiConsole.Write(shopTitle);
            
            var congratsPanel = new Panel(
                new Markup("[bold green]?? Parabéns! Você serviu 3 clientes! ??[/]\n\n" +
                          "[bold yellow]?? A loja está aberta para compras![/]\n" +
                          "[dim]?? Seleção aleatória de até 3 ingredientes disponíveis hoje![/]\n" +
                          "[dim]Aproveite para expandir seu arsenal de ingredientes[/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1),
                Header = new PanelHeader(" [bold gold1]?? LOJA DE INGREDIENTES[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(congratsPanel);
            AnsiConsole.WriteLine();
        }

        public void DisplayShopItems(List<ShopItem> items, int playerMoney)
        {
            // Status do dinheiro
            var moneyColor = playerMoney >= 50 ? "green" : playerMoney >= 20 ? "yellow" : "red";
            var moneyPanel = new Panel(
                new Markup($"[bold {moneyColor}]?? Seu dinheiro: ${playerMoney}[/]"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Gold1),
                Padding = new Padding(1, 0, 1, 0)
            };
            
            AnsiConsole.Write(moneyPanel);
            AnsiConsole.WriteLine();

            // Tabela de itens
            var shopTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Gold1)
                .Title("[bold gold1]??? ITENS DISPONÍVEIS[/]");

            shopTable.AddColumn(new TableColumn("[bold]#[/]").Centered());
            shopTable.AddColumn(new TableColumn("[bold]Tipo[/]").Centered());
            shopTable.AddColumn(new TableColumn("[bold]Nome[/]").LeftAligned());
            shopTable.AddColumn(new TableColumn("[bold]Tags[/]").LeftAligned());
            shopTable.AddColumn(new TableColumn("[bold]Qtd[/]").Centered());
            shopTable.AddColumn(new TableColumn("[bold]Preço[/]").RightAligned());

            for (int i = 0; i < items.Count; i++)
            {
                var item = items[i];
                var number = $"[bold cyan]{i + 1}[/]";
                var typeIcon = item.Type == ShopItemType.NewIngredient ? "[bold green]??[/]" : "[bold blue]??[/]";
                
                // Adiciona "NEW!" para itens novos
                var itemName = item.Type == ShopItemType.NewIngredient 
                    ? $"[bold white]{item.Name}[/] [bold red on yellow] NEW! [/]"
                    : $"[bold white]{item.Name}[/]";
                
                var tags = item.Tags.Any() 
                    ? string.Join(", ", item.Tags.Select(tag => $"[dim yellow]{tag}[/]"))
                    : "[dim gray]-[/]";
                var quantity = $"[bold white]{item.Quantity}[/]";
                
                var canAfford = playerMoney >= item.Price;
                var priceColor = canAfford ? "green" : "red";
                var price = $"[{priceColor}]${item.Price}[/]";

                shopTable.AddRow(number, typeIcon, itemName, tags, quantity, price);
            }

            AnsiConsole.Write(shopTable);
            AnsiConsole.WriteLine();
            
            // Legenda
            var legend = new Panel(
                new Markup("[bold green]??[/] = Novo ingrediente (compra única) | [bold blue]??[/] = Reposição (compra ilimitada)\n" +
                          "[bold red on yellow] NEW! [/] = Ingrediente nunca comprado antes"))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Grey),
                Padding = new Padding(1, 0, 1, 0)
            };
            
            AnsiConsole.Write(legend);
        }

        public ShopItem? GetSelectedShopItem(List<ShopItem> items)
        {
            var choice = AnsiConsole.Ask<int>(
                "[bold cyan]Digite o número do item que deseja comprar[/] [dim](0 para sair):[/]");
            
            if (choice < 0 || choice > items.Count)
            {
                AnsiConsole.MarkupLine("[bold red]? Opção inválida![/]");
                return null;
            }
            
            if (choice == 0)
                return null;
                
            return items[choice - 1];
        }

        public void DisplayPurchaseResult(bool success, ShopItem item, int remainingMoney)
        {
            AnsiConsole.WriteLine();
            
            if (success)
            {
                var successPanel = new Panel(
                    new Markup($"[bold green]? Compra realizada com sucesso![/]\n" +
                              $"[bold white]?? {item.Name}[/] [dim](x{item.Quantity})[/] [bold green]adicionado ao inventário![/]\n" +
                              $"[bold gold1]?? Dinheiro restante: ${remainingMoney}[/]"))
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Green),
                    Header = new PanelHeader(" [bold green]?? COMPRA REALIZADA[/] "),
                    Padding = new Padding(2, 1, 2, 1)
                };
                
                AnsiConsole.Write(successPanel);
            }
            else
            {
                var errorPanel = new Panel(
                    new Markup($"[bold red]? Dinheiro insuficiente para comprar {item.Name}![/]\n" +
                              $"[dim]Preço: ${item.Price} | Seu dinheiro: ${remainingMoney}[/]"))
                {
                    Border = BoxBorder.Rounded,
                    BorderStyle = new Style(Color.Red),
                    Header = new PanelHeader(" [bold red]?? COMPRA REJEITADA[/] "),
                    Padding = new Padding(2, 1, 2, 1)
                };
                
                AnsiConsole.Write(errorPanel);
            }
            
            AnsiConsole.WriteLine();
        }

        public bool AskToContinueShopping()
        {
            return AnsiConsole.Confirm("[bold cyan]Deseja continuar comprando?[/]");
        }

        private static string GetEffectColor(string effect)
        {
            return effect.ToLower() switch
            {
                "reconfortante" => "yellow",
                "energizante" => "red",
                "relaxante" => "blue",
                "revigorante" => "green",
                "neutro" => "gray",
                "doce" => "magenta1",
                "estimulante" => "orange1",
                "forte" => "red",
                "suave" => "cyan1",
                "refrescante" => "aqua",
                "aromático" => "violet",
                "picante" => "orange_red1",
                "natural" => "green",
                "ácido" => "yellow",
                "mentolado" => "cyan1",
                _ => "white"
            };
        }
    }
}