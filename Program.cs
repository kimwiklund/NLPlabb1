using Azure;
using Azure.AI.TextAnalytics;
using Microsoft.Extensions.Configuration;
using System;
using System.Text;


namespace text_analysis
{
    class Program
    {
        static void Main(string[] args)
        {

            try
            {
                // Get config settings from AppSettings
                IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
                IConfigurationRoot configuration = builder.Build();
                string cogSvcEndpoint = configuration["CognitiveServicesEndpoint"];
                string cogSvcKey = configuration["CognitiveServiceKey"];

                // Set console encoding to unicode Varför? https://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&id=utconvertq1 
                Console.InputEncoding = Encoding.Unicode;
                Console.OutputEncoding = Encoding.Unicode;

                // Create client using endpoint and key
                AzureKeyCredential credentials = new AzureKeyCredential(cogSvcKey);
                Uri endpoint = new Uri(cogSvcEndpoint);
                TextAnalyticsClient CogClient = new TextAnalyticsClient(endpoint, credentials);


                //// Analyze each text file in the reviews folder
                //var folderPath = Path.GetFullPath("./reviews");
                //DirectoryInfo folder = new DirectoryInfo(folderPath);
                //foreach (var file in folder.GetFiles("*.txt"))

                //// Read the file contents
                //Console.WriteLine("\n-------------\n" + file.Name);
                //StreamReader sr = file.OpenText();
                //var text = sr.ReadToEnd();
                //sr.Close();
                //Console.WriteLine("\n" + text);


                Console.WriteLine("Type in something and I wil try to analyze your intentions and some keys");

                // Get language
                var analyzeText = Console.ReadLine();
                DetectedLanguage detectedLanguage = CogClient.DetectLanguage(analyzeText);
                Console.WriteLine($"\nLanguage: {detectedLanguage.Name}");

                // Get sentiment
                DocumentSentiment sentimentAnalysis = CogClient.AnalyzeSentiment(analyzeText);
                Console.WriteLine($"\nSentiment: {sentimentAnalysis.Sentiment}");


                // Get key phrases
                KeyPhraseCollection phrases = CogClient.ExtractKeyPhrases(analyzeText);
                if (phrases.Count > 0)
                {
                    Console.WriteLine("\nKey Phrases:");
                    foreach (string phrase in phrases)
                    {
                        Console.WriteLine($"\t{phrase}");
                    }
                }



                // Get entities
                CategorizedEntityCollection entities = CogClient.RecognizeEntities(analyzeText);
                if (entities.Count > 0)
                {
                    Console.WriteLine("\nEntities:");
                    foreach (CategorizedEntity entity in entities)
                    {
                        Console.WriteLine($"\t{entity.Text} ({entity.Category})");
                    }
                }


                // Get linked entities
                LinkedEntityCollection linkedEntities = CogClient.RecognizeLinkedEntities(analyzeText);
                if (linkedEntities.Count > 0)
                {
                    Console.WriteLine("\nLinks:");
                    foreach (LinkedEntity linkedEntity in linkedEntities)
                    {
                        Console.WriteLine($"\t{linkedEntity.Name} ({linkedEntity.Url})");
                    }
                }

                Console.WriteLine("Nicely Done.");


            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }



    }
}