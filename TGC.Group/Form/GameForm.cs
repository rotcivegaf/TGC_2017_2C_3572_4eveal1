using System;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Shaders;
using TGC.Core.Sound;
using TGC.Core.Textures;

namespace TGC.Group.Form{
    public partial class GameForm : System.Windows.Forms.Form{
        public GameForm(){
            InitializeComponent();
        }
        
        private TgcExample Modelo { get; set; }
        private bool ApplicationRunning { get; set; }
        private TgcDirectSound DirectSound { get; set; }
        private TgcD3dInput Input { get; set; }

        private void GameForm_Load(object sender, EventArgs e){
            InitRenderLoop();//Inicio el ciclo de Render.

            InitGraphics();//Iniciar graficos.
            Text = Modelo.Name + " - " + Modelo.Description;//Titulo de la ventana principal.
            panel3D.Focus();//Focus panel3D.
            InitRenderLoop();//Inicio el ciclo de Render.
        }

        private void GameForm_FormClosing(object sender, FormClosingEventArgs e){
            if (ApplicationRunning){
                ShutDown();
            }
        }

        public void InitGraphics(){ ///Inicio todos los objetos necesarios para cargar el ejemplo y directx.
            ApplicationRunning = true;//Se inicio la aplicación
            D3DDevice.Instance.InitializeD3DDevice(panel3D);//Inicio Device
            //Inicio inputs
            Input = new TgcD3dInput();
            Input.Initialize(this, panel3D);
            //Inicio sonido
            DirectSound = new TgcDirectSound();
            DirectSound.InitializeD3DDevice(panel3D);
            //Directorio actual de ejecución
            var currentDirectory = Environment.CurrentDirectory + "\\";
            TgcShaders.Instance.loadCommonShaders(currentDirectory + Game.Default.ShadersDirectory);//Cargar shaders del framework
            //Juego a ejecutar, si quisiéramos tener diferentes modelos aquí podemos cambiar la instancia e invocar a otra clase.
            Modelo = new Model.GameModel(currentDirectory + Game.Default.MediaDirectory,
                currentDirectory + Game.Default.ShadersDirectory, this);
            //Cargar juego.
            ExecuteModel();
        }
        
        public void InitRenderLoop(){//Comienzo el loop del juego.
            while (ApplicationRunning){
                if (Modelo != null){//Renderizo si es que hay un ejemplo activo.
                    if (ApplicationActive()){//Solo renderizamos si la aplicacion tiene foco, para no consumir recursos innecesarios.
                        Modelo.Update();
                        Modelo.Render();
                    }else{
                        Thread.Sleep(100);//Si no tenemos el foco, dormir cada tanto para no consumir gran cantidad de CPU.
                    }
                }
                Application.DoEvents();// Process application messages.
            }
        }
        
        public bool ApplicationActive(){//Indica si la aplicacion esta activa. Busca si la ventana principal tiene foco o si alguna de sus hijas tiene.
            if (ContainsFocus){
                return true;
            }
            foreach (var form in OwnedForms){
                if (form.ContainsFocus){
                    return true;
                }
            }
            return false;
        }
        
        public void ExecuteModel(){//Arranca a ejecutar un ejemplo.Para el ejemplo anterior, si hay alguno.
            try{//Ejecutar Init
                Modelo.ResetDefaultConfig();
                Modelo.DirectSound = DirectSound;
                Modelo.Input = Input;
                Modelo.Init();
                panel3D.Focus();
            }catch (Exception e){
                MessageBox.Show(e.Message, "Error en Init() del juego", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public void StopCurrentExample() {//Deja de ejecutar el ejemplo actual
            if (Modelo != null) {
                Modelo.Dispose();
                Modelo = null;
            }
        }
        public void cerrar() {
            Close();
        }

        public void ShutDown(){
            ApplicationRunning = false;
            StopCurrentExample();
            //Liberar Device al finalizar la aplicacion
            D3DDevice.Instance.Dispose();
            TexturesPool.Instance.clearAll();
        }

        private void panel3D_Paint(object sender, PaintEventArgs e){
        }
    }
}
