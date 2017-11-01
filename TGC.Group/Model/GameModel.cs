using System;
using System.Collections.Generic;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Textures;
using TGC.Core.Utils;
using TGC.Core.SceneLoader;

using TGC.Group.Model.GameObjects;
using TGC.Group.Model.Camera;
using TGC.Group.Form;
using TGC.Group.Model.Optimizacion;

namespace TGC.Group.Model{
    public class GameModel : TgcExample{
        public Mapa mapa;
        public Optimizador optimizador;
        GameForm formPrincipal;

        public GameModel(string mediaDir, string shadersDir, GameForm form) : base(mediaDir, shadersDir){
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            formPrincipal = form;
        }
        
        ///Se llama una sola vez, al principio cuando se ejecuta el ejemplo. Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo procesamiento que podemos pre calcular para nuestro juego.
        public override void Init(){
            mapa = new Mapa(MediaDir);

            var posX = mapa.center.X;
            var posZ = mapa.center.Y;
            Camara = new FPCamera(new Vector3(posX, mapa.getY(posX, posZ), posZ), 200f, 500f, Input, mapa);
            optimizador = new Optimizador(mapa, Camara);
        }

        private void moverMapas() {
            if (Camara.Position.X > mapa.center.X + mapa.deltaCenter) {// izquierda
                mapa.moverSectores(0);
            } else {
                if (Camara.Position.X < mapa.center.X - mapa.deltaCenter) {// derecha
                    mapa.moverSectores(1);
                } else {
                    if (Camara.Position.Z > mapa.center.Y + mapa.deltaCenter) {// abajo
                        mapa.moverSectores(2);
                    } else {
                        if (Camara.Position.Z < mapa.center.Y - mapa.deltaCenter) {// arriba
                            mapa.moverSectores(3);
                        }
                    }
                }
            }
        }


        //Se llama en cada frame. Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones ante ellas.
        public override void Update(){
            PreUpdate();

            moverMapas();
            
            detectUserInput();
        }
        ///Se llama cada vez que hay que refrescar la pantalla.
        public override void Render() {
            PreRender(); //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.

            DrawText.drawText("Camera pos: " + Core.Utils.TgcParserUtils.printVector3(Camara.Position), 15, 20, System.Drawing.Color.Red);
            DrawText.drawText("Camera LookAt: " + Core.Utils.TgcParserUtils.printVector3(Camara.LookAt - Camara.Position), 15, 40, System.Drawing.Color.Red);
            DrawText.drawText("" + mapa.heightmap.GetLength(0), 15, 60, System.Drawing.Color.Red);

            optimizador.renderMap();

            mapa.SkyBox.Center = Camara.Position + new Vector3(-mapa.SkyBox.Size.X / 2, 0, -mapa.SkyBox.Size.Z / 2);

            PostRender();//Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
        }
        ///Se llama cuando termina la ejecución del ejemplo. Hacer Dispose() de todos los objetos creados. Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        public override void Dispose(){
            mapa.dispose();
        }

        private void detectUserInput(){
            if (Input.keyPressed(Key.E)){
                if (formPrincipal.inventarioForm.Visible == true)
                    formPrincipal.inventarioForm.Visible = false;
                else
                    formPrincipal.inventarioForm.Visible = true;
            }
        }
    }
}
