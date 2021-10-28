using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables {
    public class Form : ManagedDrawable {
        private CompositeDrawable FormContents;

        public Form() : base() {
            this.FormContents = new CompositeDrawable();

            TexturedDrawable topBar = new TexturedDrawable(FurballGame.WhitePixel, this.Position) {
                Scale = new Vector2(250, 50),
                ColorOverride = Color.Black,
                Clickable = true,
                CoverClicks = true,
                CoverHovers = true,
            };

            topBar.OnDragBegin += this.DragBegin;
            topBar.OnDrag      += this.Drag;
            topBar.OnDragEnd   += this.DragEnd;

            this.FormContents.Drawables.Add(topBar);
        }


        public override void Draw(GameTime time, DrawableBatch batch, DrawableManagerArgs args) {
            this.FormContents.Draw(time, batch, args);

            base.Draw(time, batch, args);
        }

        #region Dragging

        private Vector2 _dragBegin;

        private void DragBegin(object? sender, Point e) {
            this._dragBegin = e.ToVector2();
        }

        private void Drag(object? sender, Point e) {
            Vector2 current = e.ToVector2();
            Vector2 offset = this._dragBegin - current;

            this.Position += offset;
        }

        private void DragEnd(object? sender, Point e) {

        }

        #endregion
    }
}
