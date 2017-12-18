namespace TGC.Group.Model.GameObjects {
    public class Hora {
        public float time = 0;
        public bool pasoANoche;

        public Hora(float time) {
            this.time = time;
        }

        public float toScaleFactor() {
            var aux = toScaleFactor01();
            if (aux > 0.9)
                aux = 0.9f;
            return aux;
        }

        public float toScaleFactor01() {
            var aux = to24() % 12;
            if (to24() > 12)
                aux = 1 - (aux / 12);
            else
                aux = aux / 12;
            return aux;
        }

        public float to12() {
            var aux = to24() % 12;
            if (to24() > 12)
                aux = 12 - aux;
            return aux;
        }

        public float to24() {
            //return (time*2) % 24;
            return (time / 8) % 24;
        }

        public void updateTime(float time) {
            int oldTime = (int)to24();
            this.time += time;

            if ((oldTime == 7)&&(oldTime < (int)to24()))
                pasoANoche = !pasoANoche;
        }
    }
}
