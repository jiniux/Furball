using System.Drawing;
using System.Numerics;
using Furball.Vixie.Graphics;
using Furball.Vixie.Graphics.Renderers;
using Color=Furball.Vixie.Graphics.Color;

namespace Furball.Engine.Engine.Graphics {
    /// <summary>
    /// A Basic Abstraction for Sprite and Shape batch
    /// </summary>
    public class DrawableBatch {
        private readonly ITextureRenderer _textureRenderer;
        private          ILineRenderer    _lineRenderer;

        private bool _begun;
        public bool Begun => _begun;
        
        public DrawableBatch(ITextureRenderer textureRenderer, ILineRenderer lineRenderer) {
            this._textureRenderer = textureRenderer;
            this._lineRenderer    = lineRenderer;
        }

        public void Begin() {
            this._textureRenderer.Begin();
            this._begun = true;
        }

        public void End() {
            if(this._textureRenderer.IsBegun)
                this._textureRenderer.End();

            if(this._lineRenderer.IsBegun)
                this._lineRenderer.End();

            this._begun = false;
        }

        public void Draw(Texture texture, Vector2 position, Vector2? size = null, Vector2? scale = null, float rotation = 0f, Color? colorOverride = null, Rectangle? sourceRect = null, TextureFlip texFlip = TextureFlip.None) {
            if(this._lineRenderer.IsBegun)
                this._lineRenderer.End();

            if(!this._textureRenderer.IsBegun)
                this._textureRenderer.Begin();

            this._textureRenderer.Draw(texture, position, size, scale, rotation, colorOverride, sourceRect, texFlip);
        }

        public void DrawLine(Vector2 begin, Vector2 end, float thickness, Color color) {
            if(this._textureRenderer.IsBegun)
                this._textureRenderer.End();

            if(!this._lineRenderer.IsBegun)
                this._lineRenderer.Begin();

            this._lineRenderer.Draw(begin, end, thickness, color);
        }

        public void DrawLine(float x0, float y0, float x1, float y1, float thickness, Color color) {
            this.DrawLine(new Vector2(x0, y0), new Vector2(x1, y1), thickness, color);
        }

        public void DrawRectangle(Vector2 position, Vector2 size, float thickness, Color color) {
            Vector2 p0 = position;
            Vector2 p1 = new Vector2(position.X + size.X, position.Y);
            Vector2 p2 = new Vector2(position.X + size.X, position.Y + size.Y);
            Vector2 p3 = new Vector2(position.X, position.Y + size.Y);

            this.DrawLine(p0, p1, thickness, color);
            this.DrawLine(p1, p2, thickness, color);
            this.DrawLine(p2, p3, thickness, color);
            this.DrawLine(p3, p0, thickness, color);
        }

        public void FillRectangle(Vector2 position, Vector2 size, Color color, float rotation = 0f) {
            this.Draw(FurballGame.WhitePixel, position, null, size, rotation, color);
        }
    }
}
