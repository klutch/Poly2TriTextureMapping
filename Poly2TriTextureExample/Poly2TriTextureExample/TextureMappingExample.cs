using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Poly2Tri;

namespace Poly2TriTextureExample
{
    public class TextureMappingExample : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D _texture;
        private List<PolygonPoint> _points;
        private Polygon _polygon;
        private VertexPositionTexture[] _vertices;
        private VertexPositionTexture[] _lineVertices;
        private Vector2 _lowerBound;
        private Vector2 _upperBound;
        private Effect _primitivesEffect;
        private Texture2D _lineTexture;

        public TextureMappingExample()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Define polygon
            _points = new List<PolygonPoint>();
            _points.Add(new PolygonPoint(-8.485714f, -1.914286f));
            _points.Add(new PolygonPoint(-6.514286f, -4.914286f));
            _points.Add(new PolygonPoint(-4.514286f, -3.914286f));
            _points.Add(new PolygonPoint(-1.514286f, -6.914286f));
            _points.Add(new PolygonPoint(1.485714f, -4.914286f));
            _points.Add(new PolygonPoint(2.485714f, -5.885714f));
            _points.Add(new PolygonPoint(7.457143f, -4.914286f));
            _points.Add(new PolygonPoint(8.457143f, -0.9428571f));
            _points.Add(new PolygonPoint(6.485714f, 1f));
            _points.Add(new PolygonPoint(4.457143f, 6.028572f));
            _points.Add(new PolygonPoint(0.4857143f, 7.057143f));
            _points.Add(new PolygonPoint(-1.485714f, 3.028571f));
            _points.Add(new PolygonPoint(-6.485714f, 1.028571f));
            _polygon = new Polygon(_points);

            // Decompose polygon
            P2T.Triangulate(_polygon);

            // Define bounding box
            _lowerBound = new Vector2((float)_polygon.MinX, (float)_polygon.MinY);
            _upperBound = new Vector2((float)_polygon.MaxX, (float)_polygon.MaxY);

            // Create vertices
            createVertices();

            // Create lines
            createLineVertices();

            // Load content
            base.Initialize();

            // Set window size
            _graphics.PreferredBackBufferWidth = _texture.Width + 64;
            _graphics.PreferredBackBufferHeight = _texture.Height + 64;
            _graphics.ApplyChanges();
        }

        private void createVertices()
        {
            int index = 0;
            Vector2 boundingBoxSize = _upperBound - _lowerBound;

            _vertices = new VertexPositionTexture[_polygon.Triangles.Count * 3];
            foreach (DelaunayTriangle triangle in _polygon.Triangles)
            {
                Vector2 p1 = new Vector2(triangle.Points[0].Xf, triangle.Points[0].Yf);
                Vector2 p2 = new Vector2(triangle.Points[1].Xf, triangle.Points[1].Yf);
                Vector2 p3 = new Vector2(triangle.Points[2].Xf, triangle.Points[2].Yf);
                Vector2 relativeP1 = p1 - _lowerBound;
                Vector2 relativeP2 = p2 - _lowerBound;
                Vector2 relativeP3 = p3 - _lowerBound;

                _vertices[index++] = new VertexPositionTexture(
                    new Vector3(p1, 0),
                    relativeP1 / boundingBoxSize);
                _vertices[index++] = new VertexPositionTexture(
                    new Vector3(p2, 0),
                    relativeP2 / boundingBoxSize);
                _vertices[index++] = new VertexPositionTexture(
                    new Vector3(p3, 0),
                    relativeP3 / boundingBoxSize);
            }
        }

        private void createLineVertices()
        {
            int index = 0;
            _lineVertices = new VertexPositionTexture[_polygon.Triangles.Count * 6];
            foreach (DelaunayTriangle triangle in _polygon.Triangles)
            {
                Vector2 a1 = new Vector2(triangle.Points[0].Xf, triangle.Points[0].Yf);
                Vector2 a2 = new Vector2(triangle.Points[1].Xf, triangle.Points[1].Yf);
                Vector2 b1 = a2;
                Vector2 b2 = new Vector2(triangle.Points[2].Xf, triangle.Points[2].Yf);
                Vector2 c1 = b2;
                Vector2 c2 = new Vector2(triangle.Points[0].Xf, triangle.Points[0].Yf);

                _lineVertices[index++] = new VertexPositionTexture(new Vector3(a1, 0), Vector2.Zero);
                _lineVertices[index++] = new VertexPositionTexture(new Vector3(a2, 0), Vector2.Zero);
                _lineVertices[index++] = new VertexPositionTexture(new Vector3(b1, 0), Vector2.Zero);
                _lineVertices[index++] = new VertexPositionTexture(new Vector3(b2, 0), Vector2.Zero);
                _lineVertices[index++] = new VertexPositionTexture(new Vector3(c1, 0), Vector2.Zero);
                _lineVertices[index++] = new VertexPositionTexture(new Vector3(c2, 0), Vector2.Zero);
            }
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _texture = Content.Load<Texture2D>("terrain");
            _primitivesEffect = Content.Load<Effect>("primitives_effect");
            _lineTexture = new Texture2D(GraphicsDevice, 1, 1);
            _lineTexture.SetData<Color>(new[] { Color.White });
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _primitivesEffect.CurrentTechnique = _primitivesEffect.Techniques[0];
            _primitivesEffect.CurrentTechnique.Passes[0].Apply();
            _primitivesEffect.Parameters["World"].SetValue(Matrix.Identity);
            _primitivesEffect.Parameters["View"].SetValue(Matrix.CreateScale(new Vector3(35f, -35f, 1f)));
            _primitivesEffect.Parameters["Projection"].SetValue(Matrix.CreateOrthographic(GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0f, 1f));
            GraphicsDevice.Textures[0] = _texture;
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _vertices, 0, _polygon.Triangles.Count, VertexPositionTexture.VertexDeclaration);
            GraphicsDevice.Textures[0] = _lineTexture;
            GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, _lineVertices, 0, _polygon.Triangles.Count * 3, VertexPositionTexture.VertexDeclaration);

            base.Draw(gameTime);
        }
    }
}
