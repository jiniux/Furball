using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics;
using Furball.Engine.Engine.Graphics.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Game.Screens {
    public class QuadRenderingTest : Screen {
        private QuadRenderer    _renderer;
        private BasicEffect basicEffect;
        // private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(0), 800f / 480f, 0.01f, 100f);
        private Camera _basicCamera;

        public override void Initialize() {
            base.Initialize();

            this._renderer   = new QuadRenderer(FurballGame.DeviceContext);
            this.basicEffect = new BasicEffect(GraphicsDevice);

            this._basicCamera = FurballGame.DeviceContext.GetBasicScreenCamera();

            basicEffect.World              = this._basicCamera.WorldPosition;
            basicEffect.View               = this._basicCamera.CameraState;
            basicEffect.Projection         = this._basicCamera.Projection;
            basicEffect.VertexColorEnabled = true;
        }

        public override void Draw(GameTime gameTime) {
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                this._renderer.RenderQuad(new Vector2(-0.25f, -0.25f), new Vector2(0.25f, 0.25f));
            }

            base.Draw(gameTime);
        }
    }
}
