using System.Drawing;
using Microsoft.DirectX;
using TGC.Core.Direct3D;
using TGC.Core.Text;
using TGC.Examples.Engine2D.Core;

namespace TGC.Group.Model._2D {
    public class Barra {
        public string mediaDir;
        public int x;
        public int y;
        public int alto;

        public CustomSprite spriteIni;
        public CustomSprite spriteCenter;
        public CustomSprite spriteFin;

        public CustomSprite spriteBackIni;
        public CustomSprite spriteBackCenter;
        public CustomSprite spriteBackFin;
        public string text;

        public Barra(string mediaDir, int xIni, int yIni, string pathIni, string pathCentro, string pathFin, string text) {
            this.mediaDir = mediaDir;
            this.text = text;
            this.x = xIni;
            this.y = yIni;

            spriteBackIni =    load("2D\\barBack1.png", 1, 0, 14 , 32, 0);
            spriteBackCenter = load("2D\\barBack2.png", 0, 0, 100, 32, 14);
            spriteBackFin =    load("2D\\barBack3.png", 1, 0, 14 , 32, 114);
            
            spriteIni =    load(pathIni   , 1, 0, 14 , 32, 0  );
            spriteCenter = load(pathCentro, 0, 0, 100, 32, 14 );
            spriteFin =    load(pathFin   , 1, 0, 14 , 32, 114);
        }

        private CustomSprite load(string dir, int xIniBM, int yIniBM, int ancho, int alto, int deltaX) {
            CustomBitmap bitMap = new CustomBitmap(mediaDir + dir, D3DDevice.Instance.Device);
            CustomSprite sprite = new CustomSprite {
                SrcRect = new Rectangle(xIniBM, yIniBM, ancho, alto),
                Bitmap = bitMap,
                Position = new Vector2(x + deltaX, y),
                Scaling = new Vector2(1, 0.75f)
            };

            return sprite;
        }

        public void render(Drawer2D drawer2D, TgcText2D DrawText, int cant) {
            DrawText.drawText(text, x, y-18, System.Drawing.Color.White);

            drawer2D.DrawSprite(spriteBackIni);
            drawer2D.DrawSprite(spriteBackCenter);
            drawer2D.DrawSprite(spriteBackFin);

            if (cant > 0)
                drawer2D.DrawSprite(spriteIni);
            if (cant > 99)
                drawer2D.DrawSprite(spriteFin);
            else                    
                spriteCenter.SrcRect = new Rectangle(1, 0, cant, 32);

            drawer2D.DrawSprite(spriteCenter);
        }
    }
}
