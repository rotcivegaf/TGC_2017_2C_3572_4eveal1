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
using TGC.Group.Form;

namespace TGC.Group.Model{
    public class GameModel : TgcExample{
        public List<Mapa> mapas = new List<Mapa>();
        GameForm formPrincipal;

        public GameModel(string mediaDir, string shadersDir, GameForm form) : base(mediaDir, shadersDir){
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            formPrincipal = form;
        }
        
        ///Se llama una sola vez, al principio cuando se ejecuta el ejemplo. Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo procesamiento que podemos pre calcular para nuestro juego.
        public override void Init(){
            crearMapas();
            if (formPrincipal.Mode == 0){
                Camara = new TgcFpsCamera(new Vector3(2 * 4096, 200f, 2 * 4096), 500f, 500f, Input);
            } else{
                //Camara = new TgcFpsCamera(Input, 2 * 4096, -(2 * 4096), (2 * 4096), -(2 * 4096));
            }
        }

        public void crearMapas(){
            Mapa mapa;

            for (int i = 0; i <= 4; i++){
                for (int j = 0; j <= 4; j++){
                    mapa = new Mapa(MediaDir, (i * 4096), j * 4096);
                    mapas.Add(mapa);
                }
            }
           
        }

        //Se llama en cada frame. Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones ante ellas.
        public override void Update(){
            PreUpdate();
            detectUserInput();
        }
        ///Se llama cada vez que hay que refrescar la pantalla.
        public override void Render() {
            PreRender(); //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.

            DrawText.drawText("Camera pos: " + Core.Utils.TgcParserUtils.printVector3(Camara.Position), 5, 20, System.Drawing.Color.Red);
            DrawText.drawText("Camera LookAt: " + Core.Utils.TgcParserUtils.printVector3(Camara.LookAt), 5, 40, System.Drawing.Color.Red);

            foreach (Mapa mapa in mapas){
                mapa.render();
            }

            PostRender();//Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
        }
        ///Se llama cuando termina la ejecución del ejemplo. Hacer Dispose() de todos los objetos creados. Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        public override void Dispose(){
            foreach (Mapa mapa in mapas){
                mapa.dispose();
            }
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