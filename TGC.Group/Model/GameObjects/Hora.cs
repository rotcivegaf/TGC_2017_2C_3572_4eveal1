namespace TGC.Group.Model.GameObjects {
    public class Hora {
        public float time = 0;

        public Hora(float time) {
            this.time = time;
        }

        public float toScaleFactor() {
            var aux = toHora() % 12;
            if (toHora() > 12)
                aux =  1 - (aux / 12);
            else
                aux =  aux / 12;
            if (aux > 0.9)
                aux = 0.9f;
            return aux;
        }

        public float toHora() {
            return time % 24;
        }

        public void updateTime(float time) {
            this.time += time/4;
        }
    }
}
