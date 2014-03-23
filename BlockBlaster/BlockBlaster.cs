#region Using Statements
using System;
using System.Configuration;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using BlockBlaster.Render;
using BlockBlaster.Entities;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;
#endregion

namespace BlockBlaster
{
    public class BlockBlaster : Game
    {
        GraphicsDeviceManager graphics;

        BasicEffect basicEffect;
        Player player;
        List<Entity> shots;
        List<Entity> baddies;

        SpriteBatch spriteBatch;
        SpriteFont font;
        Boolean shotFired = false;
        Boolean done = false;
        String highScoreList;
        TimeSpan lastShot;

        private static Random rSpawn = new Random();

        private const string API_ENDPOINT = "http://localhost:49634/v1/scores";

        public BlockBlaster() : base() {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            graphics.ApplyChanges();

            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            basicEffect = new BasicEffect(graphics.GraphicsDevice);
            basicEffect.VertexColorEnabled = true;
            basicEffect.Projection = Matrix.CreateOrthographicOffCenter
               (0, graphics.GraphicsDevice.Viewport.Width,     // left, right
                graphics.GraphicsDevice.Viewport.Height, 0,    // bottom, top
                0, 1);                                         // near, far plane
 
            player = new Player();
            shots = new List<Entity>();
            baddies = new List<Entity>();

            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            font = Content.Load<SpriteFont>("spriteFont");
        }


        protected override void UnloadContent() {
            // TODO: Unload any non ContentManager content here
        }

        private void UpdateEntityList(ref List<Entity> entityList) {
            if (entityList.Count > 0) {
                List<Entity> expired = new List<Entity>();
                foreach (Entity ent in entityList)
                    if (!ent.Update())
                        expired.Add(ent);

                foreach (Entity remove in expired)
                    entityList.Remove(remove);
            }
        }

        private void HandleCollisions(ref List<Entity> shots, ref List<Entity> baddies) {
            List<Tuple<Projectile, Enemy>> removals = new List<Tuple<Projectile, Enemy>>();

            foreach (Enemy baddy in baddies)
                foreach (Projectile shot in shots) {
                    if (baddy.CheckCollision(shot.Pos[0].Position.X, shot.Pos[0].Position.Y)) {
                        player.Score += baddy.pointValue;
                        removals.Add(new Tuple<Projectile, Enemy>(shot, baddy));
                    }
                }
            
            foreach(Tuple<Projectile, Enemy> removal in removals) {
                shots.Remove(removal.Item1);
                baddies.Remove(removal.Item2);
            }
        }

        protected override void Update(GameTime gameTime) {
            KeyboardState KS = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || KS.IsKeyDown(Keys.Escape))
                Exit();

            //if (gameTime.TotalGameTime.Seconds > 14 && !done) {
            if (gameTime.TotalGameTime.Minutes > 0 && !done) {
                done = true;

                string user = ConfigurationSettings.AppSettings["player_name"];
                IEnumerable<HiScore> hiScoreObjects = JsonConvert.DeserializeObject<IEnumerable<HiScore>>(RecordHighScore(user, player.Score));
                highScoreList = "";
                foreach (HiScore score in hiScoreObjects) {
                    highScoreList += score.name + " : " + score.score.ToString("00000000") + "\n";
                }
            }

            if (!done) {
                if (KS.IsKeyDown(Keys.Left))
                    player.MoveLeft(2.8f);

                if (KS.IsKeyDown(Keys.Right))
                    player.MoveRight(2.8f);

                TimeSpan elapsedBetweenShots = gameTime.TotalGameTime - lastShot;
                if (KS.IsKeyDown(Keys.Up) && !shotFired && elapsedBetweenShots.Milliseconds > 500) {
                    shotFired = true;
                    lastShot = gameTime.TotalGameTime;
                    shots.Add(new Projectile(player.X));
                } else {
                    shotFired = false;
                }

                if (rSpawn.NextDouble() < Enemy.SPAWN_RATE)
                    baddies.Add(new Enemy());

                UpdateEntityList(ref shots);
                UpdateEntityList(ref baddies);
                HandleCollisions(ref shots, ref baddies);
            }

            base.Update(gameTime);
        }

        private string RecordHighScore(string name, int score) {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(API_ENDPOINT);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())) {
                string json = JsonConvert.SerializeObject(new { Name = name, Score = score });

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream())) {
                    return streamReader.ReadToEnd();
                }
            }
        }

        public class HiScore
        {
            public int id;
            public System.DateTime date;
            public string name;
            public long score;
        }

        /// <summary>
        /// Woo this is a doc.
        /// </summary>
        /// <param name="gameTime">This parameter is provided by XNA. It does stuff.</param>
        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            if (!done) {
                basicEffect.CurrentTechnique.Passes[0].Apply();
                graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, player.Pos, 0, player.Pos.Length / 2);

                foreach (Projectile shot in shots)
                    graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, shot.Pos, 0, shot.Pos.Length / 2);

                foreach (Enemy baddy in baddies)
                    graphics.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, baddy.Pos, 0, baddy.Pos.Length / 2);

                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Score: " + player.Score.ToString("00000000"), new Vector2(0, 0), Color.LightGreen);
                spriteBatch.DrawString(font, "Time Left: " + (60 - gameTime.TotalGameTime.Seconds), new Vector2(600, 0), Color.AliceBlue);
                spriteBatch.End();
            } else {
                spriteBatch.Begin();
                spriteBatch.DrawString(font, "Game Complete! High Scores: \n" + highScoreList, new Vector2(260, 40), Color.Green);
                spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
