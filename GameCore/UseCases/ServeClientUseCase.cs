using System;
using System.Collections.Generic;
using Bartender.GameCore.Domain.Interfaces;
using Bartender.GameCore.Domain.Models;
using Bartender.GameCore.Events;
using Bartender.GameCore.Events.Events;

namespace Bartender.GameCore.UseCases
{
    public class ServeClientUseCase
    {
        private readonly EventBus _eventBus;

        public ServeClientUseCase(EventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public virtual ClientReaction Execute(Client client, Drink drink)
        {
            // Publica evento de drink servido
            _eventBus.Publish(new DrinkServedEvent(client, drink));

            // Avalia a reação do cliente baseada no efeito desejado vs efeito do drink
            var reaction = EvaluateClientReaction(client, drink);
            
            // Gera mensagem baseada na reação
            var message = GenerateReactionMessage(client, drink, reaction);

            // Publica evento de reação do cliente
            _eventBus.Publish(new ClientReactionEvent(client, drink, reaction, message));

            return reaction;
        }

        private ClientReaction EvaluateClientReaction(Client client, Drink drink)
        {
            if (client.DesiredEffect.Equals(drink.Effect, StringComparison.OrdinalIgnoreCase))
            {
                return ClientReaction.VeryHappy;
            }

            // Verifica compatibilidade de efeitos similares
            var compatibility = GetEffectCompatibility(client.DesiredEffect, drink.Effect);
            
            return compatibility switch
            {
                > 0.8 => ClientReaction.Happy,
                > 0.5 => ClientReaction.Neutral,
                > 0.2 => ClientReaction.Disappointed,
                _ => ClientReaction.Angry
            };
        }

        private double GetEffectCompatibility(string desiredEffect, string actualEffect)
        {
            // Mapeamento simples de compatibilidade entre efeitos
            var compatibilityMap = new Dictionary<string, Dictionary<string, double>>
            {
                ["Reconfortante"] = new() { ["Energizante"] = 0.6, ["Doce"] = 0.8, ["Relaxante"] = 0.9 },
                ["Energizante"] = new() { ["Reconfortante"] = 0.6, ["Estimulante"] = 0.9, ["Forte"] = 0.8 },
                ["Relaxante"] = new() { ["Reconfortante"] = 0.9, ["Doce"] = 0.7, ["Suave"] = 0.8 }
            };

            if (compatibilityMap.ContainsKey(desiredEffect) && 
                compatibilityMap[desiredEffect].ContainsKey(actualEffect))
            {
                return compatibilityMap[desiredEffect][actualEffect];
            }

            return 0.1; // Baixa compatibilidade por padrão
        }

        private string GenerateReactionMessage(Client client, Drink drink, ClientReaction reaction)
        {
            return reaction switch
            {
                ClientReaction.VeryHappy => $"{client.Name}: \"Perfeito! Exatamente o que eu queria! Este {drink.Name} está delicioso!\"",
                ClientReaction.Happy => $"{client.Name}: \"Muito bom! Não é exatamente o que pedi, mas gostei do {drink.Name}.\"",
                ClientReaction.Neutral => $"{client.Name}: \"Hmm, ok. O {drink.Name} está razoável, mas esperava algo diferente.\"",
                ClientReaction.Disappointed => $"{client.Name}: \"Não é bem isso que eu queria... mas obrigado pelo {drink.Name}.\"",
                ClientReaction.Angry => $"{client.Name}: \"Isso não é o que pedi! Esse {drink.Name} não tem nada a ver com o que eu queria!\"",
                _ => $"{client.Name}: \"...\""
            };
        }
    }
}
