using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrepuscularRays_PostProcess.PostProcessing
{
    public class BrightPass : BasePostProcess
    {
        public float BloomThreshold;
        public BrightPass(Game game, float threshold)
            : base(game)
        {
            BloomThreshold = threshold;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
            {
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/BrightPass");
            }

            effect.Parameters["BloomThreshold"].SetValue(BloomThreshold);

            // Set Params.
            base.Draw(gameTime);
        }
    }
}
