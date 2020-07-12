using AdaptiveCards;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Search
{
    public class DocumentCard
    {
        private static string HtmlToPlainText(string html)
        {
            const string tagWhiteSpace = @"(>|$)(\W|\n|\r)+<";//matches one or more (white space or line breaks) between '>' and '<'
            const string stripFormatting = @"<[^>]*(>|$)";//match any character between '<' and '>', even when end tag is missing
            const string lineBreak = @"<(br|BR)\s{0,1}\/{0,1}>";//matches: <br>,<br/>,<br />,<BR>,<BR/>,<BR />
            var lineBreakRegex = new Regex(lineBreak, RegexOptions.Multiline);
            var stripFormattingRegex = new Regex(stripFormatting, RegexOptions.Multiline);
            var tagWhiteSpaceRegex = new Regex(tagWhiteSpace, RegexOptions.Multiline);

            var text = html;
            //Decode html specific characters
            text = System.Net.WebUtility.HtmlDecode(text);
            //Remove tag whitespace/line breaks
            text = tagWhiteSpaceRegex.Replace(text, "><");
            //Replace <br /> with line breaks
            text = lineBreakRegex.Replace(text, Environment.NewLine);
            //Strip formatting
            text = stripFormattingRegex.Replace(text, string.Empty);

            return text;
        }
        private static string GetFileIcon(string filePath)
        {
            string fileExtension = Path.GetExtension(filePath);
            string documentIcon;

            if (fileExtension == ".doc" || fileExtension == ".docx")
            {
                documentIcon = "https://predictdata.blob.core.windows.net/dummy/Word_256x256.png";
            }
            else if (fileExtension == ".pdf")
            {
                documentIcon = "https://predictdata.blob.core.windows.net/dummy/pdf-icon.png";
            }
            else if (fileExtension == ".xls" || fileExtension == ".xlsx")
            {
                documentIcon = "https://predictdata.blob.core.windows.net/dummy/Excel_256x256.png";
            }
            else if (fileExtension == ".ppt" || fileExtension == ".pptx")
            {
                documentIcon = "https://predictdata.blob.core.windows.net/dummy/PowerPoint_256x256.png";
            }
            else
            {
                documentIcon = "https://predictdata.blob.core.windows.net/dummy/Default.JPG";
            }

            return documentIcon;
        }
        public static List<Attachment> GetDocumentCard(string dataSet)
        {
            var attachments = new List<Attachment>();
            List<DocumentDto> documentDtos = JsonConvert.DeserializeObject<List<DocumentDto>>(dataSet);
            foreach (DocumentDto info in documentDtos)
            {
                string summary = HtmlToPlainText(info.Summary);
                string documentIcon = GetFileIcon(info.DocumentPath);
                //Icon fileIcon=Icon.ExtractAssociatedIcon("<fullPath>");

                var card = new AdaptiveCard("1.2");
                List<AdaptiveElement> AdaptiveElements = new List<AdaptiveElement>
                {
                     new AdaptiveColumnSet()
                     {
                        Columns =new List<AdaptiveColumn>()
                        {
                            new AdaptiveColumn()
                                {
                                    Width="100px",
                                    Items = new List<AdaptiveElement>()
                                    {
                                        //new AdaptiveImage(@"data:image/png;base64,"+ info.PersonBase64Img)
                                        new AdaptiveImage(documentIcon)
                                        {
                                           Id="documentIcon",
                                           Size = AdaptiveImageSize.Medium,
                                           Style = AdaptiveImageStyle.Default,
                                        },
                                    }
                                },
                            new AdaptiveColumn()
                            {
                                    Width=AdaptiveColumnWidth.Stretch,
                                    Items = new List<AdaptiveElement>()
                                    {
                                         new AdaptiveTextBlock()
                                         {
                                            Id="title",
                                            Text = info.Title,
                                            Size = AdaptiveTextSize.Medium,
                                            Weight = AdaptiveTextWeight.Bolder,
                                            HorizontalAlignment =AdaptiveHorizontalAlignment.Left,
                                         },
                                         new AdaptiveTextBlock()
                                         {
                                           Id="author",
                                           Text ="✍ " +info.Author,
                                           Weight = AdaptiveTextWeight.Lighter,
                                           Size=AdaptiveTextSize.Small,
                                           Color=AdaptiveTextColor.Dark,
                                           Wrap=true,
                                         },
                                         new AdaptiveTextBlock()
                                         {
                                           Id="date",
                                           Text = "🗓 "+info.CreatedDateTime,
                                           Weight = AdaptiveTextWeight.Lighter,
                                           Color=AdaptiveTextColor.Dark,
                                           Size=AdaptiveTextSize.Small,
                                           Wrap=true,
                                         },
                                    }
                            }
                        }
                     },
                     new AdaptiveColumnSet()
                     {
                            Columns =new List<AdaptiveColumn>()
                            {
                                new AdaptiveColumn()
                                {
                                    //Width="150px"
                                    Items = new List<AdaptiveElement>()
                                    {
                                        new AdaptiveTextBlock()
                                        {
                                           Id="summary",
                                           Text = summary,
                                           Weight = AdaptiveTextWeight.Default,
                                           Size=AdaptiveTextSize.Small,
                                           Color=AdaptiveTextColor.Dark,
                                           Wrap=true,
                                        },

                                    }

                                }
                            }
                     },
                     new AdaptiveActionSet()
                     {
                        Actions = new List<AdaptiveAction>(){
                           new AdaptiveOpenUrlAction()
                           {
                              Id="open_url_action",
                              Title = "View",
                              UrlString = info.DocumentPath
                           }
                        }
                     }
                };
                
                card.Body = AdaptiveElements;
                Attachment attachment = new Attachment()
                {
                    ContentType = AdaptiveCard.ContentType,
                    Content = card
                };
                attachments.Add(attachment);

            }
            return attachments;
        }
    }
}
