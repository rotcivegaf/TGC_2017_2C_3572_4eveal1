using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX;
using TGC.Core.SceneLoader;
using TGC.Core.Text;
using TGC.Group.Form;
using TGC.Group.Model.GameObject;
using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model._2D {
    public class GUI {
        public Personaje personaje;
        List<TgcMesh> barras = new List<TgcMesh>();
        public ObjectCreator objectCreator;
        public Vector3 posCamara;

        public GUI(Personaje personaje, ObjectCreator objectCreator, Vector3 posCamara) {
            this.personaje = personaje;
            //this.objectCreator = objectCreator;
            //crearBarras();
        }

        public void crearBarras() {
            barras.Add(objectCreator.dibujaBoton(posCamara + new Vector3(0, 0, -170), "GUI\\frame-hp-bar.xcf", new Vector3(1.5f, 0.5f, 0.5f)));
        }

        public void render(TgcText2D DrawText, GameForm formPrincipal) {
            int inicioY = (int)(formPrincipal.Height * 0.75);
            int inicioX = 15;

            DrawText.drawText("Hambre: " + personaje.hambre, inicioX, inicioY, System.Drawing.Color.White);
            DrawText.drawText("Sed: " + personaje.sed, inicioX, inicioY -20, System.Drawing.Color.White);
            DrawText.drawText("Temperatura: " + personaje.temperatura, inicioX, inicioY - 40, System.Drawing.Color.White);
            DrawText.drawText("Cansancio: " + personaje.cansancio, inicioX , inicioY - 60, System.Drawing.Color.White);
            /*foreach (TgcMesh barra in barras) {
                barra.render();
            }*/
        }
    }
}
