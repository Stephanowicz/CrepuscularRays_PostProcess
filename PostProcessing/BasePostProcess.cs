using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CrepuscularRays_PostProcess.PostProcessing
{
    public class BasePostProcess
    {
        public Vector2 HalfPixel;

        public ICameraService camera
        {
            get { return ((ICameraService)Game.Services.GetService(typeof(ICameraService))); }
        }

        public Texture2D DepthBuffer;

        public Texture2D BackBuffer;
        public Texture2D orgBuffer;

        public bool Enabled = true;
        protected Effect effect;

        protected Game Game;
        public RenderTarget2D newScene;

        ScreenQuad sq;

        public bool UsesVertexShader = false;

        public BasePostProcess(Game game)
        {
            Game = game;

        }

        public virtual void Draw(GameTime gameTimer)
        {
            if (Enabled)
            {
                if (sq == null)
                {
                    sq = new ScreenQuad(Game);
                    sq.Initialize();
                }
                if(!effect.Name.Contains("BrightPass")&& !effect.Name.Contains("LightSourceMask"))
                    effect.Parameters["scene"].SetValue(BackBuffer);

                effect.CurrentTechnique.Passes[0].Apply();
                sq.Draw();                
            }
        }
    }
}
