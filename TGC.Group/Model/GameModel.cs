using Microsoft.DirectX;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Group.Model.GameObjects;
using TGC.Group.Model.Camera;
using TGC.Group.Form;
using TGC.Group.Model.Optimizacion;
using TGC.Group.Model.GameObject;
using TGC.Group.Model._2D;
using TGC.Core.Geometry;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Direct3D;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Textures;

namespace TGC.Group.Model{
    public class GameModel: TgcExample {
        public Hora hora = new Hora(12);
        public Mapa mapa;
        public Optimizador optimizador;
        public Personaje personaje;
        GameForm formPrincipal;
        public FPCamera miCamara;
        public Menu menu;
        public bool gameStart = false;
        public GUI gui;
        private TgcPickingRay pickingRay;

        public float tiempoAccion;

        private bool quit = false;

        public GameModel(string mediaDir, string shadersDir, GameForm form) : base(mediaDir, shadersDir) {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            formPrincipal = form;
        }

        ///Se llama una sola vez, al principio cuando se ejecuta el ejemplo. Escribir aquí todo el código de inicialización: cargar modelos, texturas, estructuras de optimización, todo procesamiento que podemos pre calcular para nuestro juego.
        public override void Init() {
            personaje = new Personaje();
            mapa = new Mapa(MediaDir, ShadersDir);

            var posX = mapa.center.X;
            var posZ = mapa.center.Y;
            miCamara = new FPCamera(new Vector3(posX, mapa.getY(posX, posZ), posZ), Input, mapa, personaje, gameStart);
            Camara = miCamara;
            optimizador = new Optimizador(mapa, miCamara);

            var auxV3 = new Vector3(posX, mapa.getY(posX, posZ), posZ);
            gui = new GUI(personaje, auxV3, MediaDir);
            menu = new Menu(new ObjectCreator(mapa), auxV3);
            pickingRay = new TgcPickingRay(Input);

        }

        //Se llama en cada frame. Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones ante ellas.
        public override void Update() {
            hora.updateTime(ElapsedTime);
            personaje.updateTemp(hora.toScaleFactor());
            PreUpdate();
            if (!gameStart) {
                switch (menu.seleccionar(Input)) {
                    case 0:// start game
                    gameStart = true;
                    miCamara.gameStart = true;
                    miCamara.LockCam = !miCamara.LockCam;
                    break;
                    case 1:// quit
                    quit = true;
                    return;
                }
                return;
            } else {
                mapa.moverSectores(miCamara);
                mapa.testCollisions(miCamara, personaje);
                testPersonaje();
            }
            if (personaje.hambre <= 0 && personaje.sed <= 0) {
                restartGame();
            }
        } 
        ///Se llama cada vez que hay que refrescar la pantalla.
        public override void Render() {
            BeginRenderScene();
            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            ClearTextures();
            optimizador.renderMap(ElapsedTime, hora);

            if (!gameStart)
                menu.render();

            gui.render(DrawText);

            if (quit) {
                formPrincipal.ApplicationRunning = false;
                formPrincipal.StopCurrentExample();

                //Liberar Device al finalizar la aplicacion
                D3DDevice.Instance.Dispose();
                TexturesPool.Instance.clearAll();
                formPrincipal.Close();
            }

            //DrawText.drawText("Camera pos: " + Core.Utils.TgcParserUtils.printVector3(miCamara.Position), 15, 20, System.Drawing.Color.Red);
            //DrawText.drawText("Camera LookAt: " + Core.Utils.TgcParserUtils.printVector3(miCamara.LookAt - miCamara.Position), 15, 40, System.Drawing.Color.Red);
            //DrawText.drawText("" + hora.toScaleFactor(), 15, 60, System.Drawing.Color.Red);
            

            PostRender();
        }
        
        public override void Dispose() {
            mapa.dispose();
        }

        private void restartGame() {
            personaje.sed = 100;
            personaje.hambre = 100;
            personaje.cansancio = 100;
            personaje.temperatura = 50;

            gameStart = false;
            miCamara.gameStart = false;
            miCamara.LockCam = !miCamara.LockCam;
        }

        public void testPersonaje() {
            tiempoAccion += ElapsedTime;

            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT) && tiempoAccion > 0.2f) {
                pickingRay.updateRay();
                mapa.sectores[4].testPicking(pickingRay, personaje);
                tiempoAccion = 0;
            }
            if (Input.keyDown(Key.D1) && tiempoAccion > 0.2f) {
                personaje.comer();
                tiempoAccion = 0;
            }
        }
    }
}
