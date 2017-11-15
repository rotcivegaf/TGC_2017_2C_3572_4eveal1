using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model.GameObject{
    public class Personaje{
        public bool isMoving = false;

        public float sed = 100;
        public float hambre = 100;
        public float cansancio = 100;
        public float temperatura = 50;

        public Inventario inventario = new Inventario();

        public void beber() {
            if (inventario.agua > 0) {
                sed += 20;
                if (sed > 100)
                    sed = 100;
                --inventario.agua;
            }
        }

        public void comer() {
            if (inventario.banana > 0) {
                hambre += 15;
                if (hambre > 100)
                    hambre = 100;
                --inventario.banana;
            }
        }
    }
}
