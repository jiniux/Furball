using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine {
    public class Screen : DrawableGameComponent {
        protected DrawableManager Manager;
        private   RenderTarget2D  target;
        public Screen() : base(FurballGame.Instance) {}

        /// <summary>
        /// You MUST run base.Initialize before adding things to your manager!!!!
        /// </summary>
        public override void Initialize() {
            this.Manager = new();
            this.target  = new RenderTarget2D(FurballGame.Instance.GraphicsDevice, 1280, 720);

            this.Manager.ViewRectangle.Position = new Vector2(320, 200);
            this.Manager.ViewRectangle.Size     = new Vector2(640, 480);
            
            base.Initialize();
        }
        
        public override void Draw(GameTime gameTime) {
            FurballGame.Instance.GraphicsDevice.SetRenderTarget(this.target);
            this.Manager.Draw(gameTime, FurballGame.DrawableBatch);
            FurballGame.Instance.GraphicsDevice.SetRenderTarget(null);

            FurballGame.DrawableBatch.Begin();
            FurballGame.DrawableBatch.SpriteBatch.Draw(this.target, this.Manager.ViewRectangle.Position, new Rectangle(new Point(0,0), this.Manager.ViewRectangle.Size.ToPoint()), Color.White);
            FurballGame.DrawableBatch.End();

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime) {
            this.Manager.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing) {
            this.Manager.Dispose(disposing);
            
            base.Dispose(disposing);
        }
    }
}
