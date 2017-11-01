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
using TGC.Group.Model.GameObjects;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Core.Geometry;
using TGC.Core.Camara;
using TGC.Group.Model.Camera;

namespace TGC.Group.Model.Optimizacion {
    public class Optimizador {
        public double cosFOV = Math.Cos(75);
        public Mapa mapa;
        public TgcCamera camara;

        public Optimizador(Mapa mapa, TgcCamera camara) {
            this.mapa = mapa;
            this.camara = camara;
        }

        public Vector2 toVersor(Vector3 v1) {
            var modulo = Math.Abs(v1.Length());
            return new Vector2(v1.X / modulo, v1.Y / modulo );
        }

        private bool seVe(TgcCamera camara, Sector sector) {
            var versorCamara = toVersor(camara.LookAt - camara.Position);

            var inicio = mapa.center.X - (3 * mapa.deltaCenter);
            var auxVector = camara.Position - new Vector3((sector.numero.X * mapa.length) + inicio, 0, (sector.numero.Y * mapa.length) + inicio);

            float[] dotEsquinas = {
                Vector2.Dot(versorCamara, toVersor(auxVector)),
                Vector2.Dot(versorCamara, toVersor(auxVector - new Vector3(mapa.length , 0, 0))),
                Vector2.Dot(versorCamara, toVersor(auxVector - new Vector3(0 , 0, mapa.length))),
                Vector2.Dot(versorCamara, toVersor(auxVector - new Vector3(mapa.length , 0, mapa.length)))
            };

            return (dotEsquinas[0] < cosFOV) || (dotEsquinas[1] < cosFOV) || (dotEsquinas[2] < cosFOV) || (dotEsquinas[3] < cosFOV);
        }

        public void renderMap() {
            mapa.render();

            for(int i = 0; i < mapa.sectores.Length; i++) {
                if (seVe(camara, mapa.sectores[i])) {
                    mapa.sectores[i].render();
                }
            }
        }
    }
}

