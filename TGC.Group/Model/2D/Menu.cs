using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model._2D {
    public class Menu {
        public ObjectCreator objectCreator;
        public List<TgcMesh> botones = new List<TgcMesh>();
        public bool gameStart;

        public Menu(bool gameStart, ObjectCreator objectCreator, Vector3 posCamara) {
            this.objectCreator = objectCreator;
            botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(5, 30, -120), "CajaPlay\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.3f, 0.1f)));
            botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(-5, 15, -120), "Caja\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.3f, 0.1f)));
            botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(5, 0, -120), "CajaQuit\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.3f, 0.1f)));
        }

        public void render() {
            foreach (TgcMesh boton in botones) {
                boton.render();
            }
        }
    }
}
