using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System;

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Search
{
    public class MainDialogs : ComponentDialog
    {
        private readonly SearchLuisRecognizer _luisRecognizer;
        public QnAMaker SearchBotQnA { get; private set; }

        //string messageText = "What can I help you with today?";
        // Dependency injection uses this constructor to instantiate MainDialog
        public MainDialogs(SearchLuisRecognizer luisRecognizer, QnAMakerEndpoint endpoint)
            : base(nameof(MainDialogs))
        {
            _luisRecognizer = luisRecognizer;
            SearchBotQnA = new QnAMaker(endpoint);

            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                IntroStepAsync,
                ActStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

      
        private async Task<DialogTurnResult> IntroStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (!_luisRecognizer.IsConfigured)
            {
                //await stepContext.Context.SendActivityAsync( MessageFactory.Text("NOTE: LUIS is not configured. To enable all capabilities, add 'LuisAppId', 'LuisAPIKey' and 'LuisAPIHostName' to the appsettings.json file.", inputHint: InputHints.IgnoringInput), cancellationToken);

                return await stepContext.NextAsync(null, cancellationToken);
            }

            // Use the text provided in FinalStepAsync or the default if it is the first time.

            //var  promptMessage = MessageFactory.Text(messageText, messageText, InputHints.ExpectingInput);
            return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { }, cancellationToken);
        }

        private async Task<DialogTurnResult> ActStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Call LUIS and gather any potential  details. (Note the TurnContext has the response to the prompt.)
            var luisResult = await _luisRecognizer.RecognizeAsync<ContentSearchLuisRecognizer>(stepContext.Context, cancellationToken);
            APICaller aPICaller = new APICaller();
            switch (luisResult.TopIntent().intent)
            {
               

                case ContentSearchLuisRecognizer.Intent.DocumentSearch:
                    string documentName = luisResult.DocumentNameEntities != null ? luisResult.DocumentNameEntities : "";
                    string documents = aPICaller.GetDocument(documentName);
                    var docAttachments = DocumentCard.GetDocumentCard(documents);
                    await stepContext.Context.SendActivityAsync(MessageFactory.Carousel(docAttachments), cancellationToken);
                    break;

                default:

                    var results = await SearchBotQnA.GetAnswersAsync(stepContext.Context);
                    if (results.Length > 0)
                    {
                        var answer = results.First().Answer;
                        await stepContext.Context.SendActivityAsync(MessageFactory.Text(answer), cancellationToken);
                    }
                    else
                    {
                        string documentNames = aPICaller.GetDocument(stepContext.Context.Activity.Text);
                        if (!String.IsNullOrEmpty(documentNames) && documentNames != "[]")
                        {
                            var documentAttachments = DocumentCard.GetDocumentCard(documentNames);
                            await stepContext.Context.SendActivityAsync(MessageFactory.Carousel(documentAttachments), cancellationToken);
                        }
                        else
                        {
                            Activity reply = ((Activity)stepContext.Context.Activity).CreateReply();
                            reply.Text = $"😢 **Sorry!!! I found nothing** \n\n Please try to rephrase your query.";
                            await stepContext.Context.SendActivityAsync(reply);
                        }
                    }
                    break;
            }

            return await stepContext.NextAsync(null, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Restart the main dialog with a different message the second time around
            //messageText = null;// "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, null, cancellationToken);
        }
    }
}
