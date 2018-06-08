﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrepuscularRays_PostProcess.PostProcessing
{
    public class LightSceneMask : BasePostProcess
    {
        Vector3 lighSourcePos;
        public LightSceneMask(Game game, Vector3 sourcePos)
            : base(game)
        {
            lighSourcePos = sourcePos;
            UsesVertexShader = true;
        }

        public override void Draw(GameTime gameTime)
        {
            if (effect == null)
                effect = Game.Content.Load<Effect>("Shaders/PostProcessing/LightSceneMask");

            effect.CurrentTechnique = effect.Techniques["LightSourceSceneMask"];

            effect.Parameters["depthMap"].SetValue(DepthBuffer);
            effect.Parameters["halfPixel"].SetValue(HalfPixel);

            effect.Parameters["lightPosition"].SetValue(lighSourcePos);
            effect.Parameters["matVP"].SetValue(camera.View * camera.Projection);
            effect.Parameters["matInvVP"].SetValue(Matrix.Invert(camera.View * camera.Projection));

            // Set Params.
            base.Draw(gameTime);

        }
    }
}
