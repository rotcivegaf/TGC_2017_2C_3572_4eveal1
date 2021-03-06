﻿using System;
using System.Collections.Generic;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.Collision;
using TGC.Group.Model.Camera;
using TGC.Group.Model.GameObject;

namespace TGC.Group.Model.GameObjects{
    public class Mapa {
        public SkyBox SkyBoxDia1 { get; set; }
        public SkyBox SkyBoxDia2 { get; set; }
        public SkyBox SkyBoxNoche { get; set; }

        public Vector2 center;
        public TgcSceneLoader Loader;
        public Device Device;

        public Sector[] sectores;
        
        public float scaleXZ = 16f;
        public float scaleY = 0.4f;
        public float deltaCenter;

        public int[,] heightmap;
        public float length;

        public Texture terrainTexture;
        public String MediaDir;
        public int totalVertices;

        public String shaderDir;
        public bool dia = true;

        public Mapa(String mediaDir, String shaderDir) {
            Device = D3DDevice.Instance.Device;//Device de DirectX para crear primitivas.
            Loader = new TgcSceneLoader();
            this.shaderDir = shaderDir;
            MediaDir = mediaDir;
            //Path de Heightmap default del terreno y Modifier para cambiarla
            var currentHeightmap = MediaDir + "heightmap.jpg";
            var currentTexture = MediaDir + "texture.jpg";
            terrainTexture = loadTerrainTexture(Device, currentTexture);
            heightmap = loadHeightMap(currentHeightmap);
            totalVertices = 2 * 3 * heightmap.GetLength(0) * heightmap.GetLength(1);
            length = heightmap.GetLength(0) * scaleXZ;
            var aux = (1.5f) * length;
            center = new Vector2(aux, aux);
            deltaCenter = length / 2;
            crearSectores();
            createSkyBoxs();
        }

        private void createSkyBoxs() {
            SkyBoxDia1 = new SkyBox(length, center, 5, MediaDir + "\\SkyBox\\", shaderDir + "SkyBoxShader.fx");
            SkyBoxDia2 = new SkyBox(length, center, 5, MediaDir + "\\SkyBox2\\", shaderDir + "SkyBoxShader.fx");
            SkyBoxNoche = new SkyBox(length, center, 5, MediaDir + "\\SkyBox3\\", shaderDir + "SkyBoxShader.fx");
        }

        public void crearSectores() {
            ObjectCreator creador = new ObjectCreator(this);
            sectores = new Sector[9];

            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    sectores[i * 3 + j] = new Sector(new Vector2(i, j), this, creador);

            List<Sector> tmpS = new List<Sector>(sectores);
        }

        public float getY(float posX, float posZ) {
            var lengthHM = heightmap.GetLength(0);
            var lengthHM1 = heightmap.GetLength(0) - 1;

            posX = posX % (lengthHM1 * scaleXZ);
            posZ = posZ % (lengthHM1 * scaleXZ);

            int posU;
            if (posX > 0) {
                posU = (int)(posX / scaleXZ);
            } else {
                posX = Math.Abs(posX);
                posU = lengthHM1 - 1 - (int)(posX / scaleXZ);
                posX = (lengthHM1 * scaleXZ) - posX;
            }

            int posV;
            if (posZ > 0) {
                posV = (int)(posZ / scaleXZ);
            } else {
                posZ = Math.Abs(posZ);
                posV = lengthHM1 - 1 - (int)(posZ / scaleXZ);
                posZ = (lengthHM1 * scaleXZ) - posZ;
            }


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
            return Texture.FromBitmap(d3dDevice, b, Usage.AutoGenerateMipMap, Pool.Managed);
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

        public void render(Vector3 posicionCamara, Hora hora) {
            D3DDevice.Instance.Device.Transform.World = Matrix.Identity;
            
            var aux = hora.to24();

            if (aux > 5 && aux < 7) {
                var factor = (hora.to24() - 5) % 3;
                if (hora.pasoANoche) {
                    SkyBoxDia1.render(posicionCamara, factor, hora.time);
                } else {
                    SkyBoxDia2.render(posicionCamara, factor, hora.time);
                }
                SkyBoxNoche.render(posicionCamara, 1-factor, hora.time);
                dia = true;
                return;
            }
            if (aux > 17 && aux < 19) {
                var factor = (hora.to24() - 17) % 3;
                if (hora.pasoANoche) {
                    SkyBoxDia1.render(posicionCamara, 1 - factor, hora.time);
                } else {
                    SkyBoxDia2.render(posicionCamara, 1 - factor, hora.time);
                }
                SkyBoxNoche.render(posicionCamara, factor, hora.time);
                dia = false;
                return;
            }
            if (dia) {
                if (hora.pasoANoche) {
                    SkyBoxDia1.render(posicionCamara, 1, hora.time);
                } else {
                    SkyBoxDia2.render(posicionCamara, 1, hora.time);
                }
                SkyBoxNoche.render(posicionCamara, 0, hora.time);
            } else {
                if (hora.pasoANoche) {
                    SkyBoxDia1.render(posicionCamara, 0, hora.time);
                } else {
                    SkyBoxDia2.render(posicionCamara, 0, hora.time);
                }
                SkyBoxNoche.render(posicionCamara, 1, hora.time);
            }
        }

        public void moverSectores(FPCamera camara) {
            var delta = (heightmap.GetLength(0) - 1) * scaleXZ * 3;
            var deltaX = delta;
            var deltaY = delta;
            int[] s = null;
            int[] np = null;

            if (camara.Position.X > center.X + deltaCenter) {// izquierda
                deltaY = 0;
                s = new int[3] { 0, 1, 2 };
                np = new int[9] { 3, 4, 5, 6, 7, 8, 0, 1, 2 };
            } else {
                if (camara.Position.X < center.X - deltaCenter) {// derecha
                    deltaX *= -1;
                    deltaY = 0;
                    s = new int[3] { 6, 7, 8 };
                    np = new int[9] { 6, 7, 8, 0, 1, 2, 3, 4, 5 };
                } else {
                    if (camara.Position.Z > center.Y + deltaCenter) {// abajo
                        deltaX = 0;
                        s = new int[3] { 0, 3, 6 };
                        np = new int[9] { 1, 2, 0, 4, 5, 3, 7, 8, 6 };
                    } else {
                        if (camara.Position.Z < center.Y - deltaCenter) {// arriba
                            deltaX = 0;
                            deltaY *= -1;
                            s = new int[3] { 2, 5, 8 };
                            np = new int[9] { 2, 0, 1, 5, 3, 4, 8, 6, 7 };
                        } else {
                            return;
                        }
                    }
                }
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
                    if (mesh.Name == "agua" || mesh.Name == "Pasto" || mesh.Name == "Planta" || mesh.Name == "Planta2" || mesh.Name == "Planta3") {
                        camara.Collisioned = false;
                    } else {
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
        }

        public void dispose(){
            for(int i = 0; i < sectores.Length; i++)
                sectores[i].dispose();
        }
    }
}