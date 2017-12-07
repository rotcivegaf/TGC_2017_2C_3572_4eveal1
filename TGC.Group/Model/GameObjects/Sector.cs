using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.SceneLoader;
using TGC.Group.Model.GameObject;

namespace TGC.Group.Model.GameObjects {
    public class Sector {
        public Vector2 numero;
        public Mapa mapa;
        public VertexBuffer vbTerrain;
        public CustomVertex.PositionTextured[] data;
        public List<TgcMesh> ObjetosMesh = new List<TgcMesh>();
        public List<TgcMesh> ObjetosMeshSinRender = new List<TgcMesh>();
        public float[] randomVectorX = {0,  0, 1, -1, 0.71f, -0.71f,  0.71f};
        public float[] randomVectorY = {1, -1, 0,  0, 0.71f,  0.71f, -0.71f};
        public int iVector = 5;
        public int iVectorOld = 0;
        public Random random = new Random();

        public TgcMesh meshAnterior = null;
        public int golpes = 0;

        private Vector3 collisionPoint;

        public Sector(Vector2 numero, Mapa mapa, ObjectCreator creador) {
            this.numero = numero;
            this.mapa = mapa;
            this.createHeightMapMesh();
            crearObj(creador);
        }

        private Vector3 crearVertice(int i, int j) {
            return new Vector3(
                    (i * mapa.scaleXZ) + (numero.X * (mapa.heightmap.GetLength(0) - 1) * mapa.scaleXZ),
                    mapa.heightmap[i, j] * mapa.scaleY,
                    (j * mapa.scaleXZ) + (numero.Y * (mapa.heightmap.GetLength(1) - 1) * mapa.scaleXZ)
                );
        }

        private Vector2 crearTextura(int u, int v) {
            return new Vector2(u / (float)32, v / (float)32);
        }

        private void createHeightMapMesh() {
            //Crear vertexBuffer
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), mapa.totalVertices, mapa.Device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Crear array temporal de vertices
            var dataIdx = 0;
            data = new CustomVertex.PositionTextured[mapa.totalVertices];
            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < mapa.heightmap.GetLength(0) - 1; i++) {
                for (var j = 0; j < mapa.heightmap.GetLength(1) - 1; j++) {
                    //Crear los cuatro vertices que conforman este cuadrante, aplicando la escala correspondiente
                    var v1 = crearVertice(i, j);
                    var v2 = crearVertice(i, j + 1);
                    var v3 = crearVertice(i + 1, j);
                    var v4 = crearVertice(i + 1, j + 1);

                    //Crear las coordenadas de textura para los cuatro vertices del cuadrante
                    var t1 = crearTextura(i, j);
                    var t2 = crearTextura(i, j + 1);
                    var t3 = crearTextura(i + 1, j);
                    var t4 = crearTextura(i + 1, j + 1);

                    //Cargar triangulo 1
                    data[dataIdx] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 1] = new CustomVertex.PositionTextured(v2, t2.X, t2.Y);
                    data[dataIdx + 2] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);

