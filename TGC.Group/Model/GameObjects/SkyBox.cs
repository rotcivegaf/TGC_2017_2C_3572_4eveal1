using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace TGC.Group.Model.GameObjects {
    public class SkyBox: IRenderObject {
        private TGCVector3 center;
        private Vector3 centro000;

        public enum SkyFaces {
            Up = 0,
            Down = 1,
            Front = 2,
            Back = 3,
            Right = 4,
            Left = 5
        }
        public SkyBox() {
            Faces = new TgcMesh[6];
            FaceTextures = new string[6];
            SkyEpsilon = 25f;
            Color = Color.White;
            Center = TGCVector3.Empty;
            Size = new TGCVector3(1000, 1000, 1000);
        }
        public float SkyEpsilon { get; set; }
        public TGCVector3 Size { get; set; }
        public TGCVector3 Center {
            get { return center; }
            set {
                center = value;
                Traslation = TGCMatrix.Translation(center);
            }
        }

        public Color Color { get; set; }
        private TGCMatrix Traslation { get; set; }
        public TgcMesh[] Faces { get; }        
        public string[] FaceTextures { get; }
        
        public bool AlphaBlendEnable {
            get { return Faces[0].AlphaBlendEnable; }
            set {
                foreach (var face in Faces) {
                    face.AlphaBlendEnable = value;
                }
            }
        }

        public void render(Vector3 posicionCamara, Hora hora) {
            var pos = Matrix.Translation(centro000) * Matrix.RotationY(hora.time / 150) * Matrix.Translation(new TGCVector3(posicionCamara));
            
            foreach (var face in Faces) {
                face.Effect.SetValue("factor", hora.toScaleFactor01());
                face.Transform = pos;
                face.render();
            }
        }

        public void dispose() {
            foreach (var face in Faces) {
                face.dispose();
            }
        }

        public void setFaceTexture(SkyFaces face, string texturePath) {
            FaceTextures[(int)face] = texturePath;
        }

        public void Init() {
            for (var i = 0; i < Faces.Length; i++) {
                var m = new Mesh(2, 4, MeshFlags.Managed, TgcSceneLoader.DiffuseMapVertexElements, D3DDevice.Instance.Device);
                var skyFace = (SkyFaces)i;

                using (var vb = m.VertexBuffer) {
                    var data = vb.Lock(0, 0, LockFlags.None);
                    var colorRgb = Color.ToArgb();
                    cargarVertices(skyFace, data, colorRgb);
                    vb.Unlock();
                }
                using (var ib = m.IndexBuffer) {
                    var ibArray = new short[6];
                    cargarIndices(ibArray);
                    ib.SetData(ibArray, 0, LockFlags.None);
                }
                var faceName = Enum.GetName(typeof(SkyFaces), skyFace);
                var faceMesh = new TgcMesh(m, "SkyBox-" + faceName, TgcMesh.MeshRenderType.DIFFUSE_MAP);
                faceMesh.Materials = new[] { D3DDevice.DEFAULT_MATERIAL };
                faceMesh.createBoundingBox();
                faceMesh.Enabled = true;
                var texture = TgcTexture.createTexture(D3DDevice.Instance.Device, FaceTextures[i]);
                faceMesh.DiffuseMaps = new[] { texture };
                Faces[i] = faceMesh;
                centro000 = -new Vector3(Size.X / 2, 0, Size.Z / 2);
            }
        }
        
        private void cargarVertices(SkyFaces face, GraphicsStream data, int color) {
            switch (face) {
                case SkyFaces.Up:
                cargarVerticesUp(data, color);
                break;

                case SkyFaces.Down:
                cargarVerticesDown(data, color);
                break;

                case SkyFaces.Front:
                cargarVerticesFront(data, color);
                break;

                case SkyFaces.Back:
                cargarVerticesBack(data, color);
                break;

                case SkyFaces.Right:
                cargarVerticesRight(data, color);
                break;

                case SkyFaces.Left:
                cargarVerticesLeft(data, color);
                break;
            }
        }

        private void cargarVerticesUp(GraphicsStream data, int color) {
            TgcSceneLoader.DiffuseMapVertex v;
            var n = TGCVector3.Up;

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2 - SkyEpsilon,
                Center.Y + Size.Y / 2,
                Center.Z - Size.Z / 2 - SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 0;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2 - SkyEpsilon,
                Center.Y + Size.Y / 2,
                Center.Z + Size.Z / 2 + SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 0;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2 + SkyEpsilon,
                Center.Y + Size.Y / 2,
                Center.Z + Size.Z / 2 + SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2 + SkyEpsilon,
                Center.Y + Size.Y / 2,
                Center.Z - Size.Z / 2 - SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 1;
            data.Write(v);
        }

        private void cargarVerticesDown(GraphicsStream data, int color) {
            TgcSceneLoader.DiffuseMapVertex v;
            var n = TGCVector3.Down;

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2 - SkyEpsilon,
                Center.Y - Size.Y / 2,
                Center.Z + Size.Z / 2 + SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2 - SkyEpsilon,
                Center.Y - Size.Y / 2,
                Center.Z - Size.Z / 2 - SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2 + SkyEpsilon,
                Center.Y - Size.Y / 2,
                Center.Z - Size.Z / 2 - SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 0;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2 + SkyEpsilon,
                Center.Y - Size.Y / 2,
                Center.Z + Size.Z / 2 + SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 0;
            data.Write(v);
        }

        private void cargarVerticesFront(GraphicsStream data, int color) {
            TgcSceneLoader.DiffuseMapVertex v;
            var n = TGCVector3.Down;

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2 - SkyEpsilon,
                Center.Y + Size.Y / 2 + SkyEpsilon,
                Center.Z + Size.Z / 2
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 0;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2 - SkyEpsilon,
                Center.Y - Size.Y / 2 - SkyEpsilon,
                Center.Z + Size.Z / 2
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2 + SkyEpsilon,
                Center.Y - Size.Y / 2 - SkyEpsilon,
                Center.Z + Size.Z / 2
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2 + SkyEpsilon,
                Center.Y + Size.Y / 2 + SkyEpsilon,
                Center.Z + Size.Z / 2
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 0;
            data.Write(v);
        }

        private void cargarVerticesBack(GraphicsStream data, int color) {
            TgcSceneLoader.DiffuseMapVertex v;
            var n = TGCVector3.Down;

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2 + SkyEpsilon,
                Center.Y + Size.Y / 2 + SkyEpsilon,
                Center.Z - Size.Z / 2
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 0;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2 + SkyEpsilon,
                Center.Y - Size.Y / 2 - SkyEpsilon,
                Center.Z - Size.Z / 2
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2 - SkyEpsilon,
                Center.Y - Size.Y / 2 - SkyEpsilon,
                Center.Z - Size.Z / 2
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2 - SkyEpsilon,
                Center.Y + Size.Y / 2 + SkyEpsilon,
                Center.Z - Size.Z / 2
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 0;
            data.Write(v);
        }

        private void cargarVerticesRight(GraphicsStream data, int color) {
            TgcSceneLoader.DiffuseMapVertex v;
            var n = TGCVector3.Down;

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2,
                Center.Y + Size.Y / 2 + SkyEpsilon,
                Center.Z + Size.Z / 2 + SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 0;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2,
                Center.Y - Size.Y / 2 - SkyEpsilon,
                Center.Z + Size.Z / 2 + SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2,
                Center.Y - Size.Y / 2 - SkyEpsilon,
                Center.Z - Size.Z / 2 - SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X + Size.X / 2,
                Center.Y + Size.Y / 2 + SkyEpsilon,
                Center.Z - Size.Z / 2 - SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 0;
            data.Write(v);
        }

        private void cargarVerticesLeft(GraphicsStream data, int color) {
            TgcSceneLoader.DiffuseMapVertex v;
            var n = TGCVector3.Down;

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2,
                Center.Y + Size.Y / 2 + SkyEpsilon,
                Center.Z - Size.Z / 2 - SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 0;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2,
                Center.Y - Size.Y / 2 - SkyEpsilon,
                Center.Z - Size.Z / 2 - SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 0;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2,
                Center.Y - Size.Y / 2 - SkyEpsilon,
                Center.Z + Size.Z / 2 + SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 1;
            data.Write(v);

            v = new TgcSceneLoader.DiffuseMapVertex();
            v.Position = new TGCVector3(
                Center.X - Size.X / 2,
                Center.Y + Size.Y / 2 + SkyEpsilon,
                Center.Z + Size.Z / 2 + SkyEpsilon
                );
            v.Normal = n;
            v.Color = color;
            v.Tu = 1;
            v.Tv = 0;
            data.Write(v);
        }

        private void cargarIndices(short[] ibArray) {
            var i = 0;
            ibArray[i++] = 0;
            ibArray[i++] = 1;
            ibArray[i++] = 2;
            ibArray[i++] = 0;
            ibArray[i++] = 2;
            ibArray[i++] = 3;
        }

        public void render() {
            throw new NotImplementedException();
        }
    }
}