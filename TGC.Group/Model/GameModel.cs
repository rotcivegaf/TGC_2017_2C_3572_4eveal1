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
using TGC.Core.Shaders;
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

        TgcTexture alarmTexture;
        Microsoft.DirectX.Direct3D.Effect effect;
        private Surface pOldRT;
        private Surface pOldDS;
        private Texture renderTarget2D;
        private Surface depthStencil;
        private VertexBuffer screenQuadVB;

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

            var OC = new ObjectCreator(mapa);
            var auxV3 = new Vector3(posX, mapa.getY(posX, posZ), posZ);
            gui = new GUI(personaje, auxV3, MediaDir);
            menu = new Menu(OC, auxV3);
            pickingRay = new TgcPickingRay(Input);

            crearEfectoAlarma();
        }

        private void crearEfectoAlarma() {
            CustomVertex.PositionTextured[] screenQuadVertices ={
                new CustomVertex.PositionTextured(-1, 1, 1, 0, 0),
                new CustomVertex.PositionTextured(1, 1, 1, 1, 0),
                new CustomVertex.PositionTextured(-1, -1, 1, 0, 1),
                new CustomVertex.PositionTextured(1, -1, 1, 1, 1)
            };
            //vertex buffer de los triangulos
            screenQuadVB = new VertexBuffer(typeof(CustomVertex.PositionTextured),
                4, D3DDevice.Instance.Device, Usage.Dynamic | Usage.WriteOnly,
                CustomVertex.PositionTextured.Format, Pool.Default);
            screenQuadVB.SetData(screenQuadVertices, 0, LockFlags.None);

            renderTarget2D = new Texture(D3DDevice.Instance.Device,
                D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth
                , D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight, 1, Usage.RenderTarget,
                Format.X8R8G8B8, Pool.Default);

            depthStencil =
                D3DDevice.Instance.Device.CreateDepthStencilSurface(
                    D3DDevice.Instance.Device.PresentationParameters.BackBufferWidth,
                    D3DDevice.Instance.Device.PresentationParameters.BackBufferHeight,
                    DepthFormat.D24S8, MultiSampleType.None, 0, true);

            effect = TgcShaders.loadEffect(ShadersDir + "PostProcess.fx");

            //Configurar Technique dentro del shader
            effect.Technique = "AlarmaTechnique";

            //Cargar textura que se va a dibujar arriba de la escena del Render Target
            alarmTexture = TgcTexture.createTexture(D3DDevice.Instance.Device, MediaDir + "Textures\\efecto_alarma.png");

            // SkyBox: Se cambia el valor por defecto del farplane para evitar cliping de farplane.
            D3DDevice.Instance.Device.Transform.Projection =
                Matrix.PerspectiveFovLH(D3DDevice.Instance.FieldOfView,
                    D3DDevice.Instance.AspectRatio,
                    D3DDevice.Instance.ZNearPlaneDistance,
                    D3DDevice.Instance.ZFarPlaneDistance * 2560f);
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
            mapa.update(miCamara.Position, hora.toScaleFactor01());
        } 
        ///Se llama cada vez que hay que refrescar la pantalla.
        public override void Render() {
            ClearTextures();

            pOldRT = D3DDevice.Instance.Device.GetRenderTarget(0);
            pOldDS = D3DDevice.Instance.Device.DepthStencilSurface;
            var pSurf = renderTarget2D.GetSurfaceLevel(0);
            D3DDevice.Instance.Device.SetRenderTarget(0, pSurf);

            D3DDevice.Instance.Device.DepthStencilSurface = depthStencil;

            D3DDevice.Instance.Device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

            drawSceneToRenderTarget(D3DDevice.Instance.Device);
            pSurf.Dispose();

            D3DDevice.Instance.Device.SetRenderTarget(0, pOldRT);
            D3DDevice.Instance.Device.DepthStencilSurface = pOldDS;
                        
            drawNigthPostProcess(D3DDevice.Instance.Device);

            drawLastProcess(D3DDevice.Instance.Device);

            D3DDevice.Instance.Device.Present();
        }

        private void drawLastProcess(Microsoft.DirectX.Direct3D.Device d3dDevice) {
            d3dDevice.BeginScene();

            //DrawText.drawText("Camera pos: " + Core.Utils.TgcParserUtils.printVector3(miCamara.Position), 15, 20, System.Drawing.Color.Red);
            //DrawText.drawText("Camera LookAt: " + Core.Utils.TgcParserUtils.printVector3(miCamara.LookAt - miCamara.Position), 15, 40, System.Drawing.Color.Red);
            //DrawText.drawText("Camera LookAt: " + hora.to12(), 15, 60, System.Drawing.Color.Red);

            gui.render(DrawText, formPrincipal);

            d3dDevice.EndScene();
        }

        private void drawNigthPostProcess(Microsoft.DirectX.Direct3D.Device d3dDevice) {
            d3dDevice.BeginScene();

            d3dDevice.VertexFormat = CustomVertex.PositionTextured.Format;
            d3dDevice.SetStreamSource(0, screenQuadVB, 0);

            effect.Technique = "OscurecerTechnique";
            effect.SetValue("scaleFactor", hora.toScaleFactor());
            effect.SetValue("scaleFactor", 0);

            effect.SetValue("render_target2D", renderTarget2D);

            d3dDevice.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
            effect.Begin(FX.None);
            effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 2);
            effect.EndPass();
            effect.End();

            RenderFPS();
            RenderAxis();

            d3dDevice.EndScene();

            if (quit) {
                this.formPrincipal.ApplicationRunning = false;

                this.formPrincipal.StopCurrentExample();

                //Liberar Device al finalizar la aplicacion
                D3DDevice.Instance.Dispose();
                TexturesPool.Instance.clearAll();
                formPrincipal.Close();
            }
        }
        
        public void drawSceneToRenderTarget(Microsoft.DirectX.Direct3D.Device d3dDevice) {
            d3dDevice.BeginScene();
            optimizador.renderMap(ElapsedTime, hora);
            
            //miCamara.CameraBox.BoundingBox.render();

            if (!gameStart)
                menu.render();

            d3dDevice.EndScene();
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
