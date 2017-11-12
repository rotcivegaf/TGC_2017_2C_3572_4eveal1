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
        
        public GUI(Personaje personaje, Vector3 posCamara, string mediaDir) {
            this.personaje = personaje;
            this.mediaDir = mediaDir;
            drawer2D = new Drawer2D();
            
            int xIni = 30;
            int yIni = 700;
            int deltaY = 50;
            barraCansancio   = new Barra(mediaDir, xIni, yIni, "2D\\barO1.png", "2D\\barO2.png", "2D\\barO3.png", "Cansancio");
            barraSed         = new Barra(mediaDir, xIni, yIni + deltaY, "2D\\barB1.png", "2D\\barB2.png", "2D\\barB3.png", "Sed");
            barraHambre      = new Barra(mediaDir, xIni, yIni + deltaY*2, "2D\\barG1.png", "2D\\barG2.png", "2D\\barG3.png", "Hambre");
            barraTemperatura = new Barra(mediaDir, xIni, yIni + deltaY*3, "2D\\barY1.png", "2D\\barY2.png", "2D\\barY3.png", "Temperatura");
        }

        public void render(TgcText2D DrawText, GameForm formPrincipal) {
            drawer2D.BeginDrawSprite();

            barraCansancio.render(drawer2D, DrawText, (int)personaje.cansancio);
            barraSed.render(drawer2D, DrawText, (int)personaje.sed);
            barraHambre.render(drawer2D, DrawText, (int)personaje.hambre);
            barraTemperatura.render(drawer2D, DrawText, (int)personaje.temperatura);

            drawer2D.EndDrawSprite();
        }
    }
}
