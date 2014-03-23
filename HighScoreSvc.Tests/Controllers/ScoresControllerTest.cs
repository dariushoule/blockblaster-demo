using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HighScoreSvc;
using HighScoreSvc.Controllers;
using System.Transactions;
using System.IO;

namespace HighScoreSvc.Tests.Controllers
{
    [TestClass]
    public class ScoresControllerTest
    {
        private Random randomScore = new Random();

        [TestMethod]
        public void Get() {
            try {
                using (TransactionScope scope = new TransactionScope()) {
                    ScoresController controller = new ScoresController();
                    HiScoreDataContext hiScoreDB = controller.hiScoreDB;
                    hiScoreDB.HiScores.DeleteAllOnSubmit(hiScoreDB.HiScores);
                    hiScoreDB.SubmitChanges();

                    // Test retrieval of empty set
                    IEnumerable<HiScore> result = controller.Get();
                    Assert.IsNotNull(result, "Failed to retrieve a HiScore object from the controller.");
                    Assert.IsTrue(result.Count() == 0, "Database could not be cleared, integrity of test can't be validated.");

                    // Test retrieval of set > 10
                    for (int i = 0; i < 15; i++) {
                        hiScoreDB.HiScores.InsertOnSubmit(new HiScore {
                            date = DateTime.Now,
                            name = "Mr. Dude",
                            score = 500
                        });
                    }
                    hiScoreDB.SubmitChanges();

                    Assert.IsTrue(result.Count() >= 0 && result.Count() <= 10,
                        "Controller returned a number of results that was out of bounds 0-10");

                    // Test retrieval 

                    Transaction.Current.Rollback();
                }
            } catch (TransactionAbortedException ex) {
                Console.Out.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            } catch (ApplicationException ex) {
                Console.Out.WriteLine("ApplicationException Message: {0}", ex.Message);
            }
        }

        [TestMethod]
        public void Post() {
            try {
                using (TransactionScope scope = new TransactionScope()) {
                    ScoresController controller = new ScoresController();
                    HiScoreDataContext hiScoreDB = controller.hiScoreDB;
                    hiScoreDB.HiScores.DeleteAllOnSubmit(hiScoreDB.HiScores);
                    hiScoreDB.SubmitChanges();

                    // Test proper creation of 12 valid objects
                    for (int i = 1; i <= 12; i++) {
                        string simName = Path.GetRandomFileName().Replace(".", "");
                        int simScore = randomScore.Next(1, 1000);

                        IEnumerable<HiScore> result = controller.Post(new HiScorePostModel(simName, simScore));
                        Assert.IsNotNull(result, "Failed to retrieve a HiScore object from the controller.");

                        IEnumerable<HiScore> lastInserted = (from topScores in hiScoreDB.HiScores
                         orderby topScores.id descending
                         select topScores).Take(1);

                        HiScore testScore = lastInserted.First();
                        Assert.IsTrue(testScore.name.Equals(simName), "There is a discrepancy between name given vs inserted.");
                        Assert.IsTrue(testScore.score == simScore, "There is a discrepancy between score given vs inserted.");
                        Assert.IsNotNull(testScore.date, "The high score does not have an associated date.");
                    }

                    hiScoreDB.HiScores.DeleteAllOnSubmit(hiScoreDB.HiScores);
                    hiScoreDB.SubmitChanges();

                    // Test error handling on bad input
                    Boolean caughtError = false;
                    try {
                        controller.Post(new HiScorePostModel("12345678901234567", 100));
                    } catch (HttpResponseException e) {
                        caughtError = true;
                    }
                    Assert.IsTrue(caughtError, "Bad request did not throw proper exception.");

                    Transaction.Current.Rollback();
                }
            } catch (TransactionAbortedException ex) {
                Console.Out.WriteLine("TransactionAbortedException Message: {0}", ex.Message);
            } catch (ApplicationException ex) {
                Console.Out.WriteLine("ApplicationException Message: {0}", ex.Message);
            }
        }
    }
}
