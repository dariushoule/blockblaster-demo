using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Data.SqlTypes;
using System.Data.SqlClient;
using System.Net.Http;
using System.Net;

namespace HighScoreSvc.Controllers
{
    public class HiScorePostModel {
        public HiScorePostModel(string name, int score) {
            Name = name;
            Score = score;
        }

        public string Name { get; set; }
        public int Score { get; set; }
    }

    public class ScoresController : ApiController
    {
        public HiScoreDataContext hiScoreDB = new HiScoreDataContext();

        public IEnumerable<HiScore> Get()
        {
            return ( from topScores in hiScoreDB.HiScores 
                    orderby topScores.score descending
                    select topScores ).Take(10);
        }

        public IEnumerable<HiScore> Post([FromBody]HiScorePostModel newScore)
        {
            try {
                hiScoreDB.HiScores.InsertOnSubmit(new HiScore {
                    date = DateTime.Now,
                    name = newScore.Name,
                    score = newScore.Score
                });
                hiScoreDB.SubmitChanges();
            } catch(SqlException e) {
                Console.Out.WriteLine("An Exception occured when trying to create high score: " + e);
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound) {
                    Content = new StringContent("An Exception occured when trying to create high score: Make sure name is less than 16 characters and the request is formatted correctly.")
                });
            }

            return Get();
        }
    }
}
