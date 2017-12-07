using System;
using Microsoft.DirectX;
using TGC.Group.Model.GameObjects;
using TGC.Core.Camara;

namespace TGC.Group.Model.Optimizacion {
    public class Optimizador {
        public double cosFOV = Math.Cos(75);
        public Mapa mapa;
        public TgcCamera camara;
        public float time = 0;

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

        public void renderMap(float ElapsedTime, Hora hora) {
            mapa.render();
            time += ElapsedTime;

            for(int i = 0; i < mapa.sectores.Length; i++) {
                if (seVe(camara, mapa.sectores[i])) {
                    mapa.sectores[i].render(time, hora);
                }
            }
        }
    }
}