                    //Cargar triangulo 2
                    data[dataIdx + 3] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 4] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);
                    data[dataIdx + 5] = new CustomVertex.PositionTextured(v3, t3.X, t3.Y);

                    dataIdx += 6;
                }
            }

            //Llenar todo el VertexBuffer con el array temporal
            vbTerrain.SetData(data, 0, LockFlags.None);
        }

        public void crearObj(ObjectCreator creador) {
            float x = numero.X * mapa.length;
            float y = numero.Y * mapa.length;
            ObjetosMesh.AddRange(creador.createObjects(15, "Meshes\\Pasto\\Pasto-TgcScene.xml", 2, x, y, "Wind"));
            ObjetosMesh.AddRange(creador.createObjects(10, "Meshes\\Planta\\Planta-TgcScene.xml", 10, x, y, "Wind"));
            ObjetosMesh.AddRange(creador.createObjects(10, "Meshes\\Planta2\\Planta2-TgcScene.xml", 20, x, y, "Wind"));
            ObjetosMesh.AddRange(creador.createObjects(10, "Meshes\\Planta3\\Planta3-TgcScene.xml", 10, x, y, "Wind"));
            
            ObjetosMesh.AddRange(creador.crearCharco(new Vector3(x + mapa.length/2 - 226, -15, y+ mapa.length/2 - 266)));
            
            ObjetosMesh.AddRange(creador.createObjects(15, "Meshes\\Roca\\Roca-TgcScene.xml", 10, x, y, null));
            ObjetosMesh.AddRange(creador.createObjects(5, "Meshes\\Palmera\\Palmera-TgcScene.xml", 30, x, y, "Wind"));
            ObjetosMesh.AddRange(creador.createObjects(5, "Meshes\\Palmera2\\Palmera2-TgcScene.xml", 10, x, y, "Wind"));
            ObjetosMesh.AddRange(creador.createObjects(5, "Meshes\\Palmera3\\Palmera3-TgcScene.xml", 10, x, y, "Wind"));
            ObjetosMesh.AddRange(creador.createObjects(5, "Meshes\\ArbolBananas\\ArbolBananas-TgcScene.xml", 20, x, y, "Wind"));
        }

        public void mover(float x, float z) {
            for (var i = 0; i < data.GetLength(0) - 1; i++) {
                data[i].Position += new Vector3(x, 0, z);
            }
            vbTerrain.SetData(data, 0, LockFlags.None);

            ObjetosMesh.AddRange(ObjetosMeshSinRender);
            ObjetosMeshSinRender.Clear();
            foreach (TgcMesh mesh in ObjetosMesh) {
                mesh.Position += new Vector3(x, 0, z);
                mesh.Transform = Matrix.Translation(mesh.Position);
            }
        }       

        public void testPicking(TgcPickingRay pickingRay, Personaje personaje) {
            personaje.trabajo(0.5f, 0.05f, 0.1f);

            foreach (TgcMesh mesh in ObjetosMesh) {
                if (TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, mesh.BoundingBox, out collisionPoint)) {
                    if (mesh.Name == "agua") {
                        personaje.beber();
                        golpes = 0;
                        return;
                    }
                    if (golpes > 2) {
                        if (mesh.Name == "Roca")
                            if (personaje.inventario.piedra < personaje.inventario.maxCant)
                                personaje.inventario.piedra++;
                        if (mesh.Name == "ArbolBananas")
                            if (personaje.inventario.banana < personaje.inventario.maxCant)
                                personaje.inventario.banana++;
                        if (mesh.Name == "Palmera" || mesh.Name == "Palmera2" || mesh.Name == "Palmera3" || mesh.Name == "Planta" || mesh.Name == "Planta2" || mesh.Name == "Planta3")
                            if (personaje.inventario.madera < personaje.inventario.maxCant)
                                personaje.inventario.madera++;
                        ObjetosMeshSinRender.Add(mesh);
                        ObjetosMesh.Remove(mesh);
                        golpes = 0;
                    } else {
                        if (mesh.Equals(meshAnterior))
                            ++golpes;
                        meshAnterior = mesh;
                    }
                    return;
                }
            }
        }

        public void render(float ElapsedTime, Hora hora) {
            D3DDevice.Instance.Device.SetTexture(0, mapa.terrainTexture);
            D3DDevice.Instance.Device.SetTexture(1, null);
            D3DDevice.Instance.Device.Material = D3DDevice.DEFAULT_MATERIAL;
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, mapa.totalVertices / 3);
            
            /*if (hora.to12() % 6 <= 0.2 && iVector != iVectorOld) {
                iVectorOld = iVector;
                iVector = random.Next(0, 7);
            } else {
                iVectorOld = -1;
            }*/

            //var intencidad = (int)hora.toNormalScaleFactor() / (float)30;
            var intencidad = (int)2 / (float)30;
            foreach (TgcMesh mesh in ObjetosMesh) {
                
                mesh.Effect.SetValue("time", ElapsedTime);
                mesh.Effect.SetValue("windIntencidad", intencidad);
                mesh.Effect.SetValue("windNormalX", randomVectorX[iVector]);
                mesh.Effect.SetValue("windNormalZ", randomVectorY[iVector]);
                mesh.render();
                //mesh.BoundingBox.render();
            }
        }

        public void dispose() {
            foreach (TgcMesh mesh in ObjetosMesh) {
                mesh.dispose();
            }
            vbTerrain.Dispose();
        }
    }
}
