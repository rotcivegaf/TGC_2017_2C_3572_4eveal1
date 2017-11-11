using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Geometry;
using TGC.Core.Shaders;

namespace TGC.Group.Model.GameObjects {
    public class ObjectCreator {
        public static Random Rand { get; } = new Random(666);
        public Mapa mapa;

        public ObjectCreator(Mapa mapa) {
            this.mapa = mapa;
        }

        public Vector3 getRandomPositionVector(float deltaY, float xIni, float zIni) {
            int rand1 = Rand.Next(0, mapa.heightmap.GetLength(0) - 1);
            int rand2 = Rand.Next(0, mapa.heightmap.GetLength(1) - 1);

            int x = (mapa.heightmap.GetLength(0)-1 * rand1);
            int z = (mapa.heightmap.GetLength(1)-1 * rand2);
            int y = mapa.heightmap[rand1, rand2];

            return new Vector3((rand1 * mapa.scaleXZ) + xIni, (y - deltaY) * mapa.scaleY, (rand2 * mapa.scaleXZ) + zIni);
        }

        public static Vector3 getRandomScaleVector() {
            float scale = (float)Rand.NextDouble();
            if (scale < 0.4f) scale = 0.4f;
            return new Vector3(scale, scale, scale);
        }

        public TgcMesh dibujaBoton(Vector3 pos, String dir, Vector3 scala) {
            TgcMesh boton = mapa.Loader.loadSceneFromFile(mapa.MediaDir + dir).Meshes[0];
            boton.Scale = scala;
            boton.Position = pos;
            boton.Transform = Matrix.Scaling(boton.Scale) * Matrix.Translation(boton.Position);
            boton.AlphaBlendEnable = true;
            boton.Effect = TgcShaders.loadEffect(mapa.shaderDir + "rotarEjeX.fx");
            boton.Technique = "RenderScene";

            return boton;
        }

        public List<TgcMesh> createObjects(int cantidad, String dir, float deltaY, float xIni, float zIni, bool esSolido) {
            TgcMesh objeto = mapa.Loader.loadSceneFromFile(mapa.MediaDir + dir).Meshes[0];
            List<TgcMesh> lista = new List<TgcMesh>();
            TgcMesh instance;

            for (int i = 1; i <= cantidad; i++) {
                instance = objeto.createMeshInstance(objeto.Name + "_" + i);
                instance.Scale = getRandomScaleVector();
                instance.Position = getRandomPositionVector(deltaY, xIni, zIni);
                instance.Transform = Matrix.Scaling(instance.Scale) * Matrix.Translation(instance.Position);
                instance.updateBoundingBox();
                instance.AlphaBlendEnable = true;
                if (esSolido)
                    instance.BoundingBox = TgcBox.fromExtremes(new Vector3(0, 0, 0), new Vector3(0, 0, 0)).BoundingBox;
                lista.Add(instance);
            }
            return lista;
        }
    }
}