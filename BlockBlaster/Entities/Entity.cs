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
    abstract class Entity
    {
        public float X;
        public VertexPositionColor[] Pos;

        public virtual void Move(Vector3 Delta) {
            for (int i = 0; i < Pos.Length; i++) {
                Pos[i].Position += Delta;
            }
        }

        public virtual Boolean Update() {
            return false;
        } 
    }
}
