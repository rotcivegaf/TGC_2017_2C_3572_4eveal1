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
    public class Mapa{
        public TgcSceneLoader Loader;
        private Microsoft.DirectX.Direct3D.Device Device;

        public float scaleXZ;
        public float scaleY = 1f;
        public int[,] heightmap;

        public int MapaLength;
        public Texture terrainTexture;
        public String MediaDir;

        private int totalVertices;
        private VertexBuffer vbTerrain;

        public Mapa(String mediaDir, int mapaLength){
            Device = D3DDevice.Instance.Device;//Device de DirectX para crear primitivas.
            Loader = new TgcSceneLoader();

            MediaDir = mediaDir;
            MapaLength = mapaLength;
            scaleXZ = MapaLength /(64);

            //Path de Heightmap default del terreno y Modifier para cambiarla
            var currentHeightmap = MediaDir + "mapa.jpg";
            var currentTexture = MediaDir + "tierra.jpg";
            terrainTexture = loadTerrainTexture(Device, currentTexture);
            
            
            createHeightMapMesh(Device, currentHeightmap);
        }

        private Texture loadTerrainTexture(Microsoft.DirectX.Direct3D.Device d3dDevice, string path){
            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            return Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }

        private int[,] loadHeightMap(string path){
            //Cargar bitmap desde el FileSystem
            var bitmap = (Bitmap)Image.FromFile(path);
            var width = bitmap.Size.Width;
            var height = bitmap.Size.Height;
            var heightmap = new int[width, height];

            for (var i = 0; i < width; i++){
                for (var j = 0; j < height; j++){
                    //Obtener color
                    var pixel = bitmap.GetPixel(j, i);//(j, i) invertido para primero barrer filas y despues columnas
                    //Calcular intensidad en escala de grises
                    var intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = (int)intensity;
                }
            }

            return heightmap;
        }

        private void createHeightMapMesh(Microsoft.DirectX.Direct3D.Device d3dDevice, string path){
            //parsear bitmap y cargar matriz de alturas
            heightmap = loadHeightMap(path);

            //Crear vertexBuffer
            totalVertices = 2 * 3 * (heightmap.GetLength(0) - 1) * (heightmap.GetLength(1) - 1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Crear array temporal de vertices
            var dataIdx = 0;
            var data = new CustomVertex.PositionTextured[totalVertices];

            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < heightmap.GetLength(0) - 1; i++){
                for (var j = 0; j < heightmap.GetLength(1) - 1; j++){
                    //Crear los cuatro vertices que conforman este cuadrante, aplicando la escala correspondiente
                    var v1 = new Vector3(i * scaleXZ, heightmap[i, j] * scaleY, j * scaleXZ);
                    var v2 = new Vector3(i * scaleXZ, heightmap[i, j + 1] * scaleY, (j + 1) * scaleXZ);
                    var v3 = new Vector3((i + 1) * scaleXZ, heightmap[i + 1, j] * scaleY, j * scaleXZ);
                    var v4 = new Vector3((i + 1) * scaleXZ, heightmap[i + 1, j + 1] * scaleY, (j + 1) * scaleXZ);

                    //Crear las coordenadas de textura para los cuatro vertices del cuadrante
                    var t1 = new Vector2(i / (float)heightmap.GetLength(0), j / (float)heightmap.GetLength(1));
                    var t2 = new Vector2(i / (float)heightmap.GetLength(0), (j + 1) / (float)heightmap.GetLength(1));
                    var t3 = new Vector2((i + 1) / (float)heightmap.GetLength(0), j / (float)heightmap.GetLength(1));
                    var t4 = new Vector2((i + 1) / (float)heightmap.GetLength(0), (j + 1) / (float)heightmap.GetLength(1));

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
            D3DDevice.Instance.Device.SetTexture(1, null);
            D3DDevice.Instance.Device.Material = D3DDevice.DEFAULT_MATERIAL;

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerrain, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
        }

        public void dispose(){
            vbTerrain.Dispose();
            terrainTexture.Dispose();
        }
    }
}
