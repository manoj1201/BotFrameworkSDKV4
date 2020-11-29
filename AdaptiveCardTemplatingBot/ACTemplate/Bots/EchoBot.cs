// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.10.3

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using AdaptiveCards.Templating;

namespace ACTemplate.Bots
{
    public class EchoBot : ActivityHandler
    {
        private const string WelcomeText = @"This bot will introduce you to AdaptiveCards.
                                            Type anything to see an AdaptiveCard.";

        private readonly string _cards = Path.Combine(".", "Resources", "Card.json");
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendWelcomeMessageAsync(turnContext, cancellationToken);
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var adaptiveCardJson = (File.ReadAllText(_cards));
            AdaptiveCardTemplate template = new AdaptiveCardTemplate(adaptiveCardJson);
            var myData = new
            {
                id = "6",
                name = "Saturn",
                summary = "Saturn is the sixth planet from the Sun and the second-largest in the Solar System, after Jupiter. It is a gas giant with an average radius about nine times that of Earth. It has only one-eighth the average density of Earth; however, with its larger volume, Saturn is over 95 times more massive. Saturn is named after the Roman god of wealth and agriculture; its astronomical symbol (♄) represents the god's sickle.",
                solarOrbitYears = "29.46",
                solarOrbitAvgDistanceKm = "1433525000",
                numSatellites = "82",
                wikiLink = "https://en.wikipedia.org/wiki/Saturn",
                imageLink = "https://upload.wikimedia.org/wikipedia/commons/c/c7/Saturn_during_Equinox.jpg",
                imageAlt = "NASA / JPL / Space Science Institute [Public domain]"
            };
            string cardJson = template.Expand(myData);
            var cardAttachment = CreateAdaptiveCardAttachment(cardJson);
            await turnContext.SendActivityAsync(MessageFactory.Attachment(cardAttachment), cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text("Please enter any text to see another card."), cancellationToken);
        }

        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to Adaptive Cards Bot {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                }
            }
        }

        private static Attachment CreateAdaptiveCardAttachment(string _card)
        {
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(_card),
            };
            return adaptiveCardAttachment;
        }
    }
}
