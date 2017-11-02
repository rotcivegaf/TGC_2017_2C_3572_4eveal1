﻿using System;
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
using TGC.Core.Terrain;
using TGC.Core.Collision;
using TGC.Group.Model.Camera;
using TGC.Group.Model.GameObject;

namespace TGC.Group.Model.GameObjects{
    public class Mapa {
        public TgcSkyBox SkyBox { get; set; }

        public Vector2 center;
        public TgcSceneLoader Loader;
        public Microsoft.DirectX.Direct3D.Device Device;

        public Sector[] sectores;

        public float scaleXZ = 16f;
        public float scaleY = 0.4f;
        public float deltaCenter;

        public int[,] heightmap;
        public float length;

        public Texture terrainTexture;
        public String MediaDir;

        public int totalVertices;

        public Mapa(String mediaDir) {
            Device = D3DDevice.Instance.Device;//Device de DirectX para crear primitivas.
            Loader = new TgcSceneLoader();

            MediaDir = mediaDir;
            //Path de Heightmap default del terreno y Modifier para cambiarla
            var currentHeightmap = MediaDir + "b.jpg";
            var currentTexture = MediaDir + "2.jpg";
            terrainTexture = loadTerrainTexture(Device, currentTexture);
            heightmap = loadHeightMap(currentHeightmap);
            totalVertices = 2 * 3 * heightmap.GetLength(0) * heightmap.GetLength(1);

            length = heightmap.GetLength(0) * scaleXZ;

            crearSectores();
           
            var aux = (1.5f) * length;
            center = new Vector2(aux, aux);
            deltaCenter = length / 2;

            CreateSkyBox();
        }

        private void CreateSkyBox() {
            //Crear SkyBox
            SkyBox = new TgcSkyBox();
            SkyBox.Center = new Vector3(center.X/2, 0, center.Y/2);
            
            SkyBox.Size = new Vector3(length*1.5f, length*3, length*1.5f);

            var texturesPath = MediaDir + "\\SkyBox\\";
            String imgNameRoot = "clouds";
            String imgExtension = "png";
            //Configurar las texturas para cada una de las 6 caras
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + imgNameRoot + "_up." + imgExtension);
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + imgNameRoot + "_dn." + imgExtension);
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + imgNameRoot + "_lf." + imgExtension);
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + imgNameRoot + "_rt." + imgExtension);
            //Hay veces es necesario invertir las texturas Front y Back si se pasa de un sistema RightHanded a uno LeftHanded
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + imgNameRoot + "_bk." + imgExtension);
            SkyBox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + imgNameRoot + "_ft." + imgExtension);
            SkyBox.SkyEpsilon = 25f;
            //Inicializa todos los valores para crear el SkyBox
            SkyBox.Init();
        }

        public void crearSectores() {
            ObjectCreator creador = new ObjectCreator(this);
            sectores = new Sector[9];

            for (int i = 0; i < 3; i++) {
                for (int j = 0; j < 3; j++) {
                    sectores[i * 3 + j] = new Sector(new Vector2(i, j), this);
                    sectores[i * 3 + j].crearObj(creador);
                }
            }

            List<Sector> tmpS = new List<Sector>(sectores);
        }

        public float getY(float posX, float posZ) {
            posX = Math.Abs(posX) % ((heightmap.GetLength(0) - 1) * scaleXZ);
            posZ = Math.Abs(posZ) % ((heightmap.GetLength(0) - 1) * scaleXZ);
            
            int posU = (int)(posX / scaleXZ);
            int posV = (int)(posZ / scaleXZ);

            float decimalU = (posX / scaleXZ) - posU;
            float decimalV = (posZ / scaleXZ) - posV;

            float v0 = heightmap[posU, posV] * scaleY;
            float v1 = heightmap[posU + 1, posV] * scaleY;
            float v2 = heightmap[posU, posV + 1] * scaleY;

            float deltaX = (v1 - v0) * decimalU;
            float deltaZ = (v2 - v0) * decimalV;

            return v0 + deltaX + deltaZ + 10;
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
            var width = bitmap.Size.Width + 1;
            var height = bitmap.Size.Height + 1;
            var heightmap = new int[width, height];
            var i = 0;
            var j = 0;
            for (i = 0; i < width - 1; i++) {
                for (j = 0; j < height - 1; j++) {
                    //Obtener color
                    var pixel = bitmap.GetPixel(j, i);//(j, i) invertido para primero barrer filas y despues columnas
                    //Calcular intensidad en escala de grises
                    var intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = (int)intensity;
                }
            }
            for (i = 0; i < width; i++) {
                heightmap[i, width - 1] = heightmap[i, 0];
            }
            for (i = 0; i < width; i++) {
                heightmap[width - 1, i] = heightmap[0, i];
            }

            return heightmap;
        }

        public void render() {
            D3DDevice.Instance.Device.Transform.World = Matrix.Identity;
            SkyBox.render();
        }

        public void moverSectores(int dir) {
            var delta = (heightmap.GetLength(0) - 1) * scaleXZ * 3;
            var deltaX = delta;
            var deltaY = delta;
            int[] s = null;
            int[] np = null;

            switch (dir) {
                case 0:
                    deltaY = 0;
                    s = new int[3] { 0, 1, 2 };
                    np = new int[9] { 3, 4, 5, 6, 7, 8, 0, 1, 2 };
                    break;
                case 1:
                    deltaX *= -1;
                    deltaY = 0;
                    s = new int[3] { 6, 7, 8 };
                    np = new int[9] { 6, 7, 8, 0, 1, 2, 3, 4, 5 };
                    break;
                case 2:
                    deltaX = 0;
                    s = new int[3] { 0, 3, 6 };
                    np = new int[9] { 1, 2, 0, 4, 5, 3, 7, 8, 6 };
                    break;
                case 3:
                    deltaX = 0;
                    deltaY *= -1;
                    s = new int[3] { 2, 5, 8 };
                    np = new int[9] { 2, 0, 1, 5, 3, 4, 8, 6, 7 };
                    break;
            }
            center += new Vector2(deltaX / 3, deltaY / 3);
            sectores[s[0]].mover(deltaX, deltaY);
            sectores[s[1]].mover(deltaX, deltaY);
            sectores[s[2]].mover(deltaX, deltaY);
            acomodarNumeroSectores(np);
        }

        public void acomodarNumeroSectores(int[] newPos) {
            List<Sector> tmpS = new List<Sector>(sectores);

            for (int i = 0; i < newPos.Length; i++) {
                sectores[i] = tmpS[newPos[i]];
                sectores[i].numero = new Vector2((int)i / 3,  i % 3 );
            }
        }

        public void testCollisions(FPCamera camara, Personaje personaje) {
            if (personaje.isMoving) {
                foreach (TgcMesh mesh in sectores[4].ObjetosMesh) {
                    camara.CameraBox.Position = camara.Position;
                    if (TgcCollisionUtils.testAABBAABB(camara.CameraBox.BoundingBox, mesh.BoundingBox)) {
                        camara.Collisioned = true;
                        return;
                    } else {
                        camara.Collisioned = false;
                    }
                }
            }
        }

        public void dispose(){
            for(int i = 0; i < sectores.Length; i++)
                sectores[i].dispose();
        }
    }
}