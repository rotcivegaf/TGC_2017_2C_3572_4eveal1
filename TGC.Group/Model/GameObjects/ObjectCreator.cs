using Microsoft.DirectX;
using System;
using System.Collections.Generic;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using Effect = Microsoft.DirectX.Direct3D.Effect;

namespace TGC.Group.Model.GameObjects {
    public class ObjectCreator {
        public static Random Rand { get; } = new Random(666);
        public Mapa mapa;
        public Effect effect;

        public ObjectCreator(Mapa mapa) {
            this.mapa = mapa;
            effect = TgcShaders.loadEffect(mapa.shaderDir + "ObjectShader.fx");
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
            boton.Effect = TgcShaders.loadEffect(mapa.shaderDir + "MenuShader.fx");
            boton.Technique = "upDown";

            return boton;
        }

        public List<TgcMesh> crearCharco(Vector3 pos) {
            TgcMesh objeto = mapa.Loader.loadSceneFromFile(mapa.MediaDir + "agua\\CajaVerde-TgcScene.xml").Meshes[0];
            List<TgcMesh> lista = new List<TgcMesh>();
            TgcMesh instance = objeto.createMeshInstance("agua");

            instance.Effect = effect;
            instance.Technique = "AguaLight";

            instance.Scale = new Vector3(4,1,4);
            instance.Position = pos;
            instance.Transform = Matrix.Scaling(instance.Scale) * Matrix.Translation(instance.Position);
            instance.updateBoundingBox();
            instance.AlphaBlendEnable = true;
            lista.Add(instance);

            return lista;
        }

        public List<TgcMesh> createRocas(int cantidad, String dir, float deltaY, float xIni, float zIni) {
            TgcMesh objeto = mapa.Loader.loadSceneFromFile(mapa.MediaDir + dir).Meshes[0];
            List<TgcMesh> lista = new List<TgcMesh>();
            TgcMesh instance;

            for (int i = 1; i <= cantidad; i++) {
                instance = objeto.createMeshInstance(objeto.Name);

                instance.Effect = effect;
                instance.Technique = "FogLight";
                
                instance.Scale = getRandomScaleVector();
                instance.Position = getRandomPositionVector(deltaY, xIni, zIni);
                instance.Transform = Matrix.Scaling(instance.Scale) * Matrix.Translation(instance.Position);
                instance.updateBoundingBox();
                instance.AlphaBlendEnable = true;
                lista.Add(instance);
            }
            return lista;
        }
        
        public List<TgcMesh> createObjects(int cantidad, String dir, float deltaY, float xIni, float zIni) {
            TgcMesh objeto = mapa.Loader.loadSceneFromFile(mapa.MediaDir + dir).Meshes[0];
            List<TgcMesh> lista = new List<TgcMesh>();
            TgcMesh instance;

            for (int i = 1; i <= cantidad; i++) {
                instance = objeto.createMeshInstance(objeto.Name);

                instance.Effect = effect;
                instance.Technique = "FogWindLight";

                instance.Scale = getRandomScaleVector();
                instance.Position = getRandomPositionVector(deltaY, xIni, zIni);
                instance.Transform = Matrix.Scaling(instance.Scale) * Matrix.Translation(instance.Position);
                instance.updateBoundingBox();
                instance.AlphaBlendEnable = true;
                lista.Add(instance);
            }
            return lista;
        }
    }
}