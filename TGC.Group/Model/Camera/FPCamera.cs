using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Camara;
using System.Drawing;
using Microsoft.DirectX;
using TGC.Core.Input;
using TGC.Core.Direct3D;
using TGC.Core.Utils;
using System.Windows.Forms;
using Microsoft.DirectX.DirectInput;
using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model.Camera{
    class FPCamera : TgcCamera{
        private readonly Point mouseCenter; //Centro de mause 2D para ocultarlo.
        private Matrix cameraRotation;//Se mantiene la matriz rotacion para no hacer este calculo cada vez.
        private Vector3 directionView;//Direction view se calcula a partir de donde se quiere ver con la camara inicialmente. por defecto se ve en -Z.
        private float leftrightRot;
        private float updownRot;
        private bool lockCam;
        private Vector3 positionEye;
        public Mapa mapa;

        private TgcD3dInput Input { get; }
        public float MovementSpeed { get; set; }
        public float RotationSpeed { get; set; }
        public float JumpSpeed { get; set; }

        public FPCamera(Vector3 positionEye, float moveSpeed, float jumpSpeed, TgcD3dInput input, Mapa mapa) {
            Input = input;
            mouseCenter = new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
            RotationSpeed = 0.02f;
            directionView = new Vector3(0, 0, -1);
            leftrightRot = 0;
            updownRot = 0;
            cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);
            this.lockCam = true;
            Cursor.Hide();

            MovementSpeed = moveSpeed;
            JumpSpeed = jumpSpeed;
            this.positionEye = positionEye;
            this.mapa = mapa;
        }

        public bool LockCam {
            get { return lockCam; }
            set {
                if (!lockCam && value) {
                    Cursor.Position = mouseCenter;

                    Cursor.Hide();
                }
                if (lockCam && !value)
                    Cursor.Show();
                lockCam = value;
            }
        }

        ~FPCamera() {
            LockCam = false;
        }

        public override void UpdateCamera(float elapsedTime) {
            var moveVector = new Vector3(0, 0, 0);
            //Forward
            if (Input.keyDown(Key.RightShift)) {
                MovementSpeed = 50;
            }
            //Forward
            if (Input.keyDown(Key.W)) {
                moveVector += new Vector3(0, 0, -1) * MovementSpeed;
            }

            //Backward
            if (Input.keyDown(Key.S)) {
                moveVector += new Vector3(0, 0, 1) * MovementSpeed;
            }

            //Strafe right
            if (Input.keyDown(Key.D)) {
                moveVector += new Vector3(-1, 0, 0) * MovementSpeed;
            }

            //Strafe left
            if (Input.keyDown(Key.A)) {
                moveVector += new Vector3(1, 0, 0) * MovementSpeed;
            }

            //Jump
            /*if (Input.keyDown(Key.Space)) {
                moveVector += new Vector3(0, 1, 0) * JumpSpeed;
            }*/

            //Crouch
            /*if (Input.keyDown(Key.LeftControl)) {
                moveVector += new Vector3(0, -1, 0) * JumpSpeed;
            }*/

            if (Input.keyPressed(Key.L) || Input.keyPressed(Key.Escape)) {
                LockCam = !lockCam;
            }

            //Solo rotar si se esta aprentando el boton izq del mouse
            if (lockCam || Input.buttonDown(TgcD3dInput.MouseButtons.BUTTON_LEFT)) {
                leftrightRot -= -Input.XposRelative * RotationSpeed;
                updownRot -= Input.YposRelative * RotationSpeed;
                //Se actualiza matrix de rotacion, para no hacer este calculo cada vez y solo cuando en verdad es necesario.
                cameraRotation = Matrix.RotationX(updownRot) * Matrix.RotationY(leftrightRot);
            }

            if (lockCam)
                Cursor.Position = mouseCenter;

            //aca esta la papa
            var posY = mapa.getY(positionEye.X, positionEye.Z) + 20;

            if (positionEye.Y != posY) {
                positionEye.Y = (float)posY;
            }

            //Calculamos la nueva posicion del ojo segun la rotacion actual de la camara.
            var cameraRotatedPositionEye = Vector3.TransformNormal(moveVector * elapsedTime, cameraRotation);
            positionEye += cameraRotatedPositionEye;

            //Calculamos el target de la camara, segun su direccion inicial y las rotaciones en screen space x,y.
            var cameraRotatedTarget = Vector3.TransformNormal(directionView, cameraRotation);
            var cameraFinalTarget = positionEye + cameraRotatedTarget;

            var cameraOriginalUpVector = DEFAULT_UP_VECTOR;
            var cameraRotatedUpVector = Vector3.TransformNormal(cameraOriginalUpVector, cameraRotation);

            base.SetCamera(positionEye, cameraFinalTarget, cameraRotatedUpVector);
        }

        public override void SetCamera(Vector3 position, Vector3 directionView) {
            positionEye = position;
            this.directionView = directionView;
        }
    }
}