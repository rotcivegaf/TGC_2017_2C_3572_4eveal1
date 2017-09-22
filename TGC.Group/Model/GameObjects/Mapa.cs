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

namespace TGC.Group.Model.GameObjects{
    public class Mapa {
        public List<TgcMesh> ObjetosMesh = new List<TgcMesh>();
        public TgcSceneLoader Loader;
        private Microsoft.DirectX.Direct3D.Device Device;

        public float scaleXZ = 8f;
        public float scaleY = 1f;
        public int xIni;
        public int zIni;

        public int[,] heightmap;

        public Texture terrainTexture;
        public String MediaDir;

        private int totalVertices;
        private VertexBuffer vbTerrain;

        public Mapa(String mediaDir, int _xIni, int _zIni) {
            Device = D3DDevice.Instance.Device;//Device de DirectX para crear primitivas.
            Loader = new TgcSceneLoader();
            xIni = _xIni;
            zIni = _zIni;

            MediaDir = mediaDir;

            //Path de Heightmap default del terreno y Modifier para cambiarla
            var currentHeightmap = MediaDir + "mapa.jpg";
            var currentTexture = MediaDir + "2.jpg";
            terrainTexture = loadTerrainTexture(Device, currentTexture);

            createHeightMapMesh(Device, currentHeightmap);
            
            ObjectCreator creador = new ObjectCreator(this);
            ObjetosMesh.AddRange(creador.createObjects(50, "Meshes\\Pino\\Pino-TgcScene.xml", 2, xIni, zIni));
            ObjetosMesh.AddRange(creador.createObjects(50, "Meshes\\Pasto\\Pasto-TgcScene.xml", 0, xIni, zIni));
            ObjetosMesh.AddRange(creador.createObjects(50, "Meshes\\Roca\\Roca-TgcScene.xml", 3, xIni, zIni));
        }

        public float getY(float posX, float posZ, Vector3 moveVector) {
            //int xF = (int)Math.Floor(posX / 8);
            //int zF = (int)Math.Floor(posZ / 8);

            //int y1 = heightmap[(int)x, (int)z];

            //return (y1 * scaleY) + 15;
/*
            float p1 = heightmap[xF  , zF  ];
            float p2 = heightmap[xF+1, zF  ];
            float p3 = heightmap[xF  , zF+1];
            float p4 = heightmap[xF+1, zF+1];

            float absX = Math.Abs(moveVector.X);
            float absZ = Math.Abs(moveVector.Z); 

            if (moveVector.X == 0) {
                absX = 0.5f;
            }
            if (moveVector.Z == 0) {
                absZ = 1f;
            }*/
            //float a = (float)((moveVector.X / absX) * (posX-Math.Truncate(posX)));


            //float a = (float)(((() )) * (((moveVector.Z / absZ)) - Math.Truncate(posZ)));
            
            //float deltaY = a * (p1-p4);


            float y = heightmap[(int)posX/8, (int)posZ/8];

            //(p1 +  * (p1 - p2))
              //  + (((float)(posX - Math.Truncate(posX))) * (p1 - p3));
            return (y * scaleY) + 15;



            //float x = posX/8;
            //float z = posZ/8;
            /* int y11 = heightmap[(int)x - 1, (int)z - 1];
            int y12 = heightmap[(int)x - 1, (int)z];
            int y13 = heightmap[(int)x - 1, (int)z + 1];
            int y21 = heightmap[(int)x, (int)z - 1];
            int y22 = heightmap[(int)x, (int)z];
            int y23 = heightmap[(int)x, (int)z + 1];
            int y31 = heightmap[(int)x + 1, (int)z - 1];
            int y32 = heightmap[(int)x + 1, (int)z];
            int y33 = heightmap[(int)x + 1, (int)z + 1];
            */
            //int y1 = heightmap[, (int)Math.Floor(z)];
            //int y2 = heightmap[(int)Math.Ceiling(x), (int)Math.Ceiling(z)];
            //float y = (float) y2 + (float) y1 * (float) (Math.Abs(x - z) - Math.Floor(Math.Abs(x - z)));
            /*
            // int y = heightmap[(int)x, (int)z];
            x x x
            x x x
            x x x*/
        }

        private Texture loadTerrainTexture(Microsoft.DirectX.Direct3D.Device d3dDevice, string path) {
            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            return Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }

        private int[,] loadHeightMap(string path) {
            //Cargar bitmap desde el FileSystem
            var bitmap = (Bitmap)Image.FromFile(path);
            var width = bitmap.Size.Width+1;
            var height = bitmap.Size.Height+1;
            var heightmap = new int[width, height];
            var i = 0;
            var j = 0;
            for (i = 0; i < width-1; i++) {
                for (j = 0; j < height-1; j++) {
                    //Obtener color
                    var pixel = bitmap.GetPixel(j, i);//(j, i) invertido para primero barrer filas y despues columnas
                    //Calcular intensidad en escala de grises
                    var intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = (int)intensity;   
                }
            }
            for (i = 0; i < width; i++){
                heightmap[i, width-1] = heightmap[i, 0];
            }
            for (i = 0; i < width; i++){
                heightmap[width-1, i] = heightmap[0, i];
            }
            
            return heightmap;
        }
        private Vector3 crearVertice(int i, int j){
            return new Vector3((i * scaleXZ) + xIni, heightmap[i, j] * scaleY, (j * scaleXZ) + zIni);
        }
        
        private Vector2 crearTextura(int u, int v){
            return new Vector2(u / (float)32, v / (float)32);
        }

        private void createHeightMapMesh(Microsoft.DirectX.Direct3D.Device d3dDevice, string path) {
            //parsear bitmap y cargar matriz de alturas
            heightmap = loadHeightMap(path);

            //Crear vertexBuffer
            totalVertices = 2 * 3 * heightmap.GetLength(0) * heightmap.GetLength(1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Crear array temporal de vertices
            var dataIdx = 0;
            var data = new CustomVertex.PositionTextured[totalVertices]; 
            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < heightmap.GetLength(0)-1; i++) {
                for (var j = 0; j < heightmap.GetLength(1)-1; j++) {
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

        public void render(){
            D3DDevice.Instance.Device.Transform.World = Matrix.Identity;

            //Render terrain
            D3DDevice.Instance.Device.SetTexture(0, terrainTexture);
            //D3DDevice.Instance.Device.SetTexture(1, null); ???
            D3DDevice.Instance.Device.Material = D3DDevice.DEFAULT_MATERIAL;

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);

            foreach (TgcMesh mesh in ObjetosMesh){
                mesh.render();
            }
        }

        public void dispose(){
            vbTerrain.Dispose();
            terrainTexture.Dispose();

            foreach (TgcMesh mesh in ObjetosMesh){
                mesh.dispose();
            }
        }
    }
}
