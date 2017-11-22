using TGC.Group.Model.GameObjects;

namespace TGC.Group.Model.GameObject{
    public class Personaje{
        public bool isMoving = false;

        public float sed = 100 ;
        public float hambre = 100;
        public float cansancio = 100;
        public float temperatura = 50;

        public Inventario inventario = new Inventario();

        public void beber() {
            sed += 20;
            if (sed > 100)
                sed = 100;
        }

        public void comer() {
            if (inventario.banana > 0) {
                hambre += 15;
                if (hambre > 100)
                    hambre = 100;
                --inventario.banana;
            }
        }

        public void updateTemp(float delta) {
            if(delta < 0.55) {
                temperatura += 0.01f;
            } else {
                temperatura -= 0.01f;
            }

            if (temperatura > 100)
                temperatura = 100;
            if (temperatura < 0)
                temperatura = 0;
        }

        private float deltaTemp() {
            float ret;
            if (temperatura >= 50) {
                ret = temperatura - 50;
            } else {
                ret = 50 - temperatura;
            }
            
            return ret / 15;
        }

        public void trabajo(float cansancio, float sed, float hambre) {
            this.cansancio -= cansancio + deltaTemp();
            this.sed -= sed + deltaTemp();
            this.hambre -= hambre + deltaTemp();
            if (cansancio < 0)
                cansancio = 0;
            if (sed < 0)
                sed = 0;
            if (hambre < 0)
                hambre = 0;
        }

        public void trabajoRecuperar() {
            cansancio++;

            if (cansancio > 100)
                cansancio = 100;
        }
    }
}
