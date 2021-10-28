using Furball.Engine;
using Furball.Engine.Engine;
using Furball.Engine.Engine.Graphics.Drawables;
using Microsoft.Xna.Framework;

namespace Furball.Game.Screens {
    public class FormTestScreen : Screen {
        public override void Initialize() {
            base.Initialize();

            TexturedDrawable background = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                ColorOverride = Color.BlueViolet,
                Scale         = new Vector2(1280, 720),
                Depth         = 1f
            };

            this.Manager.Add(background);

            Form form = new() {
                Position = new Vector2(400, 200),
                Visible = true
            };

            this.Manager.Add(form);
        }
    }
}
