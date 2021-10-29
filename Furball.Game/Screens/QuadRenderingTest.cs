using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Renderers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Game.Screens {
    public class QuadRenderingTest : Screen {
        private QuadRenderer    _renderer;
        private BasicEffect basicEffect;
        // private Matrix projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(0), 800f / 480f, 0.01f, 100f);


        private Matrix world      = Matrix.CreateTranslation(0, 0, 0);
        private Matrix view       = Matrix.CreateLookAt(new Vector3(0, 0, 3), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
        private Matrix projection = Matrix.CreatePerspective(1280, 720, 0.01f, 100f);

        public override void Initialize() {
            base.Initialize();

            this._renderer   = new QuadRenderer(FurballGame.DeviceContext);
            this.basicEffect = new BasicEffect(GraphicsDevice);
        }

        public override void Draw(GameTime gameTime) {
            base.Draw(gameTime);

            world      = Matrix.CreateTranslation(0f, 0, 0);
            view       = Matrix.CreateLookAt(new Vector3(1, 1, 3), new Vector3(.5f, 0, 0), new Vector3(0, 1, 0));
            projection = Matrix.CreatePerspective(1f, 1f, .5f, 100f);

            basicEffect.World              = world;
            basicEffect.View               = view;
            basicEffect.Projection         = projection;
            basicEffect.VertexColorEnabled = true;

            RasterizerState rasterizerState = new RasterizerState();
            rasterizerState.CullMode       = CullMode.None;
            GraphicsDevice.RasterizerState = rasterizerState;


            foreach (EffectPass pass in this.basicEffect.CurrentTechnique.Passes) {
                pass.Apply();
                this._renderer.RenderQuad(new Vector2(-1f,1), new Vector2(0, 0));
            }
        }
    }
}
