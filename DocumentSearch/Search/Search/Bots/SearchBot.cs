using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Search
{
    public class SearchBot : DialogBot<MainDialogs>
    {
        public SearchBot(ConversationState conversationState, UserState userState, MainDialogs dialog)
            : base(conversationState, userState, dialog) { }
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded ?? Array.Empty<ChannelAccount>())
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    //if (turnContext.Activity.MembersAdded[0].Name == "USER_NAME")
                    //{
                    Activity reply = ((Activity)turnContext.Activity).CreateReply();
                    reply.Text = $" 😀 **Hi, I am Virtual Assistant!!** \n\n I am here to assist you.";
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    //}
                }
            }
            await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
        }
        protected override async Task OnTeamsMembersAddedAsync(IList<TeamsChannelAccount> membersAdded, TeamInfo teamInfo, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var teamMember in membersAdded)
            {
                Activity reply = ((Activity)turnContext.Activity).CreateReply();
                reply.Text = $" 😀 **Hi, I am Virtual Assistant!!** \n\n I am here to assist you.";
                await turnContext.SendActivityAsync(reply, cancellationToken);
            }
        }
    }
}
