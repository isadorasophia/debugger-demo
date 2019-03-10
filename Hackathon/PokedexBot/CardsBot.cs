// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using PokeAPI;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This bot will respond to the user's input with rich card content.
    /// Microsoft Bot Framework currently supports eight types of rich cards.
    /// We will demonstrate the use of each of these types in this project.
    /// Not all card types are supported on all channels.
    /// Please view the documentation in the ReadMe.md file in this project for more information.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service.  Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class CardsBot : IBot
    {
        private const string WelcomeText = "This bot will show you different information from the Pokémon universe.\n\nPlease enter a Pokémon name to start.";

        private CardsBotAccessors _accessors;

        private DialogSet _dialogs;

        /// <summary>
        /// Initializes a new instance of the <see cref="CardsBot"/> class.
        /// In the constructor for the bot we are instantiating our <see cref="DialogSet"/>, giving our field a value,
        /// and adding our <see cref="WaterfallDialog"/> and <see cref="ChoicePrompt"/> to the dialog set.
        /// </summary>
        /// <param name="accessors">A class containing <see cref="IStatePropertyAccessor{T}"/> used to manage state.</param>
        public CardsBot(CardsBotAccessors accessors)
        {
            this._accessors = accessors ?? throw new ArgumentNullException(nameof(accessors));
            this._dialogs = new DialogSet(accessors.ConversationDialogState);
            this._dialogs.Add(new WaterfallDialog("cardSelector", new WaterfallStep[] { ShowCardStepAsync }));
        }

        /// <summary>
        /// This controls what happens when an activity gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                var dialogContext = await this._dialogs.CreateContextAsync(turnContext, cancellationToken);
                var results = await dialogContext.ContinueDialogAsync(cancellationToken);

                if (results.Status == DialogTurnStatus.Empty)
                {
                    await dialogContext.BeginDialogAsync("cardSelector", cancellationToken: cancellationToken);
                }
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }

            // Save the dialog state into the conversation state.
            await _accessors.ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
        }

        /// <summary>
        /// Sends a welcome message to the user.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = turnContext.Activity.CreateReply();
                    reply.Text = $"Welcome to the Pokédex Bot, {member.Name}! {WelcomeText}";
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }

        /// <summary>
        /// This method uses the text of the activity to decide which type
        /// of card to respond with and reply with that card to the user.
        /// </summary>
        /// <param name="step">A <see cref="WaterfallStepContext"/> provides context for the current waterfall step.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="DialogTurnResult"/> indicating the turn has ended.</returns>
        /// <remarks>Related types <see cref="Attachment"/> and <see cref="AttachmentLayoutTypes"/>.</remarks>
        private static async Task<DialogTurnResult> ShowCardStepAsync(WaterfallStepContext step, CancellationToken cancellationToken)
        {
            // Get the pokemon name from the activity 
            var pokemonName = step.Context.Activity.Text.ToLowerInvariant().Split(' ')[0];

            // Reply to the activity we received with an activity.
            var reply = step.Context.Activity.CreateReply();

            // Cards are sent as Attachments in the Bot Framework.
            // So we need to create a list of attachments on the activity.
            reply.Attachments = new List<Attachment>();

            Pokemon pokemon = await DataFetcher.GetNamedApiObject<Pokemon>(pokemonName);
            PokemonSpecies pokemonSpecies = await DataFetcher.GetNamedApiObject<PokemonSpecies>(pokemonName);

            reply.Attachments.Add(GetThumbnailCard(pokemon, pokemonSpecies).ToAttachment());

            // Send the card(s) to the user as an attachment to the activity
            await step.Context.SendActivityAsync(reply, cancellationToken);

            // Give the user instructions about what to do next
            await step.Context.SendActivityAsync("Type another Pokémon name to see information about it!", cancellationToken: cancellationToken);

            return await step.EndDialogAsync(cancellationToken: cancellationToken);
        }

        // The following methods are all used to generate cards

        /// <summary>
        /// Creates a <see cref="ThumbnailCard"/>.
        /// </summary>
        /// <returns>A <see cref="ThumbnailCard"/> the user can view and/or interact with.</returns>
        /// <remarks>Related types <see cref="CardImage"/>, <see cref="CardAction"/>,
        /// and <see cref="ActionTypes"/>.</remarks>
        private static ThumbnailCard GetThumbnailCard(Pokemon pokemon, PokemonSpecies pokemonSpecies)
        {
            var heroCard = new ThumbnailCard
            {
                Title = UppercaseFirst(pokemon.Name),
                Subtitle = $"Type: {ParsePokemonTypes(pokemon.Types)}",
                Text = pokemonSpecies.FlavorTexts.Where(flavor => flavor.Language.Name == "en").Select(flavor => flavor.FlavorText).First(),
                Images = new List<CardImage> { new CardImage(pokemon.Sprites.FrontMale) },
                Buttons = new List<CardAction> { new CardAction(ActionTypes.OpenUrl, "Learn More", value: $"https://www.serebii.net/pokedex-sm/{pokemon.ID.ToString("D3")}.shtml") },
            };

            return heroCard;
        }

        private static string ParsePokemonTypes(PokemonTypeMap[] typeMap)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendJoin(", ", typeMap.Select((map, _) => UppercaseFirst(map.Type.Name)));

            return builder.ToString();
        }

        private static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

    }
}
