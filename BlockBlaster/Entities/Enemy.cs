using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using BlockBlaster.Entities;

namespace BlockBlaster.Entities
{
    class Enemy : Entity
    {
        public static double SPAWN_RATE = 0.01;
        public int pointValue;
        private static Random rWidth = new Random();
        private static Random rX = new Random();

        public Enemy() : base() {
            int width = rWidth.Next(6, 60);
            pointValue = (60 - width) * (60 - width);

            int randX = rX.Next(0, 800 - width);

            Pos = new VertexPositionColor[8];
            Pos[0].Position = new Vector3(randX, 0, 0);
            Pos[0].Color = Color.Green;
            Pos[1].Position = new Vector3(randX, width, 0);
            Pos[1].Color = Color.Green;

            Pos[2].Position = new Vector3(randX, width, 0);
            Pos[2].Color = Color.Green;
            Pos[3].Position = new Vector3(randX + width, width, 0);
            Pos[3].Color = Color.Green;

            Pos[4].Position = new Vector3(randX + width, width, 0);
            Pos[4].Color = Color.Green;
            Pos[5].Position = new Vector3(randX + width, 0, 0);
            Pos[5].Color = Color.Green;

            Pos[6].Position = new Vector3(randX + width, 0, 0);
            Pos[6].Color = Color.Green;
            Pos[7].Position = new Vector3(randX, 0, 0);
            Pos[7].Color = Color.Green;
        }

        override public void Move(Vector3 Delta) {
            base.Move(Delta);
        }

        public Boolean CheckCollision(float testX, float testY) {
            return (testX >= Pos[0].Position.X && testY >= Pos[0].Position.Y &&
                    testX <= Pos[4].Position.X && testY <= Pos[4].Position.Y);
        }

        override public Boolean Update() {
            Move(new Vector3(0, 1.2f, 0));
            return (Pos[0].Position.Y < 600);
        }
    }
}
