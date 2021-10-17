using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics {
    public class ViewRectangle {
        public const int DEFAULT_WIDTH  = FurballGame.DEFAULT_WINDOW_WIDTH;
        public const int DEFAULT_HEIGHT = FurballGame.DEFAULT_WINDOW_HEIGHT;
        public const int DEFAULT_POS_X  = 0;
        public const int DEFAULT_POS_Y  = 0;

        public Vector2 Size;
        public Vector2 Position;

        public float VerticalRatio   => (float) Size.Y / (float) DEFAULT_HEIGHT;
        public float HorizontalRatio => (float) Size.X / (float) DEFAULT_WIDTH;

        public Rectangle Rectangle => new Rectangle((int) this.Position.X, (int) this.Position.Y, (int) this.Size.X, (int) this.Size.Y);

        public ViewRectangle() {
            this.Size     = new Vector2(DEFAULT_WIDTH, DEFAULT_HEIGHT);
            this.Position = new Vector2(DEFAULT_POS_X, DEFAULT_POS_Y);
        }
    }
}
