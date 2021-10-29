using Furball.Engine.Engine.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Renderers {
    public class QuadRenderer {
        private GraphicsDeviceContext _device;

        private readonly VertexPositionColorTexture[] _vertexBuffer;
        private readonly short[]                 _indexBuffer;

        public QuadRenderer(GraphicsDeviceContext device) {
            this._device = device;

            this._vertexBuffer = new VertexPositionColorTexture[4];

            this._vertexBuffer[0] = new VertexPositionColorTexture(new Vector3(-1, 1,  1), Color.White, new Vector2(0, 0));
            this._vertexBuffer[1] = new VertexPositionColorTexture(new Vector3(1,  1,  1), Color.White, new Vector2(1, 0));
            this._vertexBuffer[2] = new VertexPositionColorTexture(new Vector3(-1, -1, 1), Color.White, new Vector2(0, 1));
            this._vertexBuffer[3] = new VertexPositionColorTexture(new Vector3(1,  -1, 1), Color.White, new Vector2(1, 1));

            this._indexBuffer = new short[] {
                0, 3, 2, 0, 1, 3
            };
        }

        public void RenderQuad(Vector2 upperLeftCorner, Vector2 lowerRightCorner) {
            _vertexBuffer[0].Position.X = upperLeftCorner.X;
            _vertexBuffer[0].Position.Y = lowerRightCorner.Y;

            _vertexBuffer[1].Position.X = lowerRightCorner.X;
            _vertexBuffer[1].Position.Y = lowerRightCorner.Y;

            _vertexBuffer[2].Position.X = upperLeftCorner.X;
            _vertexBuffer[2].Position.Y = upperLeftCorner.Y;

            _vertexBuffer[3].Position.X = lowerRightCorner.X;
            _vertexBuffer[3].Position.Y = upperLeftCorner.Y;

            this._device
                .GetGraphicsDevice()
                .DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this._vertexBuffer, 0, 4, this._indexBuffer, 0, 2);
        }
    }
}
