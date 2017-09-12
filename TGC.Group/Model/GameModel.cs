using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.SceneLoader;

using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model{
    public class GameModel : TgcExample{
        public List<TgcMesh> ObjetosMesh = new List<TgcMesh>();
        public Mapa mapa;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir){
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }
        
        ///Se llama una sola vez, al principio cuando se ejecuta el ejemplo. Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo procesamiento que podemos pre calcular para nuestro juego.
        public override void Init(){
            mapa = new Mapa(MediaDir, 1024);

            ObjectCreator creador = new ObjectCreator();

            ObjetosMesh.AddRange(creador.createObjects(100, "Meshes\\Pino\\Pino-TgcScene.xml", 2, mapa));

            //CreateObjects(100*8, "Meshes\\Pasto\\Pasto-TgcScene.xml", 5);
            //CreateObjects(100*8, "Meshes\\Roca\\roca-TgcScene.xml", 5);

            Camara = new TgcFpsCamera(new Vector3(1024*4, 200f, 1024*4), 500f, 500f, Input);
        }

        //Se llama en cada frame. Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones ante ellas.
        public override void Update(){
            PreUpdate();
            
        }
        ///Se llama cada vez que hay que refrescar la pantalla.
        public override void Render() {
            PreRender(); //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.

            mapa.render();
            foreach (TgcMesh mesh in ObjetosMesh){
                mesh.render();
            }

            PostRender();//Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
        }
        ///Se llama cuando termina la ejecución del ejemplo. Hacer Dispose() de todos los objetos creados. Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        public override void Dispose(){
            mapa.dispose();
            foreach (TgcMesh mesh in ObjetosMesh){
                mesh.dispose();
            }
        }
    }
}