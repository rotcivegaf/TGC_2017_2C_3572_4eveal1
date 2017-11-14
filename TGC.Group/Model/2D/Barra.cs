using System.Drawing;
using Microsoft.DirectX;
using TGC.Core.Direct3D;
using TGC.Core.Text;
using TGC.Examples.Engine2D.Core;

namespace TGC.Group.Model._2D {
    public class Barra {
        public Vector2 posIni;
        public int alto;

        public CustomSprite spriteIni;
        public CustomSprite spriteCenter;
        public CustomSprite spriteFin;

        public CustomSprite spriteBackIni;
        public CustomSprite spriteBackCenter;
        public CustomSprite spriteBackFin;
        public string text;

        public Barra(string mediaDir, Vector2 posIni, string pathIni, string pathCentro, string pathFin, string text, Drawer2D drawer2D) {
            this.text = text;
            this.posIni = posIni;

            Vector2 scala = new Vector2(1, 0.75f);

            spriteBackIni = drawer2D.load(mediaDir + "2D\\barBack1.png", new Vector2(1, 0), new Vector2(14, 32), posIni, scala);
            spriteIni = drawer2D.load(mediaDir + pathIni, new Vector2(1, 0), new Vector2(14, 32), posIni, scala);
            posIni += new Vector2(14, 0);
            spriteBackCenter = drawer2D.load(mediaDir + "2D\\barBack2.png", new Vector2(0, 0), new Vector2(100, 32), posIni, scala);
            spriteCenter = drawer2D.load(mediaDir + pathCentro, new Vector2(0, 0), new Vector2(100, 32), posIni, scala);
            posIni += new Vector2(100, 0);
            spriteBackFin = drawer2D.load(mediaDir + "2D\\barBack3.png", new Vector2(1, 0), new Vector2(14, 32), posIni, scala);
            spriteFin = drawer2D.load(mediaDir + pathFin, new Vector2(1, 0), new Vector2(14, 32), posIni, scala);
        }

        public void render(Drawer2D drawer2D, TgcText2D DrawText, int cant) {
            DrawText.drawText(text, (int)posIni.X, (int)posIni.Y - 18, System.Drawing.Color.White);

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
