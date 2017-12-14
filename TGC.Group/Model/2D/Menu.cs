using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model._2D {
    public class Menu {
        public ObjectCreator objectCreator;
        public List<TgcMesh> botones = new List<TgcMesh>();
        public int bSelec = 0;

        public Menu(ObjectCreator objectCreator, Vector3 posCamara) {
            this.objectCreator = objectCreator;

            botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(5, 15, -120), "Menu\\CajaPlay\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.3f, 0.1f)));
            //botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(-5, 5, -120), "Menu\\CajaOpcion\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.3f, 0.1f)));
            botones.Add(objectCreator.dibujaBoton(posCamara + new Vector3(5, -5, -120), "Menu\\CajaQuit\\CajaVerde-TgcScene.xml", new Vector3(1.5f, 0.3f, 0.1f)));
        }

        public int seleccionar(TgcD3dInput input) {
            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.W)) 
                if(bSelec == 0)
                    bSelec = 1;
                else
                    --bSelec;
            else
                if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.S))
                    bSelec = (++bSelec) % 2;

            if (input.keyPressed(Microsoft.DirectX.DirectInput.Key.Return))
                return bSelec;

            return -1;
        }

        public void render() {
            botones[bSelec].Effect.SetValue("time", (float)(DateTime.Now.Millisecond * Math.PI));
            foreach (TgcMesh boton in botones)
                boton.render();
        }
    }
}
