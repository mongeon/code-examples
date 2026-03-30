using Microsoft.ML;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System;
using System.Threading.Tasks;

namespace BlazorMLNET.Models
{
    public class MLService
    {
        private PredictionEngine<SentimentData, SentimentPrediction>? _predictionEngine;
        private readonly string _modelPath;

        public MLService(IWebHostEnvironment env)
        {
            _modelPath = Path.Combine(env.WebRootPath, "sentiment_model.zip");
        }

        public async Task TrainAndSaveModelAsync()
        {
            await Task.Run(() =>
            {
                var mlContext = new MLContext();

                var sampleData = new[]
                {
                    // Positif
                    new SentimentData { SentimentText = "J'adore ML.NET !", Sentiment = true },
                    new SentimentData { SentimentText = "C'est un excellent produit.", Sentiment = true },
                    new SentimentData { SentimentText = "Je le recommanderais à mes amis.", Sentiment = true },
                    new SentimentData { SentimentText = "La documentation est claire et utile.", Sentiment = true },
                    new SentimentData { SentimentText = "L'API est facile à utiliser.", Sentiment = true },
                    new SentimentData { SentimentText = "Le service est rapide et efficace.", Sentiment = true },
                    new SentimentData { SentimentText = "Je suis très satisfait de mon achat.", Sentiment = true },
                    new SentimentData { SentimentText = "L'équipe de support est formidable.", Sentiment = true },
                    new SentimentData { SentimentText = "La qualité est au rendez-vous, bravo !", Sentiment = true },
                    new SentimentData { SentimentText = "Une expérience utilisateur vraiment agréable.", Sentiment = true },
                    new SentimentData { SentimentText = "Tout fonctionne parfaitement, merci.", Sentiment = true },
                    new SentimentData { SentimentText = "L'interface est intuitive et moderne.", Sentiment = true },
                    new SentimentData { SentimentText = "Les mises à jour sont régulières et pertinentes.", Sentiment = true },
                    new SentimentData { SentimentText = "Rapport qualité-prix imbattable.", Sentiment = true },
                    new SentimentData { SentimentText = "J'utilise ce produit tous les jours avec plaisir.", Sentiment = true },
                    new SentimentData { SentimentText = "Le déploiement a été simple et sans problème.", Sentiment = true },
                    new SentimentData { SentimentText = "Excellente performance, rien à redire.", Sentiment = true },
                    new SentimentData { SentimentText = "Les fonctionnalités répondent à tous mes besoins.", Sentiment = true },
                    new SentimentData { SentimentText = "C'est exactement ce que je cherchais.", Sentiment = true },
                    new SentimentData { SentimentText = "La prise en main est très rapide.", Sentiment = true },
                    new SentimentData { SentimentText = "Un outil indispensable pour notre équipe.", Sentiment = true },
                    new SentimentData { SentimentText = "Le résultat dépasse mes attentes.", Sentiment = true },
                    new SentimentData { SentimentText = "Je suis impressionné par la stabilité du logiciel.", Sentiment = true },
                    new SentimentData { SentimentText = "Livraison rapide et produit conforme.", Sentiment = true },
                    new SentimentData { SentimentText = "Super application, je la recommande vivement.", Sentiment = true },
                    new SentimentData { SentimentText = "L'intégration avec nos outils existants est parfaite.", Sentiment = true },
                    new SentimentData { SentimentText = "Très bonne réactivité du service client.", Sentiment = true },
                    new SentimentData { SentimentText = "Le tutoriel m'a permis de démarrer en quelques minutes.", Sentiment = true },
                    new SentimentData { SentimentText = "Produit fiable et bien conçu.", Sentiment = true },
                    new SentimentData { SentimentText = "Je n'ai rencontré aucun bug depuis le début.", Sentiment = true },

                    // Positive (English)
                    new SentimentData { SentimentText = "I love this product, it's amazing!", Sentiment = true },
                    new SentimentData { SentimentText = "Fantastic experience, highly recommended.", Sentiment = true },
                    new SentimentData { SentimentText = "The best tool I've ever used.", Sentiment = true },
                    new SentimentData { SentimentText = "Everything works flawlessly, great job!", Sentiment = true },
                    new SentimentData { SentimentText = "Super easy to set up and use.", Sentiment = true },
                    new SentimentData { SentimentText = "Customer support was incredibly helpful.", Sentiment = true },
                    new SentimentData { SentimentText = "This app is a game changer for our team.", Sentiment = true },
                    new SentimentData { SentimentText = "Really impressed with the performance.", Sentiment = true },
                    new SentimentData { SentimentText = "Worth every penny, absolutely brilliant.", Sentiment = true },
                    new SentimentData { SentimentText = "The UI is clean and intuitive.", Sentiment = true },
                    new SentimentData { SentimentText = "I freaking love how fast this is!", Sentiment = true },
                    new SentimentData { SentimentText = "Holy crap, this thing is awesome!", Sentiment = true },
                    new SentimentData { SentimentText = "Damn, that's a good feature!", Sentiment = true },
                    new SentimentData { SentimentText = "No way, this actually works perfectly!", Sentiment = true },
                    new SentimentData { SentimentText = "Hell yeah, best update ever!", Sentiment = true },

                    // Négatif
                    new SentimentData { SentimentText = "Je n'aime pas du tout cela.", Sentiment = false },
                    new SentimentData { SentimentText = "Ce produit est terrible.", Sentiment = false },
                    new SentimentData { SentimentText = "J'ai eu une mauvaise expérience avec ceci.", Sentiment = false },
                    new SentimentData { SentimentText = "Le service client n'était pas utile.", Sentiment = false },
                    new SentimentData { SentimentText = "Je suis très déçu.", Sentiment = false },
                    new SentimentData { SentimentText = "L'application plante constamment.", Sentiment = false },
                    new SentimentData { SentimentText = "Les performances sont catastrophiques.", Sentiment = false },
                    new SentimentData { SentimentText = "Impossible de configurer correctement le produit.", Sentiment = false },
                    new SentimentData { SentimentText = "Je regrette mon achat, c'est inutilisable.", Sentiment = false },
                    new SentimentData { SentimentText = "L'interface est confuse et mal pensée.", Sentiment = false },
                    new SentimentData { SentimentText = "Aucune réponse du support technique depuis une semaine.", Sentiment = false },
                    new SentimentData { SentimentText = "Le produit ne correspond pas à la description.", Sentiment = false },
                    new SentimentData { SentimentText = "Trop de bugs, c'est frustrant.", Sentiment = false },
                    new SentimentData { SentimentText = "La mise à jour a tout cassé.", Sentiment = false },
                    new SentimentData { SentimentText = "Je ne recommande pas ce service.", Sentiment = false },
                    new SentimentData { SentimentText = "La qualité a vraiment baissé ces derniers mois.", Sentiment = false },
                    new SentimentData { SentimentText = "C'est lent et ça ne fonctionne pas bien.", Sentiment = false },
                    new SentimentData { SentimentText = "J'ai perdu des données à cause de ce logiciel.", Sentiment = false },
                    new SentimentData { SentimentText = "Le prix est beaucoup trop élevé pour ce que c'est.", Sentiment = false },
                    new SentimentData { SentimentText = "Documentation inexistante, impossible de s'en sortir.", Sentiment = false },
                    new SentimentData { SentimentText = "Le produit est instable et peu fiable.", Sentiment = false },
                    new SentimentData { SentimentText = "Expérience désastreuse, à éviter absolument.", Sentiment = false },
                    new SentimentData { SentimentText = "Les fonctionnalités promises ne sont pas présentes.", Sentiment = false },
                    new SentimentData { SentimentText = "Installation compliquée et mal documentée.", Sentiment = false },
                    new SentimentData { SentimentText = "Je suis passé à un concurrent, bien mieux.", Sentiment = false },
                    new SentimentData { SentimentText = "Temps de réponse inacceptable.", Sentiment = false },
                    new SentimentData { SentimentText = "L'ergonomie est vraiment à revoir.", Sentiment = false },
                    new SentimentData { SentimentText = "Rien ne marche comme prévu.", Sentiment = false },
                    new SentimentData { SentimentText = "Le service après-vente est inexistant.", Sentiment = false },
                    new SentimentData { SentimentText = "Je déconseille fortement ce produit.", Sentiment = false },

                    // Positif (Québécois)
                    new SentimentData { SentimentText = "Tabarouette que c'est bon ce produit-là !", Sentiment = true },
                    new SentimentData { SentimentText = "Câline, ça marche vraiment bien !", Sentiment = true },
                    new SentimentData { SentimentText = "Baptême que c'est une bonne application !", Sentiment = true },
                    new SentimentData { SentimentText = "Mautadit que c'est efficace, j'en reviens pas !", Sentiment = true },
                    new SentimentData { SentimentText = "Simonac, l'interface est belle en crime !", Sentiment = true },
                    new SentimentData { SentimentText = "Bonyeu que le support est bon !", Sentiment = true },
                    new SentimentData { SentimentText = "Torrieu, ça c'est du bon logiciel !", Sentiment = true },
                    new SentimentData { SentimentText = "Sacramouille que c'est rapide !", Sentiment = true },

                    // Négatif (Québécois)
                    new SentimentData { SentimentText = "Tabarnouche, ça plante encore !", Sentiment = false },
                    new SentimentData { SentimentText = "Câliboire, c'est ben trop lent !", Sentiment = false },
                    new SentimentData { SentimentText = "Crisse que c'est mal fait !", Sentiment = false },
                    new SentimentData { SentimentText = "Ostifie, j'ai encore perdu mes données !", Sentiment = false },
                    new SentimentData { SentimentText = "Tabarslak, le service client est nul !", Sentiment = false },
                    new SentimentData { SentimentText = "Viarge, c'est la pire application que j'ai vue !", Sentiment = false },
                    new SentimentData { SentimentText = "Maudit que c'est pas fiable ce logiciel-là !", Sentiment = false },
                    new SentimentData { SentimentText = "Sacrament, rien marche comme du monde !", Sentiment = false },

                    // Negative (English)
                    new SentimentData { SentimentText = "This product is absolutely terrible.", Sentiment = false },
                    new SentimentData { SentimentText = "Worst purchase I've ever made.", Sentiment = false },
                    new SentimentData { SentimentText = "The app keeps crashing, so frustrating.", Sentiment = false },
                    new SentimentData { SentimentText = "I can't believe how bad this is.", Sentiment = false },
                    new SentimentData { SentimentText = "Total waste of money, do not buy.", Sentiment = false },
                    new SentimentData { SentimentText = "Support never responded to my ticket.", Sentiment = false },
                    new SentimentData { SentimentText = "The interface is confusing and ugly.", Sentiment = false },
                    new SentimentData { SentimentText = "Nothing works as advertised.", Sentiment = false },
                    new SentimentData { SentimentText = "I regret buying this, it's useless.", Sentiment = false },
                    new SentimentData { SentimentText = "Buggy as heck, crashes every five minutes.", Sentiment = false },
                    new SentimentData { SentimentText = "What the heck, this broke everything!", Sentiment = false },
                    new SentimentData { SentimentText = "Crap, I lost all my data again.", Sentiment = false },
                    new SentimentData { SentimentText = "This darn thing never works right.", Sentiment = false },
                    new SentimentData { SentimentText = "Are you freaking kidding me with this update?", Sentiment = false },
                    new SentimentData { SentimentText = "Holy cow, this is the worst app ever.", Sentiment = false },
                };

                var dataView = mlContext.Data.LoadFromEnumerable(sampleData);
                var dataSplit = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

                var pipeline = mlContext.Transforms.Text.FeaturizeText("Features", "SentimentText")
                    .Append(mlContext.BinaryClassification.Trainers.FastTree(numberOfLeaves: 50, numberOfTrees: 50));

                Console.WriteLine("Entraînement du modèle...");
                var model = pipeline.Fit(dataSplit.TrainSet);

                var predictions = model.Transform(dataSplit.TestSet);
                var metrics = mlContext.BinaryClassification.Evaluate(predictions);
                Console.WriteLine($"Précision : {metrics.Accuracy:P2}, F1 : {metrics.F1Score:P2}");

                mlContext.Model.Save(model, dataView.Schema, _modelPath);
                _predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
            });
        }

        public bool LoadModel()
        {
            var mlContext = new MLContext();

            if (File.Exists(_modelPath))
            {
                var model = mlContext.Model.Load(_modelPath, out var _);
                _predictionEngine = mlContext.Model.CreatePredictionEngine<SentimentData, SentimentPrediction>(model);
                return true;
            }

            return false;
        }

        public SentimentPrediction PredictSentiment(string text)
        {
            if (_predictionEngine == null)
            {
                LoadModel();
                if (_predictionEngine == null)
                    throw new InvalidOperationException("Le modèle n'est pas chargé. Entraînez-le d'abord.");
            }

            return _predictionEngine.Predict(new SentimentData { SentimentText = text });
        }
    }
}
