using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EbonianMod.Effects.Prims
{
    public struct VertexTest : IVertexType
    {
        private static VertexDeclaration _vertexDeclaration = new VertexDeclaration((VertexElement[])(object)new VertexElement[3]
        {
            new VertexElement(0, (VertexElementFormat)1, (VertexElementUsage)0, 0),
            new VertexElement(8, (VertexElementFormat)4, (VertexElementUsage)1, 0),
            new VertexElement(12, (VertexElementFormat)2, (VertexElementUsage)2, 0)
        });

        public Vector2 Position;

        public Color Color;

        public Vector3 TexCoord;

        public VertexDeclaration VertexDeclaration => _vertexDeclaration;

        public VertexTest(Vector2 position, Color color, Vector3 texCoord)
        {
            Position = position;
            Color = color;
            TexCoord = texCoord;
        }
    }

    public struct VertexInfo2 : IVertexType
    {
        private static VertexDeclaration _vertexDeclaration = new(new VertexElement[3]
        {
            new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
            new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
            new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
        });
        public Vector2 Position;
        public Color Color;
        public Vector3 TexCoord;
        public VertexInfo2(Vector2 position, Vector3 texCoord, Color color)
        {
            Position = position;
            TexCoord = texCoord;
            Color = color;
        }
        public VertexDeclaration VertexDeclaration
        {
            get => _vertexDeclaration;
        }
    }
}
