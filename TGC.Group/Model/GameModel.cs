using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Core.Utils;



namespace TGC.Group.Model{
    public class GameModel : TgcExample{
        public static Random Rand { get; } = new Random(666);
        public TgcPlane Floor { get; set; }
        public int FloorLength = 10000;
        
        private TgcSceneLoader Loader;
        private List<Object> Trees = new List<Object>();

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir){
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        ///Se llama una sola vez, al principio cuando se ejecuta el ejemplo.
        ///Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo procesamiento que podemos pre calcular para nuestro juego.
        public override void Init(){
            var d3dDevice = D3DDevice.Instance.Device;//Device de DirectX para crear primitivas.

            Camara = new TgcFpsCamera(new Vector3(0f, 200f, 0f), 50f, 50f, Input);

            Loader = new TgcSceneLoader();

            //genero el mapa
            var floorTexture = TgcTexture.createTexture(d3dDevice, MediaDir + "tierra.jpg");
            Floor = new TgcPlane(new Vector3(-FloorLength / 2, 0, -FloorLength/2), new Vector3(FloorLength, 0, FloorLength), TgcPlane.Orientations.XZplane, floorTexture, 70f, 70f );

            CreateTrees(10000);

        }

        public Vector3 getRandomPositionVector(){
            int rand1 = Rand.Next(0, FloorLength) - FloorLength / 2;
            int rand2 = Rand.Next(0, FloorLength) - FloorLength / 2;

            return new Vector3(rand1, 0, rand2);
        }

        public static Vector3 getRandomScaleVector(){
            float scale = (float)Rand.NextDouble();
            if (scale < 0.3f) scale = 0.3f;
            return new Vector3(scale, scale, scale);
        }

        private void CreateTrees(int cantidad){
            TgcMesh tree = Loader.loadSceneFromFile(MediaDir + "Meshes\\Pino\\Pino-TgcScene.xml").Meshes[0];

            TgcMesh instance;

            for (int i = 1; i <= cantidad; i++){
                instance = tree.createMeshInstance(tree.Name + "_" + i);
                instance.Scale = getRandomScaleVector();
                instance.Position = getRandomPositionVector();
                instance.Transform = Matrix.Scaling(instance.Scale) * Matrix.Translation(instance.Position);
                instance.updateBoundingBox();
                instance.AlphaBlendEnable = true;
                Trees.Add(instance);
            }
        }

        //Se llama en cada frame. Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones ante ellas.
        public override void Update(){
            PreUpdate();
            
        }
        ///Se llama cada vez que hay que refrescar la pantalla.
        public override void Render() {
            PreRender(); //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.

            Floor.render();
            foreach (TgcMesh mesh in Trees) {
                mesh.render();
            }
            PostRender();//Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
        }
        ///Se llama cuando termina la ejecución del ejemplo.
        ///Hacer Dispose() de todos los objetos creados.
        ///Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        public override void Dispose(){
            foreach (TgcMesh mesh in Trees){
                mesh.dispose();
            }
        }
    }
}