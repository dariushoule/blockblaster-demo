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

namespace BlockBlaster.Render
{
    class Player : Entity
    {
        public int Score = 0;

        public Player() : base() {
            Pos = new VertexPositionColor[6];
            Pos[0].Position = new Vector3(420, 580, 0);
            Pos[0].Color = Color.Green;
            Pos[1].Position = new Vector3(380, 580, 0);
            Pos[1].Color = Color.Green;

            Pos[2].Position = new Vector3(400, 550, 0);
            Pos[2].Color = Color.Green;
            Pos[3].Position = new Vector3(380, 580, 0);
            Pos[3].Color = Color.Green;

            Pos[4].Position = new Vector3(400, 550, 0);
            Pos[4].Color = Color.Green;
            Pos[5].Position = new Vector3(420, 580, 0);
            Pos[5].Color = Color.Green;

            X = Pos[2].Position.X;
        }

        override public void Move(Vector3 Delta) {
            base.Move(Delta);
            X = Pos[2].Position.X;
        }

        public void MoveLeft(float Power) {
            Move(new Vector3(-Power, 0, 0));
        }

        public void MoveRight(float Power) {
            Move(new Vector3(Power, 0, 0));
        }
    }
}
