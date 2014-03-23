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

namespace BlockBlaster.Entities
{
    class Projectile : Entity
    {
        public Projectile(float FiredX) : base() {
            Pos = new VertexPositionColor[2];
            Pos[0].Position = new Vector3(FiredX, 550, 0);
            Pos[0].Color = Color.Yellow;
            Pos[1].Position = new Vector3(FiredX, 510, 0);
            Pos[1].Color = Color.Yellow;

            X = Pos[0].Position.X;
        }

        override public void Move(Vector3 Delta) {
            base.Move(Delta);
            X = Pos[0].Position.X;
        }

        override public Boolean Update() {
            Move(new Vector3(0,-6.2f,0));
            return (Pos[0].Position.Y > 0);
        }
    }
}
