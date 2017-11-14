using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model.GameObject{
    public class Personaje{
        public bool isMoving = false;

        public float sed = 104;
        public float hambre = 104;
        public float cansancio = 104;
        public float temperatura = 50;

        public Inventario inventario = new Inventario();

        public void beber() {
            if (inventario.agua > 0) {
                sed += 20;
                if (sed > 104)
                    sed = 104;
                --inventario.agua;
            }
        }

        public void comer() {
            if (inventario.banana > 0) {
                hambre += 15;
                if (hambre > 104)
                    hambre = 104;
                --inventario.banana;
            }
        }
    }
}
