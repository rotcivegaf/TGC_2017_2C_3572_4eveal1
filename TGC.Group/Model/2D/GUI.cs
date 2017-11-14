using System.Collections.Generic;
using System.Drawing;
using Microsoft.DirectX;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Examples.Engine2D.Core;
using TGC.Group.Form;
using TGC.Group.Model.GameObject;
using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model._2D {
    public class GUI {
        public Personaje personaje;
        public Vector3 posCamara;
        public Drawer2D drawer2D;
        public string mediaDir;

        public Barra barraCansancio;
        public Barra barraSed;
        public Barra barraTemperatura;
        public Barra barraHambre;

        public CustomSprite banana;
        public CustomSprite piedra;
        public CustomSprite agua;
        public CustomSprite madera;

        public GUI(Personaje personaje, Vector3 posCamara, string mediaDir) {
            this.personaje = personaje;
            this.mediaDir = mediaDir;
            drawer2D = new Drawer2D();


            agua = drawer2D.load(mediaDir + "2D\\agua.png", new Vector2(0, 0), new Vector2(257, 515), new Vector2(30, 470), new Vector2(0.2f, 0.1f));
            banana = drawer2D.load(mediaDir + "2D\\banana.png", new Vector2(0, 0), new Vector2(500, 400), new Vector2(25, 520), new Vector2(0.13f, 0.17f));
            piedra = drawer2D.load(mediaDir + "2D\\roca.png", new Vector2(0, 0), new Vector2(500, 400), new Vector2(25, 570), new Vector2(0.25f, 0.27f));
            madera = drawer2D.load(mediaDir + "2D\\madera.png", new Vector2(0, 0), new Vector2(500, 400), new Vector2(25, 620), new Vector2(0.13f, 0.18f));

            Vector2 posIni = new Vector2(30, 700);
            int deltaY = 50;
            barraCansancio   = new Barra(mediaDir, posIni, "2D\\barO1.png", "2D\\barO2.png", "2D\\barO3.png", "Cansancio", drawer2D);
            posIni += new Vector2(0, deltaY);
            barraSed         = new Barra(mediaDir, posIni, "2D\\barB1.png", "2D\\barB2.png", "2D\\barB3.png", "Sed", drawer2D);
            posIni += new Vector2(0, deltaY);
            barraHambre      = new Barra(mediaDir, posIni, "2D\\barG1.png", "2D\\barG2.png", "2D\\barG3.png", "Hambre", drawer2D);
            posIni += new Vector2(0, deltaY);
            barraTemperatura = new Barra(mediaDir, posIni, "2D\\barY1.png", "2D\\barY2.png", "2D\\barY3.png", "Temperatura", drawer2D);
        }

        public void render(TgcText2D DrawText, GameForm formPrincipal) {
            drawer2D.BeginDrawSprite();

            DrawText.drawText(""+personaje.inventario.agua, 90, 500, System.Drawing.Color.White);
            drawer2D.DrawSprite(agua);
            DrawText.drawText("" + personaje.inventario.banana, 90, 550, System.Drawing.Color.White);
            drawer2D.DrawSprite(banana);
            DrawText.drawText("" + personaje.inventario.piedra, 90, 600, System.Drawing.Color.White);
            drawer2D.DrawSprite(piedra);
            DrawText.drawText("" + personaje.inventario.madera, 90, 650, System.Drawing.Color.White);
            drawer2D.DrawSprite(madera);

            barraCansancio.render(drawer2D, DrawText, (int)personaje.cansancio);
            barraSed.render(drawer2D, DrawText, (int)personaje.sed);
            barraHambre.render(drawer2D, DrawText, (int)personaje.hambre);
            barraTemperatura.render(drawer2D, DrawText, (int)personaje.temperatura);

            drawer2D.EndDrawSprite();
        }
    }
}
