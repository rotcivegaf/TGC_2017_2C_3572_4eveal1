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

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Geometry;

namespace TGC.Group.Model{
    public class Mapa{
        public static Random Rand { get; } = new Random(666);
        public List<TgcMesh> Trees = new List<TgcMesh>();
        private Microsoft.DirectX.Direct3D.Device Device;
        public TgcPlane Floor { get; set; }
        public int MapaLength;
        private TgcSceneLoader Loader;
        String MediaDir;

        public Mapa(String mediaDir, Microsoft.DirectX.Direct3D.Device device, TgcSceneLoader loader, int mapaLength){
            MediaDir = mediaDir;
            MapaLength = mapaLength;
            Loader = loader;
            Device = device;

            var floorTexture = TgcTexture.createTexture(device, MediaDir + "tierra.jpg");
            Floor = new TgcPlane(new Vector3(-mapaLength / 2, 0, -mapaLength / 2), new Vector3(mapaLength, 0, mapaLength), TgcPlane.Orientations.XZplane, floorTexture, 70f, 70f);

            CreateTrees(10000);
        }

        public Vector3 getRandomPositionVector(){
            int rand1 = Rand.Next(0, MapaLength) - MapaLength / 2;
            int rand2 = Rand.Next(0, MapaLength) - MapaLength / 2;
            
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

        public void renderAll(){
           Floor.render();
            foreach (TgcMesh mesh in Trees) {
                mesh.render();
            }
        }

        public void disposeAll(){
            foreach (TgcMesh mesh in Trees){
                mesh.dispose();
            }
        }
    }
}
