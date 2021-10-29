using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Furball.Engine.Engine.Graphics.Renderers {
    public class QuadRenderer {
        private GraphicsDeviceContext _device;

        private readonly VertexPositionTexture[] _vertexBuffer;
        private readonly short[]                 _indexBuffer;

        public QuadRenderer(GraphicsDeviceContext device) {
            this._device = device;

            this._vertexBuffer = new VertexPositionTexture[4];

            this._vertexBuffer[0] = new VertexPositionTexture(new Vector3(-1, 1,  1), new Vector2(0, 0));
            this._vertexBuffer[1] = new VertexPositionTexture(new Vector3(1,  1,  1), new Vector2(1, 0));
            this._vertexBuffer[2] = new VertexPositionTexture(new Vector3(-1, -1, 1), new Vector2(0, 1));
            this._vertexBuffer[3] = new VertexPositionTexture(new Vector3(1,  -1, 1), new Vector2(1, 1));

            this._indexBuffer = new short[] {
                0, 3, 2, 0, 1, 3
            };
        }

        public void RenderQuad(Vector2 v1, Vector2 v2) {
            _vertexBuffer[0].Position.X = v1.X;
            _vertexBuffer[0].Position.Y = v2.Y;

            _vertexBuffer[1].Position.X = v2.X;
            _vertexBuffer[1].Position.Y = v2.Y;

            _vertexBuffer[2].Position.X = v1.X;
            _vertexBuffer[2].Position.Y = v1.Y;

            _vertexBuffer[3].Position.X = v2.X;
            _vertexBuffer[3].Position.Y = v1.Y;

            this._device
                .GetGraphicsDevice()
                .DrawUserIndexedPrimitives(PrimitiveType.TriangleList, this._vertexBuffer, 0, 4, this._indexBuffer, 0, 2);
        }
    }
}
