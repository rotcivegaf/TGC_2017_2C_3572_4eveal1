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
            botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(0, 0, -170), "CajaVerde\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.5f, 0.5f)));
            botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(0, 25, -175), "CajaVerde\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.5f, 0.5f)));
            botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(0, 50, -180), "CajaVerde\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.5f, 0.5f)));
        }

        public void render() {
            foreach (TgcMesh boton in botones) {
                boton.render();
            }
        }
    }
}
