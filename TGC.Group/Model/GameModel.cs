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

namespace TGC.Group.Model{
    public class GameModel: TgcExample {
        public bool AUX;
        public Mapa mapa;
        public Optimizador optimizador;
        public Personaje personaje;
        GameForm formPrincipal;
        public FPCamera miCamara;
        public Menu menu;
        public bool gameStart = false;
        public GUI gui;
        private TgcPickingRay pickingRay;

        public Key lastKey;
        public float tiempoAccion;

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

            var OC = new ObjectCreator(mapa);
            var auxV3 = new Vector3(posX, mapa.getY(posX, posZ), posZ);
            gui = new GUI(personaje, auxV3, MediaDir);
            menu = new Menu(OC, auxV3);
            pickingRay = new TgcPickingRay(Input);
        }

        private void moverMapas() {
            if (miCamara.Position.X > mapa.center.X + mapa.deltaCenter) {// izquierda
                mapa.moverSectores(0);
            } else {
                if (miCamara.Position.X < mapa.center.X - mapa.deltaCenter) {// derecha
                    mapa.moverSectores(1);
                } else {
                    if (miCamara.Position.Z > mapa.center.Y + mapa.deltaCenter) {// abajo
                        mapa.moverSectores(2);
                    } else {
                        if (miCamara.Position.Z < mapa.center.Y - mapa.deltaCenter) {// arriba
                            mapa.moverSectores(3);
                        }
                    }
                }
            }
        }


        //Se llama en cada frame. Se debe escribir toda la lógica de computo del modelo, así como también verificar entradas del usuario y reacciones ante ellas.
        public override void Update() {
            PreUpdate();
            if (!gameStart) {
                switch (menu.seleccionar(Input)) {
                    case 0:// start game
                    gameStart = true;
                    miCamara.gameStart = true;
                    miCamara.LockCam = !miCamara.LockCam;
                    break;
                    case 1:// opciones

                    break;
                    case 2:// quit
                    this.Dispose();
                    formPrincipal.Close();
                    return;
                }
            } else {
                moverMapas();
                mapa.testCollisions(miCamara, personaje);
                testPersonaje();
            }
            if (personaje.hambre <= 0 && personaje.sed <= 0)
                gameStart = false;
        }
        ///Se llama cada vez que hay que refrescar la pantalla.
        public override void Render() {
            PreRender(); //Inicio el render de la escena, para ejemplos simples. Cuando tenemos postprocesado o shaders es mejor realizar las operaciones según nuestra conveniencia.

            DrawText.drawText("Camera pos: " + Core.Utils.TgcParserUtils.printVector3(miCamara.Position), 15, 20, System.Drawing.Color.Red);
            DrawText.drawText("Camera LookAt: " + Core.Utils.TgcParserUtils.printVector3(miCamara.LookAt - miCamara.Position), 15, 40, System.Drawing.Color.Red);
            DrawText.drawText("Camera LookAt: " + AUX, 15, 60, System.Drawing.Color.Red);

            optimizador.renderMap();

            mapa.SkyBox.Center = miCamara.Position + new Vector3(-mapa.SkyBox.Size.X / 2, 0, -mapa.SkyBox.Size.Z / 2);
            //miCamara.CameraBox.BoundingBox.render();
            gui.render(DrawText, formPrincipal);

            if (!gameStart)
                menu.render();

            PostRender();//Finaliza el render y presenta en pantalla, al igual que el preRender se debe para casos puntuales es mejor utilizar a mano las operaciones de EndScene y PresentScene
        }
        ///Se llama cuando termina la ejecución del ejemplo. Hacer Dispose() de todos los objetos creados. Es muy importante liberar los recursos, sobretodo los gráficos ya que quedan bloqueados en el device de video.
        public override void Dispose() {
            mapa.dispose();
        }

        public void testPersonaje() {
            tiempoAccion += ElapsedTime;

            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT) && tiempoAccion > 0.2f) {
                pickingRay.updateRay();
                mapa.sectores[4].testPicking(pickingRay);
                tiempoAccion = 0;
            }
            if (Input.keyDown(Key.D1) && tiempoAccion > 0.2f) {
                personaje.beber();
                lastKey = Key.D1;
                tiempoAccion = 0;
            }
            if (Input.keyDown(Key.D2) && tiempoAccion > 0.2f) {
                personaje.comer();
                lastKey = Key.D2;
                tiempoAccion = 0;
            }
        }
    }
}
