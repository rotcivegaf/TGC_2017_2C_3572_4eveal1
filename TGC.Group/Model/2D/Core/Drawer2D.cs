using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Direct3D;

namespace TGC.Examples.Engine2D.Core {
    public class Drawer2D {
        private readonly Sprite DxSprite;
        private readonly Line line;

        private readonly CustomVertex.PositionColoredTextured[] LineVertexData =
            new CustomVertex.PositionColoredTextured[2];

        public Drawer2D() {
            DxSprite = new Sprite(D3DDevice.Instance.Device);
            line = new Line(D3DDevice.Instance.Device);
        }

        public void BeginDrawSprite() {
            DxSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthFrontToBack);
        }
        
        public void EndDrawSprite() {
            DxSprite.End();
        }
        
        public void DrawSprite(CustomSprite sprite) {
            DxSprite.Transform = sprite.TransformationMatrix;
            DxSprite.Draw(sprite.Bitmap.D3dTexture, sprite.SrcRect, Vector3.Empty, Vector3.Empty, sprite.Color);
        }

        public void DrawPoint(Vector2 position, Color color) {
            LineVertexData[0].X = position.X;
            LineVertexData[0].Y = position.Y;
            LineVertexData[0].Color = color.ToArgb();

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionColoredTextured.Format;

            D3DDevice.Instance.Device.DrawUserPrimitives(PrimitiveType.PointList, 1, LineVertexData);
        }
        public void DrawLine(Vector2 position1, Vector2 position2, Color color, int width, bool antiAlias) {
            var positionList = new Vector2[2] { position1, position2 };
            DrawPolyline(positionList, color, width, antiAlias);
        }

        public void DrawPolyline(Vector2[] positionList, Color color, int width, bool antiAlias) {
            line.Antialias = antiAlias;
            line.Width = width;
            line.Draw(positionList, color);
        }

        public CustomSprite load(string dir, Vector2 posIniBM, Vector2 cuerpoBM, Vector2 posIni, Vector2 scala) {
            CustomBitmap bitMap = new CustomBitmap(dir, D3DDevice.Instance.Device);
            CustomSprite sprite = new CustomSprite {
                SrcRect = new Rectangle((int)posIniBM.X, (int)posIniBM.Y, (int)cuerpoBM.X, (int)cuerpoBM.Y),
                Bitmap = bitMap,
                Position = posIni,
                Scaling = scala
            };

            return sprite;
        }
    }
}