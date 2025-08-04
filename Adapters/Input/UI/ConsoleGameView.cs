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
            
            // T�tulo estilizado
            var title = new FigletText("BARTENDER GAME")
                .Centered()
                .Color(Color.Cyan1);
            
            AnsiConsole.Write(title);
            
            // Panel de boas-vindas
            var welcomePanel = new Panel(
                new Markup("[bold yellow]?? Prepare drinks para satisfazer seus clientes! ??[/]\n\n" +
                          "[dim]� Misture ingredientes para criar drinks �nicos[/]\n" +
                          "[dim]� Satisfa�a as prefer�ncias dos clientes[/]\n" +
                          "[dim]� Ganhe dinheiro e expanda seu bar[/]\n" +
                          "[dim]� Visite a loja a cada 3 clientes![/]\n" +
                          "[dim]� Trabalhe 3 dias em cada local (10 rodadas + 1 boss por dia)[/]\n" +
                          "[dim]� Satisfa�a 2 dos 3 bosses para desbloquear novos locais![/]\n" +
                          "[dim]� Hor�rio de trabalho: 18:00 - 02:00[/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Gold1),
                Header = new PanelHeader(" [bold gold1]?? BEM-VINDO[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(welcomePanel);
            
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para come�ar sua jornada...[/]");
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
                new Markup("[bold yellow]?? Escolha onde voc� quer trabalhar como bartender! ??[/]\n\n" +
                          "[dim]Cada local tem suas pr�prias caracter�sticas:[/]\n" +
                          "[dim]� B�nus e penalidades �nicos[/]\n" +
                          "[dim]� 3 dias de trabalho por local[/]\n" +
                          "[dim]� 10 rodadas + 1 boss por dia[/]\n" +
                          "[dim]� Satisfa�a 2/3 bosses para desbloquear pr�ximo local[/]"))
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
                .Title("[bold gold1]?? LOCAIS DISPON�VEIS[/]");

            gameModeTable.AddColumn(new TableColumn("[bold]#[/]").Centered());
            gameModeTable.AddColumn(new TableColumn("[bold]Status[/]").Centered());
            gameModeTable.AddColumn(new TableColumn("[bold]Local[/]").LeftAligned());
            gameModeTable.AddColumn(new TableColumn("[bold]Descri��o[/]").LeftAligned());
            gameModeTable.AddColumn(new TableColumn("[bold]Modificadores[/]").LeftAligned());

            for (int i = 0; i < gameModes.Count; i++)
            {
                var gameMode = gameModes[i];
                var number = $"[bold cyan]{i + 1}[/]";
                var status = gameMode.IsUnlocked ? "[bold green]? Dispon�vel[/]" : "[bold red]?? Bloqueado[/]";
                var name = gameMode.IsUnlocked ? $"[bold white]{gameMode.Name}[/]" : $"[dim]{gameMode.Name}[/]";
                var description = gameMode.IsUnlocked ? $"[dim]{gameMode.Description}[/]" : "[dim]???[/]";
                
                var modifiers = "";
                if (gameMode.IsUnlocked && gameMode.Modifiers.Any())
                {
                    modifiers = string.Join("\n", gameMode.Modifiers.Select(m => $"[dim yellow]� {m.Description}[/]"));
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
                AnsiConsole.MarkupLine("[bold red]? Nenhum local dispon�vel![/]");
                AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para continuar...[/]");
                Console.ReadKey();
                return null;
            }

            var choice = AnsiConsole.Ask<int>(
                $"[bold cyan]Digite o n�mero do local onde deseja trabalhar[/] [dim](1-{gameModes.Count}, 0 para sair):[/]");
            
            if (choice < 1 || choice > gameModes.Count)
                return null;
                
            var selectedGameMode = gameModes[choice - 1];
            
            if (!selectedGameMode.IsUnlocked)
            {
                AnsiConsole.MarkupLine("[bold red]? Este local ainda est� bloqueado![/]");
                AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para continuar...[/]");
                Console.ReadKey();
                return null;
            }
            
            // Confirma��o
            var confirmPanel = new Panel(
                new Markup($"[bold white]Voc� escolheu:[/] [bold yellow]{selectedGameMode.Name}[/]\n" +
                          $"[dim]{selectedGameMode.Description}[/]\n\n" +
                          $"[bold white]Modificadores ativos:[/]\n" +
                          string.Join("\n", selectedGameMode.Modifiers.Select(m => $"[dim yellow]� {m.Description}[/]"))))
            {
                Border = BoxBorder.Rounded,
                BorderStyle = new Style(Color.Yellow),
                Header = new PanelHeader(" [bold yellow]?? CONFIRMA��O[/] "),
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
            AnsiConsole.Clear();
            
            // Header compacto com informa��es principais
            var headerTable = new Table()
                .Border(TableBorder.None)
                .HideHeaders();

            headerTable.AddColumn(new TableColumn("Col1"));
            headerTable.AddColumn(new TableColumn("Col2"));
            headerTable.AddColumn(new TableColumn("Col3"));
            headerTable.AddColumn(new TableColumn("Col4"));

            // Primeira linha: Estabelecimento e Dia
            var establishmentInfo = $"[bold cyan]?? {matchState.GameMode.Name}[/]";
            var dayInfo = $"[bold yellow]?? Dia {matchState.CurrentDay}/{matchState.GameMode.DaysCount}[/]";
            
            // Segunda linha: Hor�rio e Rodada
            var timeInfo = $"[bold green]?? {matchState.CurrentTime}[/]";
            var roundInfo = matchState.IsBossRound 
                ? "[bold red]?? BOSS CLIENT[/]" 
                : $"[bold white]?? Rodada {matchState.CurrentRoundInDay}/{matchState.GameMode.RoundsPerDay}[/]";

            headerTable.AddRow(establishmentInfo, dayInfo, timeInfo, roundInfo);

            AnsiConsole.Write(headerTable);
            
            // Linha separadora
            var separator = new Rule()
            {
                Style = Style.Parse("grey"),
                Justification = Justify.Center
            };
            AnsiConsole.Write(separator);
            AnsiConsole.WriteLine();
            
            // Informa��es de dinheiro (mais compacto)
            var moneyColor = matchState.TotalMoney >= 200 ? "green" : matchState.TotalMoney >= 100 ? "yellow" : "red";
            var moneyInfo = $"[bold gold1]?? ${matchState.TotalMoney}[/]   [bold green]?? ${matchState.TotalTips}[/]";
            
            AnsiConsole.MarkupLine(moneyInfo);
            AnsiConsole.WriteLine();
            
            // Pr�ximas lojas (s� para rodadas normais)
            if (!matchState.IsBossRound)
            {
                var nextShopRounds = new List<int>();
                for (int i = matchState.CurrentRoundInDay + 1; i <= matchState.GameMode.RoundsPerDay; i++)
                {
                    if (i % 3 == 0)
                        nextShopRounds.Add(i);
                }
                
                if (nextShopRounds.Any())
                {
                    var shopInfo = $"[dim]?? Pr�ximas lojas: Rodadas {string.Join(", ", nextShopRounds)}[/]";
                    AnsiConsole.MarkupLine(shopInfo);
                    AnsiConsole.WriteLine();
                }
            }
            
            // Status dos bosses (se houver algum)
            if (matchState.BossSatisfactionByDay.Any())
            {
                var bossStatus = string.Join(" ", matchState.BossSatisfactionByDay.Select((satisfied, index) => 
                    satisfied ? $"[green]D{index + 1}:?[/]" : $"[red]D{index + 1}:?[/]"));
                AnsiConsole.MarkupLine($"[dim]?? Bosses: {bossStatus}[/]");
                AnsiConsole.WriteLine();
            }
        }

        public void DisplayMatchCompleted(MatchState matchState)
        {
            AnsiConsole.Clear();
            
            var completedTitle = new FigletText("MATCH COMPLETED")
                .Centered()
                .Color(Color.Green);
            
            AnsiConsole.Write(completedTitle);
            
            // Calcular estat�sticas
            int satisfiedBosses = matchState.BossSatisfactionByDay.Count(satisfied => satisfied);
            bool canUnlock = matchState.CanUnlockNextLocation();
            
            var summaryTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Green)
                .Title($"[bold green]?? RESUMO - {matchState.GameMode.Name}[/]");

            summaryTable.AddColumn(new TableColumn("[bold]Informa��o[/]").LeftAligned());
            summaryTable.AddColumn(new TableColumn("[bold]Resultado[/]").RightAligned());

            summaryTable.AddRow("[bold yellow]?? Local[/]", $"[bold white]{matchState.GameMode.Name}[/]");
            summaryTable.AddRow("[bold yellow]?? Dias Trabalhados[/]", $"[bold white]{matchState.GameMode.DaysCount}[/]");
            summaryTable.AddRow("[bold gold1]?? Dinheiro Total[/]", $"[bold green]${matchState.TotalMoney}[/]");
            summaryTable.AddRow("[bold gold1]?? Gorjetas[/]", $"[bold green]${matchState.TotalTips}[/]");
            
            // Status dos bosses
            var bossStatus = string.Join(" ", matchState.BossSatisfactionByDay.Select((satisfied, index) => 
                satisfied ? $"[green]Dia {index + 1}: ?[/]" : $"[red]Dia {index + 1}: ?[/]"));
            summaryTable.AddRow("[bold purple]?? Bosses Satisfeitos[/]", $"[bold white]{satisfiedBosses}/{matchState.GameMode.DaysCount}[/]");
            summaryTable.AddRow("[bold purple]?? Status Detalhado[/]", bossStatus);
            
            // Status de desbloqueio
            if (canUnlock)
            {
                summaryTable.AddRow("[bold green]?? Pr�ximo Local[/]", "[bold green]DESBLOQUEADO! ?[/]");
            }
            else
            {
                summaryTable.AddRow("[bold red]?? Pr�ximo Local[/]", "[bold red]Bloqueado - Precisa satisfazer 2/3 bosses[/]");
            }

            AnsiConsole.Write(summaryTable);
            AnsiConsole.WriteLine();
            
            // Mensagem final
            if (canUnlock)
            {
                var successPanel = new Panel(
                    new Markup("[bold green]?? PARAB�NS! ??[/]\n\n" +
                              "[bold yellow]Voc� impressionou os clientes VIP![/]\n" +
                              "[dim]Um novo local de trabalho foi desbloqueado![/]"))
                {
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(Color.Green),
                    Header = new PanelHeader(" [bold green]?? SUCESSO[/] "),
                    Padding = new Padding(2, 1, 2, 1)
                };
                
                AnsiConsole.Write(successPanel);
            }
            else
            {
                var failPanel = new Panel(
                    new Markup("[bold yellow]?? CONTINUE TENTANDO! ??[/]\n\n" +
                              "[bold red]Voc� precisa satisfazer pelo menos 2 dos 3 bosses[/]\n" +
                              "[dim]para desbloquear o pr�ximo local.[/]\n\n" +
                              "[dim]Tente novamente neste local ou escolha outro![/]"))
                {
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(Color.Yellow),
                    Header = new PanelHeader(" [bold yellow]?? QUASE L�[/] "),
                    Padding = new Padding(2, 1, 2, 1)
                };
                
                AnsiConsole.Write(failPanel);
            }
            
            AnsiConsole.WriteLine();
            AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para continuar...[/]");
            Console.ReadKey();
        }

        public void DisplayBossClientArrival(BossClient bossClient)
        {
            AnsiConsole.Clear();
            
            var bossTitle = new FigletText("BOSS CLIENT")
                .Centered()
                .Color(Color.Red);
            
            AnsiConsole.Write(bossTitle);
            
            var bossPanel = new Panel(
                new Markup($"[bold red]?? CLIENTE ESPECIAL CHEGOU! ??[/]\n\n" +
                          $"[bold yellow]Nome:[/] [bold white]{bossClient.Name}[/]\n\n" +
                          $"[bold yellow]Pistas:[/]\n" +
                          $"[dim]?? \"{string.Join("\"", bossClient.Clues.Take(1))}\"[/]\n\n" +
                          $"[bold red]??  Este � um cliente muito especial![/]\n" +
                          $"[dim]Se ele ficar satisfeito, voc� ganhar� um b�nus amanh�![/]"))
            {
                Border = BoxBorder.Double,
                BorderStyle = new Style(Color.Red),
                Header = new PanelHeader(" [bold red]?? CLIENTE VIP[/] "),
                Padding = new Padding(2, 1, 2, 1)
            };
            
            AnsiConsole.Write(bossPanel);
            AnsiConsole.WriteLine();
            
            AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para continuar...[/]");
            Console.ReadKey();
        }

        public void DisplayClientArrival(Client client)
        {
            // Se � um cliente chefe, usar o m�todo espec�fico
            if (client is BossClient bossClient)
            {
                DisplayBossClientArrival(bossClient);
                return;
            }
            
            // N�O limpar a tela para preservar as informa��es de progresso
            // AnsiConsole.Clear(); -- REMOVIDO
            
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
                Header = new PanelHeader(" [bold blue]??? Pedido[/] "),
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
                .Title("[bold green]?? INGREDIENTES DISPON�VEIS[/]");

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
                new Markup("[bold cyan]?? Digite os n�meros dos ingredientes que deseja usar[/]\n" +
                          "[dim](separados por v�rgula)[/]\n\n" +
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
                .Title("[bold green]?? INGREDIENTES DISPON�VEIS[/]");

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
                Header = new PanelHeader(" [bold blue]?? REA��O DO CLIENTE[/] "),
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

            statusTable.AddColumn(new TableColumn("[bold]Informa��o[/]").LeftAligned());
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
                Header = new PanelHeader(" [bold gold1]?? AT� A PR�XIMA[/] "),
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
                new Markup("[bold green]?? Parab�ns! Voc� serviu 3 clientes! ??[/]\n\n" +
                          "[bold yellow]?? A loja est� aberta para compras![/]\n" +
                          "[dim]?? Sele��o aleat�ria de at� 3 ingredientes dispon�veis hoje![/]\n" +
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
                .Title("[bold gold1]??? ITENS DISPON�VEIS[/]");

            shopTable.AddColumn(new TableColumn("[bold]#[/]").Centered());
            shopTable.AddColumn(new TableColumn("[bold]Tipo[/]").Centered());
            shopTable.AddColumn(new TableColumn("[bold]Nome[/]").LeftAligned());
            shopTable.AddColumn(new TableColumn("[bold]Tags[/]").LeftAligned());
            shopTable.AddColumn(new TableColumn("[bold]Qtd[/]").Centered());
            shopTable.AddColumn(new TableColumn("[bold]Pre�o[/]").RightAligned());

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
                new Markup("[bold green]??[/] = Novo ingrediente (compra �nica) | [bold blue]??[/] = Reposi��o (compra ilimitada)\n" +
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
                "[bold cyan]Digite o n�mero do item que deseja comprar[/] [dim](0 para sair):[/]");
            
            if (choice < 0 || choice > items.Count)
            {
                AnsiConsole.MarkupLine("[bold red]? Op��o inv�lida![/]");
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
                              $"[bold white]?? {item.Name}[/] [dim](x{item.Quantity})[/] [bold green]adicionado ao invent�rio![/]\n" +
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
                              $"[dim]Pre�o: ${item.Price} | Seu dinheiro: ${remainingMoney}[/]"))
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
                "arom�tico" => "violet",
                "picante" => "orange_red1",
                "natural" => "green",
                "�cido" => "yellow",
                "mentolado" => "cyan1",
                _ => "white"
            };
        }

        public void DisplayDayCompleted(MatchState matchState, bool bossWasSatisfied)
        {
            AnsiConsole.Clear();
            
            var dayCompletedTitle = new FigletText("FIM DO DIA")
                .Centered()
                .Color(Color.Gold1);
            
            AnsiConsole.Write(dayCompletedTitle);
            
            // Resumo do dia
            var dayResultsTable = new Table()
                .Border(TableBorder.Rounded)
                .BorderColor(Color.Cyan1)
                .Title($"[bold cyan1]?? RESUMO DO DIA {matchState.CurrentDay} - {matchState.GameMode.Name}[/]");

            dayResultsTable.AddColumn(new TableColumn("[bold]Informa��o[/]").LeftAligned());
            dayResultsTable.AddColumn(new TableColumn("[bold]Resultado[/]").RightAligned());

            // Informa��es do dia
            dayResultsTable.AddRow("[bold yellow]?? Local[/]", $"[bold white]{matchState.GameMode.Name}[/]");
            dayResultsTable.AddRow("[bold yellow]?? Dia Conclu�do[/]", $"[bold white]{matchState.CurrentDay}/{matchState.GameMode.DaysCount}[/]");
            dayResultsTable.AddRow("[bold yellow]?? Hor�rio Final[/]", $"[bold white]02:00 (Fechamento)[/]");
            dayResultsTable.AddRow("[bold yellow]?? Rodadas Completas[/]", $"[bold white]10 + 1 Boss[/]");
            
            // Status do Boss
            if (bossWasSatisfied)
            {
                dayResultsTable.AddRow("[bold green]?? Cliente VIP[/]", "[bold green]SATISFEITO ?[/]");
                
                // Mostrar b�nus se n�o for o �ltimo dia
                if (matchState.CurrentDay < matchState.GameMode.DaysCount)
                {
                    dayResultsTable.AddRow("[bold gold1]?? B�nus Amanh�[/]", "[bold yellow]Sim! +5% nos pagamentos[/]");
                }
            }
            else
            {
                dayResultsTable.AddRow("[bold red]?? Cliente VIP[/]", "[bold red]INSATISFEITO ?[/]");
                dayResultsTable.AddRow("[bold gray]?? B�nus Amanh�[/]", "[bold gray]Nenhum[/]");
            }
            
            // Dinheiro atual
            var moneyColor = matchState.TotalMoney >= 200 ? "green" : matchState.TotalMoney >= 100 ? "yellow" : "red";
            dayResultsTable.AddRow("[bold gold1]?? Dinheiro Total[/]", $"[bold {moneyColor}]${matchState.TotalMoney}[/]");
            dayResultsTable.AddRow("[bold gold1]?? Gorjetas[/]", $"[bold green]${matchState.TotalTips}[/]");

            AnsiConsole.Write(dayResultsTable);
            AnsiConsole.WriteLine();
            
            // Progresso dos bosses at� agora
            if (matchState.BossSatisfactionByDay.Any())
            {
                var bossProgressTable = new Table()
                    .Border(TableBorder.Rounded)
                    .BorderColor(Color.Purple)
                    .Title("[bold purple]?? PROGRESSO DOS BOSSES[/]");

                bossProgressTable.AddColumn(new TableColumn("[bold]Dia[/]").Centered());
                bossProgressTable.AddColumn(new TableColumn("[bold]Status[/]").Centered());

                for (int i = 0; i < matchState.BossSatisfactionByDay.Count; i++)
                {
                    var day = i + 1;
                    var satisfied = matchState.BossSatisfactionByDay[i];
                    var status = satisfied ? "[bold green]? Satisfeito[/]" : "[bold red]? Insatisfeito[/]";
                    bossProgressTable.AddRow($"[bold white]Dia {day}[/]", status);
                }

                AnsiConsole.Write(bossProgressTable);
                AnsiConsole.WriteLine();
            }
            
            // Mensagem final baseada no status
            if (matchState.CurrentDay < matchState.GameMode.DaysCount)
            {
                // Ainda h� dias restantes
                var nextDayPanel = new Panel(
                    new Markup($"[bold yellow]?? PR�XIMO DIA[/]\n\n" +
                              $"[bold white]Dia {matchState.CurrentDay + 1} de {matchState.GameMode.DaysCount}[/]\n" +
                              $"[dim]Hor�rio: 18:00 - 02:00[/]\n" +
                              $"[dim]10 rodadas + 1 cliente VIP[/]\n\n" +
                              (bossWasSatisfied ? "[bold green]Com b�nus de satisfa��o do cliente VIP![/]" : "")))
                {
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(Color.Gold1),
                    Header = new PanelHeader(" [bold gold1]??? CONTINUAR TRABALHANDO[/] "),
                    Padding = new Padding(2, 1, 2, 1)
                };
                
                AnsiConsole.Write(nextDayPanel);
                AnsiConsole.WriteLine();
                AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para ir para o pr�ximo dia...[/]");
            }
            else
            {
                // �ltimo dia - trabalho no local terminou
                var completedPanel = new Panel(
                    new Markup($"[bold gold1]?? TRABALHO CONCLU�DO![/]\n\n" +
                              $"[bold white]Voc� completou os 3 dias em {matchState.GameMode.Name}[/]\n" +
                              $"[dim]Agora voc� pode escolher um novo local de trabalho[/]\n\n" +
                              $"[bold purple]Status final: {matchState.BossSatisfactionByDay.Count(s => s)}/3 bosses satisfeitos[/]"))
                {
                    Border = BoxBorder.Double,
                    BorderStyle = new Style(Color.Green),
                    Header = new PanelHeader(" [bold green]? LOCAL FINALIZADO[/] "),
                    Padding = new Padding(2, 1, 2, 1)
                };
                
                AnsiConsole.Write(completedPanel);
                AnsiConsole.WriteLine();
                AnsiConsole.Markup("[bold cyan]Pressione qualquer tecla para ver o resumo final...[/]");
            }
            
            Console.ReadKey();
        }
    }
}