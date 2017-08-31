using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model{
    public class GameModel : TgcExample{
        private TgcSceneLoader Loader;
        public Mapa mapa;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir){
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        ///Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo procesamiento que podemos pre calcular para nuestro juego.
        public override void Init(){
            var d3dDevice = D3DDevice.Instance.Device;//Device de DirectX para crear primitivas.
            Loader = new TgcSceneLoader();

            mapa = new Mapa(MediaDir, d3dDevice, Loader, 10000);

            Camara = new TgcFpsCamera(new Vector3(0f, 200f, 0f), 100f, 50f, Input);

        }

        //Se llama en cada frame. Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones ante ellas.
        public override void Update(){
            PreUpdate();
            
        }
        ///Se llama cada vez que hay que refrescar la pantalla.
        public override void Render() {
            PreRender(); //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.

            mapa.renderAll();

            PostRender();//Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
        }
        ///Se llama cuando termina la ejecución del ejemplo.
        ///Hacer Dispose() de todos los objetos creados.
        ///Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        public override void Dispose(){
            mapa.disposeAll();
        }
    }
}