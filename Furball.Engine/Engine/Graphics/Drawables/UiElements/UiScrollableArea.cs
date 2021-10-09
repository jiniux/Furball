using System;
using Furball.Engine.Engine.Graphics.Drawables.Managers;
using Furball.Engine.Engine.Input;
using Microsoft.Xna.Framework;

namespace Furball.Engine.Engine.Graphics.Drawables.UiElements {
    public class UiScrollableArea : UnmanagedDrawable {
        private DrawableManager _manager;

        private bool _isDisplayed = false;

        private TexturedDrawable _verticalScrollBar;
        private TexturedDrawable _verticalScrollDragger;

        private bool    _scrollingVertically;
        private Vector2 _scrollPosition;
        private Vector2 _scrollMouseOffset;
        private Vector2 _scrollMousePosition;
        private Vector2 _scrollStartPosition;

        private Vector2   _contentDimensions;
        private Rectangle _displayRectangle;

        public UiScrollableArea(Rectangle displayRectangle, Vector2 contentDimensions) {
            this.OnDrag += this.HandleOnDrag;
            this.OnDragEnd += this.HandleOnDragEnd;

            this._manager = new DrawableManager();

            this._verticalScrollBar = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                Depth = 1.5f,
                ColorOverride = new Color(40, 40, 40),
                OriginType = OriginType.TopLeft,
                Scale = new Vector2(1.6f),
            };

            this._verticalScrollBar.OnClick += this.VerticalScrollBarOnOnClick;

            this._verticalScrollDragger = new TexturedDrawable(FurballGame.WhitePixel, Vector2.Zero) {
                Depth = 1.6f,
                ColorOverride = Color.White,
                Scale = new Vector2(1.6f),
            };

            this._manager.Add(this._verticalScrollBar);
            this._manager.Add(this._verticalScrollDragger);

            this._contentDimensions = contentDimensions;

            this.SetDisplayRectangle(displayRectangle);
        }

        public void SetContentDimensions(Vector2 contentDimensions) {
            this._contentDimensions = contentDimensions;
        }

        public void SetDisplayRectangle(Rectangle displayRectangle) {
            this._displayRectangle = displayRectangle;
            //TODO: do this
            this._manager.SetVisibleArea(displayRectangle);

            this._verticalScrollDragger.Position = new Vector2(this._displayRectangle.Width - 10, 0);
            this._verticalScrollDragger.Scale = new Vector2(10, this._displayRectangle.Height);

            this.SetContentDimensions(this._contentDimensions);
        }

        public void SetScrollPosition(Vector2 scrollPosition) {
            this._scrollPosition = new Vector2(0, Math.Clamp(scrollPosition.Y, 0, this._contentDimensions.Y - this._displayRectangle.Height));

            if (float.IsNaN(this._scrollPosition.X))
                this._scrollPosition.X = 0;
            if (float.IsNaN(this._scrollPosition.Y))
                this._scrollPosition.Y = 0;

            this._manager.SetViewOffset(this._scrollPosition);

            this.UpdateVerticalDragger();
        }

        private void UpdateVerticalDragger() {
            this._verticalScrollDragger.Position = new Vector2(
                this._displayRectangle.Width - 8,
                2 + (this._scrollPosition.Y / (this._contentDimensions.Y - this._displayRectangle.Height)) * (this._displayRectangle.Height - this._verticalScrollDragger.Scale.Y - 4)
            );

            this._verticalScrollDragger.Scale = new Vector2(6, Math.Min(1, this._displayRectangle.Height / this._contentDimensions.Y) * this._displayRectangle.Height - 4);
        }

        public void Show() {
            this._isDisplayed = true;
        }

        public void Hide() {
            this._isDisplayed = false;
        }

        private void VerticalScrollBarOnOnClick(object? sender, Point e) {
            this.SetScrollPosition(new Vector2(0, ((e.Y * (1 / FurballGame.VerticalRatio)) - this._displayRectangle.Y) / this._displayRectangle.Height * (this._contentDimensions.Y - this._displayRectangle.Height)));
        }
        private void HandleOnDrag(object? sender, Point e) {
            if(!this._isDisplayed)
                return;

            if (!this._scrollingVertically && this._verticalScrollDragger.IsHovered) {
                this._scrollingVertically = true;

                this._scrollMouseOffset   = e.ToVector2() * (1 / FurballGame.VerticalRatio);
                this._scrollStartPosition = this._scrollPosition;
            }

            if (this._scrollingVertically) {
                //peppy what the fuck
                SetScrollPosition(this._scrollStartPosition + ((e.ToVector2() * (1 / FurballGame.VerticalRatio) - this._scrollMouseOffset) * (this._contentDimensions.Y - this._displayRectangle.Height) / (this._displayRectangle.Height - this._verticalScrollDragger.Scale.Y - 4)));
            }
        }

        private void HandleOnDragEnd(object? sender, Point e) {
            if (!this._isDisplayed)
                return;

            if (this._scrollingVertically)
                this._scrollingVertically = false;
        }

        public override void Draw(GameTime time, DrawableBatch drawableBatch, DrawableManagerArgs args = null) {
            if(this._isDisplayed)
                this._manager.Draw(time, drawableBatch, args);
        }
    }
}
